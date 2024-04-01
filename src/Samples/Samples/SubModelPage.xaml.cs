namespace Elmish.Uno.Samples.SubModel;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.SubModel.Program;

public partial class SubModelPage : Page
{
    public SubModelPage()
    {
        InitializeComponent();
        UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
