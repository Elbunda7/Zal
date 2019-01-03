using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain.ActiveRecords;

namespace Zal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPage
    {
        public WebViewPage(ISimpleItem item)
        {
            InitializeComponent();
            Title = item.Title;
            HtmlWebViewSource htmlSource = new HtmlWebViewSource
            {
                Html = "<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'></head><body>" +
                        item.Text + "</body></html>"
            };
            Browser.Source = htmlSource;
        }
    }
}