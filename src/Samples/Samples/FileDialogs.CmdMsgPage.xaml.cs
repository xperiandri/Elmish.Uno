namespace Elmish.Uno.Samples.FileDialogsCmdMsg;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.FileDialogsCmdMsg.Program;

public partial class FileDialogsCmdMsgPage : Page
{
    public FileDialogsCmdMsgPage()
    {
        InitializeComponent();
        UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
