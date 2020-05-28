using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool IsDeviceVertically { get; private set; } = true;
        private ToolbarItem yearToolbarItem;
        private ToolbarItem creatorToolbarItem;

        private const int THUMB_SIZE = 200;
        private const string ALBUM_URI = "http://zalesak.hlucin.com/galerie/albums/";
        private const int ITEMS_PER_PAGE = 30;

        private double maxImgSize = 0;
        private Gallery gallery;
        private bool IsConcreteGallery => gallery != null;
        private bool isVerticalGridReady = false;
        private bool isHorizontalGridReady = false;
        private bool isLoaded = false;
        private int selectedPart = -1;
        private int numOfParts = -1;

        public GalleryPage ()
		{
			InitializeComponent ();
            Title = "Galerie";
            AddToolbarItem_Creator("vytvořit novou galerii");
            AddToolbarItem_Year();
            Analytics.TrackEvent("GaleryPage-main");
		}

        public GalleryPage(Gallery gallery)
        {
            InitializeComponent();
            Title = gallery.Name;
            this.gallery = gallery;
            AddToolbarItem_Creator("přidat fotky");
        }

        private void AddToolbarItem_Creator(string text)
        {
            creatorToolbarItem = new ToolbarItem
            {
                Text = text,
                Order = ToolbarItemOrder.Secondary
            };
            creatorToolbarItem.Clicked += AddGallery_ToolbarItemClicked;
            ToolbarItems.Add(creatorToolbarItem);
        }

        private void AddToolbarItem_Year()
        {
            yearToolbarItem = new ToolbarItem
            {
                Order = ToolbarItemOrder.Primary
            };
            yearToolbarItem.Clicked += YearToolbarItem_Clicked;
            ToolbarItems.Add(yearToolbarItem);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!isLoaded) Synchronize();
        }

        private async void Synchronize()
        {
            if (IsConcreteGallery)
            {
                var images = await gallery.ImagesLazyLoad();
                selectedPart = 1;
                numOfParts = (images.Count() + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE;
                if (numOfParts > 1)
                {
                    PageNavigationLayout.IsVisible = true;
                    NumOfPagesLabel.Text = "/ " + numOfParts;
                }
            }
            else
            {
                await Zalesak.Galleries.ReSynchronize();
                var years = Zalesak.Galleries.Data.GroupBy(x => x.Year).Select(x => x.Key).OrderByDescending(x => x);
                selectedPart = years.FirstOrDefault();
                picker.ItemsSource = years.ToList();
                yearToolbarItem.Text = selectedPart.ToString();
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
                if (maxImgSize > THUMB_SIZE) maxImgSize = THUMB_SIZE;
            }
            if (isVertically != IsDeviceVertically)
            {
                IsDeviceVertically = isVertically;
                UpdateGrid();
            }
        }

        private void UpdateGrid()
        {            
            if (IsDeviceVertically)
            {
                VerticalGrid.IsVisible = true;
                HorizontalGrid.IsVisible = false;
                if (!isVerticalGridReady)
                {
                    InitGrid();
                    isVerticalGridReady = true;
                }
            }
            else
            {
                VerticalGrid.IsVisible = false;
                HorizontalGrid.IsVisible = true;
                if (!isHorizontalGridReady)
                {
                    InitGrid();
                    isHorizontalGridReady = true;
                }
            }
        }

        private async void InitGrid()//todo when uploadin deleting images -> redraw
        {
            int numOfColumns = (int)((Width - 50) / maxImgSize + 1);
            double spacingSize = VerticalGrid.ColumnSpacing * (numOfColumns - 1);
            double itemSize = (Width - spacingSize) / numOfColumns;

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
                items = items.Skip((selectedPart - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
            }
            else
            {
                items = Zalesak.Galleries.Data.Where(x => x.Year == selectedPart);
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
                        cell = MakeCellForGallery(items.ElementAt(index) as Gallery, itemSize);
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

        private View MakeCellForGallery(Gallery gal, double itemHeight)
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
                HeightRequest = itemHeight,
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

        private void YearToolbarItem_Clicked(object sender, EventArgs e)
        {
            picker.Focus();
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
            if (pickedYear == selectedPart) return;
            yearToolbarItem.Text = pickedYear.ToString();
            ChangePage(pickedYear);
        }

        private void PageLabel_Tapped(object sender, EventArgs e)
        {
            PageEntry.Focus();
        }

        private async void PageEntry_Focused(object sender, FocusEventArgs e)
        {
            await Task.Delay(50);
            PageEntry.CursorPosition = 0;
            PageEntry.SelectionLength = PageEntry.Text.Length;
        }

        private void PageEntry_Unfocused(object sender, FocusEventArgs e)
        {
            int page;
            if (int.TryParse(PageEntry.Text, out page) && page > 0)
            {
                if (page > numOfParts) page = numOfParts;
            }
            else
            {
                page = 1;
            }
            if (page == selectedPart) return;
            ChangePage(page);
            PageEntry.Text = selectedPart.ToString();
        }

        private void RigthArrow_Tapped(object sender, EventArgs e)
        {
            if (selectedPart >= numOfParts) return;
            ChangePage(selectedPart + 1);
            PageEntry.Text = selectedPart.ToString();
        }

        private void LeftArrow_Tapped(object sender, EventArgs e)
        {
            if (selectedPart <= 1) return;
            ChangePage(selectedPart - 1);
            PageEntry.Text = selectedPart.ToString();
        }

        private void ChangePage(int page)
        {
            scrollView.ScrollToAsync(0, 0, false);
            selectedPart = page;
            isHorizontalGridReady = false;
            isVerticalGridReady = false;
            UpdateGrid();
        }
    }
}