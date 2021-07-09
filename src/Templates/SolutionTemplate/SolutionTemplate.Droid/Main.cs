using System;
using Android.Runtime;
using Com.Nostra13.Universalimageloader.Core;
using Windows.UI.Xaml.Media;

namespace SolutionTemplate.Droid
{
    [global::Android.App.ApplicationAttribute(
        Label = "@string/ApplicationName",
        LargeHeap = true,
        HardwareAccelerated = true,
        Theme = "@style/AppTheme"
    )]
    public class Application : Windows.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new App(), javaReference, transfer)
        {
            ConfigureUniversalImageLoader();
        }

        private static void ConfigureUniversalImageLoader()
        {
            // Create global configuration and initialize ImageLoader with this config
            using var builder = new ImageLoaderConfiguration.Builder(Context);
            ImageLoaderConfiguration config = builder.Build();

            ImageLoader.Instance.Init(config);

            ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
        }
    }
}
