using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Zal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            MasterBehavior = MasterBehavior.Popover;
        }

        public async Task NavigateFromMenu(Type targetType)
        {
            await HideMenu();
            var loadingPage = new LoadingPage();
            NavigationPage.SetHasNavigationBar(loadingPage, false);
            Detail = new NavigationPage(loadingPage);
            loadingPage.Loaded += async () =>
            {
                Page page = (Page)Activator.CreateInstance(targetType);
                await Detail.Navigation.PushAsync(page, false);
                Detail.Navigation.RemovePage(loadingPage);
            };
        }

        public async Task HideMenu()
        {
            if (IsPresented)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    await Task.Delay(100);
                }
                IsPresented = false;
            }
        }
    }
}