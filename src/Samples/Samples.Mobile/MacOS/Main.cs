namespace Elmish.Uno.Samples;

#pragma warning disable CA1506 // 'App' is coupled with too many different types from too many different namespaces. Rewrite or refactor the code to decrease its class coupling below '96'.

using AppKit;

public partial class App
{
    private static void Main(string[] args)
    {
        NSApplication.Init();
        NSApplication.SharedApplication.Delegate = new App();
        NSApplication.Main(args);
    }
}

