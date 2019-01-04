using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;

namespace Zal.Views.Pages.Actions
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
            string type = TypeEntry.Text ?? "";
            string name = NameEntry.Text ?? "";
            int days = DaysEntry.Text == null ? 0 : int.Parse(DaysEntry.Text);
            await Zalesak.Actions.AddNewActionAsync(name, type, datePicker.Date, datePicker.Date, 0, true);
            await Navigation.PopAsync();
        }
    }
}