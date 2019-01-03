using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;

namespace Zal.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainActualityPage : ContentPage
	{
        private new bool IsBusy {
            get {
                return activityIndicator.IsVisible;
            }
            set {
                activityIndicator.IsVisible = value;
            }
        }

        public MainActualityPage()
        {
            InitializeComponent();
            Title = "Novinky";
            MyListView.ItemsSource = Zalesak.Actualities.Data;
            //todo setbinding for isBusy https://docs.microsoft.com/cs-cz/dotnet/api/xamarin.forms.bindableobjectextensions.setbinding?view=xamarin-forms
            Button button = new Button()
            {
                Text = "Load next",
            };
            button.Clicked += Button_Clicked;
            MyListView.Footer = button;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Synchronize();
        }

        private async void Synchronize()
        {
            IsBusy = true;
            await Zalesak.Actualities.Synchronize();
            IsBusy = false;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;
            await Zalesak.Actualities.LoadNext();
            IsBusy = false;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Article)
            {
                Article item = e.Item as Article;
                await Navigation.PushAsync(new WebViewPage(item));
                (sender as ListView).SelectedItem = null;
            }
        }

        private async void AddButton_Clicked(object sender, EventArgs args)
        {
            //await Navigation.PushAsync(new ArticleCreator());
        }
    }
}