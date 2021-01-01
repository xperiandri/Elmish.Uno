using System;
using Elmish.Uno.Samples.FileDialogs;
using static Elmish.Uno.Samples.FileDialogs.Program;

namespace FileDialogs.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
