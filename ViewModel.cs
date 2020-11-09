using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Reflection;
using System.Diagnostics.Contracts;

namespace OpGameCalc
{
    class ViewModel : INotifyPropertyChanged
    {
        bool _calculated = false;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool isCalculated
        {
            get => _calculated;
            set
            {
                _calculated = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(isCalculated)));
            }
        }
        public ItemsObservableCollection<Flank> FlanksCollection { get; set; } = new ItemsObservableCollection<Flank>() { DataFilterList = { "Number", "Select" } };
        public ItemsObservableCollection<DType> TypesCollection { get; set; } = new ItemsObservableCollection<DType>() { DataFilterList = { "Number", "Select" } };
        public NamedTablePresenter<IGameResultItem> ResultCollection { get; set; }
            = new NamedTablePresenter<IGameResultItem>("FlankName", "TypeName", "Result");
        public AttackForce AttackForce { get; set; } = new AttackForce();
        public ViewModel()
        {
            FlanksCollection.CollectionChanged += (o, e) => DataChange();
            FlanksCollection.DataChanged += (o, oe, ne) => DataChange();
            TypesCollection.CollectionChanged += (o, e) => DataChange();
            TypesCollection.DataChanged += (o, oe, ne) => DataChange();
        }
        public ViewModel(IGameDataModel loaded) : this()
        {
            int i = 1;
            foreach (var it in loaded.FlanksCollection)
            {
                FlanksCollection.Add(new Flank(it, i));
                i++;
            }
            i = 1;
            foreach (var it in loaded.TypesCollection)
            {
                TypesCollection.Add(new DType(it, i));
                i++;
            }
        }
        public SaveFile GetSave()
        {
            return new SaveFile()
            {
                FlanksCollection = new List<SaveFlankInstance>(FlanksCollection.Select(i => new SaveFlankInstance() { Name = i.Name, Rate = i.Rate })),
                TypesCollection = new List<SaveTypeInstance>(TypesCollection.Select(i => new SaveTypeInstance() { Name = i.Name, Count = i.Count, Efficiency = i.Efficiency })),
                ResultCollection = isCalculated ? new List<SaveResultInstance>(ResultCollection.Source.Select(i => new SaveResultInstance() { FlankName = i.FlankName, TypeName = i.TypeName, Result = i.Result })) : null
            };
        }

        public void DataChange() => isCalculated = false;
        public void ViewCalculate(IEnumerable<IGameResultItem> result)
        {
            if (result == null) return;
            ResultCollection.SetTable(result);
            isCalculated = true;
        }
    }
}
