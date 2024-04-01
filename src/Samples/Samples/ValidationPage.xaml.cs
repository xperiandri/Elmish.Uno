namespace Elmish.Uno.Samples.Validation;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.Validation.Program;

public partial class ValidationPage : Page
{
    public ValidationPage()
    {
        InitializeComponent();
        UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
