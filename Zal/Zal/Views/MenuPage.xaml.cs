using Zal.Models;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Elements;
using Zal.Views.Pages;

namespace Zal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        HomeMenuItem SelectedItem;


        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem("Aktuality", typeof(ActualityMainPage), "ic_explore_black_24dp.png"),
                new HomeMenuItem("Plán akcí", typeof(ActionMainPage), "ic_event_black_24dp.png"),
                new HomeMenuItem("Členové", typeof(MembersMainPage), "ic_people_black_24dp.png"),
                new HomeMenuItem("Galerie", typeof(GaleryMainPage), "ic_photo_library_black_24dp.png"),
                new HomeMenuItem("Studnice vědění", typeof(InfoMainPage), "ic_apps_black_24dp.png"),
            };
            ListViewMenu.SelectionMode = ListViewSelectionMode.None;
            ListViewMenu.ItemsSource = menuItems;
            ListViewMenu.ItemTapped += ListViewMenu_ItemTapped;

            SelectedItem = menuItems[0];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetItemAsSelected(menuItems[0]);
        }

        private async void SetItemAsSelected(HomeMenuItem item)
        {
            SelectedItem.IsSelected = false;
            item.IsSelected = true;
            SelectedItem = item;
            await RootPage.NavigateFromMenu(item.TargetType);
        }

        private void ListViewMenu_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            HomeMenuItem item = e.Item as HomeMenuItem;//todo isBusy
            if (item == null) return;
            if (item != SelectedItem)
            {
                SetItemAsSelected(item);
            }
            else
            {
                RootPage.HideMenu();
            }
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {

        }

        private void LogoutButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}