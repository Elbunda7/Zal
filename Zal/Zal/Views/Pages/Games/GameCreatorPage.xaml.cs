using System;
using System.Collections.Generic;
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
		public GameCreatorPage ()
		{
			InitializeComponent ();
		}

        private void ConfirmationButton_Click(object sender, EventArgs e)
        {

        }
    }
}