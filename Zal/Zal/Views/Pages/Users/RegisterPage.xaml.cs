using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;

namespace Zal.Views.Pages.Users
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
		public RegisterPage ()
		{
			InitializeComponent ();
            Title = "Registrace";
            Analytics.TrackEvent("RegisterPage");
        }

        private void ConfirmatedPassword_Completed(object sender, EventArgs e)
        {
            if (PassEntry.Text == PassConfirmEntry.Text)
            {
                RegistrationButton.IsEnabled = true;
            }
        }

        private async void RegistrationButton_Click(object sender, EventArgs args)
        {
            bool isRegistered = await Zalesak.Session.RegisterAsync(NameEntry.Text, SurnameEntry.Text, PhoneEntry.Text, EmailEntry.Text, PassEntry.Text);
            if (isRegistered)
            {
                await ShowProfile();
            }
            else
            {
                await DisplayAlert("Registrace", "Registrace se nezdařila", "Ok");
            }
            Analytics.TrackEvent("RegisterPage_Registration", new Dictionary<string, string>() { { "isSuccess", isRegistered.ToString() } });
        }

        private async Task ShowProfile()
        {
            Navigation.InsertPageBefore(new ProfilePage(), Navigation.NavigationStack.First());
            await Navigation.PopToRootAsync();
        }

        private async void ToLogin_Click()
        {
            await Navigation.PushAsync(new LoginPage());
            Navigation.RemovePage(this);
        }
    }
}