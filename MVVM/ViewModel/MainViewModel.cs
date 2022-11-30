using MFDictionary.Core;
using MFDictionary.MVVM.Model;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MFDictionary.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private void NavigateToPage(string pageName)
        {
            foreach (Window window in Application.Current.Windows)
                if (window.GetType() == typeof(MainWindow))
                    (window as MainWindow).MainWindowFrame.Navigate(new Uri(string.Format("{0}{1}{2}", "MVVM/View/", pageName, "View.xaml"), UriKind.RelativeOrAbsolute));
        }

        public MainViewModel() { }

        public RelayCommand MenuSelectionCommand
        {
            get
            {
                return new RelayCommand((pageName) =>
                {
                    NavigateToPage(pageName as string);
                });
            }
        }
    }
}
