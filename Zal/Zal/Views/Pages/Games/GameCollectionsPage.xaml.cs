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
	public partial class GameCollectionsPage : ContentPage
	{
        private ActionEvent action;

        public GameCollectionsPage(ActionEvent action):this()
        {
            this.action = action;
        }

        public GameCollectionsPage ()
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
            var gameCollections = await action.GamesLazyLoad();
            if (gameCollections.Count() == 1)
            {
                await NavigateToLowerLevels(gameCollections.First());
                Navigation.RemovePage(this);
            }
            else
            {
                MyListView.ItemsSource = gameCollections;
            }
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as GameCollection;
            await NavigateToLowerLevels(item);
        }

        private async Task NavigateToLowerLevels(GameCollection gameColl)
        {
            if (gameColl.HasManyGames) await Navigation.PushAsync(new MultiGamePage(gameColl.GameList));
            else if (gameColl.HasOneMultiGame) await Navigation.PushAsync(new GamePage(gameColl.GameList.First()));
            else if (gameColl.HasOneSimpleGame) await Navigation.PushAsync(new SingleGamePage(gameColl.GameList.First()));
        }
    }
}