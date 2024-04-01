namespace Elmish.Uno.Samples.NewWindow;

using Microsoft.FSharp.Core;
using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.NewWindow.Program;

public partial class NewWindowPage : Page
{
    public NewWindowPage()
    {
        InitializeComponent();
        var program = ElmishProgram.CreateProgram<Window1Page, Window2Page>(FuncConvert.FromFunc(() => this.DataContext));
        UnoProgram.StartElmishLoop(this, program);
    }
}
