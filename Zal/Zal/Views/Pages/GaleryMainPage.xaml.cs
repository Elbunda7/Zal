using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Zal.Views.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GaleryMainPage : ContentPage
	{
		public GaleryMainPage ()
		{
			InitializeComponent ();
            Analytics.TrackEvent("GaleryMainPage");
            throw new Exception("test");
		}
	}
}