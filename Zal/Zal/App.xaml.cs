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
        }

        private async void InitializeAppData()
        {
            Zalesak.CommandExecutedOffline += OnCommandExecutedOffline;
            await Task.Run(async () => {
                //if (Current.Properties.ContainsKey(LOCAL_DATA))//todo future akce se zobrazí jeno když dojde k synchronizaci
                //{
                //    var fileData = (string)Current.Properties[LOCAL_DATA];
                //    Zalesak.LoadDataFrom(fileData);
                //    var isLogged = await Zalesak.Session.TryLoginWithTokenAsync();
                //    await Zalesak.StartSynchronizingAsync();//todo synchronizovat vše?
                //}
                //lastVersion = await Zalesak.CurrentVersion();
                //appVersion = new Version(VersionTracking.CurrentVersion);
                //Zalesak.LoadOfflineCommands(LoadFromStorage(OFFLINE_COMMANDS_FILE));
            });
            var loadingPage = new LoadingPage();
            loadingPage.Loaded += LoadingPage_Loaded;
            MainPage = loadingPage;
        }

        private void LoadingPage_Loaded()
        {
            OnAppReady();
        }

        private void OnCommandExecutedOffline(XDocument commands)
        {
            throw new NotImplementedException();
        }

        private void OnAppReady()
        {
            MainPage = new MainPage();
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
