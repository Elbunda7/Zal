using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Zal
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            InitializeAppData();


            MainPage = new MainPage();
        }

        private async void InitializeAppData()
        {
            //Zal.CommandExecutedOffline += OnCommandExecutedOffline;
            await Task.Run(async () => {
                //Zal.LoadOfflineCommands(LoadFromStorage(OFFLINE_COMMANDS_FILE));
                //Zal.LoadDataFrom(await LoadFromStorageAsync(LOCAL_DATA_FILE));
                //var a = await LoadFromStorageAsync(LOCAL_DATA_FILE);
                //Zal.LoadDataFrom(a);
                //var b = await Zalesak.Session.TryLoginWithTokenAsync();
                var c = await Zalesak.Session.LoginAsync("pepa3@email.cz", "password", false);
                //await Zal.StartSynchronizingAsync();

            });
            OnAppReady();
        }

        private void OnAppReady()
        {
            //MainPage = new SideMenu.SideMenu();
        }


        protected override void OnStart()
        {
            object a;
            if (Current.Properties.ContainsKey("key2"))
            {
               a = Current.Properties["key2"];

            }
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            Current.Properties["key2"] = "value2";
            Current.SavePropertiesAsync();
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
