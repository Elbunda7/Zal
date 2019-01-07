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
	public partial class ReportCreatorPage : ContentPage
	{
        ActionEvent action;

        public ReportCreatorPage(ActionEvent action)
        {
            this.action = action;
            InitializeComponent();
            Title = action.Name + " - zápis z akce";
        }

        private async void AddButton_Clicked(object sender, EventArgs args)
        {
            Analytics.TrackEvent("ReportCreator_add", new Dictionary<string, string>() { { "title", Title } });
            await action.AddNewReportAsync(Title, textEditor.Text);
            await Navigation.PopAsync();
        }
    }
}