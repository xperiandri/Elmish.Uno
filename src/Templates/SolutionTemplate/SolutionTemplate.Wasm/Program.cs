namespace SolutionTemplate.Wasm
{
    public partial class Program
    {
#pragma warning disable IDE1006 // Naming Styles
        private static App app;
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable CA1801 // Review unused parameters
        private static int Main(string[] args)
#pragma warning restore CA1801 // Review unused parameters
#pragma warning restore RCS1163 // Unused parameter.
        {
            Windows.UI.Xaml.Application.Start(_ => app = new App());

            return 0;
        }
    }
}
