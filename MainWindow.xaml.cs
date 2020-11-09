using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace OpGameCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel model = new ViewModel();
        IDataLoader dataLoader = new DataLoader();
        IGameCalculator gameCalculator = new CppOpGameCalcInct();
        public MainWindow()
        {
            InitializeComponent();
            typesGrid.DataContextChanged += (s, e) => fkcheckbox.DataContext = e.NewValue;
            DataContext = model;
        }
        //load
        private void DataLoadClick(object sender, RoutedEventArgs e)
        {
            IGameDataModel data;
            if (dataLoader.LoadData(out data))
            {
                model = new ViewModel(data);
                DataContext = model;
                model.ViewCalculate(data.ResultCollection);
                gameCalculator.DefensePotential = data.DefensePotential;
            }
        }
        //save
        private void DataSaveClick(object sender, RoutedEventArgs e)
        {
            var get = model.GetSave();
            get.DefensePotential = gameCalculator.DefensePotential;
            dataLoader.SaveData(get);
        }
        private void RemoveCheckedClick(object sender, RoutedEventArgs e)
        {
            var collection = (dynamic)((Control)sender).DataContext;
            int k = 0;
            for (var i = 0; i < collection.Count; i++)
            {
                var tempI = (DataItemModel)collection[i];
                if (tempI.Select)
                {
                    collection.Remove((dynamic)tempI);
                    i--;
                    k++;
                    continue;
                }
                tempI.Number -= k;
            }
        }
        private void AddRowClick(object sender, RoutedEventArgs e)
        {
            var collection = ((Control)sender).DataContext;
            var item = (DataItemModel)collection.GetType().GetGenericArguments().First().GetConstructor(new Type[] { }).Invoke(null);
            dynamic col = collection;
            item.Number = col.Count + 1;
            //item.Name = item.Number.ToString();
            col.Add((dynamic)item);
            ((UIElement)sender).RaiseEvent(new MouseWheelEventArgs(Mouse.PrimaryDevice, (int)DateTime.Now.Ticks, int.MinValue) { RoutedEvent = MouseWheelEvent });
        }
        
        private async void CalcClick(object sender, RoutedEventArgs e)
        {
            if (model.isCalculated) model.DataChange();
            var bt = (Button)sender;
            if (!ButtonProgressAssist.GetIsIndeterminate(bt))
            {
                ButtonProgressAssist.SetIsIndeterminate(bt, true);
                var res = await Calculate();
                ButtonProgressAssist.SetIsIndeterminate(bt, false);
                model.ViewCalculate(res);
            }
        }
        private async Task<IEnumerable<IGameResultItem>> Calculate()
        {
            IEnumerable<IGameResultItem> Do()
            {
                gameCalculator.Inicialize(model.FlanksCollection, model.TypesCollection);
                return gameCalculator.Calculate();
            }
            return await Task.Run(Do);
        }

        private void DataGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var clonedEvent = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent
            };
            ((UIElement)sender).RaiseEvent(clonedEvent);
        }
        private void CheckBoxSelectChange(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox)sender;
            var selected = (bool)box.IsChecked;
            var col = (dynamic)box.DataContext;
            foreach (var i in col)
            {
                ((DataItemModel)i).Select = selected;
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var bt = (Button)sender;
            if (!ButtonProgressAssist.GetIsIndeterminate(bt))
            {
                ButtonProgressAssist.SetIsIndicatorVisible(bt, true);
                ButtonProgressAssist.SetIsIndeterminate(bt, true);
                var res = await Task.Run(Do);
                ButtonProgressAssist.SetIsIndeterminate(bt, false);
                ButtonProgressAssist.SetIsIndicatorVisible(bt, false);
                model.AttackForce.Result = res;
            }

            double Do()
            {
                return gameCalculator.AssuredResult(model.AttackForce.Force);
            }
        }
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }
        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
            ((Button)sender).Click -= MaximizeWindow;
            ((Button)sender).Click += RestoreWindow;
            ((Button)sender).Content = Resources["restoreWindowIcon"];
        }
        private void RestoreWindow(object sender, RoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
            ((Button)sender).Click -= RestoreWindow;
            ((Button)sender).Click += MaximizeWindow;
            ((Button)sender).Content = Resources["maximizeWindowIcon"];
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
