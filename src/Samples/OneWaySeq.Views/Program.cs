using System;
using Elmish.Uno.Samples.OneWaySeq;
using static Elmish.Uno.Samples.OneWaySeq.Program;

namespace OneWaySeq.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
