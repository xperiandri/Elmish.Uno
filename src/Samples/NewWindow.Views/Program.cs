using System;
using Elmish.Uno.Samples.NewWindow;
using static Elmish.Uno.Samples.NewWindow.Program;

namespace NewWindow.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow(), () => new Window1(), () => new Window2());
  }
}
