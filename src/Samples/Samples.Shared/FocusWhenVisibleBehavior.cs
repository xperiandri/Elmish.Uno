using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Elmish.Uno.Samples.EventBindingsAndBehaviors
{
    public sealed partial class FocusAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(Control), typeof(FocusAction), new PropertyMetadata((object)null));

        public Control TargetObject
        {
            get => (Control)this.GetValue(TargetObjectProperty);
            set => this.SetValue(TargetObjectProperty, (object)value);
        }

        public object Execute(object sender, object parameter)
        {
            Control val = ((object)TargetObject == null) ? (sender as Control) : TargetObject;
            if (val != null)
                val.Focus(FocusState.Programmatic);
            return null;
        }
    }
}
