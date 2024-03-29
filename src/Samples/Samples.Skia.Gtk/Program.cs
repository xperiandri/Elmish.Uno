namespace Elmish.Uno.Samples.Skia.Gtk;

using System;
using GLib;
using global::Uno.UI.Runtime.Skia.Gtk;

public static class Program
{
    public static void Main(string[] args)
    {
        ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs)
        {
            Console.WriteLine("GLIB UNHANDLED EXCEPTION" + expArgs.ExceptionObject.ToString());
            expArgs.ExitApplication = true;
        };

        var host = new GtkHost(() => new AppHead());

        host.Run();
    }
}
