namespace Elmish.Uno.Samples.WPF.Host;

using System.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		root.Content = new global::Uno.UI.Skia.Platform.WpfHost(Dispatcher, () => new Samples.App());
	}
}
