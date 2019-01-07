using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Views;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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

            MainPage = new MainPage();
        }

        private async void InitializeAppData()
        {
            //Zal.CommandExecutedOffline += OnCommandExecutedOffline;
            await Task.Run(async () => {
                if (Current.Properties.ContainsKey(LOCAL_DATA))
                {
                    var fileData = (string)Current.Properties[LOCAL_DATA];
                    Zalesak.LoadDataFrom(fileData);
                    var isLogged = await Zalesak.Session.TryLoginWithTokenAsync();
                    await Zalesak.StartSynchronizingAsync();
                }
                //Zal.LoadOfflineCommands(LoadFromStorage(OFFLINE_COMMANDS_FILE));
                //var c = await Zalesak.Session.LoginAsync("pepa3@email.cz", "password", false);
            });
            OnAppReady();
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
