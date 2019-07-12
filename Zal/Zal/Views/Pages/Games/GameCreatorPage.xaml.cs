using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Zal.Views.Pages.Games
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GameCreatorPage : ContentPage
	{
        private GameCreatorVisibilitySettings visibilitySettings;

        public GameCreatorPage ()
		{
			InitializeComponent ();
            visibilitySettings = new GameCreatorVisibilitySettings { DoCollection = true, DoGame = true };
            BindingContext = visibilitySettings;

        }

        private void ConfirmationButton_Click(object sender, EventArgs e)
        {

        }
    }

    public class GameCreatorVisibilitySettings : INotifyPropertyChanged
    {
        private bool doCollection;
        public bool DoCollection {
            get {
                return doCollection;
            }
            set {
                if (doCollection != value)
                {
                    doCollection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoCollection)));
                }
            }
        }

        private bool doGame;
        public bool DoGame {
            get {
                return doGame;
            }
            set {
                if (doGame != value)
                {
                    doGame = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoGame)));
                }
            }
        }    

        public event PropertyChangedEventHandler PropertyChanged;
    }
}