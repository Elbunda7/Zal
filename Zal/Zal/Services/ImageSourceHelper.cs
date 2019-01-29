using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Zal.Domain.Models;

namespace Zal.Services
{
    public class ImageSourceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource img = "profile_boy.png";
            NamedSize size = parameter is NamedSize ? (NamedSize)parameter : NamedSize.Medium;
            if (value is UserImageInfo)
            {
                img = ImageSourceHelper.UserImg(value as UserImageInfo, size);
            }
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class ImageSourceHelper
    {
        private static readonly string baseUri = "http://zalesak.hlucin.com/";

        internal static ImageSource UserImg(UserImageInfo s, NamedSize size)
        {
            if (string.IsNullOrEmpty(s.ImageName))
            {
                if (s.IsBoy) return "profile_boy.png";
                return "profile_girl.png";
            }

            if (size == NamedSize.Default) return UserImg(s.ImageName);

            string sizeName = "_";
            double density = DeviceDisplay.MainDisplayInfo.Density;

            if (size == NamedSize.Micro || size == NamedSize.Small)
            {
                sizeName += density < 2.5 ? NamedSize.Micro : NamedSize.Small;
            }
            else if (size == NamedSize.Medium)
            {
                sizeName += density < 1.7 ? NamedSize.Small : NamedSize.Medium;
            }
            else if (size == NamedSize.Large)
            {
                sizeName += density < 2.5 ? NamedSize.Medium : NamedSize.Large;
            }

            string imgName = s.ImageName + sizeName + ".jpg";
            Uri uri = new Uri(baseUri + "clenove_obrazky/sized/" + imgName);
            return ImageSource.FromUri(uri);
        }

        internal static ImageSource UserImg(string name)
        {
            Uri uri = new Uri(baseUri + "clenove_obrazky/" + name + ".jpg");
            return ImageSource.FromUri(uri);
        }
    }
}
