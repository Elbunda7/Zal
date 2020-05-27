using Microsoft.AppCenter.Analytics;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;

namespace Zal.Views.Pages.Galleries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GalleryPage : ContentPage
	{
        public bool IsDeviceVertically { get; private set; } = true;

        const int thumbnailSize = 200;
        private double maxImgSize = 0;
        private int numOfColumns = 3;
        private double itemSize = 0;
        private Gallery gallery;
        private bool IsConcreteGallery => gallery != null;
        private bool isVerticalGridReady = false;
        private bool isHorizontalGridReady = false;
        private bool isLoaded = false;
        private int selectedYear = -1;

        public GalleryPage ()
		{
			InitializeComponent ();
            Title = "Galerie";
            var newGalleryToolbarItem = new ToolbarItem()
            {
                Text = "vytvořit novou galerii",
                Order = ToolbarItemOrder.Secondary
            };
            newGalleryToolbarItem.Clicked += AddGallery_ToolbarItemClicked;
            ToolbarItems.Add(newGalleryToolbarItem);
            Analytics.TrackEvent("GaleryPage-main");
		}

        private void YearToolbarItem_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public GalleryPage(Gallery gallery)
        {
            InitializeComponent();
            Title = gallery.Name;
            this.gallery = gallery;
            var toolbarItem = new ToolbarItem()
            {
                Text = "přidat fotky",
                Order = ToolbarItemOrder.Secondary
            };
            toolbarItem.Clicked += AddGallery_ToolbarItemClicked;
            ToolbarItems.Add(toolbarItem);
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
            if (IsConcreteGallery)
            {
                await gallery.ImagesLazyLoad();
            }
            else
            {
                await Zalesak.Galleries.ReSynchronize();
                int lastYear = Zalesak.Galleries.Data.Max(x => x.Year);
                selectedYear = lastYear;
                var yearToolbarItem = new ToolbarItem()
                {
                    Text = lastYear.ToString(),
                    Order = ToolbarItemOrder.Primary
                };
                yearToolbarItem.Clicked += YearToolbarItem_Clicked;
                ToolbarItems.Add(yearToolbarItem);

            }
            isLoaded = true;
            OnSizeAllocated(Width, Height);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (!isLoaded) return;
            bool isVertically = width < height;
            if (maxImgSize == 0)
            {
                IsDeviceVertically = !isVertically;
                maxImgSize = isVertically ? width / 3 : height / 3;
                if (maxImgSize > thumbnailSize) maxImgSize = thumbnailSize;
            }
            if (isVertically != IsDeviceVertically)
            {
                IsDeviceVertically = isVertically;
                OnOrientationChanged(width, height);
            }
        }

        private void OnOrientationChanged(double width, double height)
        {
            numOfColumns = (int)((width - 50) / maxImgSize + 1);
            double spacingSize = VerticalGrid.ColumnSpacing * (numOfColumns - 1);
            itemSize = (width - spacingSize) / numOfColumns;

            if (IsDeviceVertically)
            {
                if (!isVerticalGridReady)
                {
                    InitGrid();
                    isVerticalGridReady = true;
                }
                VerticalGrid.IsVisible = true;
                HorizontalGrid.IsVisible = false;
            }
            else
            {
                if (!isHorizontalGridReady)
                {
                    InitGrid();
                    isHorizontalGridReady = true;
                }
                VerticalGrid.IsVisible = false;
                HorizontalGrid.IsVisible = true;
            }
        }

        private async void InitGrid()//todo when uploadin deleting images -> redraw
        {
            Grid grid = IsDeviceVertically ? VerticalGrid : HorizontalGrid;
            grid.RowDefinitions = new RowDefinitionCollection();
            grid.ColumnDefinitions = new ColumnDefinitionCollection();
            grid.Children.Clear();
            for (int i = 0; i < numOfColumns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            }
            int index = 0;
            var images = IsConcreteGallery ? await gallery.ImagesLazyLoad() : Zalesak.Galleries.Data.Where(x=>x.Year == selectedYear).Select(x=>x.MainImg);
            double height = IsConcreteGallery ? itemSize : itemSize + 50;
            int imagesCount = images.Count();
            int numOfRows = (imagesCount + numOfColumns - 1) / numOfColumns;
            for (int j = 0; j < numOfRows; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = height });
                for (int i = 0; i < numOfColumns; i++)
                {
                    if (index >= imagesCount) break;
                    Gallery gal = IsConcreteGallery ? gallery : Zalesak.Galleries.Data.ElementAt(index);
                    string imgName = images.ElementAt(index);
                    string imgLink = "http://zalesak.hlucin.com/galerie/albums/" + gal.File;

                    View cell = IsConcreteGallery ? MakeCell_ConcreteGallery(imgLink, imgName) : MakeCell_Galleries(imgLink, imgName, gal);//todo save localy
                    grid.Children.Add(cell, i, j);
                    index++;
                }
            }
        }

        private View MakeCell_ConcreteGallery(string imgLink, string imgName)
        {
            Image img = new Image()
            {
                Source = imgLink + "small/" + imgName,
                ClassId = imgName,
            };
            var onClick = new TapGestureRecognizer();
            onClick.Tapped += OpenImage_Tapped;
            onClick.CommandParameter = imgLink + imgName;
            img.GestureRecognizers.Add(onClick);
            return img;
        }

        private View MakeCell_Galleries(string imgLink, string imgName, Gallery gal)
        {
            StackLayout cell = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 0,
            };
            Image img = new Image()
            {
                Source = imgLink + "small/" + imgName,
                ClassId = imgName,
                HeightRequest = itemSize,
            };
            Label label = new Label()
            {
                Text = gal.Name,
                TextColor = (Color)Application.Current.Resources["PrimaryDark"],
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(5, 0, 5, 5),
                MaxLines = 2
            };
            cell.Children.Add(img);
            cell.Children.Add(label);
            var onClick = new TapGestureRecognizer();
            onClick.Tapped += OpenGallery_Tapped;
            onClick.CommandParameter = gal;
            cell.GestureRecognizers.Add(onClick);
            return cell;
        }

        private async void AddGallery_ToolbarItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GalleryCreatorPage(gallery));
        }

        private async void OpenImage_Tapped(object sender, EventArgs e)
        {
            string image = (e as TappedEventArgs).Parameter as string;
            await Navigation.PushAsync(new ImagePage(image));
        }

        private async void OpenGallery_Tapped(object sender, EventArgs e)
        {            
            Gallery gal = (e as TappedEventArgs).Parameter as Gallery;
            await Navigation.PushAsync(new GalleryPage(gal));
        }
    }
}