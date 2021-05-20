using System;
using Windows.UI.Xaml;

namespace SolutionTemplate.Wasm
{
    public static class Program
    {
        private static App app;

#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable CA1801 // Review unused parameters
        private static int Main(string[] args)
#pragma warning restore CA1801 // Review unused parameters
#pragma warning restore RCS1163 // Unused parameter.
        {
            var callback = new ApplicationInitializationCallback(_ => app = new App());
            Application.Start(callback);

            return 0;
        }
    }
}
