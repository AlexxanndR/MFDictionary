using Avalonia.Controls.Primitives;
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
using System.Threading;
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

        private int _wordIndex;

        private string _resultBorderValue;

        public string ResultBorderValue
        {
            get { return _resultBorderValue; }
            set
            {
                _resultBorderValue = value;
                OnPropertyChanged();
            }
        }

        private string _resultImageUri;

        public string ResultImageUri
        {
            get { return _resultImageUri; }
            set
            {
                _resultImageUri = value;
                OnPropertyChanged();
            }
        }

        private int _progressBarValue;

        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
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
                _examplesTranslation = value;
                OnPropertyChanged();
            }
        }

        public TestViewModel()
        {
            _wordsDboAdapter = new WordsDboAdapter();
            _words = new List<Word>();
            _resultBorderValue = "Black";
            _progressBarValue = 0;
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

        private void SetTestFields()
        {
            Text = String.Empty;
            Translations = null;
            Examples = null;
            ExamplesTranslation = null;

            switch (ApplicationContext.SelectedTestType)
            {
                case TestType.Type.WordByTranslations:
                    Translations = _currentWord.Translation;
                    Examples = _currentWord.Examples;
                    break;
                case TestType.Type.TranslationsByWord:
                    Text = _currentWord.Text;
                    Examples = _currentWord.ExamplesTranslation;
                    break;
                case TestType.Type.WordByExamples:
                    Examples = Parser.ParseExamplesForTest(_currentWord.Examples, _currentWord.Text);
                    ExamplesTranslation = _currentWord.ExamplesTranslation;
                    break;
            }  
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

            _currentWord = _words.FirstOrDefault();
            _wordIndex = 0;

            SetTestFields();
        }

        public RelayCommand WindowLoadedCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    Init();
                });
            }
        }

        public RelayCommand NextWordCommand
        {
            get
            {
                return new RelayCommand(async (obj) =>
                {
                    bool isRightAnswer = false;

                    switch (ApplicationContext.SelectedTestType)
                    {
                        case TestType.Type.WordByTranslations:
                            if (Answer == _currentWord.Text)
                                isRightAnswer = true;
                            else
                                Answer = _currentWord.Text;
                            break;
                        case TestType.Type.TranslationsByWord:
                            if (_currentWord.Translation.Any(x => x == Answer))
                                isRightAnswer = true;
                            else
                                Answer = _currentWord.Translation.FirstOrDefault();
                            break;
                        case TestType.Type.WordByExamples:
                            if (Answer == _currentWord.Text)
                                isRightAnswer = true;
                            else
                                Answer = _currentWord.Text;
                            break;
                    }

                    if (isRightAnswer)
                    {
                        ResultBorderValue = "Blue";
                        ResultImageUri = "../../Icons/done_icon.png";
                    }
                    else
                    {
                        ResultBorderValue = "Red";
                        ResultImageUri = "../../Icons/wrong_icon.png";
                    }

                    bool isProgressEnd = false;

                    await Task.Run(() =>
                    {
                        for (int i = 0; i <= 100; i++)
                        {
                            ProgressBarValue = i;
                            Thread.Sleep(25);
                        }

                        ProgressBarValue = 0;
                        isProgressEnd = true;
                    });

                    if (isProgressEnd)
                    {
                        _wordIndex++;

                        if (_words.ElementAtOrDefault(_wordIndex) != null)
                            _currentWord = _words[_wordIndex];
                        else
                        {
                            //TO DO
                        }

                        SetTestFields();

                        ResultBorderValue = "Black";
                        ResultImageUri = String.Empty;
                    }
                });
            }
        }
    }
}
