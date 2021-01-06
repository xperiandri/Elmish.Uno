using Windows.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.Validation.Program;

namespace Elmish.Uno.Samples.Validation
{
    public partial class ValidationPage : Page
    {
        public ValidationPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
