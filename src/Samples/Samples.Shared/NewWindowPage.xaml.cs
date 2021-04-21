using Microsoft.FSharp.Core;
using Microsoft.UI.Xaml.Controls;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.NewWindow.Program;

namespace Elmish.Uno.Samples.NewWindow
{
    public partial class NewWindowPage : Page
    {
        public NewWindowPage()
        {
            InitializeComponent();
            var program = ElmishProgram.CreateProgram<Window1Page, Window2Page>(FuncConvert.FromFunc(() => this.DataContext));
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, program);
        }
    }
}
