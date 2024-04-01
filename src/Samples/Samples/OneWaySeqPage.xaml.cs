namespace Elmish.Uno.Samples.OneWaySeq;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.OneWaySeq.Program;

public partial class OneWaySeqPage : Page
{
    public OneWaySeqPage()
    {
        InitializeComponent();
        DataContext = new ElmishProgram.ViewModel();
        //UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
