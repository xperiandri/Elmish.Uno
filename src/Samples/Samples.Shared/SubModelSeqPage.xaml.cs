using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.SubModelSeq.Program;

namespace Elmish.Uno.Samples.SubModelSeq
{
    public partial class SubModelSeqPage : Page
    {
        public SubModelSeqPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
