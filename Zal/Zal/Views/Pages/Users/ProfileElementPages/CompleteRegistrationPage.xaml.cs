using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;

namespace Zal.Views.Pages.Users.ProfileElementPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CompleteRegistrationPage : ContentPage
	{
        private User user;

        public CompleteRegistrationPage ()
		{
			InitializeComponent ();
		}

        public CompleteRegistrationPage(User user)
        {
            InitializeComponent();
            this.user = user;
        }

        private async void ConfirmButton_Click(object sender, EventArgs e)
        {
            string phone = PhoneEntry.Text;
            string nick = NicknameEntry.Text;
            bool isBoy = genderSwitch.IsToggled;
            DateTime birth = datePicker.Date;
            await user.BecomeMember(phone, birth, isBoy, nick);
        }
    }
}