namespace Elmish.Uno.Samples.EventBindingsAndBehaviors;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.EventBindingsAndBehaviors.Program;

public partial class EventBindingsAndBehaviorsPage : Page
{
    public EventBindingsAndBehaviorsPage()
    {
        InitializeComponent();
        UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
