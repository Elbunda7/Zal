using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Zal.Elements
{
    public class InfoGridItem : AbsoluteLayout
    {
        public event Action OnClick;

        private Image icon;
        private Label title;
        private static Random random = new Random();

        internal static void Randomize(int seed)
        {
            random = new Random(seed);
        }

        public string Icon {
            get { return GetValue(IconProperty) as string; }
            set { SetValue(IconProperty, value); }
        }

        public string Text {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty IconProperty =
            BindableProperty.Create("Icon", typeof(string), typeof(InfoGridItem), "", BindingMode.TwoWay, null,
                new BindableProperty.BindingPropertyChangedDelegate(IconChanged));

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create("Text", typeof(string), typeof(InfoGridItem), "", BindingMode.TwoWay, null,
                new BindableProperty.BindingPropertyChangedDelegate(TextChanged));

        private static void IconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as InfoGridItem).icon.Source = newValue as string;
        }

        private static void TextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as InfoGridItem).title.Text = newValue as string;
        }

        public InfoGridItem()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = GetRandomColor();
            icon = new Image();
            title = new Label()
            {
                TextColor = Color.White,
                Text = "Title",
                BackgroundColor = Color.Black,
                Opacity = 0.54,
            };
            Button button = new Button()
            {
                BackgroundColor = Color.Black,
                Opacity = 0,
            };
            Children.Add(icon);
            Children.Add(title, new Rectangle(0, 1, 1, 0.3), AbsoluteLayoutFlags.All);
            Children.Add(button, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            button.Clicked += Button_Clicked;
        }

        Color GetRandomColor()
        {
            Color itemColor = (Color)Application.Current.Resources["Accent"];
            //itemColor = Color.DarkGray;
            return itemColor.WithLuminosity(random.Next(4, 9) / 10.0 + 0.05);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (OnClick != null)
            {
                OnClick.Invoke();
            }
        }

    }
}
