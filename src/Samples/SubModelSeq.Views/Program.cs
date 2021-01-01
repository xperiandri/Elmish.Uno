using System;
using Elmish.Uno.Samples.SubModelSeq;
using static Elmish.Uno.Samples.SubModelSeq.Program;

namespace SubModelSeq.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
