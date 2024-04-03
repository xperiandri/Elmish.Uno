namespace SolutionTemplate;

using Android.App;
using Android.Views;

[Activity(
        MainLauncher = true,
        ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
    )]
public class MainActivity : global::Windows.UI.Xaml.ApplicationActivity
{
}
