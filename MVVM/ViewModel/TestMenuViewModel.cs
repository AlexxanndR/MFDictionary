using Avalonia.Animation;
using BespokeFusion;
using MFDictionary.Core;
using MFDictionary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
            TestTypes = ApplicationContext.TestTypes;
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
                    ApplicationContext.TestWordsNum = MaxWordsNum;

                    if (MaxWordsNum <= 0)
                    {
                        ShowMessageBox("Incorrect words number!");
                        return;
                    }

                    if (String.IsNullOrWhiteSpace(TestType))
                    {
                        ShowMessageBox("No test type selected!");
                        return;
                    }

                    ApplicationContext.SelectedTestType = TestType;

                    foreach (Window window in Application.Current.Windows)
                        if (window.GetType() == typeof(MainWindow))
                            (window as MainWindow).MainWindowFrame.Navigate(new Uri("MVVM/View/TestView.xaml", UriKind.RelativeOrAbsolute));
                });
            }
        }
    }
}
