namespace Elmish.Uno.Samples;

#pragma warning disable CA1501 // 'MainActivity' has an object hierarchy too many levels deep within the defining module.

using Android.App;
using Android.Content;
using Android.Views;

[Activity(
        MainLauncher = true,
        ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
    )]
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
}
