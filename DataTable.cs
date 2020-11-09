using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace OpGameCalc
{
    public class DataTable : Grid
    {
        public static readonly DependencyProperty RowCountProperty;
        public static readonly DependencyProperty ColumnCountProperty;
        public static readonly DependencyProperty TableSourceProperty;

        public static readonly DependencyProperty DataStyleProperty;
        public static readonly DependencyProperty ColumnStyleProperty;
        public static readonly DependencyProperty RowStyleProperty;
        public static readonly DependencyProperty ZeroStyleProperty;

        Dictionary<(int Column, int Row), ContentControl> createdList = new Dictionary<(int Column, int Row), ContentControl>();

        public static readonly RoutedEvent TableSourceChangedEvent;
        public event RoutedEventHandler TableSourceChanged
        {
            add
            {
                AddHandler(TableSourceChangedEvent, value);
            }
            remove
            {
                RemoveHandler(TableSourceChangedEvent, value);
            }
        }
        static DataTable()
        {
            RowCountProperty = DependencyProperty.Register(nameof(RowCount), typeof(int), typeof(DataTable),new PropertyMetadata(SetRowCallBack));
            ColumnCountProperty = DependencyProperty.Register(nameof(ColumnCount), typeof(int), typeof(DataTable), new PropertyMetadata(SetColumnCallBack));
            TableSourceProperty = DependencyProperty.Register(nameof(TableSource), typeof(object[,]), typeof(DataTable), new PropertyMetadata(SetTableSourceCallBack));

            DataStyleProperty = DependencyProperty.Register(nameof(DataStyle), typeof(Style), typeof(DataTable), new PropertyMetadata(), isValidStyle);
            ColumnStyleProperty = DependencyProperty.Register(nameof(ColumnStyle), typeof(Style), typeof(DataTable), new PropertyMetadata(), isValidStyle);
            RowStyleProperty = DependencyProperty.Register(nameof(RowStyle), typeof(Style), typeof(DataTable), new PropertyMetadata(), isValidStyle);
            ZeroStyleProperty = DependencyProperty.Register(nameof(ZeroStyle), typeof(Style), typeof(DataTable), new PropertyMetadata(), isValidStyle);

            TableSourceChangedEvent = EventManager.RegisterRoutedEvent(nameof(TableSourceChanged),
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DataTable));
        }
        public static bool isValidStyle(object value)
        {
            return value == null || ((Style)value).TargetType == typeof(ContentControl);
        }

        #region Styles
        public Style ColumnStyle
        {
            get
            {
                return (Style)GetValue(ColumnStyleProperty);
            }
            set
            {
                SetValue(ColumnStyleProperty, value);
            }
        }
        public Style RowStyle
        {
            get
            {
                return (Style)GetValue(RowStyleProperty);
            }
            set
            {
                SetValue(RowStyleProperty, value);
            }
        }
        public Style ZeroStyle
        {
            get
            {
                return (Style)GetValue(ZeroStyleProperty);
            }
            set
            {
                SetValue(ZeroStyleProperty, value);
            }
        }
        public Style DataStyle
        {
            get
            {
                return (Style)GetValue(DataStyleProperty);
            }
            set
            {
                SetValue(DataStyleProperty, value);
            }
        }
        #endregion
        public object[,] TableSource
        {
            get { return (object[,])GetValue(TableSourceProperty); }
            set
            {
                SetTableSourceProp(value);
                SetValue(TableSourceProperty, value);
                RaiseEvent(new RoutedEventArgs(TableSourceChangedEvent, this));
            }
        }
        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set
            {
                SetRowProp(value, RowCount);
                SetValue(RowCountProperty, value);
            }
        }
        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set
            {
                SetColumnProp(value, ColumnCount);
                SetValue(ColumnCountProperty, value);
            }
        }
        static void SetRowCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataTable)d).SetRowProp((int)e.NewValue, (int)e.OldValue);
        }
        static void SetColumnCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataTable)d).SetColumnProp((int)e.NewValue,(int)e.OldValue);
        }
        static void SetTableSourceCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataTable)d).SetTableSourceProp((object[,])e.NewValue);
        }
        public void SetColumnProp(int value, int old)
        {
            if (old < value)
            {
                for (int i = old; i < value; i++)
                {
                    ColumnDefinitions.Add(new ColumnDefinition());
                    for (int j = 0; j < RowCount; j++)
                    {
                        var now = new ContentControl() { Style = ChooseStyle(i, j) };
                        createdList.Add((i, j), now);
                        Children.Add(now);
                        SetRow(now, j);
                        SetColumn(now, i);
                    }
                }
            }
            if (old > value)
            {
                ColumnDefinitions.RemoveRange(value, old - value);
                for (int i = value; i < old; i++)
                {             
                    for (int j = 0; j < RowCount; j++)
                    {
                        createdList.Remove((i, j));
                    }
                }
            }
        }
        public void SetRowProp(int value, int old)
        {
            if (old < value)
            {
                for (int i = old; i < value; i++)
                {
                    RowDefinitions.Add(new RowDefinition());
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        var now = new ContentControl() { Style = ChooseStyle(j, i) };
                        createdList.Add((j, i), now);
                        Children.Add(now);
                        SetRow(now, i);
                        SetColumn(now, j);
                    }
                }
            }
            if (old > value)
            {
                RowDefinitions.RemoveRange(value, old - value);
                for (int i = value; i < old; i++)
                {
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        createdList.Remove((j, i));
                    }
                }
            }
        }
        Style ChooseStyle(int columnIndx, int rowIndx)
        {
            if (columnIndx == 0 && rowIndx == 0)
            {
                return ZeroStyle;
            }
            if (rowIndx == 0)
            {
                return ColumnStyle;
            }
            if (columnIndx == 0)
            {
                return RowStyle;
            }
            return DataStyle;
        }
        public void SetTableSourceProp(object[,] value)
        {
            if (value == null) return;
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                {
                    createdList[(j, i)].DataContext = value[j, i];
                }
            }
        }
    }
}
