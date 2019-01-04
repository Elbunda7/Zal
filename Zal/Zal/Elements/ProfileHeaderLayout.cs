using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ZalXamarin.Elements
{
    public class ProfileHeaderLayout : StackLayout
    {
        private const int HEIGHT_SIZE = 150;

        private CircleImage mainImage;
        private CircleImage minorImage;
        private CircleImage minorImage2;
        private Label nameLabel;
        private Label ageLabel;

        public string Name {
            get { return GetValue(NameProperty) as string; }
            set { SetValue(NameProperty, value); }
        }

        public int? Age {
            get { return (int?)GetValue(AgeProperty); }
            set { SetValue(AgeProperty, value); }
        }

        public static readonly BindableProperty NameProperty =
            BindableProperty.Create("Name", typeof(string), typeof(ProfileHeaderLayout), "Name", BindingMode.TwoWay, null,
                new BindableProperty.BindingPropertyChangedDelegate(NameChanged));

        public static readonly BindableProperty AgeProperty =
            BindableProperty.Create("Age", typeof(int?), typeof(ProfileHeaderLayout), -1, BindingMode.TwoWay, null,
                new BindableProperty.BindingPropertyChangedDelegate(AgeChanged));

        private static void NameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as ProfileHeaderLayout).nameLabel.Text = newValue as string;
        }

        private static void AgeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as ProfileHeaderLayout).ageLabel.Text = (int?)newValue + " let";//todo rok, roky or invisible
        }




        public ProfileHeaderLayout()
        {
            HeightRequest = HEIGHT_SIZE;
            BackgroundColor = (Color)Application.Current.Resources["Primary"];
            HorizontalOptions = LayoutOptions.Fill;
            Orientation = StackOrientation.Horizontal;
            Children.Add(CreateProfileImage());
            Children.Add(CreateInfoLayout());
        }

        private View CreateProfileImage()
        {
            mainImage = new CircleImage
            {
                Source = "profile_boy.png",
                BorderThickness = 1,
                BorderColor = (Color)Application.Current.Resources["Primary"]
            };
            minorImage = new CircleImage
            {
                Source = "profile_girl.png",
                HeightRequest = WidthRequest = 40,
                BorderThickness = 2,
                BorderColor = (Color)Application.Current.Resources["PrimaryDark"]
            };
            minorImage2 = new CircleImage
            {
                Source = "profile_girl.png",
                HeightRequest = WidthRequest = 40,
                BorderThickness = 2,
                BorderColor = (Color)Application.Current.Resources["Accent"]
            };
            AbsoluteLayout imageLayout = new AbsoluteLayout()
            {
                HeightRequest = WidthRequest = 144,
                Margin = new Thickness(10, 0, -5, 0)
            };
            imageLayout.Children.Add(mainImage);
            imageLayout.Children.Add(minorImage, new Rectangle(1.3, 0.4, 48, 48), AbsoluteLayoutFlags.PositionProportional);
            imageLayout.Children.Add(minorImage2, new Rectangle(0.8, 0.9, 48, 48), AbsoluteLayoutFlags.PositionProportional);
            return imageLayout;
        }

        private View CreateInfoLayout()
        {
            StackLayout infoLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            nameLabel = new Label()
            {
                Text = "Přezdívka",
                LineBreakMode = LineBreakMode.TailTruncation,
                TextColor = (Color)Application.Current.Resources["LightTextColor"],
                FontSize = 24,
                HorizontalOptions = LayoutOptions.Center
            };
            ageLabel = new Label()
            {
                Text = "? let",
                TextColor = (Color)Application.Current.Resources["LightTextColor"],
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Center
            };
            infoLayout.Children.Add(nameLabel);
            infoLayout.Children.Add(ageLabel);
            return infoLayout;
        }
    }
}
