using System;
using Elmish.Uno.Samples.SubModelSelectedItem;
using static Elmish.Uno.Samples.SubModelSelectedItem.Program;

namespace SubModelSelectedItem.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
