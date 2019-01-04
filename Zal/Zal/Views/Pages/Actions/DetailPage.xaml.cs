using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain.ActiveRecords;

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

        private async void JoinButton_ClickedAsync(object sender, EventArgs args)
        {
            await action.Join();
        }

        private async void ParticipateButton_ClickedAsync(object sender, EventArgs args)
        {
            bool result = await action.Join();
        }

        private async void ClickableImageLabel_OnClickAsync()
        {
            await Navigation.PushAsync(new WebViewPage(await action.ReportLazyLoad()));
        }
    }
}