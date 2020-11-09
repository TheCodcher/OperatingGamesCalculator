using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Windows.Documents;

namespace OpGameCalc
{
    interface IDataChanged
    {
        event NotifyDataChangedEventHandler DataChanged;
    }
        delegate void NotifyDataChangedEventHandler(object sender, object olddata = null, object newdata = null);
    class ItemsObservableCollection<T> : List<T>, INotifyCollectionChanged, IDataChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event NotifyDataChangedEventHandler DataChanged;

        public List<string> DataFilterList { get; set; } = new List<string>();
        public bool BlackListActive { get; set; } = true;
        public new void Add(T obj)
        {
            SubscribeManage(obj);
            base.Add(obj);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj));
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (BlackListActive && !DataFilterList.Contains(e.PropertyName) || !BlackListActive && DataFilterList.Contains(e.PropertyName))
            {
                DataChanged?.Invoke(this, null, sender);
            }
        }
        private void SubscribeManage(object obj, bool sub = true)
        {
            var changeObj = obj as INotifyPropertyChanged;
            if (changeObj != null)
            {
                if (sub)
                {
                    changeObj.PropertyChanged += ItemPropertyChanged;
                }
                else
                {
                    changeObj.PropertyChanged -= ItemPropertyChanged;
                }
            }
        }
        public new void Remove(T obj)
        {
            SubscribeManage(obj, false);
            base.Remove(obj);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj));
        }

        #region IList<T>,IList
        
        #endregion
    }
}
