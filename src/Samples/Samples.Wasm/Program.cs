﻿using System;

using Windows.UI.Xaml;

namespace Elmish.Uno.Samples.Wasm
{
    public static class Program
    {
        private static App app;

        private static int Main(string[] args)
        {
            Windows.UI.Xaml.Application.Start(_ => app = new App());

            return 0;
        }
    }
}
