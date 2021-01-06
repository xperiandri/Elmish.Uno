using Windows.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.EventBindingsAndBehaviors.Program;

namespace Elmish.Uno.Samples.EventBindingsAndBehaviors
{
    public partial class EventBindingsAndBehaviorsPage : Page
    {
        public EventBindingsAndBehaviorsPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
