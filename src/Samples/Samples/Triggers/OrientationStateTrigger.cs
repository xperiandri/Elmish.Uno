namespace Microsoft.Toolkit.Uwp.UI.Triggers;

#if !WINDOWS
// Copyright (c) Morten Nielsen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommunityToolkit.WinUI.Helpers;

using Elmish.Uno.Samples;

using Microsoft.Graphics.Display;
using Microsoft.UI.Xaml;

using Windows.Devices.Sensors;
using Windows.Graphics.Display;

//internal static class DesignModeHelpers
//{
//    private static bool inDesignModeCached;
//    private static bool inDesignMode;
//    private static bool GetIsInDesignMode()
//    {
//        if (DesignMode.DesignModeEnabled) return true;

//        if (ApiInformation.IsPropertyPresent("Windows.ApplicationModel.DesignMode", "DesignMode2Enabled"))
//        {
//            return DesignMode.DesignMode2Enabled;
//        }

//        return false;
//    }

//    public static bool IsInDesignMode
//    {
//        get
//        {
//            if (!inDesignModeCached)
//            {
//                inDesignMode = GetIsInDesignMode();
//                inDesignModeCached = true;
//            }

//            return inDesignMode;
//        }
//    }
//}

/// <summary>
/// Trigger for switching when the screen orientation changes
/// </summary>
public class OrientationStateTrigger : StateTriggerBase
{
    private readonly SimpleOrientationSensor simpleOrientationSensor = SimpleOrientationSensor.GetDefault();

    /// <summary>
    /// Initializes a new instance of the <see cref="OrientationStateTrigger"/> class.
    /// </summary>
    public OrientationStateTrigger()
    {
        //if (!DesignModeHelpers.IsInDesignMode)
        //{
        var weakEvent =
            new WeakEventListener<OrientationStateTrigger, SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventArgs) => OrientationStateTrigger_OrientationChanged(source, eventArgs),
                OnDetachAction = weakEventListener => simpleOrientationSensor.OrientationChanged -= weakEventListener.OnEvent
            };
        //DisplayInformation.GetForCurrentView().OrientationChanged += weakEvent.OnEvent;
        //}
        this.SetValue(OrientationsProperty, DependencyProperty.UnsetValue);
    }

    private void OrientationStateTrigger_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
     => UpdateTrigger(ToDisplayOrientations(args.Orientation));

    private void UpdateTrigger(DisplayOrientations orientation)
     => SetActive((Orientations & orientation) == orientation);

    /// <summary>
    /// Gets or sets the orientation to trigger on.
    /// </summary>
    public DisplayOrientations Orientations
    {
        get => (DisplayOrientations)GetValue(OrientationsProperty);
        set => SetValue(OrientationsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Orientations"/> parameter.
    /// </summary>
    public static readonly DependencyProperty OrientationsProperty =
        DependencyProperty.Register("Orientations", typeof(DisplayOrientations), typeof(OrientationStateTrigger),
        new PropertyMetadata(DisplayOrientations.None, OnOrientationPropertyChanged));

    private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var obj = (OrientationStateTrigger)d;
        //if (!DesignModeHelpers.IsInDesignMode)
        //{
        var orientation = ToDisplayOrientations(obj.simpleOrientationSensor.GetCurrentOrientation());
        obj.UpdateTrigger(orientation);
        //}
    }

    private static DisplayOrientations ToDisplayOrientations(SimpleOrientation orientation)
    {
        switch (orientation)
        {
            case SimpleOrientation.NotRotated:
                return DisplayOrientations.None;
            case SimpleOrientation.Rotated90DegreesCounterclockwise:
                return DisplayOrientations.LandscapeFlipped;
            case SimpleOrientation.Rotated180DegreesCounterclockwise:
                return DisplayOrientations.PortraitFlipped;
            case SimpleOrientation.Rotated270DegreesCounterclockwise:
                return DisplayOrientations.Landscape;
            default:
                return DisplayOrientations.None;
        }
    }
}
#endif
