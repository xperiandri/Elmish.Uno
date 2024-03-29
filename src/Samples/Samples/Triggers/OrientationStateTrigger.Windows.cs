namespace Microsoft.Toolkit.Uwp.UI.Triggers;

#if WINDOWS
// Copyright (c) Morten Nielsen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Elmish.Uno.Samples;

using Microsoft.UI.Xaml;

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
    /// <summary>
    /// Initializes a new instance of the <see cref="OrientationStateTrigger"/> class.
    /// </summary>
    public OrientationStateTrigger()
    {
    }

    /// <summary>
    /// Gets or sets the orientation to trigger on.
    /// </summary>
    public object Orientations
    {
        get => GetValue(OrientationsProperty);
        set => SetValue(OrientationsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Orientations"/> parameter.
    /// </summary>
    public static readonly DependencyProperty OrientationsProperty =
        DependencyProperty.Register("Orientations", typeof(object), typeof(OrientationStateTrigger),
        new PropertyMetadata("None"));
}
#endif
