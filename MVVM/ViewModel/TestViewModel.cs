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

namespace MFDictionary.MVVM.ViewModel
{
    internal class TestViewModel : ObservableObject
    {
        WordsDboAdapter _wordsDboAdapter;

        private ObservableCollection<TestWord> _testWordsList;

        public ObservableCollection<TestWord> TestWordsList
        {
            get { return _testWordsList; }
            set
            {
                _testWordsList = value;
                OnPropertyChanged();
            }
        }

        private bool _resultButtonPressed;

        public bool ResultButtonPressed
        {
            get { return _resultButtonPressed; }
            set
            {
                _resultButtonPressed = value;
                OnPropertyChanged();
            }
        }

        public TestViewModel()
        {
            _wordsDboAdapter = new WordsDboAdapter();
            _testWordsList= new ObservableCollection<TestWord>();
            ResultButtonPressed = false;
        }

        private void Init()
        {
            MessageBoxResult messageBoxResult = MessageBoxResult.None;

            long recordsCount = _wordsDboAdapter.GetRecordsCount();

            if (recordsCount < 10)
                messageBoxResult = MessageBox.Show("There are less than 10 words in the dictionary. Must be at least 10!", "Warning",
                                   MessageBoxButton.OK, MessageBoxImage.Warning,
                                   MessageBoxResult.OK, MessageBoxOptions.None);

            if (messageBoxResult == MessageBoxResult.OK)
                return;

            List<Word> words = _wordsDboAdapter.GetRandomWords(10);          
/*            foreach (Word word in words)
                TestWordsList.Add(word.GetTestWord());*/
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

        public RelayCommand GetResultCommand
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
        }

    }
}
