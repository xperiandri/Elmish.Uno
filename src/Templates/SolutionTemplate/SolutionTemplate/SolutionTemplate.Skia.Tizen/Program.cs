using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace SolutionTemplate.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new SolutionTemplate.App(), args);
            host.Run();
        }
    }
}
