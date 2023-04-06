using BespokeFusion;
using MFDictionary.Core;
using MFDictionary.Helpers;
using MFDictionary.MVVM.Model;
using MFDictionary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MFDictionary.MVVM.ViewModel
{
    internal class TestViewModel : ObservableObject
    {
        private WordsDboAdapter _wordsDboAdapter;

        private List<Word> _words;

        private Word _currentWord;

        public Word CurrentWord
        {
            get { return _currentWord; }
            set
            {
                _currentWord = value;
                OnPropertyChanged();
            }
        }

        public TestViewModel()
        {
            _wordsDboAdapter = new WordsDboAdapter();
            _words = new List<Word>();

        }
        private void Init()
        {
            long recordsCount = _wordsDboAdapter.GetRecordsCount();

            var wordsNum = ApplicationContext.TestWordsNum;

            if (recordsCount < wordsNum)
            {
                CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                {
                    FontFamily = new FontFamily("Oswald Light"),
                    TxtMessage = { Text = String.Format("There are fewer than {0} words in your dictionary!", wordsNum),
                                   Foreground = Brushes.Black,
                                   FontSize = 20 },
                    TxtTitle = { Text = "Warning", Foreground = Brushes.Black },
                    BtnOk = { Content = "Ok", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black },
                    BtnCancel = { Content = "Cancel", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black },
                    MainContentControl = { Background = Brushes.White },
                    TitleBackgroundPanel = { Background = Brushes.MistyRose },
                    BorderThickness = new Thickness(0),
                    WindowStyle = WindowStyle.None
                };
                msg.Show();

                foreach (Window window in Application.Current.Windows)
                    if (window.GetType() == typeof(MainWindow))
                        (window as MainWindow).MainWindowFrame.Navigate(new Uri("MVVM/View/TestMenuView.xaml", UriKind.RelativeOrAbsolute));

                return;
            }

            _words = _wordsDboAdapter.GetRandomWords(10);

            CurrentWord = _words.FirstOrDefault();
        }

        public RelayCommand WindowLoadedCommand
        {
            get
            {
                return new RelayCommand((loaded) =>
                {
                    Init();
                });
            }
        }

/*        public RelayCommand GetResultCommand
        {
            get
            {
                return new RelayCommand((loaded) =>
                {
                    ResultButtonPressed = true;
                    int correctAnswersNum = 0;

                    if (TestWordsList.Count == 0)
                    {
                        MessageBox.Show("There are no words in test!", "Warning",
                                        MessageBoxButton.OK, MessageBoxImage.Warning,
                                        MessageBoxResult.OK, MessageBoxOptions.None);
                        return;
                    }

                    foreach (TestWord word in TestWordsList)
                    {
                        if (String.Equals(word.GivenTranslation, word.Translation, StringComparison.OrdinalIgnoreCase))
                        {
                            correctAnswersNum++;
                            word.ResultColor = "Blue";
                        } 
                        else
                            word.ResultColor = "Red";
                    }

                    float passPercent = ((float)correctAnswersNum / 10) * 100;

                    MessageBox.Show(String.Format("You passed the test by {0:0.00}%", passPercent), "Information",
                                    MessageBoxButton.OK, MessageBoxImage.Information,
                                    MessageBoxResult.OK, MessageBoxOptions.None);
                });
            }
        }*/

    }
}
