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

        private string _answer;

        public string Answer
        {
            get { return _answer; }
            set
            {
                _answer = value;
                OnPropertyChanged();
            }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        private List<string> _translations;

        public List<string> Translations
        {
            get { return _translations; }
            set
            {
                _translations = value;
                OnPropertyChanged();
            }
        }

        private List<string> _examples;

        public List<string> Examples
        {
            get { return _examples; }
            set
            {
                _examples = value;
                OnPropertyChanged();
            }
        }

        private List<string> _examplesTranslation;

        public List<string> ExamplesTranslation
        {
            get { return _examplesTranslation; }
            set
            {
                _examples = value;
                OnPropertyChanged();
            }
        }

        public TestViewModel()
        {
            _wordsDboAdapter = new WordsDboAdapter();
            _words = new List<Word>();
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

        private void Init()
        {
            long recordsCount = _wordsDboAdapter.GetRecordsCount();

            var wordsNum = ApplicationContext.TestWordsNum;

            if (recordsCount < wordsNum)
            {
                ShowMessageBox(String.Format("There are fewer than {0} words in your dictionary!", wordsNum));

                foreach (Window window in Application.Current.Windows)
                    if (window.GetType() == typeof(MainWindow))
                        (window as MainWindow).MainWindowFrame.Navigate(new Uri("MVVM/View/TestMenuView.xaml", UriKind.RelativeOrAbsolute));

                return;
            }

            _words = _wordsDboAdapter.GetRandomWords(wordsNum);

            CurrentWord = _words.FirstOrDefault();

            if (ApplicationContext.SelectedTestType == ApplicationContext.TestTypes[0])
            {
                Translations = CurrentWord.Translation;
                Examples = CurrentWord.Examples;
            }
            else if (ApplicationContext.SelectedTestType == ApplicationContext.TestTypes[1])
            {
                Text = CurrentWord.Text;
                Examples = CurrentWord.ExamplesTranslation;
            } 
            else if (ApplicationContext.SelectedTestType == ApplicationContext.TestTypes[1])
            {
                //TO DO
            }
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
    }
}
