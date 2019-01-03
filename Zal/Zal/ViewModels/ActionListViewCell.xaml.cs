using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Zal.ViewModels
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActionListViewCell : ViewCell
	{
		public ActionListViewCell ()
		{
			InitializeComponent ();
		}
	}
}