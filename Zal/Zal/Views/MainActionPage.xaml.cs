using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Zal.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainActionPage : TabbedPage
    {
		public MainActionPage ()
        {
            InitializeComponent();
            Title = "Plán akcí";
            var toolbarItem = new ToolbarItem()
            {
                Text = "Nový",
                Order = ToolbarItemOrder.Secondary
            };
            toolbarItem.Clicked += NewActionEvent_ToolbarItemClicked;
            ToolbarItems.Add(toolbarItem);
            Children.Add(new ActionsPage());
            Children.Add(new ActionsPage(DateTime.Today.Year));
        }

        private async void NewActionEvent_ToolbarItemClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new ActionCreator());
        }
    }
}