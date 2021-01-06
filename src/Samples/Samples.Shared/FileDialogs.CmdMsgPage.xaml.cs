using Windows.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.FileDialogsCmdMsg.Program;

namespace Elmish.Uno.Samples.FileDialogsCmdMsg
{
    public partial class FileDialogsCmdMsgPage : Page
    {
        public FileDialogsCmdMsgPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
