using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.FileDialogs.Program;

namespace Elmish.Uno.Samples.FileDialogs
{
    public partial class FileDialogsPage : Page
    {
        public FileDialogsPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
