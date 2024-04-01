namespace Elmish.Uno.Samples.SubModelSelectedItem;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.SubModelSelectedItem.Program;

public partial class SubModelSelectedItemPage : Page
{
    public SubModelSelectedItemPage()
    {
        InitializeComponent();
        UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
