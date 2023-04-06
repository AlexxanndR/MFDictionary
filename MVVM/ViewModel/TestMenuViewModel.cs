using Avalonia.Animation;
using MFDictionary.Core;
using MFDictionary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MFDictionary.MVVM.ViewModel
{
    internal class TestMenuViewModel : ObservableObject
    {
        private int _maxWordsNum;

        public int MaxWordsNum
        {
            get { return _maxWordsNum; }
            set
            {
                _maxWordsNum = value;
                OnPropertyChanged();
            }
        }

        public List<string> TestTypes { get; private set; }

        private string _testType;

        public string TestType
        {
            get { return _testType; }
            set
            {
                _testType = value;
                OnPropertyChanged();
            }
        }

        public TestMenuViewModel() 
        {
            TestTypes = new List<string>()
            {
                "Writing words by using translation",
                "Writing translations by using word",
                "Writing words in examples"
            };
        }

        public RelayCommand ContinueCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    ApplicationContext.TestWordsNum = MaxWordsNum;

                    //TO DO

                    foreach (Window window in Application.Current.Windows)
                        if (window.GetType() == typeof(MainWindow))
                            (window as MainWindow).MainWindowFrame.Navigate(new Uri("MVVM/View/TestView.xaml", UriKind.RelativeOrAbsolute));
                });
            }
        }
    }
}
