namespace Elmish.Uno.Samples.SubModelOpt;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.SubModelOpt.Program;

public partial class SubModelOptPage : Page
{
    public SubModelOptPage()
    {
        InitializeComponent();
        UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
