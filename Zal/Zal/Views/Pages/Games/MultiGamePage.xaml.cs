using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;

namespace Zal.Views.Pages.Games
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MultiGamePage : ContentPage
	{
        private List<MultiGame> gameList;

        public MultiGamePage ()
		{
			InitializeComponent ();
        }

        public MultiGamePage(List<MultiGame> gameList):this()
        {
            this.gameList = gameList;
            MyListView.ItemsSource = gameList;
        }

        /*protected override void OnAppearing()
        {
            base.OnAppearing();
            Synchronize();
        }

        private async void Synchronize()
        {
            MyListView.ItemsSource = await Zalesak.Games.GetGameCollection();
        }*/

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new GamePage());
        }
    }
}