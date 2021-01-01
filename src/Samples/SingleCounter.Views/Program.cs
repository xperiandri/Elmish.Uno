using System;
using Elmish.Uno.Samples.SingleCounter;
using static Elmish.Uno.Samples.SingleCounter.Program;

namespace SingleCounter.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
