namespace SolutionTemplate.Wasm
{
    public class Program
    {
#pragma warning disable IDE1006 // Naming Styles
        private static App _app;
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE0040 // Add accessibility modifiers
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable CA1801 // Review unused parameters
        static int Main(string[] args)
#pragma warning restore CA1801 // Review unused parameters
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0040 // Add accessibility modifiers
        {
            Windows.UI.Xaml.Application.Start(_ => _app = new App());

            return 0;
        }
    }
}
