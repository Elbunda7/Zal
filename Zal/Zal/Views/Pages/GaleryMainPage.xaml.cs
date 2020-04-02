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
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GaleryMainPage : ContentPage
	{
        private int numOfColumns = 3;
        private double itemSize = 0;
        private double pageWidthSize = 0;

		public GaleryMainPage ()
		{
			InitializeComponent ();
            Title = "Galerie";
            Analytics.TrackEvent("GaleryMainPage");
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Synchronize();
        }

        private async void Synchronize()
        {
            await Zalesak.Galleries.ReSynchronize();
            InitGrid();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (pageWidthSize != width)
            {
                pageWidthSize = width;
                OnOrientationChanged(width, height);
            }
        }

        private void OnOrientationChanged(double width, double height)
        {
            if (width > height)
            {
                numOfColumns = 5;
            }
            else
            {
                numOfColumns = 3;
            }
            double spaces = ContentGrid.ColumnSpacing;
            double newItemSize = (width - spaces * (numOfColumns + 1)) / numOfColumns;
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
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = itemSize });
                for (int i = 0; i < numOfColumns; i++)
                {
                    if (index >= Zalesak.Galleries.Data.Count) break;
                    var gallery = Zalesak.Galleries.Data.ElementAt(index);
                    string imgPath = "http://zalesak.hlucin.com/galerie/albums/" + gallery.File + "small/" + gallery.MainImg;
                    Image img = new Image()
                    {
                        Source = imgPath,
                        ClassId = index.ToString(),
                    };
                    ContentGrid.Children.Add(img, i, j);
                    var onClick = new TapGestureRecognizer();
                    onClick.Tapped += TapGestureRecognizer_Tapped;
                    onClick.CommandParameter = gallery;
                    img.GestureRecognizers.Add(onClick);
                    index++;
                }
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Gallery gallery = (e as TappedEventArgs).Parameter as Gallery;
            await Navigation.PushAsync(new GalleryPage(gallery));
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
                    mainImage.Source = b;
                    mediaFile[0].Dispose();
                }
                
                
            }
        }
    }
}