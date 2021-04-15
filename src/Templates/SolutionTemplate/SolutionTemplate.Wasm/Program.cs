using System;
using Windows.UI.Xaml;

namespace SolutionTemplate.Wasm
{
    public class Program
    {
        private static App app;

        private static int Main(string[] args)
        {
            var callback = new ApplicationInitializationCallback(_ => app = new App());
            Application.Start(callback);

            return 0;
        }
    }
}
