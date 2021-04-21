using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace Elmish.Uno.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public IReadOnlyDictionary<string, object> SingleCounterParameter { get; set; }
            = new Dictionary<string, object>
        {
            ["count"] = 5
        };

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SampleItem;
            Frame.Navigate(Type.GetType(item.PageTypeName), item.Parameter);
        }
    }

    public class SampleItem
    {
        public string Title { get; set; }
        public string PageTypeName => $"Elmish.Uno.Samples.{Title}.{Title}Page";
        public object Parameter { get; set; }
    }
}
