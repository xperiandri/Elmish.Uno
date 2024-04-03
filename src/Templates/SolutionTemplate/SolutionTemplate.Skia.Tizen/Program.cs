namespace SolutionTemplate;

using Uno.UI.Runtime.Skia;

public partial class App
{
    private static void Main(string[] args)
    {
        var host = new TizenHost(() => new SolutionTemplate.App(), args);
        host.Run();
    }
}
