using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Services;

namespace Zal.Views.Pages.Galleries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleryCreatorPage : ContentPage
    {
        List<MediaFile> mediaFiles;

        public GalleryCreatorPage()
        {
            InitializeComponent();
            Title = "Nová galerie";
        }

        private async void PickPhotos_Click(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (await HavePermission.For<Permissions.StorageRead>())
            {
                var pickingTask = CrossMedia.Current.PickPhotosAsync(new PickMediaOptions() { CompressionQuality = 92, });
                await Task.Delay(300);
                IndicateActivity(true);
                var tmpMediaFiles = await pickingTask;
                if (tmpMediaFiles != null) mediaFiles = tmpMediaFiles;
                IndicateActivity(false);
                if (mediaFiles != null)
                {
                    var b = ImageSource.FromFile(mediaFiles[0].Path);
                    //firstImage.Source = b;
                    infoLabel.Text = "Fotek vybráno: " + mediaFiles.Count;
                    infoFrame.IsVisible = true;
                }
            }
        }

        private void IndicateActivity(bool isRunning)
        {
            if (isRunning) infoFrame.IsVisible = !isRunning;
            pickingImagesIndicator.IsVisible = isRunning;
        }

        private async void SaveGallery_Click(object sender, EventArgs e)
        {
            foreach (MediaFile mf in mediaFiles)
            {
                byte[] rawImage = File.ReadAllBytes(mediaFiles[0].Path);
                //var gallery = await Zalesak.Galleries.Add(galleryEntry.Text, DateTime.Now.Year, DateTime.Now);
                //mf.Dispose();
            }
            await Navigation.PopAsync();
        }
    }
}