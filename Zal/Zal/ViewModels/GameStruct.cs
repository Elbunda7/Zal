using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Zal.ViewModels
{
    public class GameStruct
    {
        private Entry nameEntry;
        private Entry varibEntry;
        private Image sort;
        private bool isSortingDown;

        public string Name {
            get {
                return nameEntry.Text;
            }
            set {
                nameEntry.Text = value;
            }
        }

        public string Variable {
            get {
                return varibEntry.Text;
            }
            set {
                varibEntry.Text = value;
            }
        }

        public bool IsSortingDown {
            get {
                return isSortingDown;
            }
            set {
                if (isSortingDown != value)
                {
                    SortImg_Tapped(null);
                }
            }
        }

        public GameStruct()
        {
            nameEntry = new Entry();
            varibEntry = new Entry();
            isSortingDown = true;
            sort = new Image
            {
                Source = "ic_sort_down_24dp.png",
            };
            sort.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(SortImg_Tapped) });
        }

        private void SortImg_Tapped(object obj)
        {
            if (isSortingDown) sort.Source = "ic_sort_up_24dp.png";
            else sort.Source = "ic_sort_down_24dp.png";
            isSortingDown = !isSortingDown;
        }

        public void SetVisibility(bool isVisible)
        {
            nameEntry.IsVisible = isVisible;
            varibEntry.IsVisible = isVisible;
            sort.IsVisible = isVisible;
        }

        public void MapToGrid(Grid grid, int row)
        {
            grid.Children.Add(nameEntry, 0, row);
            grid.Children.Add(varibEntry, 1, row);
            grid.Children.Add(sort, 2, row);
        }
    }
}
