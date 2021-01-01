using System;
using Elmish.Uno.Samples.FileDialogs.CmdMsg;
using static Elmish.Uno.Samples.FileDialogs.CmdMsg.Program;

namespace FileDialogs.CmdMsg.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
