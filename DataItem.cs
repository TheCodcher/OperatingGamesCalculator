using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Reflection;

namespace OpGameCalc
{
    abstract class BiggerZeroValidation : IDataErrorInfo
    {
        public BiggerZeroValidation(params string[] propNames)
        {
            validationProps = propNames;
        }
        public virtual string Error => "value must be bigger zero";

        public string this[string columnName]
        {
            get
            {
                return isValid(columnName) ? String.Empty : Error;
            }
        }
        string[] validationProps;
        bool isValid(string name)
        {
            if (validationProps.Contains(name))
            {
                return (double)GetType().GetProperty(name).GetValue(this) >= 0;
            }
            else
            {
                return true;
            }
        }
    }
    abstract class DataItemModel : BiggerZeroValidation, INotifyPropertyChanged
    {
        bool _select;
        int _number;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                PropertyChange(nameof(Number));
            }
        }
        public bool Select
        {
            get => _select;
            set
            {
                _select = value;
                PropertyChange(nameof(Select));
            }
        }
        protected void PropertyChange(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        string _name;
        public string Name
        {
            get => _name;
            set 
            { 
                _name = value;
                PropertyChange(nameof(Name));
            }
        }


        public Type ItemType;

        public event PropertyChangedEventHandler PropertyChanged;

        protected DataItemModel(Type itemType, params string[] propNames) : base(propNames)
        {
            ItemType = itemType;
        }
    }
    class Flank : DataItemModel, IFlank
    {
        public Flank() : base(typeof(Flank), nameof(Rate)) { }
        double _rate;
        public double Rate 
        { 
            get => _rate;
            set
            {
                _rate = value;
                PropertyChange(nameof(Rate));
            }
        }
        public Flank(IFlank flank, int number) : this()
        {
            Name = flank.Name;
            Rate = flank.Rate;
            Number = number;
        }

    }
    class DType : DataItemModel, IDefType
    {
        public DType() : base(typeof(DType), nameof(Efficiency), nameof(Count)) { }
        double _efficiency;
        public double Efficiency 
        {
            get => _efficiency;
            set
            {
                _efficiency = value;
                PropertyChange(nameof(Efficiency));
            }
        }
        double _count;
        public double Count
        {
            get => _count;
            set
            {
                _count = value;
                PropertyChange(nameof(Count));
            }
        }
        public DType(IDefType defType, int number) : this()
        {
            Name = defType.Name;
            Efficiency = defType.Efficiency;
            Count = defType.Count;
            Number = number;
        }
    }
    class AttackForce : BiggerZeroValidation, INotifyPropertyChanged
    {
        public double Force { get; set; }
        double _result;
        public double Result
        {
            get => _result;
            set
            {
                _result = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
            }
        }
        public AttackForce() : base(nameof(Force))
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
