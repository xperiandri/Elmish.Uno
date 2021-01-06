using Windows.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.SubModelOpt.Program;

namespace Elmish.Uno.Samples.SubModelOpt
{
    public partial class SubModelOptPage : Page
    {
        public SubModelOptPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
