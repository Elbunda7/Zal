using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;

namespace Zal.Domain.Tools.ARSets
{
    public class ObservableSortedSet<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : IActiveRecord {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime LastSynchronization { get; set; } = ZAL.DATE_OF_ORIGIN;

        readonly SortedSet<T> InnerCollection;

        public ObservableSortedSet(IComparer<T> comparer) {
            InnerCollection = new SortedSet<T>(comparer);
        }

        public ObservableSortedSet(IEnumerable<T> enumerable, IComparer<T> comparer) {
            InnerCollection = new SortedSet<T>(enumerable, comparer);
        }

        public int Count => InnerCollection.Count;

        public bool IsReadOnly => (InnerCollection as ICollection<T>).IsReadOnly;

        public void Add(T item) {
            InnerCollection.Add(item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void AddAll(IEnumerable<T> items) {
            foreach (T item in items) {
                InnerCollection.Add(item);
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public void AddOrUpdateAll(IEnumerable<T> items) {
            foreach (T item in items) {
                AddOrUpdate(item);
            }
        }

        public void AddOrUpdate(T item) {
            if (InnerCollection.Any(x => x.Id == item.Id)) {
                InnerCollection.Remove(InnerCollection.Single(x => x.Id == item.Id));
            }
            InnerCollection.Add(item);
        }

        public void Clear() {
            LastSynchronization = ZAL.DATE_OF_ORIGIN;
            InnerCollection.Clear();
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void RemoveByIds(int[] ids) {
            foreach (int id in ids) {
                RemoveById(id);
            }
        }

        public bool RemoveById(int id) {
            T toDelete = InnerCollection.Single(x => x.Id == id);
            return Remove(toDelete);
        }

        public bool Remove(T item) {
            bool isRemoved = InnerCollection.Remove(item);
            if (isRemoved) {
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
            return isRemoved;
        }

        public bool Contains(T item) {
            return InnerCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            InnerCollection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator() {
            return InnerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args) {
            if (CollectionChanged != null) {
                CollectionChanged.Invoke(this, args);
            }
        }
    }
}
