namespace Elmish.Uno.Samples.FileDialogs;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.FileDialogs.Program;

public partial class FileDialogsPage : Page
{
    public FileDialogsPage()
    {
        InitializeComponent();
        UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
