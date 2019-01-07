using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Microsoft.AppCenter.Crashes;

namespace Zal.Droid
{
    [Activity(Label = "Zal", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            //AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
          
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
            IconEntry.FormsPlugin.Android.IconEntryRenderer.Init();
            LoadApplication(new App());
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            Crashes.TrackError(new Exception("Unhandled", e.Exception));
        }
    }
}