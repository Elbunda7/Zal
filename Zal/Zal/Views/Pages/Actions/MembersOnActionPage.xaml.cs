using Microsoft.AppCenter.Analytics;
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

namespace Zal.Views.Pages.Actions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MembersOnActionPage : ContentPage
    {
        private ActionEvent action;


        public MembersOnActionPage(ActionEvent action) : this()
        {
            this.action = action;
        }

        public MembersOnActionPage()
        {
            InitializeComponent();
            Analytics.TrackEvent("MembersOnActionPage");
            Title = "Členové";

            //dev
            var toolbarFilterOne = new ToolbarItem()
            {
                Text = "Oddíloví",
                Order = ToolbarItemOrder.Secondary
            };
            toolbarFilterOne.Clicked += FilterOne_ToolbarItemClicked;
            ToolbarItems.Add(toolbarFilterOne);

            var toolbarFilterTwo = new ToolbarItem()
            {
                Text = "Všichni",
                Order = ToolbarItemOrder.Secondary
            };
            toolbarFilterTwo.Clicked += FilterTwo_ToolbarItemClicked;
            ToolbarItems.Add(toolbarFilterTwo);
            //dev
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Synchronize();
        }

        private async void Synchronize()
        {
            MyListView.ItemsSource = await action.Members();
        }

        private async void FilterOne_ToolbarItemClicked(object sender, EventArgs e)
        {
            await Zalesak.Users.Synchronize();
        }

        private async void FilterTwo_ToolbarItemClicked(object sender, EventArgs e)
        {
            var a = UserFilterModel.Default;
            a.Groups = Domain.Consts.ZAL.Group.CompletlyAll;
            await Zalesak.Users.Synchronize(a);
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