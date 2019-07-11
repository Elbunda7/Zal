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
	public partial class SingleGamePage : ContentPage
	{
        private MultiGame multiGame;
        private Game game;

        public SingleGamePage(MultiGame multiGame) : this()
        {
            this.multiGame = multiGame;
            Title = multiGame.Name;
        }

        public SingleGamePage(Game game):this()
        {
            this.game = game;
            Title = game.Name;
        }

        public SingleGamePage ()
		{
			InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Synchronize();
        }

        private async void Synchronize()
        {
            if (game == null) game = (await multiGame.GamesLazyLoad()).First();
            MyListView.ItemsSource = game.Scores;
        }

        private void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
    }
}