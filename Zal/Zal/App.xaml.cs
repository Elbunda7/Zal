using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Views;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Xml.Linq;
using Zal.Views.Pages.Galleries;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Zal
{
    public partial class App : Application
    {
        private const string LOCAL_DATA = "data";
        private const string OFFLINE_COMANDS = "commands";

        public App()
        {
            InitializeComponent();
            InitializeAppData();

            AppCenter.Start("android=555f3d1a-d1f9-485d-8a9d-983344faa20b;", typeof(Analytics), typeof(Crashes));

            MainPage = new AboutPage();//todo loading page
        }

        private async void InitializeAppData()
        {
            Zalesak.CommandExecutedOffline += OnCommandExecutedOffline;
            await Task.Run(async () => {
                if (Current.Properties.ContainsKey(LOCAL_DATA))//todo future akce se zobrazí jeno když dojde k synchronizaci
                {
                    var fileData = (string)Current.Properties[LOCAL_DATA];
                    Zalesak.LoadDataFrom(fileData);
                    var isLogged = await Zalesak.Session.TryLoginWithTokenAsync();
                    await Zalesak.StartSynchronizingAsync();//todo synchronizovat vše?
                }
                //Zalesak.LoadOfflineCommands(LoadFromStorage(OFFLINE_COMMANDS_FILE));
            });
            OnAppReady();
        }

        private void OnCommandExecutedOffline(XDocument commands)
        {
            throw new NotImplementedException();
        }

        private void OnAppReady()
        {
             MainPage = new MainPage();
            //MainPage = new ImagePage("http://zalesak.hlucin.com/galerie/albums/2020/MezinarodnisrazKCTRymarov/IMG_20200201_101326.jpg");
            //MainPage = new ImagePage("http://zalesak.hlucin.com/galerie/albums/2019/vanocka/DSC_1060.jpg");
           // MainPage = new ImagePage("http://zalesak.hlucin.com/galerie/albums/2020/Horcovavyzva/landscapeTest.jpg");
            //MainPage = new ImagePage("http://zalesak.hlucin.com/galerie/albums/2020/Horcovavyzva/portraitTest.jpg");
        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            Current.Properties[LOCAL_DATA] = Zalesak.GetDataJson();
            Current.SavePropertiesAsync();
        }

        protected override void OnResume()
        {
        }
    }
}
