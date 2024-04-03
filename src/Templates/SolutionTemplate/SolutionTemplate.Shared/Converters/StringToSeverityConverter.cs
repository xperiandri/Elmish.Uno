namespace SolutionTemplate.Converters;

using System;
using System.Diagnostics;

using global::Windows.UI.Xaml.Data;

using Microsoft.UI.Xaml.Controls;

public class StringToSeverityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter != null)
        {
            throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
        }

        switch (value)
        {
            case null:
                Debug.WriteLine("Converting null to InfoBarSeverity by using Informational value");
                return InfoBarSeverity.Informational;
            case string str:
                Debug.WriteLine("Converting string {str} to InfoBarSeverity");
                return Enum.Parse(typeof(InfoBarSeverity), str);
            case object obj when Enum.IsDefined(typeof(InfoBarSeverity), obj):
                Debug.WriteLine("Converting number {obj} to InfoBarSeverity");
                return (InfoBarSeverity)obj;
            default:
                throw new ArgumentException($"Value must either be null, a string or a number. Got {value} ({value.GetType().FullName})");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
     => throw new NotSupportedException();
}
