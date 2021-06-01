using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace SolutionTemplate.Controls
{
    public sealed partial class ValidationControl : ContentControl
    {
        #region PropertyName

        /// <summary>
        /// PropertyName Dependency Property
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(ValidationControl),
                new PropertyMetadata((string)null,
                    (d, e) => ((ValidationControl)d).OnPropertyNameChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// Gets or sets the PropertyName property. This dependency property
        /// indicates ....
        /// </summary>
        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the PropertyName property.
        /// </summary>
#pragma warning disable CA1801 // Review unused parameters
        private void OnPropertyNameChanged(string oldPropertyName, string newPropertyName)
#pragma warning restore CA1801 // Review unused parameters
         => ResetErrors(newPropertyName);

        #endregion

        #region Errors

        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register(
              "Errors",
              typeof(ICollection<object>),
              typeof(ValidationControl),
              new PropertyMetadata(null));

        public ICollection<object> Errors
        {
            get => (ICollection<object>)GetValue(ErrorsProperty);
            private set => SetValue(ErrorsProperty, value);
        }

        #endregion

        #region ErrorTemplate

        /// <summary>
        /// IsCreative Dependency Property
        /// </summary>
        public static readonly DependencyProperty ErrorTemplateProperty =
            DependencyProperty.Register(nameof(ErrorTemplate), typeof(DataTemplate), typeof(ValidationControl),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the IsCreative property. This dependency property
        /// indicates ....
        /// </summary>
        public DataTemplate ErrorTemplate
        {
            get => (DataTemplate)GetValue(ErrorTemplateProperty);
            set => SetValue(ErrorTemplateProperty, value);
        }

        #endregion

        #region ErrorContentStyle

        /// <summary>
        /// ErrorContentStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ErrorContentStyleProperty =
            DependencyProperty.Register(nameof(ErrorContentStyle), typeof(Style), typeof(ValidationControl),
                new PropertyMetadata((Style)null,
                    (d, e) => ((ValidationControl)d).OnErrorContentStyleChanged((Style)e.OldValue, (Style)e.NewValue)));

        /// <summary>
        /// Gets or sets the ErrorContentStyle property. This dependency property
        /// indicates ....
        /// </summary>
        public Style ErrorContentStyle
        {
            get => (Style)GetValue(ErrorContentStyleProperty);
            set => SetValue(ErrorContentStyleProperty, value);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ErrorContentStyle property.
        /// </summary>
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable RCS1163 // Unused parameter.
        private void OnErrorContentStyleChanged(Style oldErrorContentStyle, Style newErrorContentStyle)
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore CA1801 // Review unused parameters
         => TrySetStyle();

        #endregion

        public ValidationControl()
        {
            this.DefaultStyleKey = typeof(ValidationControl);
            this.DataContextChanged += OnDataContextChanged;
        }

        private Style lastErrorStyle;

        private void TrySetStyle()
        {
            if (Errors?.Any() ?? false)
            {
                if (lastErrorStyle != null)
                {
                    this.Resources.Remove(ErrorContentStyle.TargetType);
                    lastErrorStyle = null;
                }
            }
            else
            {
                if (ErrorContentStyle != null)
                {
                    lastErrorStyle = ErrorContentStyle;
                    this.Resources[lastErrorStyle.TargetType] = lastErrorStyle;
                }
            }
        }

        private INotifyDataErrorInfo currentINotifyDataErrorInfo = null;

        private void OnDataContextChanged(object element, DataContextChangedEventArgs e)
        {
            if (currentINotifyDataErrorInfo != null)
            {
                currentINotifyDataErrorInfo.ErrorsChanged -= OnErrorsChanged;
                currentINotifyDataErrorInfo = null;
            }
            if (e.NewValue is INotifyDataErrorInfo DataContext)
            {
                DataContext.ErrorsChanged += OnErrorsChanged;
                currentINotifyDataErrorInfo = DataContext;
            }
        }

        private void ResetErrors(string value)
        {
            if (currentINotifyDataErrorInfo != null)
            {
                var errors = currentINotifyDataErrorInfo.GetErrors(value);
                if (errors is ObservableCollection<object> collection)
                {
                    if (Errors != errors)
                        Errors = collection;
                }
                else
                {
                    var errorsList = errors?.Cast<object>().ToList();
                    if (Errors is ObservableCollection<object> ourCollection)
                    {
                        if (errorsList != null)
                        {
                            for (int i = ourCollection.Count - 1; i <= 0; i--)
                                if (!errorsList.Contains(ourCollection[i]))
                                {
                                    ourCollection.RemoveAt(i);
                                }
                            for (int i = 0; i < errorsList.Count; i++)
                                if (!ourCollection.Contains(errorsList[i]))
                                {
                                    ourCollection.Add(errorsList[i]);
                                }
                        }
                        else
                            for (int i = ourCollection.Count - 1; i <= 0; i--)
                            {
                                ourCollection.RemoveAt(i);
                            }
                    }
                    else
                        Errors = (errorsList == null) ? null : new ObservableCollection<object>(errorsList);
                }
            }
            TrySetStyle();
        }

        private void OnErrorsChanged(object obj, DataErrorsChangedEventArgs args) => ResetErrors(PropertyName);
    }

}
