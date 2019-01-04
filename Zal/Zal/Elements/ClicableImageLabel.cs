using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Zal.Elements
{
    public class ClickableImageLabel : StackLayout
    {
        public event Action OnClick;

        private Image icon;
        private Label title;

        public string Icon {
            get { return GetValue(IconProperty) as string; }
            set { SetValue(IconProperty, value); }
        }

        public string Text {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty IconProperty =
            BindableProperty.Create("Icon", typeof(string), typeof(ClickableImageLabel), "", BindingMode.TwoWay, null,
                new BindableProperty.BindingPropertyChangedDelegate(IconChanged));

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create("Text", typeof(string), typeof(ClickableImageLabel), "", BindingMode.TwoWay, null,
                new BindableProperty.BindingPropertyChangedDelegate(TextChanged));

        private static void IconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as ClickableImageLabel).icon.Source = newValue as string;
        }

        private static void TextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as ClickableImageLabel).title.Text = newValue as string;
        }

        public ClickableImageLabel()
        {
            Orientation = StackOrientation.Horizontal;
            HorizontalOptions = LayoutOptions.Center;
            Padding = new Thickness(15, 8);
            icon = new Image();
            title = new Label()
            {
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
            };
            Children.Add(icon);
            Children.Add(title);
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => RaiseOnClick()),
            });
        }

        private void RaiseOnClick()
        {
            if (OnClick != null)
            {
                OnClick.Invoke();
            }
        }
    }
}
