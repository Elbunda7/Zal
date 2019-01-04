using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.Consts;

namespace Zal.Views.Pages.Users
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatorPage : ContentPage
	{
		public CreatorPage ()
		{
			InitializeComponent ();
        }

        private async void AddButton_Clicked(object sender, EventArgs args)
        {
            string name = NameEntry.Text ?? "defaultName";
            string surname = SurnameEntry.Text ?? "defaultSurname";
            await Zalesak.Users.AddNewUser(name, surname, (int)ZAL.Group.Non);
            await Navigation.PopAsync();
        }
    }
}