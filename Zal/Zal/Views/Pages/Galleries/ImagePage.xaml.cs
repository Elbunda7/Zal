using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain.ActiveRecords;

namespace Zal.Views.Pages.Galleries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePage : ContentPage
    {
        private GraphPhoto photo;

        public ImagePage(GraphPhoto photo)
        {
            this.photo = photo;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            imageView.Source = photo.ThumbUrl;//todo call bigger photo
        }
    }
}