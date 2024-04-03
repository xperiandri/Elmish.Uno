namespace SolutionTemplate;

#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable RCS1163 // Unused parameter.

public partial class App
{
    private static App app;

    private static int Main(string[] args)
    {
        global::Windows.UI.Xaml.Application.Start(_ => app = new App());

        return 0;
    }
}
