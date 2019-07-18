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
using Zal.Views.Pages.Games;

namespace Zal.Views.Pages.Actions
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetailPage : ContentPage
	{
        private ActionEvent action;

        public DetailPage(ActionEvent action):this()
        {
            this.action = action;
            BindingContext = action;
            ParticipateCrossroadView.IsVisible = Zalesak.Session.IsUserLogged && action.DoIParticipate == ZAL.Joining.Unknow;
            ParticipateView.IsVisible = Zalesak.Session.IsUserLogged && action.DoIParticipate != ZAL.Joining.Unknow;
            var toolbarItem = new ToolbarItem()
            {
                Text = "Nový",
                Order = ToolbarItemOrder.Secondary
            };
            toolbarItem.Clicked += NewGame_ToolbarItemClicked;
            ToolbarItems.Add(toolbarItem);
        }

        private async void NewGame_ToolbarItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GameCreatorPage(action, makeGame:true));
        }

        public DetailPage()
        {
            InitializeComponent();
        }

        private async void InfoButton_ClickedAsync(object sender, EventArgs args)
        {
            if (action.HasInfo)
            {
                await Navigation.PushAsync(new WebViewPage(await action.InfoLazyLoad()));
            }
            else
            {
                await Navigation.PushAsync(new Actualities.InfoCreatorPage(action));
            }
        }

        private async void RecordButton_ClickedAsync(object sender, EventArgs args)
        {
            if (action.HasReport)
            {
                await Navigation.PushAsync(new WebViewPage(await action.ReportLazyLoad()));
            }
            else
            {
                await Navigation.PushAsync(new Actualities.ReportCreatorPage(action));
            }
        }

        private async void DontJoinButton_Clicked()
        {
            await action.Join(ZAL.Joining.False);
            ParticipateCrossroadView.IsVisible = false;
            ParticipateView.IsVisible = true;
        }

        private async void MaybeJoinButton_ClickedAsync()
        {
            await action.Join(ZAL.Joining.Maybe);
            ParticipateCrossroadView.IsVisible = false;
            ParticipateView.IsVisible = true;
        }

        private async void JoinButton_ClickedAsync()
        {
            bool isSuccess = await action.Join(ZAL.Joining.True);
            ParticipateCrossroadView.IsVisible = false;
            ParticipateView.IsVisible = true;
        }

        private async void GalleryButton_ClickedAsync(object sender, EventArgs e)
        {

        }

        private async void OnMembers_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MembersOnActionPage(action));
        }

        private void ParticipationButton_Clicked(object sender, EventArgs e)
        {
            ParticipateCrossroadView.IsVisible = true;
        }

        private async void GameButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GameCollectionsPage(action));
        }
    }
}