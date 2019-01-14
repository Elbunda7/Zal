using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;

namespace Zal.Views.Pages.Actions
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetailPage : ContentPage
	{
        private ActionEvent action;

        public DetailPage()
        {
            InitializeComponent();
        }

        public DetailPage(ActionEvent action)
        {
            this.action = action;
            InitializeComponent();
            BindingContext = action;
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

        private async void NotJoinButton_ClickedAsync()
        {
            await action.Join(ZAL.Joining.False);
        }

        private async void MaybeJoinButton_ClickedAsync()
        {
            await action.Join(ZAL.Joining.Maybe);
        }

        private async void JoinButton_ClickedAsync()
        {
            bool isSuccess = await action.Join(ZAL.Joining.True);
        }
    }
}