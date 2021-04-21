// Copyright (c) Morten Nielsen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.UI.Xaml;

using Windows.Graphics.Display;

namespace Microsoft.Toolkit.Uwp.UI.Triggers
{
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
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var weakEvent =
                    new WeakEventListener<OrientationStateTrigger, DisplayInformation, object>(this)
                    {
                        OnEventAction = (instance, source, eventArgs) => OrientationStateTrigger_OrientationChanged(source, eventArgs),
                        OnDetachAction = weakEventListener => DisplayInformation.GetForCurrentView().OrientationChanged -= weakEventListener.OnEvent
                    };
                DisplayInformation.GetForCurrentView().OrientationChanged += weakEvent.OnEvent;
            }
        }

        private void OrientationStateTrigger_OrientationChanged(DisplayInformation sender, object args)
         => UpdateTrigger(sender.CurrentOrientation);

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
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var orientation = DisplayInformation.GetForCurrentView().CurrentOrientation;
                obj.UpdateTrigger(orientation);
            }
        }
    }
}
