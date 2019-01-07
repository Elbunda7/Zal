using Microsoft.AppCenter.Analytics;
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
        private User currentUser;

        public ProfilePage() : this(Zalesak.Session.CurrentUser) { }

        public ProfilePage(User user)
        {
            Analytics.TrackEvent("Profile_Page", new Dictionary<string, string>() { { "user", user.NickName } });
            currentUser = user;
            //LogOutButton.IsVisible = false;
            InitializeComponent();
            BindingContext = currentUser;
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
            await Zalesak.Session.Logout();
            Navigation.InsertPageBefore(new LoginPage(), Navigation.NavigationStack.First());
            await Navigation.PopToRootAsync();
        }
    }
}