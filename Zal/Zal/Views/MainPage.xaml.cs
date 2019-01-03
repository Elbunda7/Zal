using Zal.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Zal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        //Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            //MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(Type targetType)
        {
            Page page = (Page)Activator.CreateInstance(targetType);
            Detail = new NavigationPage(page);
            if (Device.RuntimePlatform == Device.Android)
            {
                await Task.Delay(100);
            }
            HideMenu();
        }

        public void HideMenu()
        {
            IsPresented = false;
        }
    }
}