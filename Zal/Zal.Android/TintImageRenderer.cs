using System;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Graphics;

using Zal.Elements;
using Zal.Droid;

[assembly: ExportRenderer(typeof(TintImage), typeof(TintImageRenderer))]

namespace Zal.Droid
{
    public class TintImageRenderer : ImageRenderer
    {

        public TintImageRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                SetTint();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(TintImage.TintColor))
            {
                SetTint();
            }
        }

        protected void SetTint()
        {
            var element = (TintImage)Element;

            if (element.TintColor == Xamarin.Forms.Color.Default)
            {
                Control.ClearColorFilter();
            }
            else
            {
                Control.SetColorFilter(element.TintColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            }
        }
    }
}