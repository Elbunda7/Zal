using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain.ActiveRecords;

namespace Zal.Views.Pages.Actualities
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InfoCreatorPage : ContentPage
	{
        ActionEvent action;

        public InfoCreatorPage()
        {
            InitializeComponent();
        }

        public InfoCreatorPage(ActionEvent action)
        {
            this.action = action;
            InitializeComponent();
            Title = action.Name + " - informačka";
        }

        private async void AddButton_Clicked(object sender, EventArgs args)
        {
            Analytics.TrackEvent("InfoCreator_add", new Dictionary<string, string>() { { "title", Title } });
            await action.AddNewInfoAsync(Title, textEditor.Text);
            await Navigation.PopAsync();
        }
    }
}