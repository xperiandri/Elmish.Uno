using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Nostra13.Universalimageloader.Core;
using Microsoft.UI.Xaml.Media;

namespace Elmish.Uno.Samples
{
	[global::Android.App.ApplicationAttribute(
		Label = "@string/ApplicationName",
		LargeHeap = true,
		HardwareAccelerated = true,
		Theme = "@style/AppTheme"
	)]
    public class AndroidApplication : Microsoft.UI.Xaml.NativeApplication
	{
        public AndroidApplication(IntPtr javaReference, JniHandleOwnership transfer)
			: base(() => new App(), javaReference, transfer)
         => ConfigureUniversalImageLoader();

		private void ConfigureUniversalImageLoader()
		{
            using (var builder = new ImageLoaderConfiguration
                            .Builder(Context))
            {
#pragma warning disable DF0010 // Marks undisposed local variables.
                ImageLoaderConfiguration config = builder.Build();
			ImageLoader.Instance.Init(config);
#pragma warning restore DF0010 // Marks undisposed local variables.

			ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
		}
	}
}
}
