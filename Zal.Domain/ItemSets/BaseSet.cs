using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Domain.ItemSets
{
    public class BaseSet : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        bool isBusy = false;
        public bool IsBusy {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;

            backingStore = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        internal async Task<T> ExecuteTask<T>(Task<T> task)
        {
            IsBusy = true;
            T respond = await task;
            IsBusy = false;
            return respond;
        }
    }
}
