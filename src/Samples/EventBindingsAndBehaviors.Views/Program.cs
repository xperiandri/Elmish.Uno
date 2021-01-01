using System;
using Elmish.Uno.Samples.EventBindingsAndBehaviors;
using static Elmish.Uno.Samples.EventBindingsAndBehaviors.Program;

namespace EventBindingsAndBehaviors.Views {
  public static class Program {
    [STAThread]
    public static void Main() =>
      main(new MainWindow());
  }
}
