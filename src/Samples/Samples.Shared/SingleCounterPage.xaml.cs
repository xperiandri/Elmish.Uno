using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Elmish.Uno;
using ElmishProgram = Elmish.Uno.Samples.SingleCounter.Program;

namespace Elmish.Uno.Samples.SingleCounter
{
    public partial class SingleCounterPage : Page
    {
        public SingleCounterPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameters = e.Parameter as IReadOnlyDictionary<string, object>;
            var count = Convert.ToInt32(parameters?["count"]);
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.runWith, ElmishProgram.Program, count);
        }
    }
}
