namespace Elmish.Uno.Samples;

public partial class App
{
    private static App app;

    private static int Main(string[] args)
    {
        Microsoft.UI.Xaml.Application.Start(_ => app = new App());

        return 0;
    }
}
