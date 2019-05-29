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
using Zal.Domain.Consts;
using Zal.Domain.Models;
using Zal.ViewModels;

namespace Zal.Views.Pages.Actions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MembersOnActionPage : ContentPage
    {
        private ActionEvent action;
        private List<MembersListModel> groupedMembersList;

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
            var toolbarEditOne = new ToolbarItem()
            {
                Text = "Upravit seznam zúčastněných",
                Order = ToolbarItemOrder.Secondary
            };
            toolbarEditOne.Clicked += SelectMembers_ToolbarItemClicked;
            ToolbarItems.Add(toolbarEditOne);

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
            var allMembers = await action.MembersLazyLoad();
            var membersTrue = allMembers.Where(x => x.Joining == ZAL.Joining.True).Select(x => x.Member);
            var membersMaybe = allMembers.Where(x => x.Joining == ZAL.Joining.Maybe).Select(x => x.Member);
            var membersFalse = allMembers.Where(x => x.Joining == ZAL.Joining.False).Select(x => x.Member);
            var leadersTrue = membersTrue.Where(x => x.Rank >= ZAL.Rank.Vedouci);
            membersTrue = membersTrue.Where(x => x.Rank < ZAL.Rank.Vedouci);

            groupedMembersList = new List<MembersListModel>();
            if (leadersTrue.Count() > 0) groupedMembersList.Add(new MembersListModel(leadersTrue, "Vedoucí"));
            if (membersTrue.Count() > 0) groupedMembersList.Add(new MembersListModel(membersTrue, "Členové"));
            if (membersMaybe.Count() > 0) groupedMembersList.Add(new MembersListModel(membersMaybe, "Možná pojedou"));
            if (membersFalse.Count() > 0) groupedMembersList.Add(new MembersListModel(membersFalse, "Nepojedou"));

            MyListView.ItemsSource = groupedMembersList;
        }

        private async void SelectMembers_ToolbarItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MembersMainPage(action.RawMembers(), OnSelectionDone));
        }

        private async void OnSelectionDone(List<int> selections)
        {
            var joiningList = new List<UserJoiningAction>();
            var currentMembers = action.RawMembers();
            var unselected = currentMembers.Except(selections);
            var selected = selections.Except(currentMembers).ToList();

            if (selected.Count != 0 || unselected.Count() != 0)
            {
                await action.Join(selected, unselected);
                await action.MembersLazyLoad(reload: true);
            }
            Synchronize();
        }

        private async void FilterTwo_ToolbarItemClicked(object sender, EventArgs e)
        {
            var a = UserFilterModel.Default;
            a.Groups = ZAL.Group.CompletlyAll;
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