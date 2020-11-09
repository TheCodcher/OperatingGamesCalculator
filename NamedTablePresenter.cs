using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Documents;

namespace OpGameCalc
{
    class NamedTablePresenter<T> : INotifyPropertyChanged
    {
        private int _tableRow;
        public int TableRow
        {
            get { return _tableRow; }
            set
            {
                _tableRow = value;
                Changed(nameof(TableRow));
            }
        }


        private int _tableColumn;
        public int TableColumn
        {
            get { return _tableColumn; }
            set
            {
                _tableColumn = value;
                Changed(nameof(TableColumn));
            }
        }


        private IEnumerable<T> _source;
        public IEnumerable<T> Source
        {
            get { return _source; }
            set
            {
                _source = value;
                Changed(nameof(Source));
            }
        }


        private object[,] _tableViewSource;
        public object[,] TableViewSource
        {
            get { return _tableViewSource; }
            set
            {
                _tableViewSource = value;
                Changed(nameof(TableViewSource));
            }
        }

        PropertyInfo ColumnNameProp;
        PropertyInfo RowNameProp;
        PropertyInfo ValueNameProp;

        void Changed(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public NamedTablePresenter(string columnNameProp, string rowNameProp, string valueNameProp)
        {
            var ty = typeof(T);
            ColumnNameProp = ty.GetProperty(columnNameProp);
            RowNameProp = ty.GetProperty(rowNameProp);
            ValueNameProp = ty.GetProperty(valueNameProp);
        }

        public void SetTable(IEnumerable<T> source)
        {
            var col = new Separator(1);
            var row = new Separator(1);
            var prep = PrepareTable(source);
            foreach(var item in prep)
            {
                col.GetPlace(item.ColumnName);
                row.GetPlace(item.RowName);
            }
            SetTable(prep, col, row, row.Count, col.Count);
        }
        IEnumerable<(object ColumnName, object RowName, object Value)> PrepareTable(IEnumerable<T> source)
        {
            Source = source;
            return source.Select(item => (ColumnNameProp.GetValue(item), RowNameProp.GetValue(item), ValueNameProp.GetValue(item)));

            //foreach (var item in source)
            //{
            //    var column = ColumnNameProp.GetValue(item);
            //    var row = RowNameProp.GetValue(item);
            //    var val = ValueNameProp.GetValue(item);
            //    yield return (column, row, val);
            //}
        }
        void SetTable(IEnumerable<(object ColumnName, object RowName, object Value)> preparedSource, Separator colSep, Separator rowSep, int rowCount, int columnCount)
        {
            var result = new object[columnCount, rowCount];
            TableColumn = columnCount;
            TableRow = rowCount;
            foreach (var item in preparedSource)
            {
                int colIndx = colSep.GetPlace(item.ColumnName);
                int rowIndx = rowSep.GetPlace(item.RowName);
                result[colIndx, 0] ??= item.ColumnName;
                result[0, rowIndx] ??= item.RowName;
                result[colIndx, rowIndx] = item.Value;
            }
            TableViewSource = result;
        }
        public void SetTable(IEnumerable<T> source, int rowCount, int columnCount)
        {
            SetTable(PrepareTable(source), new Separator(1), new Separator(1), rowCount, columnCount);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fabric">1 параметр - имя столбца, 2ой - имя строки, 3ий - значение</param>
        public void SetTable(object[,] source, int rowCount, int columnCount, Func<object,object,object,T> fabric)
        {
            TableColumn = columnCount;
            TableRow = rowCount;
            TableViewSource = source;
            List<T> result = new List<T>();
            if (fabric != null)
            {
                for (int i = 1; i < TableColumn; i++)
                {
                    for (int j = 1; j < TableRow; j++)
                    {
                        result.Add(fabric(TableViewSource[i, 0], TableViewSource[0, j], TableViewSource[i, j]));
                    }
                }
            }
            Source = result;
        }
        class Separator
        {
            int toplaceCounter = 0;
            public Separator(int startindx)
            {
                toplaceCounter = startindx;
            }
            Dictionary<object, int> toplaceDict = new Dictionary<object, int>();
            public int GetPlace(object name)
            {
                int indx;
                if (!toplaceDict.TryGetValue(name,out indx))
                {
                    toplaceDict.Add(name, toplaceCounter);
                    indx = toplaceCounter;
                    toplaceCounter++;
                }
                return indx;
            }
            public int Count => toplaceCounter;
        }
    }
}
