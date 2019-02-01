using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Zal.Views.Pages.Users.ProfileElements
{
    public class ConfirmEmail : ProfileElement
    {
        public ConfirmEmail()
        {
            Content = new Label()
            {
                Text = "Potvrď email!",
            };
        }
    }
}
