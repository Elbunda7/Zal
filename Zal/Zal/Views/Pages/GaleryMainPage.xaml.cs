using Microsoft.AppCenter.Analytics;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;
using Zal.Services;
using Zal.Views.Pages.Galleries;

namespace Zal.Views.Pages
{
    //todo deprecated smazat
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GaleryMainPage : ContentPage
	{
        const int thumbnailSize = 200;
        private int numOfColumns = 3;
        private double itemSize = 0;
        private bool wasDeviceVertically = true;
        private double maxImgSize = 0;
        private bool isLoaded = false;

		public GaleryMainPage ()
		{
			InitializeComponent ();
            Title = "Galerie";
            Analytics.TrackEvent("GaleryMainPage");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!isLoaded)
            {
                Synchronize();
            }
        }

        private async void Synchronize()
        {
            await Zalesak.Galleries.ReSynchronize();
            isLoaded = true;
            OnSizeAllocated(Width, Height);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (!isLoaded) return;
            bool isDeviceVertically = width < height;
            if (maxImgSize == 0)
            {
                wasDeviceVertically = !isDeviceVertically;
                maxImgSize = isDeviceVertically ? width / 3 : height / 3;
                if (maxImgSize > thumbnailSize) maxImgSize = thumbnailSize;
            }
            if (isDeviceVertically != wasDeviceVertically)
            {
                wasDeviceVertically = isDeviceVertically;
                OnOrientationChanged(width, height);
            }
        }

        private void OnOrientationChanged(double width, double height)
        {
            numOfColumns = (int)((width - 50) / maxImgSize + 1);
            double spacingSize = ContentGrid.ColumnSpacing * (numOfColumns - 1);
            double newItemSize = (width - spacingSize) / numOfColumns;
            if (itemSize != newItemSize)
            {
                itemSize = newItemSize;
                InitGrid();
            }
        }

        private void InitGrid()
        {
            ContentGrid.RowDefinitions = new RowDefinitionCollection();
            ContentGrid.ColumnDefinitions = new ColumnDefinitionCollection();
            ContentGrid.Children.Clear();
            for (int i = 0; i < numOfColumns; i++)
            {
                ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            }
            int index = 0;
            int numOfRows = (Zalesak.Galleries.Data.Count + numOfColumns - 1) / numOfColumns;
            for (int j = 0; j < numOfRows; j++)
            {
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = itemSize + 50 });
                for (int i = 0; i < numOfColumns; i++)
                {
                    if (index >= Zalesak.Galleries.Data.Count) break;
                    var gallery = Zalesak.Galleries.Data.ElementAt(index);
                    string imgPath = "http://zalesak.hlucin.com/galerie/albums/" + gallery.File + "small/" + gallery.MainImg;
                    StackLayout cell = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Padding = 0,
                    };
                    Image img = new Image()
                    {
                        Source = imgPath,
                        ClassId = index.ToString(),
                        HeightRequest = itemSize,
                    };
                    Label label = new Label()
                    {
                        Text = gallery.Name,
                        TextColor = (Color)Application.Current.Resources["PrimaryDark"],
                        HorizontalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(5, 0, 5, 5),
                        MaxLines = 2
                    };
                    cell.Children.Add(img);
                    cell.Children.Add(label);
                    ContentGrid.Children.Add(cell, i, j);
                    var onClick = new TapGestureRecognizer();
                    onClick.Tapped += TapGestureRecognizer_Tapped;
                    onClick.CommandParameter = gallery;
                    cell.GestureRecognizers.Add(onClick);
                    index++;
                }
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Gallery gallery = (e as TappedEventArgs).Parameter as Gallery;
            await Navigation.PushAsync(new GalleryPage(/*gallery*/));
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (await HavePermission.For<Permissions.StorageRead>())
            {
                var mediaFile = await CrossMedia.Current.PickPhotosAsync(new PickMediaOptions() { CompressionQuality = 92, });
                if (mediaFile != null)
                {
                    byte[] rawImage = File.ReadAllBytes(mediaFile[0].Path);
                    //var gallery = await Zalesak.Galleries.Add(galleryEntry.Text, DateTime.Now.Year, DateTime.Now);
                    var b = ImageSource.FromFile(mediaFile[0].Path);
                    mediaFile[0].Dispose();
                }
                
                
            }
        }
    }
}