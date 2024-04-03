using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.SubModel.Program;

namespace Elmish.Uno.Samples.SubModel
{
    public partial class SubModelPage : Page
    {
        public SubModelPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
