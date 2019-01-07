using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;

namespace Zal.Views.Pages.Actualities
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ArticleCreatorPage : ContentPage
	{
		public ArticleCreatorPage ()
		{
			InitializeComponent ();
            Title = "Nový článek";
        }

        private async void AddButton_Clicked(object sender, EventArgs args)
        {
            Analytics.TrackEvent("ArticleCreator_add", new Dictionary<string, string>() { { "title", "title" } });
            await Zalesak.Actualities.AddNewArticle("ApplicationTitle", textEditor.Text, 0);
            await Navigation.PopAsync();
        }
    }
}