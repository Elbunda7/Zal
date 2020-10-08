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
using Zal.Domain;
using Zal.Domain.ActiveRecords;
using Zal.Services;

namespace Zal.Views.Pages.Galleries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleryCreatorPage : ContentPage
    {
        //private List<MediaFile> mediaFiles;
        //private IEnumerable<byte[]> byteImages;
        private GraphGallery gallery = null;
        //private bool CreateNewGallery => gallery == null;

        public GalleryCreatorPage()
        {
            InitializeComponent();
            ShowRelevantLayouts();
        }

        public GalleryCreatorPage(GraphGallery gal)
        {
            InitializeComponent();
            gallery = gal;
            ShowRelevantLayouts();
        }

        private void ShowRelevantLayouts()
        {
            bool x = gallery == null;
            Title = x ? "Nová galerie" : "Upravit galerii";
            Ent_Name.Text = x ? "" : gallery.Name;
            Ent_Year.Text = x ? DateTime.Now.Year.ToString() : gallery.Year.ToString();
            Ent_Year.IsEnabled = x;
            Btn_PickPhotos.IsVisible = !x;
            //settingsFrame.IsVisible = true;// CreateNewGallery;
        }

        private async void PickPhotos_Click(object sender, EventArgs e)
        {
            IndicateActivity(true);
            string link = await gallery.GetSharingLink();
            await Browser.OpenAsync(link, BrowserLaunchMode.SystemPreferred);
            IndicateActivity(false);
            //await CrossMedia.Current.Initialize();
            //if (await HavePermission.For<Permissions.StorageRead>())
            //{
            //    var pickingTask = CrossMedia.Current.PickPhotosAsync(new PickMediaOptions() { CompressionQuality = 92, });
            //    await Task.Delay(300);
            //    IndicateActivity(true);
            //    var tmpMediaFiles = await pickingTask;
            //    if (tmpMediaFiles != null) mediaFiles = tmpMediaFiles;
            //    IndicateActivity(false);
            //    if (mediaFiles != null)
            //    {
            //        byteImages = mediaFiles.Select(x => File.ReadAllBytes(x.Path));
            //        double dataSize = byteImages.Sum(x => x.Length) / 1000000.0;
            //        var b = ImageSource.FromFile(mediaFiles[0].Path);
            //        firstImage.Source = ImageSource.FromStream(() =>
            //        {
            //            var stream = mediaFiles[0].GetStream();
            //            return stream;
            //        });
            //        infoLabel.Text = "Fotek vybráno: " + mediaFiles.Count;
            //        infoLabel2.Text = string.Format("Velikost dat: {0:0.#} MB", dataSize);
            //        infoFrame.IsVisible = true;
            //    }
            //}
        }

        private void IndicateActivity(bool isRunning)
        {
            //if (isRunning) infoFrame.IsVisible = !isRunning;
            Btn_PickPhotos.IsEnabled = !isRunning;
            Indictr_PickingImages.IsVisible = isRunning;
        }

        private async void SaveGallery_Click(object sender, EventArgs e)
        {
            //if (CreateNewGallery)
            //{
            //    gallery = await Zalesak.GraphGalleries.Add(nameEntry.Text, int.Parse(yearEntry.Text), DateTime.Now);//todo oneDrive vytvořit složku
            //}
            //for (int i = 0; i < mediaFiles.Count; i++)
            //{
            //    string imgName = mediaFiles[i].Path.Split('/').Last();
            //    await gallery.Upload(imgName, byteImages.ElementAt(i));
            //    mediaFiles[i].Dispose();
            //}
            //todo nebudu ukládat z aplikace všechny fotky
            await Navigation.PopAsync();
        }
    }
}