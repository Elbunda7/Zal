﻿using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Models;

namespace Zal.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MembersMainPage : ContentPage
    {
        public MembersMainPage()
        {
            InitializeComponent();
            Analytics.TrackEvent("MembersMainPage");
            Title = "Členové";
            MyListView.ItemsSource = Zalesak.Users.Users.Where(x => x.Meets(UserFilterModel.Default));

            var toolbarItem = new ToolbarItem()
            {
                Text = "Nový",
                Order = ToolbarItemOrder.Secondary
            };
            toolbarItem.Clicked += NewUser_ToolbarItemClicked;
            ToolbarItems.Add(toolbarItem);
        }

        private async void NewUser_ToolbarItemClicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent("MembersMainPage_newUser");
            await Navigation.PushAsync(new Users.CreatorPage());
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is User)
            {
                User currentUser = e.Item as User;
                Analytics.TrackEvent("MembersMainPage_showUser", new Dictionary<string, string>() { { "toShow", currentUser.NickName } });
                await Navigation.PushAsync(new Users.ProfilePage(currentUser));
                (sender as ListView).SelectedItem = null;
            }
        }
    }
}