using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Zal.Elements
{
    public class SideMenuViewCell : ViewCell
    {
        private Color ColorForActiveText = (Color)Application.Current.Resources["Primary"];
        private Color ColorForNormalText = Color.Black;
        private Color ColorForActiveBg = (Color)Application.Current.Resources["GrayMenuActiveColor"];
        private Color ColorForNormalBg = Color.Transparent;
        private const double TRANSPARENT = 0.54;
        private const double SOLID = 1;

        private StackLayout MenuItem { get; set; }
        private TintImage icon;
        private Label title;

        public string IconSource {
            get { return GetValue(IconSourceProperty) as string; }
            set { SetValue(IconSourceProperty, value); }
        }

        public string Title {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        public bool IsSelected {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly BindableProperty IconSourceProperty =
            BindableProperty.Create("IconSource", typeof(string), typeof(SideMenuViewCell), "", BindingMode.TwoWay, null,
                new BindableProperty.BindingPropertyChangedDelegate(IconSourceChanged));

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create("Title", typeof(string), typeof(SideMenuViewCell), "", BindingMode.TwoWay, null,
                new BindableProperty.BindingPropertyChangedDelegate(TitleChanged));

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create("IsSelected", typeof(bool), typeof(SideMenuViewCell), false, BindingMode.TwoWay, null,
                new BindableProperty.BindingPropertyChangedDelegate(IsSelectedChanged));

        private static void IconSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as SideMenuViewCell).icon.Source = newValue as string;
        }

        private static void TitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as SideMenuViewCell).title.Text = newValue as string;
        }

        private static void IsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            bool isSelected = (bool)newValue;
            if (isSelected)
            {
                (bindable as SideMenuViewCell).Highlite();
            }
            else
            {
                (bindable as SideMenuViewCell).UnHighlite();
            }
        }

        public SideMenuViewCell()
        {
            icon = new TintImage()
            {
                Opacity = TRANSPARENT,
                Margin = new Thickness(0, 0, 26, 0)
            };
            title = new Label()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 20,
                Opacity = TRANSPARENT,
                TextColor = ColorForNormalText,
            };
            MenuItem = new StackLayout()
            {
                Padding = new Thickness(16, 10),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal
            };
            View = MenuItem;
            MenuItem.Children.Add(icon);
            MenuItem.Children.Add(title);
        }

        public void Highlite()
        {
            MenuItem.BackgroundColor = ColorForActiveBg;
            title.TextColor = ColorForActiveText;
            title.Opacity = SOLID;
            icon.TintColor = ColorForActiveText;
            icon.Opacity = SOLID;
        }

        public void UnHighlite()
        {
            MenuItem.BackgroundColor = ColorForNormalBg;
            title.TextColor = ColorForNormalText;
            title.Opacity = TRANSPARENT;
            icon.TintColor = ColorForNormalText;
            icon.Opacity = TRANSPARENT;
        }
    }
}