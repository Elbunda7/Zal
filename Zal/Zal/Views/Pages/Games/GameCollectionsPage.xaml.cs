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
            Title = "Kolekce her";
            var toolbarItem = new ToolbarItem()
            {
                Text = "Nová kolekce",
                Order = ToolbarItemOrder.Secondary
            };
            toolbarItem.Clicked += NewCollection_ToolbarItemClicked;
            ToolbarItems.Add(toolbarItem);
        }

        private async void NewCollection_ToolbarItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GameCreatorPage(action, makeGame: false));
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
            await Navigation.PushAsync(new MultiGamePage(item));
        }

        private async Task NavigateToLowerLevels(GameCollection gameColl)
        {
            if (gameColl.HasManyGames) await Navigation.PushAsync(new MultiGamePage(gameColl));
            else if (gameColl.HasOneMultiGame) await Navigation.PushAsync(new GamePage(gameColl, gameColl.GameList.First()));
            else if (gameColl.HasOneSimpleGame) await Navigation.PushAsync(new SingleGamePage(gameColl, gameColl.GameList.First()));
            //todo jedna kolekce ale nic dalšího
        }
    }
}