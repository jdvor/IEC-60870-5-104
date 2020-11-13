param (
    [string]
    $NugetApiKey
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Push-Location (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

if ($NugetApiKey -eq '') {
    $NugetApiKey = $env:NUGET_APIKEY_IEC608705104
    if ($NugetApiKey -eq '') {
        Throw 'Missing nuget API key'
    }
}

$publishDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath('./publish')
if (-not (Test-Path $publishDir)) {
    Throw 'Nothing to publish.'
}

$published = $False
Get-ChildItem $publishDir -Include '*.nupkg' -Recurse | ForEach-Object {
    & dotnet nuget push $_.FullName -k $NugetApiKey -s https://api.nuget.org/v3/index.json
    $published = $True
}

if (-not $published) {
    Throw 'Nothing to publish.'
}

Pop-Location