using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Zal.Services
{
    public static class ImageSourceHelper
    {
        private static readonly string baseUri = "http://zalesak.hlucin.com/";

        internal static ImageSource UserImg(string name, NamedSize size)
        {
            if (size == NamedSize.Default) return UserImg(name);

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

            string imgName = name + sizeName + ".jpg";
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
