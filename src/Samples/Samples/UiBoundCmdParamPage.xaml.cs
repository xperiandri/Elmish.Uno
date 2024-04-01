namespace Elmish.Uno.Samples.UiBoundCmdParam;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.UiBoundCmdParam.Program;

public partial class UiBoundCmdParamPage : Page
{
    public UiBoundCmdParamPage()
    {
        InitializeComponent();
        UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
