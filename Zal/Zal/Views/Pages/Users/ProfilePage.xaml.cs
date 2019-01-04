using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;

namespace Zal.Views.Pages.Users
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : ContentPage
	{
        User CurrentUser;

        public ProfilePage(User user = null)
        {
            if (user == null)
            {
                CurrentUser = Zalesak.Session.CurrentUser;
            }
            else
            {
                CurrentUser = user;
                //LogOutButton.IsVisible = false;
            }
            InitializeComponent();
            BindingContext = CurrentUser;
            InitProfileProperties();
        }

        private void InitProfileProperties()
        {
            //PropLayout.Children.Add(new PropertyBadges());
            //PropLayout.Children.Add(new PropertyPoints());
            SetOnClickEvents();
        }

        private void SetOnClickEvents()
        {
            //foreach (ProfilePropertyFrame profileProperty in PropLayout.Children)
            //{
            //    profileProperty.OnClick += Property_Clicked;
            //}
        }

        private void Property_Clicked(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new EmptyPage());
        }

        private async void LogOutButton_Clicked(object sender, EventArgs args)
        {
            Zalesak.Logout();
            Navigation.InsertPageBefore(new LoginPage(), Navigation.NavigationStack.First());
            await Navigation.PopToRootAsync();
        }
    }
}