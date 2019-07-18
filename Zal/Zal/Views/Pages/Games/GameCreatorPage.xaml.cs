using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;
using Zal.Domain.Models;
using Zal.ViewModels;

namespace Zal.Views.Pages.Games
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GameCreatorPage : ContentPage
	{
        private GameCreatorVisibilitySettings creatorSettings;
        private GameCollection gameCollection;
        private ActionEvent action;

        List<MembersListModel> groupedCategoryList;
        private List<GameStruct> games = new List<GameStruct>();
        private int multiGameRowCounter = 0;

        private bool isSortingDown = true;

        public GameCreatorPage (ActionEvent action, bool makeGame) :this()
		{
            this.action = action;
            creatorSettings = new GameCreatorVisibilitySettings { DoCollection = true, DoGame = makeGame };
            AddGameEntries();
            AddGameEntries();
            BindingContext = creatorSettings;
            SetCategoryList();
        }

        public GameCreatorPage(GameCollection coll) : this()
        {
            gameCollection = coll;
            creatorSettings = new GameCreatorVisibilitySettings { DoCollection = false, DoGame = true };
            AddGameEntries();
            AddGameEntries();
            BindingContext = creatorSettings;
        }

        public GameCreatorPage()
        {
            InitializeComponent();
        }

        private async void ConfirmationButton_Click(object sender, EventArgs e)
        {
            if (creatorSettings.DoCollection)
            {
                gameCollection = await action.AddNewGameCollection(GameCollNameEntry.Text, false, IsIndividualSwitch.IsToggled);//todo dopracovat ať funguje
            }
            if (creatorSettings.DoGame)
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
                    var multiGame = await gameCollection.AddMultiGame(GameNameEntry.Text, models);
                    await Navigation.PushAsync(new GamePage(multiGame));
                }
                else
                {
                    var multiGame = await gameCollection.AddMultiGame(GameNameEntry.Text, new GameBaseModel
                    {
                        Name = GameNameEntry.Text,
                        RatingStyle = isSortingDown,
                        Variable = UnitsEntry.Text,
                    });
                    await Navigation.PushAsync(new SingleGamePage(multiGame));
                }
            }
            else
            {
                await Navigation.PushAsync(new GameCollectionsPage(action));
            }
            Navigation.RemovePage(this);
        }

        private async void SetCategoryList()
        {
            var allMembers = await action.MembersLazyLoad();
            var members = allMembers.Where(x => x.Joining == ZAL.Joining.True).Select(x => x.Member);
            var leaders = members.Where(x => x.Rank >= ZAL.Rank.Vedouci);
            members = members.Where(x => x.Rank < ZAL.Rank.Vedouci);

            groupedCategoryList = new List<MembersListModel>();
            groupedCategoryList.Add(new MembersListModel(members, "Kategorie 1"));
            groupedCategoryList.Add(new MembersListModel(leaders, "Nehrající"));

            CategoryList.ItemsSource = groupedCategoryList; //todo https://github.com/daniel-luberda/DLToolkit.Forms.Controls/tree/master/FlowListView
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

        private async void CategoryItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            var group = e.Group as MembersListModel;
            var item = e.Item as User;

            var categories = groupedCategoryList.Select(x => x.GroupTitle).ToArray();
            string action = await DisplayActionSheet(item.NickName + " - Zařadit do kategorie", "Cancel", null, categories);

            if (action != "Cancel")
            {
                group.Remove(item);
                groupedCategoryList.Single(x=>x.GroupTitle == action).Add(item);
            }
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