using System;
using Elmish.Uno.Samples.Validation;
using static Elmish.Uno.Samples.Validation.Program;

namespace Validation.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
