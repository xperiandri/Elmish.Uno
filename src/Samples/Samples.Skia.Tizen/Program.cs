using Tizen.Applications;

using Uno.UI.Runtime.Skia;

namespace Elmish.Uno.Samples.Skia.Tizen
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var host = new TizenHost(() => new Samples.App(), args);
            host.Run();
        }
    }
}
