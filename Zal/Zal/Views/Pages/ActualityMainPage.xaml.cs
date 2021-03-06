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
using Zal.Domain.ItemSets;

namespace Zal.Views.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActualityMainPage : ContentPage
	{
        public ActualityMainPage()
        {
            InitializeComponent();
            Analytics.TrackEvent("ActualityMainPage");
            Title = "Novinky";
            MyListView.ItemsSource = Zalesak.Actualities.Data;
            MyListView.SelectionMode = ListViewSelectionMode.None;
            activityIndicator.SetBinding(IsVisibleProperty, nameof(BaseSet.IsBusy));
            activityIndicator.BindingContext = Zalesak.Actualities;
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
            await Zalesak.Actualities.Synchronize();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Analytics.TrackEvent("ActualityMainPage_loadNext");
            await Zalesak.Actualities.LoadNext();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Article)
            {
                Article item = e.Item as Article;
                Analytics.TrackEvent("ActualityMainPage_showArticle", new Dictionary<string, string>() { { "toShow", item.Id + ". " + item.Title } });
                await Navigation.PushAsync(new WebViewPage(item));
                (sender as ListView).SelectedItem = null;
            }
        }

        private async void AddButton_Clicked(object sender, EventArgs args)
        {
            Analytics.TrackEvent("ActualityMainPage_createArticle");
            await Navigation.PushAsync(new Actualities.ArticleCreatorPage());
        }
    }
}