using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace ObservableCollectionExample
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Creates an ObservableCollectionShim that adapts this ObservableCollection.
        /// </summary>
        public static object ToObservableVector(this INotifyCollectionChanged collection)
        {
            Type genericItemType = collection.GetType().GenericTypeArguments[0];
            Type shimType = typeof(ObservableCollectionShim<>);
            Type genericShimType = shimType.MakeGenericType(new Type[] { genericItemType });
            return Activator.CreateInstance(genericShimType, new object[] { collection });
        }
    }

    /// <summary>
    /// Adapts an ObservableCollection to implement IObservableVector
    /// </summary>
    public class ObservableCollectionShim<T> : IObservableVector<T>
    {
        private ObservableCollection<T> _adaptee;

        public ObservableCollectionShim(ObservableCollection<T> adaptee)
        {
            _adaptee = adaptee;
            _adaptee.CollectionChanged += Adaptee_CollectionChanged;
        }

        /// <summary>
        /// Handles and adapts CollectionChanged events
        /// </summary>
        private void Adaptee_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            VectorChangedEventArgs args = new VectorChangedEventArgs();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    args.CollectionChange = CollectionChange.ItemInserted;
                    args.Index = (uint)e.NewStartingIndex;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    args.CollectionChange = CollectionChange.ItemRemoved;
                    args.Index = (uint)e.OldStartingIndex;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    args.CollectionChange = CollectionChange.ItemChanged;
                    args.Index = (uint)e.NewStartingIndex;
                    break;
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Move:
                    args.CollectionChange = CollectionChange.Reset;
                    break;
            }
            OnVectorChanged(args);
        }

        #region IObservableVector interface

        public event VectorChangedEventHandler<T> VectorChanged;

        protected void OnVectorChanged(IVectorChangedEventArgs args)
        {
            if (VectorChanged != null)
            {
                VectorChanged(this, args);
            }
        }

        #endregion

        #region IList<T> implementation

        public int IndexOf(T item)
        {
            return _adaptee.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _adaptee.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _adaptee.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _adaptee[index];
            }
            set
            {
                _adaptee[index] = value;
            }
        }

        public void Add(T item)
        {
            _adaptee.Add(item);
        }

        public void Clear()
        {
            _adaptee.Clear();
        }

        public bool Contains(T item)
        {
            return _adaptee.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _adaptee.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _adaptee.Count; ; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return _adaptee.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _adaptee.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _adaptee.GetEnumerator();
        }

        #endregion
    }

    public class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        public CollectionChange CollectionChange
        {
            get;
            set;
        }

        public uint Index
        {
            get;
            set;
        }
    }
}
