using Microsoft.AppCenter.Analytics;
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
        int? year;

		public ActionsListPage (int? year = null)
        {
            InitializeComponent();
            this.year = year;
            Title = year.HasValue ? year.Value.ToString() : "Nadcházející";
            MyListView.SelectionMode = ListViewSelectionMode.None;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Analytics.TrackEvent("ArtionsListPage", new Dictionary<string, string>() { { "year", Title } });
            StartInitializingItems();
        }

        private async void StartInitializingItems()
        {
            if (year.HasValue)
            {
                MyListView.ItemsSource = await Zalesak.Actions.GetPassedActionEventsByYear(year.Value);
            }
            else
            {
                MyListView.ItemsSource = Zalesak.Actions.UpcomingActionEvents;
            }
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is ActionEvent)
            {
                ActionEvent currentEvent = e.Item as ActionEvent;
                Analytics.TrackEvent("ArticleCreator_show", new Dictionary<string, string>() { { "toShow", currentEvent.Id + ". " + currentEvent.Name } });
                await Navigation.PushAsync(new Actions.DetailPage(currentEvent));
                (sender as ListView).SelectedItem = null;
            }
        }
    }
}