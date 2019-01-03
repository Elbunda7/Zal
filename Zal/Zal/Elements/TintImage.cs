using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Zal.Elements
{
    public class TintImage : Image
    {
        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
            "TintColor", typeof(Color), typeof(TintImage), Color.Default
        );

        public Color TintColor {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }
    }
}
