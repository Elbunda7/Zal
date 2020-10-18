using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;

namespace Zal.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoadingPage : ContentPage
	{
        public event Action Loaded;

		public LoadingPage ()
		{
			InitializeComponent ();
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Version appVersion =  new Version(VersionTracking.CurrentVersion);
            Version lastVersion = await Zalesak.CurrentVersion();
            if (appVersion == lastVersion)
            {
                await DisplayAlert("Verze aplikace", $"aktuální {appVersion}", "OK");
            }
            else
            {
                await DisplayAlert("Verze aplikace", $"máte {appVersion} a poslední je {lastVersion}", "Aktualizovat");
            }
            Loaded?.Invoke();
        }
    }
}