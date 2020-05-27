using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
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
        ToolbarItem yearToolbarItem;

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
        private const string ALBUM_URI = "http://zalesak.hlucin.com/galerie/albums/";

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
            picker.Focus();
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
                var years = Zalesak.Galleries.Data.GroupBy(x => x.Year).Select(x => x.Key).OrderByDescending(x => x);
                selectedYear = years.FirstOrDefault();
                picker.ItemsSource = years.ToList();
                yearToolbarItem = new ToolbarItem()
                {
                    Text = selectedYear.ToString(),
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
            IEnumerable<object> items;
            double rowHeight = itemSize;
            if (IsConcreteGallery)
            {
                items = await gallery.ImagesLazyLoad();
            }
            else
            {
                items = Zalesak.Galleries.Data.Where(x => x.Year == selectedYear);
                rowHeight += 50;
            }
            int itemsCount = items.Count();
            int numOfRows = (itemsCount + numOfColumns - 1) / numOfColumns;
            for (int j = 0; j < numOfRows; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = rowHeight });
                for (int i = 0; i < numOfColumns; i++)
                {
                    if (index >= itemsCount) break;
                    View cell;
                    if (IsConcreteGallery)
                    {
                        cell = MakeCellForPhoto(gallery.File, items.ElementAt(index) as string);
                    }
                    else
                    {
                        cell = MakeCellForGallery(items.ElementAt(index) as Gallery);
                    }
                    grid.Children.Add(cell, i, j);
                    index++;
                }
            }
        }

        private View MakeCellForPhoto(string imgFile, string imgName)
        {
            Image img = new Image()
            {
                Source = ALBUM_URI + imgFile + "small/" + imgName,
                ClassId = imgName,
            };
            var onClick = new TapGestureRecognizer();
            onClick.Tapped += OpenImage_Tapped;
            onClick.CommandParameter = ALBUM_URI + imgFile + imgName;
            img.GestureRecognizers.Add(onClick);
            return img;
        }

        private View MakeCellForGallery(Gallery gal)
        {
            StackLayout cell = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 0,
            };
            Image img = new Image()
            {
                Source = ALBUM_URI + gal.File + "small/" + gal.MainImg,
                ClassId = gal.MainImg,
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

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var pickedYear = (int)picker.SelectedItem;
            if (pickedYear == selectedYear) return;
            selectedYear = pickedYear;
            yearToolbarItem.Text = pickedYear.ToString();
            isHorizontalGridReady = false;
            isVerticalGridReady = false;
            OnOrientationChanged(Width, Height);
        }
    }
}