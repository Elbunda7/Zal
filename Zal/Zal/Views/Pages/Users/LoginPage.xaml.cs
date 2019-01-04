using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.Models;
using Zal.Domain.Tools;

namespace Zal.Views.Pages.Users
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        public LoginPage()
        {
            InitializeComponent();
            EmailEntry.Text = "pepa3@email.cz";
            PasswordEntry.Text = "password";
            HideErrorLabels();
        }

        private void HideErrorLabels()
        {
            EmailErrorLabel.Text = "";
            PassErrorLabel.Text = "";
        }

        private async void LoginButton_Click(object sender, EventArgs args)
        {
            if (ValidateInputs())
            {
                LoginErrorModel response = await Zalesak.Session.LoginAsync(EmailEntry.Text, PasswordEntry.Text, StayLoggedSwitch.IsToggled);
                if (response.HasAnyErrors)
                {
                    HandleErrors(response);
                }
                else
                {
                    await ShowProfile();
                }
            }
        }

        private bool ValidateInputs()
        {
            HideErrorLabels();
            bool isValid = Validator.IsValidEmail(EmailEntry.Text);
            if (!isValid)
            {
                EmailErrorLabel.Text = "Email není napsán správně";
            }
            return isValid;
        }

        private void HandleErrors(LoginErrorModel loginError)
        {
            if (!loginError.IsExist)
            {//cleanCode?
                EmailErrorLabel.Text = "Email nebyl nalezen";
            }
            else if (!loginError.IsPasswordCorrect)
            {
                PassErrorLabel.Text = "Nesprávné heslo";
            }
        }

        private async Task ShowProfile()
        {
            Navigation.InsertPageBefore(new ProfilePage(), Navigation.NavigationStack.First());
            await Navigation.PopToRootAsync();
        }

        private async void ToRegistration_Click()
        {
            await Navigation.PushAsync(new Users.RegisterPage());
            Navigation.RemovePage(this);
        }

        private void DevLoginLeader_Click(object sender, EventArgs args)
        {
            //LoginUser("Leader@email.cz", "pass");
        }

        private void DevLoginMember_Click(object sender, EventArgs args)
        {
            //LoginUser("Member@email.cz", "pass");
        }

        private void DevLoginNovice_Click(object sender, EventArgs args)
        {
            //LoginUser("Novice@email.cz", "pass");
        }
    }
}