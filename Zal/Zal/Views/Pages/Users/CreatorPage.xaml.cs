using Microsoft.AppCenter.Analytics;
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

        public CreatorPage() : this(ZAL.Group.Non) { }

        public CreatorPage(ZAL.Group group)
        {
            InitializeComponent();
            Title = "Nový člen";
            GroupPicker.ItemsSource = ZAL.GROUP_NAME_SINGULAR;
            GroupPicker.SelectedIndex = (int)group / 2;
        }

        private async void AddButton_Clicked(object sender, EventArgs args)
        {
            string name = NameEntry.Text;
            string surname = SurnameEntry.Text;
            int group = (GroupPicker.SelectedIndex + 1) * 2;
            Analytics.TrackEvent("UserCreator_addUser", new Dictionary<string, string>() { { "toAdd", name + " " + surname } });
            await Zalesak.Users.AddNewUser(name, surname, group);
            await Navigation.PopAsync();
        }

        private void Arrow_OnTapped(object sender, EventArgs e) => GroupPicker.Focus();

        private void Entry_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AddButton.IsEnabled = !string.IsNullOrWhiteSpace(NameEntry.Text) && !string.IsNullOrWhiteSpace(SurnameEntry.Text);
        }
    }
}