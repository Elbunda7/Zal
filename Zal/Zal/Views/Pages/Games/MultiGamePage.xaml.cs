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
        private GameCollection gameColl;

        public MultiGamePage(GameCollection gameColl):this()
        {
            this.gameColl = gameColl;
            Title = gameColl.Name;
            var toolbarItem = new ToolbarItem()
            {
                Text = "Nová hra",
                Order = ToolbarItemOrder.Secondary
            };
            toolbarItem.Clicked += NewGame_ToolbarItemClicked;
            ToolbarItems.Add(toolbarItem);
        }

        public MultiGamePage ()
		{
			InitializeComponent ();
        }

        private async void NewGame_ToolbarItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GameCreatorPage(gameColl));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MyListView.ItemsSource = null;
            MyListView.ItemsSource = gameColl.GameList;
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var multiGame = e.Item as MultiGame;
            if (multiGame.HasMultipleParts) await Navigation.PushAsync(new GamePage(multiGame));
            else if (multiGame.HasOnePart) await Navigation.PushAsync(new SingleGamePage(multiGame)); 
        }
    }
}