using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Elmish.Uno.Samples.Controls
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
        /// indicates which property of a DataContext object to monitor for errors.
        /// </summary>
        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable RCS1163 // Unused parameter.
        private void OnPropertyNameChanged(string oldPropertyName, string newPropertyName)
         => ResetErrors(newPropertyName);
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore CA1801 // Review unused parameters


        #endregion PropertyName

        #region Errors

        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register(nameof(Errors), typeof(object), typeof(ValidationControl),
                new PropertyMetadata(null, (d, e) => ((ValidationControl)d).OnErrorsChanged(e.OldValue, e.NewValue)));

        private void OnErrorsChanged(object oldValue, object newValue)
        {
            if (oldValue is INotifyCollectionChanged oldObservableCollection)
                oldObservableCollection.CollectionChanged -= OnErrorsCollectionChanged;
            if (newValue is INotifyCollectionChanged newObservableCollection)
                newObservableCollection.CollectionChanged += OnErrorsCollectionChanged;
        }

        /// <summary>
        /// Gets the Errors property. This dependency property contains errors list displayed.
        /// </summary>
        /// <remarks>
        /// For internal use only!
        /// </remarks>
        public object Errors
        {
            get => GetValue(ErrorsProperty);
            private set => SetValue(ErrorsProperty, value);
        }

        #endregion Errors

        #region ErrorItemTemplate

        /// <summary>
        /// ErrorItemTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty ErrorItemTemplateProperty =
            DependencyProperty.Register(nameof(ErrorItemTemplate), typeof(DataTemplate), typeof(ValidationControl),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ErrorItemTemplate property. This dependency property
        /// contains a DataTemplate to display error item content.
        /// </summary>
        public DataTemplate ErrorItemTemplate
        {
            get => (DataTemplate)GetValue(ErrorItemTemplateProperty);
            set => SetValue(ErrorItemTemplateProperty, value);
        }

        #endregion ErrorItemTemplate

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
        /// contains a Style to apply to content if errors are present.
        /// </summary>
        public Style ErrorContentStyle
        {
            get => (Style)GetValue(ErrorContentStyleProperty);
            set => SetValue(ErrorContentStyleProperty, value);
        }

#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable RCS1163 // Unused parameter.
        private void OnErrorContentStyleChanged(Style oldErrorContentStyle, Style newErrorContentStyle)
        {
            UnapplyErrorContentStyle();
            TrySetStyle(newErrorContentStyle);
        }
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore CA1801 // Review unused parameters

        #endregion ErrorContentStyle

        public ValidationControl()
        {
            this.DefaultStyleKey = typeof(ValidationControl);
            this.DataContextChanged += OnDataContextChanged;
        }

        private Style lastErrorStyle;

        private void TrySetStyle(Style style)
        {
            var errors = this.Errors as IEnumerable;
            if (errors?.GetEnumerator().MoveNext() ?? false)
            {
                if (style != null && lastErrorStyle == null)
                {
                    lastErrorStyle = style;
                    this.Resources[lastErrorStyle.TargetType] = lastErrorStyle;
                }
            }
            else
            {
                UnapplyErrorContentStyle();
            }
        }

        private void UnapplyErrorContentStyle()
        {
            if (lastErrorStyle != null)
            {
                this.Resources.Remove(lastErrorStyle.TargetType);
                lastErrorStyle = null;
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

        private static readonly Type observableCollectionType = typeof(ObservableCollection<>);
        private readonly Lazy<ObservableCollection<object>> errorsProxy = new Lazy<ObservableCollection<object>>();
        private ObservableCollection<object> ErrorsProxy => errorsProxy.Value;

        private void OnErrorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => TrySetStyle(ErrorContentStyle);

        private void ResetErrors(string propertyName)
        {
            if (currentINotifyDataErrorInfo != null)
            {
                var errors = currentINotifyDataErrorInfo.GetErrors(propertyName);
                if (Errors == errors) return;

                if (errors?.GetType().GetGenericTypeDefinition() == observableCollectionType)
                {
                    Errors = errors;
                }
                else
                {
                    var errorsList = errors?.Cast<object>().ToList();
                    if (errorsList == null && errorsProxy.IsValueCreated && Errors == ErrorsProxy)
                    {

                        for (int i = ErrorsProxy.Count - 1; i >= 0; i--)
                        {
                            ErrorsProxy.RemoveAt(i);
                        }
                    }
                    else if (errorsList != null)
                    {
                        Errors = ErrorsProxy;
                        for (int i = ErrorsProxy.Count - 1; i >= 0; i--)
                            if (!errorsList.Contains(ErrorsProxy[i]))
                            {
                                ErrorsProxy.RemoveAt(i);
                            }
                        for (int i = 0; i < errorsList.Count; i++)
                            if (!ErrorsProxy.Contains(errorsList[i]))
                            {
                                ErrorsProxy.Add(errorsList[i]);
                            }
                    }
                }
            }
        }

        private void OnErrorsChanged(object obj, DataErrorsChangedEventArgs args)
        {
            if (args.PropertyName == PropertyName) ResetErrors(PropertyName);
        }
    }
}
