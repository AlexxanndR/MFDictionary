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

        private void ShowMessageBox(string message)
        {
            CustomMaterialMessageBox msg = new CustomMaterialMessageBox
            {
                FontFamily = new FontFamily("Oswald Light"),
                TxtMessage = { Text = String.Format(message),
                               Foreground = Brushes.Black,
                               FontSize = 20, 
                               HorizontalAlignment = HorizontalAlignment.Center },
                TxtTitle = { Text = "Warning", Foreground = Brushes.Black },
                BtnOk = { Content = "Ok", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black },
                BtnCancel = { Content = "Cancel", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black },
                MainContentControl = { Background = Brushes.White },
                TitleBackgroundPanel = { Background = Brushes.MistyRose },
                BorderThickness = new Thickness(0),
                WindowStyle = WindowStyle.None
            };

            msg.Show();
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
                        ShowMessageBox("Incorrect words number!");
                        return;
                    }

                    if (String.IsNullOrWhiteSpace(SelectedTestType))
                    {
                        ShowMessageBox("No test type selected!");
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
