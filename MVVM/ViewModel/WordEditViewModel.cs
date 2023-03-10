using MFDictionary.Core;
using MFDictionary.MVVM.Model;
using MFDictionary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MFDictionary.MVVM.ViewModel
{
    internal class WordEditViewModel : ObservableObject
    {
        YandexService _yandexService;

        public DictionaryViewModel DictionaryViewModel { get; private set; }

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

        private string _transcription;

        public string Transcription
        {
            get { return _transcription; }
            set
            {
                _transcription = value;
                OnPropertyChanged();
            }
        }

        private string _translation;

        public string Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _translations;

        public ObservableCollection<string> Translations
        {
            get { return _translations; }
            set
            {
                _translations = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _examples;

        public ObservableCollection<string> Examples
        {
            get { return _examples; }
            set
            {
                _examples = value;
                OnPropertyChanged();
            }
        }

        public WordEditViewModel()
        {
            _translations = new ObservableCollection<string>()
            {
                "adsas", "asdasasdasdaasdd", "adasd", "asdasasdasdaasdd", "asdasasdasdaasdd",
                "asdasasdasdaasdd", "asdasasdasdaasdd", "asdasasdasdaasdd", "asdasasdasdaasdd",
                "asdasasdasdaasdd", "asdasasdasdaasdd", "asdasasdasdaasdd"
            };
            DictionaryViewModel = App.Current.
            _yandexService = new YandexService();
            _langsRatio = new Dictionary<string, List<string>>();
            _langsFrom = new List<string>();
            _langsTo = new List<string>();
        }

        private async Task Init()
        {
            var langs = await _yandexService.GetLangsAsync();

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
                return new RelayCommand(async (parameter) =>
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

        public RelayCommand ExitCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    foreach (Window window in Application.Current.Windows)
                        if (window.GetType() == typeof(MainWindow))
                            (window as MainWindow).MainWindowFrame.Navigate(new Uri(string.Format("MVVM/View/DictionaryView.xaml"), UriKind.RelativeOrAbsolute));
                });
            }
        }

        public RelayCommand LangFromChangedCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (LangFrom != null)
                        LangsTo = _langsRatio[LangFrom];
                });
            }
        }

        public RelayCommand AddTranslationCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                   Translations.Add(Translation);
                });
            }
        }

        /*        public RelayCommand FindWordCommand
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
                            var langTo = _langsShortForm.FirstOrDefault(x => x.Value == LangTo).Key;

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
                }*/
    }
}
