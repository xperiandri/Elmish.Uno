namespace Elmish.Uno.Samples.SubModelSeq;

using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.SubModelSeq.Program;

public partial class SubModelSeqPage : Page
{
    public SubModelSeqPage()
    {
        InitializeComponent();
        UnoProgram.StartElmishLoop(this, ElmishProgram.Program);
    }
}
