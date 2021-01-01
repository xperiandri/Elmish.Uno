using System;
using Elmish.Uno.Samples.SubModelOpt;
using static Elmish.Uno.Samples.SubModelOpt.Program;

namespace SubModelOpt.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
