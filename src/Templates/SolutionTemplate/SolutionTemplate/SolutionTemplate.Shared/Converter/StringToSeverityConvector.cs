using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;


namespace SolutionTemplate.Converter
{
    public class StringToSeverityConvector : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null)
            {
                throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
            }

            if (value != null && !(value is string))
            {
                throw new ArgumentException($"Value must either be null or of type string. Got {value} ({value.GetType().FullName})");
            }

            return Enum.Parse(typeof(InfoBarSeverity), ((string)value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
