using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.Tools;

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
            HideErrorLabels();
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
            if (ValidateInputs())
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
        }

        private bool ValidateInputs()
        {
            HideErrorLabels();
            bool isValid = true;
            isValid &= InputCondition(PassEntry.Text?.Length > 0, "Zadejte heslo", PassErrorLabel);
            isValid &= InputCondition(PassEntry.Text?.Length >= 2, "Heslo je příliš krátké", PassErrorLabel);
            if (isValid) isValid &= InputCondition(PassEntry.Text.Equals(PassConfirmEntry.Text), "Hesla se neshodují", Pass2ErrorLabel);
            isValid &= InputCondition(Validator.IsValidEmail(EmailEntry.Text), "Email není napsán správně", EmailErrorLabel);
            isValid &= InputCondition(NameEntry.Text?.Length >= 2, "Zadejte své jméno", NameErrorLabel);
            isValid &= InputCondition(SurnameEntry.Text?.Length >= 2, "Zadejte své příjmení", SurnameErrorLabel);
            return isValid;
        }

        private bool InputCondition(bool isValid, string errorText, Label errorLabel)
        {
            if (!isValid) errorLabel.Text = errorText;
            return isValid;
        }

        private void HideErrorLabels()
        {
            EmailErrorLabel.Text = "";
            NameErrorLabel.Text = "";
            SurnameErrorLabel.Text = "";
            PhoneErrorLabel.Text = "";
            PassErrorLabel.Text = "";
            Pass2ErrorLabel.Text = "";
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