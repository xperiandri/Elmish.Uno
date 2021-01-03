using AppKit;

namespace Elmish.Uno.Samples
{
	public partial class App
	{
		private static void Main(string[] args)
		{
			NSApplication.Init();
			NSApplication.SharedApplication.Delegate = new App();
			NSApplication.Main(args);
		}
	}
}

