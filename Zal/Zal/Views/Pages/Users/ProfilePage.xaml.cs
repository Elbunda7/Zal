using Microsoft.AppCenter.Analytics;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
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
using Zal.Domain.Consts;
using Zal.Services;
using Zal.Views.Pages.Users.ProfileElementPages;
using Zal.Views.Pages.Users.ProfileElements;

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
            if (currentUser.Attribs.HasFlag(ZAL.UserAttribs.Registered)) {
                if (!currentUser.Attribs.HasFlag(ZAL.UserAttribs.ConfirmedEmail))
                {
                    profileElements.Children.Add(new ConfirmEmail());
                }
                else if (!currentUser.Attribs.HasFlag(ZAL.UserAttribs.RegCompleted))
                {
                    profileElements.Children.Add(new CompleteRegistration());
                }
                else if (!currentUser.Attribs.HasFlag(ZAL.UserAttribs.Approved))
                {
                    profileElements.Children.Add(new LabelElement("Dokončení registrace čeká na vyřízení"));
                }
            }
            //PropLayout.Children.Add(new PropertyBadges());
            //PropLayout.Children.Add(new PropertyPoints());
            SetOnClickEvents();
        }

        private void SetOnClickEvents()
        {
            foreach (ProfileElement element in profileElements.Children)
            {
                element.OnClick += Property_Clicked;
            }
        }

        private async void Property_Clicked(object sender, EventArgs e)
        {
            if (sender is CompleteRegistration) await Navigation.PushAsync(new CompleteRegistrationPage(currentUser));
            else await DisplayAlert("Click", "", "Ok");
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
            if (await HavePermission.For<Permissions.StorageRead>())
            {
                var mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions() { CompressionQuality = 90, });
                if (mediaFile != null)
                {
                    byte[] rawImage = File.ReadAllBytes(mediaFile.Path);
                    var isSuccess = await currentUser.UploadProfileImage(rawImage);
                    mediaFile.Dispose();
                }
            }
        }

        private void ProfileImage_Tapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentUser.Image))
            {
                EditButton_Clicked(this, null);
            }
            else
            {
                popupWindow.IsVisible = true;
                imageDetail.Source = ImageSourceHelper.UserImg(currentUser.ImageInfo, NamedSize.Large);
            }
        }

        private void BackFromPopup_Tapped(object sender, EventArgs e)
        {
            popupWindow.IsVisible = false;
        }

        protected override bool OnBackButtonPressed()
        {
            if (popupWindow.IsVisible)
            {
                popupWindow.IsVisible = false;
                return true;
            }
            return base.OnBackButtonPressed();
        }
    }
}