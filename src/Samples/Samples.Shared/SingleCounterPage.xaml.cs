namespace Elmish.Uno.Samples.SingleCounter;

#pragma warning disable CA1305 //The behavior of ... could vary based on the current user's locale settings. Replace this call in ... with a call to ...

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ElmishProgram = Elmish.Uno.Samples.SingleCounter.Program;

public partial class SingleCounterPage : Page
{
    public SingleCounterPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Contract.Assume(e != null);
        var parameters = e.Parameter as IReadOnlyDictionary<string, object>;
        var count = Convert.ToInt32(parameters?["count"], CultureInfo.InvariantCulture);
        ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.runWith, ElmishProgram.Program, count);
    }
}
