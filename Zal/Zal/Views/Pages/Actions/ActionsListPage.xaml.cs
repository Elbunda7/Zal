using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;

namespace Zal.Views.Pages.Actions
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActionsListPage : ContentPage
	{
		public ActionsListPage (int? year = null)
        {
            InitializeComponent();
            StartInitializingItems(year);
        }

        private async void StartInitializingItems(int? year)
        {
            if (year.HasValue)
            {
                Title = year.Value.ToString();
                MyListView.ItemsSource = await Zalesak.Actions.GetPassedActionEventsByYear(year.Value);
            }
            else
            {
                Title = "Nadcházející";
                MyListView.ItemsSource = Zalesak.Actions.UpcomingActionEvents;
            }
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is ActionEvent)
            {
                ActionEvent currentEvent = e.Item as ActionEvent;
                await Navigation.PushAsync(new Actions.DetailPage(currentEvent));
                (sender as ListView).SelectedItem = null;
            }
        }
    }
}