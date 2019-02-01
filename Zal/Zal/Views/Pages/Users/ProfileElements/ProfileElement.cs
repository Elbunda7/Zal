using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Zal.Views.Pages.Users.ProfileElements
{
    public class ProfileElement : Frame
    {
        public event EventHandler OnClick;

        public ProfileElement()
        {
            //Margin = new Thickness(16, 8);
            Padding = new Thickness(10);
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => RaiseOnClick(this, new EventArgs())),
            });
        }

        protected virtual void RaiseOnClick(object sender, EventArgs e)
        {
            OnClick?.Invoke(sender, e);
        }
    }
}
