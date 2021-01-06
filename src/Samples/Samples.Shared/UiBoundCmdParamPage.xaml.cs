using Windows.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.UiBoundCmdParam.Program;

namespace Elmish.Uno.Samples.UiBoundCmdParam
{
    public partial class UiBoundCmdParamPage : Page
    {
        public UiBoundCmdParamPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
