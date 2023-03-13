using MFDictionary.Core;
using MFDictionary.Helpers;
using MFDictionary.MVVM.Model;
using MFDictionary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MFDictionary.MVVM.ViewModel
{
    internal class DictionaryViewModel : ObservableObject
    {

        private  WordsDboAdapter _wordsDboAdapter;

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

        public DictionaryViewModel()
        {
            _wordsDboAdapter = new WordsDboAdapter();
            _wordsList = new ObservableCollection<Word>();
        }
        
        private void NavigateToPage(string pageName)
        {
            foreach (Window window in Application.Current.Windows)
                if (window.GetType() == typeof(MainWindow))
                    (window as MainWindow).MainWindowFrame.Navigate(new Uri(string.Format("MVVM/View/{0}View.xaml", pageName), UriKind.RelativeOrAbsolute));
        }

        public RelayCommand WindowLoadedCommand
        {
            get
            {
                return new RelayCommand(async (loaded) =>
                {
                    WordsList = await _wordsDboAdapter.GetAllAsync();
                });
            }
        }

        public RelayCommand SelectWordCommand
        {
            get
            {
                return new RelayCommand((wordId) =>
                {
                    ApplicationContext.SelectedId = (int)wordId;
                    NavigateToPage("WordEdit");
                });
            }
        }

        public RelayCommand AddWordCommand
        {
            get
            {
                return new RelayCommand((add) =>
                {
                    ApplicationContext.SelectedId = -1;
                    NavigateToPage("WordEdit");
                });
            }
        }
        public RelayCommand DeleteWordCommand
        {
            get
            {
                return new RelayCommand((wordId) =>
                {
                    _wordsDboAdapter.Delete((int)wordId);
                    WordsList.Remove(WordsList.Single(x => x.Id == (int)wordId));
                });
            }
        }

    }
}
