using System;
using Elmish.Uno.Samples.UiBoundCmdParam;
using static Elmish.Uno.Samples.UiBoundCmdParam.Program;

namespace UiBoundCmdParam.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
