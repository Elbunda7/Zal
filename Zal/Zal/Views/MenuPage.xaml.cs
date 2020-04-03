using Zal.Models;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Elements;
using Zal.Views.Pages;
using Zal.Domain;
using Zal.Domain.ActiveRecords;
using Zal.Services;
using Zal.Views.Pages.Galleries;

namespace Zal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        HomeMenuItem SelectedItem;
        List<HomeMenuItem> menuItems;
        HomeMenuItem LoginItem;
        HomeMenuItem ProfileItem;

        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        public MenuPage()
        {
            InitializeComponent();

            LoginItem = new HomeMenuItem("Přihlásit se", typeof(Pages.Users.LoginPage));
            ProfileItem = new HomeMenuItem("Profil", typeof(Pages.Users.ProfilePage));
            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem("Aktuality", typeof(ActualityMainPage), "ic_explore_black_24dp.png"),
                new HomeMenuItem("Plán akcí", typeof(ActionMainPage), "ic_event_black_24dp.png"),
                new HomeMenuItem("Členové", typeof(MembersMainPage), "ic_people_black_24dp.png"),
                new HomeMenuItem("Galerie", typeof(GalleryPage), "ic_photo_library_black_24dp.png"),
                new HomeMenuItem("Studnice vědění", typeof(InfoMainPage), "ic_apps_black_24dp.png"),
            };
            ListViewMenu.SelectionMode = ListViewSelectionMode.None;
            ListViewMenu.ItemsSource = menuItems;
            ListViewMenu.ItemTapped += ListViewMenu_ItemTapped;

            SelectedItem = menuItems[0];

            Zalesak.Session.UserStateChanged += OnUserStateChanged;
            OnUserStateChanged(Zalesak.Session);
        }

        private void OnUserStateChanged(Session session)
        {
            if (session.IsUserLogged)
            {
                ImageSource imageSource = ImageSourceHelper.UserImg(session.CurrentUser.ImageInfo, NamedSize.Medium);
                NameLabel.Text = session.CurrentUser.NickName;
                ProfileImage.Source = imageSource;
                LoginButton.Text = "Odhlásit se";
            }
            else
            {
                NameLabel.Text = "TOM Zálesák";
                ProfileImage.Source = "icon.png";
                LoginButton.Text = "Přihlásit se";
            }
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
            if (item != SelectedItem || !SelectedItem.IsSelected)
            {
                SetItemAsSelected(item);
            }
            else
            {
                RootPage.HideMenu();
            }
        }

        private void ProfileImage_Tapped(object sender, EventArgs e)
        {
            if (Zalesak.Session.IsUserLogged)
            {
                SetItemAsSelected(ProfileItem);
            }
            else
            {
                SetItemAsSelected(LoginItem);
            }
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            if (Zalesak.Session.IsUserLogged)
            {
                await Zalesak.Session.Logout();
            }
            else
            {
                SetItemAsSelected(LoginItem);
            }
        }
    }
}