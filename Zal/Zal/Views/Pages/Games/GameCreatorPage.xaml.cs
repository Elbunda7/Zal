using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Models;
using Zal.ViewModels;

namespace Zal.Views.Pages.Games
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GameCreatorPage : ContentPage
	{
        private GameCreatorVisibilitySettings visibilitySettings;
        private ActionEvent action;

        private List<GameStruct> games = new List<GameStruct>();
        private int multiGameRowCounter = 0;

        private bool isSortingDown = true;

        public GameCreatorPage (ActionEvent action):this()
		{
            this.action = action;
            visibilitySettings = new GameCreatorVisibilitySettings { DoCollection = true, DoGame = true };
            BindingContext = visibilitySettings;
        }

        public GameCreatorPage()
        {
            InitializeComponent();
        }

        private async void ConfirmationButton_Click(object sender, EventArgs e)
        {
            GameCollection gameCollection = await action.AddNewGameCollection(GameCollNameEntry.Text, false, IsIndividualSwitch.IsToggled);//todo dopracovat ať funguje
            if (visibilitySettings.DoGame)
            {
                if (IsMultiGameSwitch.IsToggled)
                {
                    GameBaseModel[] models = new GameBaseModel[multiGameRowCounter];
                    for (int i = 0; i < multiGameRowCounter; i++)
                    {
                        models[i] = new GameBaseModel
                        {
                            Name = games[i].Name,
                            RatingStyle = games[i].IsSortingDown,
                            Variable = games[i].Variable,
                        };
                    }
                    await gameCollection.AddMultiGame(GameNameEntry.Text, models);
                }
                else
                {
                    await gameCollection.AddMultiGame(GameNameEntry.Text, new GameBaseModel
                    {
                        Name = GameNameEntry.Text,
                        RatingStyle = isSortingDown,
                        Variable = UnitsEntry.Text,
                    });
                }
            }

        }

        private void AddGameButton_Clicked(object sender, EventArgs e)
        {
            AddGameEntries();
        }

        private void AddGameEntries()
        {
            int i = multiGameRowCounter++;
            int totalRows = MultiGamesGrid.RowDefinitions.Count;
            if (multiGameRowCounter < totalRows)
            {
                games[i].SetVisibility(true);
            }
            else
            {
                games.Add(new GameStruct());
                MultiGamesGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                games[i].MapToGrid(MultiGamesGrid, totalRows);
            }
        }

        private void HideGameEntries()
        {
            if (multiGameRowCounter != 0)
            {                
                int i = --multiGameRowCounter;
                games[i].SetVisibility(false);
            }
        }

        private void HideGameButton_Clicked(object sender, EventArgs e)
        {
            HideGameEntries();
        }

        private void OnSortImg_Tapped(object sender, EventArgs e)
        {
            if (isSortingDown) SortImg.Source = "ic_sort_up_24dp.png";
            else SortImg.Source = "ic_sort_down_24dp.png";
            isSortingDown = !isSortingDown;
        }
    }

    public class GameCreatorVisibilitySettings : INotifyPropertyChanged
    {
        private bool doCollection;
        public bool DoCollection {
            get {
                return doCollection;
            }
            set {
                if (doCollection != value)
                {
                    doCollection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoCollection)));
                }
            }
        }

        private bool doGame;
        public bool DoGame {
            get {
                return doGame;
            }
            set {
                if (doGame != value)
                {
                    doGame = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoGame)));
                }
            }
        }    

        public event PropertyChangedEventHandler PropertyChanged;
    }
}