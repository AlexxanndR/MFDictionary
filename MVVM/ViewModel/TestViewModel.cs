using MFDictionary.Core;
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

        private ObservableCollection<Word> _wordsList;
        public ObservableCollection<Word> WordsList
        {
            get { return _wordsList; }
            set
            {
                _wordsList = value;
                OnPropertyChanged();
            }
        }

        public TestViewModel()
        {
            _wordsDboAdapter = new WordsDboAdapter();
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

            WordsList = _wordsDboAdapter.GetRandomWords(10);
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
