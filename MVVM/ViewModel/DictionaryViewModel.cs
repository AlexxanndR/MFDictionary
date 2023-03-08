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
        YandexService _yandexService;

        WordsDboAdapter _wordsDboAdapter;

        private Dictionary<string, string> _langsShortForm = new Dictionary<string, string>()
        {
            { "ru", "Russian" },
            { "en", "English" },
            { "be", "Belarussian" },
            { "uk", "Ukrainian"  },
            { "bg", "Bulgarian" },
            { "cs", "Czech" },
            { "da", "Danish" },
            { "de", "German" },
            { "el", "Greek" },
            { "es", "Spanish" },
            { "et", "Estonian" },
            { "fi", "Finnish" },
            { "fr", "French" },
            { "hu", "Hungarian" },
            { "it", "Italian" },
            { "lt", "Lithuanian" },
            { "lv", "Latvian" },
            { "mhr", "Eastern Mari " },
            { "mrj", "Western Mari" },
            { "nl", "Dutch" },
            { "no", "Norwegian" },
            { "pl", "Polish" },
            { "pt", "Portuguese" },
            { "sk", "Slovak" },
            { "sv", "Swedish" },
            { "tr", "Turkish" },
            { "tt", "Tatar" },
            { "zh", "Chinese" },
        };

        private Dictionary<string, List<string>> _langsRatio;

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

        private List<string> _langsFrom;

        public List<string> LangsFrom
        {
            get { return _langsFrom; }
            private set
            {
                _langsFrom = value;
                OnPropertyChanged();
            }
        }

        private string _langFrom;

        public string LangFrom
        {
            get { return _langFrom; }
            set
            {
                _langFrom = value;
                OnPropertyChanged();
            }
        }

        private List<string> _langsTo;

        public List<string> LangsTo
        {
            get { return _langsTo; }
            private set
            {
                _langsTo = value;
                OnPropertyChanged();
            }
        }

        private string _langTo;

        public string LangTo
        {
            get { return _langTo; }
            set
            {
                _langTo = value;
                OnPropertyChanged();
            }
        }

        private string _searchWord;

        public string SearchWord
        {
            get { return _searchWord; }
            set
            {
                _searchWord = value;
                OnPropertyChanged();
            }
        }

        public DictionaryViewModel()
        {
            _yandexService = new YandexService();
            _wordsDboAdapter = new WordsDboAdapter();
            _langsRatio = new Dictionary<string, List<string>>();
            _langsFrom = new List<string>();
            _langsTo = new List<string>();
            _wordsList = new ObservableCollection<Word>();
        }

        private async Task Init()
        {
            var langs = await _yandexService.GetLangsAsync();

            WordsList = _wordsDboAdapter.GetAll();

            foreach (var lang in langs)
            {
                var from = _langsShortForm[lang.Split('-')[0]];
                var to = _langsShortForm[lang.Split('-')[1]];

                if (_langsRatio.ContainsKey(from) == true)
                    _langsRatio[from].Add(to);
                else
                {
                    LangsFrom.Add(from);
                    _langsRatio.Add(from, new List<string> { to });
                }
            }
        }

        public RelayCommand WindowLoadedCommand
        {
            get
            {
                return new RelayCommand(async (loaded) =>
                {
                    try
                    {
                        await Init();
                    } 
                    catch (Exception ex)
                    {
                        if (ex.InnerException is HttpRequestException)
                        {
                            MessageBox.Show("Oops! No internet connection.", "Warning", MessageBoxButton.OK,
                                            MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.None);
                        }
                    }
                });
            }
        }

        public RelayCommand SelectWordCommand
        {
            get
            {
                return new RelayCommand((wordId) =>
                {
                    foreach (Window window in Application.Current.Windows)
                        if (window.GetType() == typeof(MainWindow))
                            (window as MainWindow).MainWindowFrame.Navigate(new Uri(string.Format("MVVM/View/WordEditView.xaml?parameter={0}", wordId), UriKind.RelativeOrAbsolute));
                });
            }
        }

        public RelayCommand LangFromChangedCommand
        {
            get
            {
                return new RelayCommand((change) =>
                {
                    if (LangFrom != null)
                        LangsTo = _langsRatio[LangFrom];
                });
            }
        }

        public RelayCommand FindWordCommand
        {
            get
            {
                return new RelayCommand(async (action) =>
                {
                    MessageBoxResult messageBoxResult = MessageBoxResult.None;

                    if (String.IsNullOrWhiteSpace(SearchWord))
                    {
                        messageBoxResult = MessageBox.Show("The search word is missing!", "Warning", MessageBoxButton.OK,
                                                           MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.None);
                        return;
                    }

                    if (LangFrom == null || LangTo == null)
                    {
                        messageBoxResult = MessageBox.Show("Translation direction not selected!", "Warning", MessageBoxButton.OK,
                                                           MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.None);
                        return;
                    }

                    var langFrom = _langsShortForm.FirstOrDefault(x => x.Value == LangFrom).Key;
                    var langTo   = _langsShortForm.FirstOrDefault(x => x.Value == LangTo).Key;

                    var wordInfo = await _yandexService.LookupAsync(SearchWord, langFrom, langTo);
                    Word word = wordInfo.DictionaryAnswer.GetWord();

                    if (word == null)
                    {
                        messageBoxResult = MessageBox.Show("Oops! The word '" + SearchWord + "' not found! Spelling error or non-existent word.", "Error",
                                                           MessageBoxButton.OK, MessageBoxImage.Error,
                                                           MessageBoxResult.OK, MessageBoxOptions.None);
                        return;
                    }

                    if (WordsList.Any(x => x.Text == word.Text && x.Translation == word.Translation))
                    {
                        messageBoxResult = MessageBox.Show("Oops! The word '" + SearchWord + "' has already been added to the dictionary!", "Warning",
                                                           MessageBoxButton.OK, MessageBoxImage.Warning,
                                                           MessageBoxResult.OK, MessageBoxOptions.None);
                        return;
                    }

                    messageBoxResult = MessageBox.Show("Word '" + SearchWord + "' was successfully found!", "Information",
                                                       MessageBoxButton.OK, MessageBoxImage.Information,
                                                       MessageBoxResult.OK, MessageBoxOptions.None);


                    WordsList.Add(word);
                    _wordsDboAdapter.Insert(word);        
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
