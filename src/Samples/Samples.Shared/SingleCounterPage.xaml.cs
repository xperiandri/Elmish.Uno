using Windows.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.SingleCounter.Program;

namespace Elmish.Uno.Samples.SingleCounter
{
    public partial class SingleCounterPage : Page
    {
        public SingleCounterPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
