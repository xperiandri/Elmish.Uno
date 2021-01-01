using System;
using Elmish.Uno.Samples.SubModel;
using static Elmish.Uno.Samples.SubModel.Program;

namespace SubModel.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
