using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.OneWaySeq.Program;

namespace Elmish.Uno.Samples.OneWaySeq
{
    public partial class OneWaySeqPage : Page
    {
        public OneWaySeqPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
