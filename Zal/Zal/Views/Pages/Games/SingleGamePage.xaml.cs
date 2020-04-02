using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain.ActiveRecords;
using Zal.ViewModels;

namespace Zal.Views.Pages.Games
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingleGamePage : ContentPage
	{
        private GameCollection gameColl;
        private MultiGame multiGame;
        private Game game;

        private Score selectedScore;

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
            Title = $"{game.Name} [{game.Variable}]";
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
            if (game == null)
            {
                game = (await multiGame.GamesLazyLoad()).First();
                Title = $"{multiGame.Name} [{game.Variable}]";
            }
            UserList.ItemsSource = game.GetCategorizedScores(gameColl.Categories);
        }

        private async void UserItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            selectedScore = e.Item as Score;
            await PopupNavigation.Instance.PushAsync(new EntryPopup(selectedScore.NickName + " - upravit výsledek", SaveNewValue, selectedScore.Value.ToString(), selectedScore.Value.ToString(), Keyboard.Numeric));
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            var entry = sender as Entry;            
            selectedScore = (entry.Parent.Parent as ViewCell).BindingContext as Score;
            SaveNewValue(entry.Text);            
        }

        private async void SaveNewValue(string txt)
        {
            if (double.TryParse(txt, out double value))
            {
                selectedScore.UnitOfWork.ToUpdate.Value = value;
                bool isSuccess = await selectedScore.UnitOfWork.CommitAsync();
            }
        }
    }
}