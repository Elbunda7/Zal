using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Zal.Views.Pages.Users.ProfileElements
{
    public class LabelElement : ProfileElement
    {
        public LabelElement(string text)
        {
            Content = new Label
            {
                Text = text,
            };
        }
    }
}
