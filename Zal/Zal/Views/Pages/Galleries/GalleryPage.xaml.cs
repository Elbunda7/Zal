using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;

namespace Zal.Views.Pages.Galleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GalleryPage : ContentPage
	{
        private int numOfColumns = 3;
        private double itemSize = 0;
        private double pageWidthSize = 0;
        private Gallery gallery;

        public GalleryPage ()
		{
			InitializeComponent ();
		}

        public GalleryPage(Gallery gallery)
        {
            InitializeComponent();
            Title = gallery.Name;
            this.gallery = gallery;
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

        private async void InitGrid()
        {
            ContentGrid.RowDefinitions = new RowDefinitionCollection();
            ContentGrid.ColumnDefinitions = new ColumnDefinitionCollection();
            ContentGrid.Children.Clear();
            for (int i = 0; i < numOfColumns; i++)
            {
                ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            }
            int index = 0;
            var images = await gallery.ImagesLazyLoad();
            int numOfRows = (images.Count() + numOfColumns - 1) / numOfColumns;
            for (int j = 0; j < numOfRows; j++)
            {
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = itemSize });
                for (int i = 0; i < numOfColumns; i++)
                {
                    if (index >= images.Count()) break;
                    string imgPath = "http://zalesak.hlucin.com/" + gallery.File + "small/" + images.ElementAt(index);
                    Image img = new Image()
                    {
                        Source = imgPath,
                        ClassId = index.ToString(),
                    };
                    ContentGrid.Children.Add(img, i, j);
                    var onClick = new TapGestureRecognizer();
                    onClick.Tapped += TapGestureRecognizer_Tapped;
                    onClick.CommandParameter = images.ElementAt(index);
                    img.GestureRecognizers.Add(onClick);
                    index++;
                }
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            string image = (e as TappedEventArgs).Parameter as string;
        }

    }
}