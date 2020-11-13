namespace Iec608705104
{
    using System.Diagnostics;

    internal static class Check
    {
        public const string CompileSymbol = "CHECK";

        [Conditional(CompileSymbol)]
        internal static void Check1()
        {
        }
    }
}
