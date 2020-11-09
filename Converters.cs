using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace OpGameCalc
{
    class RoundConverter : IValueConverter
    {
        const int BASE_ROUND = 3;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            try
            {
                var temp = Math.Round((double)value, BASE_ROUND);
                return temp;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    class VisibleConverter : IValueConverter
    {
        const Visibility VISIBLE = Visibility.Visible;
        const Visibility INVISIBLE = Visibility.Collapsed;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            try
            {
                return (bool)value ? VISIBLE : INVISIBLE;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            try
            {
                return (Visibility)value == VISIBLE;
            }
            catch
            {
                return value;
            }
        }
    }
}
