using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Zal.Views.Pages.Users.ProfileElements
{
    public class CompleteRegistration : ProfileElement
    {
        public CompleteRegistration()
        {
            var mainLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
            };
            var layout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };
            var label = new Label()
            {
                Text = "Dokončit registraci",
            };
            var memberButton = new Button()
            {
                Image = "jesterka_40dp.png",
                Text = "člen",
                ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 5),
            };
            var otherButton = new Button()
            {
                Image = "svist_40dp.png",
                Text = "přítel oddílu",
                ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 5),
            };
            memberButton.Clicked += base.RaiseOnClick;
            otherButton.Clicked += base.RaiseOnClick;

            mainLayout.Children.Add(label);
            mainLayout.Children.Add(layout);
            layout.Children.Add(memberButton);
            layout.Children.Add(otherButton);
            Content = mainLayout;
        }
    }
}
