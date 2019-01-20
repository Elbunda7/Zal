using Microsoft.AppCenter.Analytics;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.ActiveRecords;
using Zal.Services;

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

        private async void EditButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (await HavePermission.For(Permission.Storage))
            {
                var mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions() { CompressionQuality = 90, });
                byte[] rawImage = File.ReadAllBytes(mediaFile.Path);
                var isSuccess = await currentUser.UploadProfileImage(rawImage);
                mediaFile.Dispose();
            }
        }
    }
}