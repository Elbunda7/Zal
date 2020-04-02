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
	public partial class GamesPage : ContentPage
	{
        private GameCollection gameColl;
        private MultiGame multiGame;

        public GamesPage(GameCollection gameColl, MultiGame multiGame):this()
        {
            this.gameColl = gameColl;
            this.multiGame = multiGame;
            Title = multiGame.Name;
        }

        public GamesPage ()
		{
			InitializeComponent ();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Synchronize();
        }

        private async void Synchronize()
        {
            MyListView.ItemsSource = await multiGame.GamesLazyLoad();
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new SingleGamePage(gameColl, e.Item as Game));
        }
    }
}