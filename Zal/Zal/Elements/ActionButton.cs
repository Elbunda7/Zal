using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Zal.Elements
{
    public class ActionButton : RelativeLayout
    {

        public event EventHandler Clicked;

        private Button button;
        private Image shadow;

        public string Image {
            get {
                return button.Image;
            }
            set {
                button.Image = value;
            }
        }

        public ActionButton() : base()
        {
            WidthRequest = 74;
            HeightRequest = 74;
            Margin = new Thickness(0, 0, 8, 10);
            shadow = new Image
            {
                Source = ImageSource.FromFile("fab_bg_normal.png")
            };
            button = new CircleButton(56);
            button.Clicked += Button_Clicked;
            Children.Add(shadow, Constraint.Constant(0));
            Children.Add(button, Constraint.Constant(9), Constraint.Constant(6));
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (Clicked != null)
            {
                Clicked.Invoke(sender, e);
            }
        }

        public void AnimateIn()
        {

        }

        public void AnimateOut()
        {

        }
    }
}
