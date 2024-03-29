namespace Elmish.Uno.Samples.Wasm;

public static class Program
{
    private static App? app;

    public static int Main(string[] args)
    {
        Microsoft.UI.Xaml.Application.Start(_ => app = new AppHead());

        return 0;
    }
}
