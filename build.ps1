param (
    [string]
    [Parameter(HelpMessage='Semantic version for the library; example: 1.0.0.2')]
    [ValidatePattern('^\d+(\.\d+){2,3}$')]
    $VersionPrefix = '0.1.0',

    [Parameter(HelpMessage='Git branch name; assuming "master" if not set')]
    [string] $BranchName,

    [string]
    [ValidateSet('Debug', 'Release')]
    $Configuration = 'Release',

    [Parameter(HelpMessage='Code analysis will be skipped.')]
    [switch]
    $SkipCodeAnalysis,

    [Parameter(HelpMessage='Code analysis warnings will be ignored; otherwise the build would fail if there are any.')]
    [switch]
    $AllowWarnings,

    [Parameter(HelpMessage='Run unit tests.')]
    [switch]
    $Tests,

    [Parameter(HelpMessage='Filter tests. https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test#filter-option-details')]
    [string]
    [ValidateSet('Debug', 'Release')]
    $TestsFilter,

    [Parameter(HelpMessage='Create NuGet package.')]
    [switch]
    $Pack,

    [Parameter(HelpMessage='Publish NuGet package locally.')]
    [switch]
    $PublishLocally
)

function Label {
    param([string] $msg)
    Write-Host ">> $msg" -ForegroundColor Yellow
}

function OrDie {
    param([string] $msg)
    if ($LastExitCode -ne 0) {
        Write-Host
        Write-Host "ERROR: $msg" -ForegroundColor Red
        Exit 1
    }
}

function Get-VersionSuffix {
    param ([string] $bn)
    if ([string]::IsNullOrEmpty($bn) -or ($bn -eq 'master')) {
        return ''
    }
    $bn = $bn.Trim()
    if ($bn.Length -gt 30) {
        $bn = $bn.Substring(0, 30);
    }
    $bn = $bn -replace '[^\w\-]',''
    $bn = $bn.Replace('_', '-');
    return $bn.ToLower()
}

function Get-NugetLocalFeedPath {
    if (Test-Path 'D:\LocalNugetPackages') {
        return 'D:\LocalNugetPackages';
    }
    if (Test-Path 'C:\LocalNugetPackages') {
        return 'C:\LocalNugetPackages';
    }
    if (Test-Path 'E:\LocalNugetPackages') {
        return 'E:\LocalNugetPackages';
    }
    $nlfd = Join-Path $env:TEMP 'LocalNugetPackages'
    if (-not (Test-Path $nlfd)) {
        New-Item -ItemType Directory -Path $nlfd
    }
    return $nlfd
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Push-Location (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

$publishDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath('./publish')
$GitBranch = ((Invoke-Expression 'git rev-parse --abbrev-ref HEAD') | Out-String).Trim()
$GitCommit = ((Invoke-Expression 'git rev-parse HEAD') | Out-String).Trim()
$branch = if ($BranchName) { $BranchName } else { $GitBranch }
$VersionSuffix = Get-VersionSuffix $branch
$Version = if ($VersionSuffix -eq '') { $VersionPrefix } else { "$VersionPrefix-$VersionSuffix" }
$NugetPackagesRoot = (Invoke-Expression "nuget locals global-packages -list" | Out-String).Replace("global-packages:", "").Trim()

Write-Host "Version: $Version"
Write-Host "Commit: $GitCommit"
Write-Host "Branch: $GitBranch"
Write-Host "NugetPackagesRoot: $NugetPackagesRoot"

# clean
Label 'Clean'
Get-ChildItem ./ -Include bin,obj,publish,packages -Recurse | ForEach-Object { Remove-Item $_.FullName -Force -Recurse -ErrorAction SilentlyContinue }

# build
Label 'Build'
$sln = Resolve-Path 'IEC-60870-5-104.sln'
$WarnAsErrors = if ($AllowWarnings) { "false" } else { "true" }
$CodeAnalysis = if ($SkipCodeAnalysis) { "false" } else { "true" }
$null = & dotnet build $sln -c $Configuration /p:VersionPrefix=$VersionPrefix /p:VersionSuffix=$VersionSuffix /p:WarnAsErrors=$WarnAsErrors /p:CodeAnalysis=$CodeAnalysis --nologo
OrDie 'Build has failed.'

# test
if ($Tests) {
    Get-ChildItem './test' -Include '*.Tests.csproj' -Recurse | ForEach-Object {
        $projName = [System.IO.Path]::GetFileNameWithoutExtension($_.FullName)
        Label "Unit tests $projName"
        if ($TestsFilter) {
            & dotnet test $_.FullName -c $Configuration --filter $TestsFilter --no-build --no-restore --nologo
        } else {
            & dotnet test $_.FullName -c $Configuration --no-build --no-restore --nologo
        }
        OrDie "Unit tests $projName have failed."
    }
}

# pack
if ($Pack -or $PublishLocally) {
    Label 'Pack'
    $csproj = Resolve-Path 'src/Iec608705104/Iec608705104.csproj'
    & dotnet pack $csproj -c $Configuration /p:VersionPrefix=$VersionPrefix /p:VersionSuffix=$VersionSuffix -o $publishDir --no-restore --no-build --nologo
    OrDie 'Packaging has failed.'
}

# publish locally
if ($PublishLocally) {
    Label 'Publish locally'
    $NugetLocalFeed = Get-NugetLocalFeedPath
    [regex]$rx = "^(?<name>.+)\.\d+(\.\d+){2,3}(\-[a-zA-Z0-9\-]+)?\.nupkg$"
    Get-ChildItem $publishDir -Include '*.nupkg' -Recurse | ForEach-Object {
        $nugetFilename = Split-Path $_.FullName -Leaf
        $match = $rx.Match($nugetFilename)
        $name = $match.Groups['name'].Value
        $null = & nuget delete $name $Version -Source $NugetPackagesRoot -NonInteractive
        $null = & nuget delete $name $Version -Source $NugetLocalFeed -NonInteractive
        & nuget add $_.FullName -Source $NugetLocalFeed -NonInteractive
    }
}

Pop-Location