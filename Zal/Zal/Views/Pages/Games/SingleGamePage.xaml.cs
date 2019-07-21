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
        private GameCollection gameColl;
        private MultiGame multiGame;
        private Game game;

        public SingleGamePage(GameCollection gameColl, MultiGame multiGame) : this()
        {
            this.gameColl = gameColl;
            this.multiGame = multiGame;
            Title = multiGame.Name;
        }

        public SingleGamePage(GameCollection gameColl, Game game):this()
        {
            this.gameColl = gameColl;
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
            //UserList.ItemsSource = game.Scores;
            UserList.ItemsSource = game.GetCategorizedScores(gameColl.Categories);
        }

        private void UserItem_Tapped(object sender, ItemTappedEventArgs e)
        {

        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            double value;
            bool isNumber = double.TryParse(entry.Text, out value);
            if (isNumber)
            {
                var cell = (entry.Parent.Parent as ViewCell);
                var score = cell.BindingContext as Score;
                score.UnitOfWork.ToUpdate.Value = entry.Text;
                bool isSuccess = await score.UnitOfWork.CommitAsync();
            }
            else
            {
                entry.Text = "";
            }
        }
    }
}