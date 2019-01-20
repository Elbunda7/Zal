using Microsoft.AppCenter.Analytics;
using Plugin.Media;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Services;

namespace Zal.Views.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GaleryMainPage : ContentPage
	{
		public GaleryMainPage ()
		{
			InitializeComponent ();
            Analytics.TrackEvent("GaleryMainPage");
		}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (await HavePermission.For(Permission.Storage))
            {
                var a = await CrossMedia.Current.PickPhotoAsync();
                var b = ImageSource.FromFile(a.Path);
                mainImage.Source = b;
            }
        }
    }
}