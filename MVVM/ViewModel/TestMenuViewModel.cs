using BespokeFusion;
using MFDictionary.Core;
using MFDictionary.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MFDictionary.MVVM.ViewModel
{
    internal class TestMenuViewModel : ObservableObject
    {
        private int _testWordsNum;

        public int TestWordsNum
        {
            get { return _testWordsNum; }
            set
            {
                _testWordsNum = value;
                OnPropertyChanged();
            }
        }

        public List<string> TestTypes { get; private set; }

        private string _selectedTestType;

        public string SelectedTestType
        {
            get { return _selectedTestType; }
            set
            {
                _selectedTestType = value;
                OnPropertyChanged();
            }
        }

        public TestMenuViewModel() 
        {
            TestTypes = TestType.TypesInStrings;
        }

        public RelayCommand ContinueCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    ApplicationContext.TestWordsNum = TestWordsNum;

                    if (TestWordsNum <= 0)
                    {
                        CustomMessageBox.ShowWarning("Incorrect words number!");
                        return;
                    }

                    if (String.IsNullOrWhiteSpace(SelectedTestType))
                    {
                        CustomMessageBox.ShowWarning("No test type selected!");
                        return;
                    }

                    ApplicationContext.SelectedTestType = TestType.StringToTypeRatio[SelectedTestType];

                    foreach (Window window in Application.Current.Windows)
                        if (window.GetType() == typeof(MainWindow))
                            (window as MainWindow).MainWindowFrame.Navigate(new Uri("MVVM/View/TestView.xaml", UriKind.RelativeOrAbsolute));
                });
            }
        }
    }
}
