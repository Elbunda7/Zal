using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Zal.Models
{
    public enum MenuItemType
    {
        Browse,
        About
    }

    public class HomeMenuItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }
        public string Icon { get; set; }
        public Type TargetType { get; set; }

        private bool isSelected;
        public bool IsSelected {
            get {
                return isSelected;
            }
            set {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public HomeMenuItem(string title, Type targetType, string icon)
        {
            Title = title;
            TargetType = targetType;
            Icon = icon;
            isSelected = false;
        }
    }
}
