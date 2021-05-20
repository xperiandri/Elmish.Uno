using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SolutionTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
#pragma warning disable CA1501
#pragma warning disable CA1010 // Generic interface should also be implemented
    public sealed partial class MainPage : Page
#pragma warning restore CA1010 // Generic interface should also be implemented
#pragma warning restore CA1501
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }
}
