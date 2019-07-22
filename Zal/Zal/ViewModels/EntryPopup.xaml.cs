using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Zal.ViewModels
{
    public partial class EntryPopup : PopupPage
    {
        private Action<string> _action;

        public EntryPopup(string title, Action<string> action, string text = "", string placeholder = "", Keyboard keyboard = null) : this()
        {
            TitleLabel.Text = title;
            _action = action;
            MyEntry.Text = text;
            MyEntry.Placeholder = placeholder;
            if (keyboard != null) MyEntry.Keyboard = keyboard;
        }

        public EntryPopup()
        {
            InitializeComponent();
        }

        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
            MyEntry.Focus();
            MyEntry.CursorPosition = MyEntry.Text.Length;
        }

        private async void OnClose_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void OnSave_Clicked(object sender, EventArgs e)
        {
            _action?.Invoke(MyEntry.Text);
            await PopupNavigation.Instance.PopAsync();
        }
    }
}