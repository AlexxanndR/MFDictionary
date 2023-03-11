using MFDictionary.Core;
using MFDictionary.Helpers;
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
        private YandexService _yandexService;

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

        private string _translatableWord;

        public string TranslatableWord
        {
            get { return _translatableWord; }
            set
            {
                _translatableWord = value;
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

        private ObservableCollection<string> _examplesTranslation;

        public ObservableCollection<string> ExamplesTranslation
        {
            get { return _examplesTranslation; }
            set
            {
                _examplesTranslation = value;
                OnPropertyChanged();
            }
        }

        public WordEditViewModel()
        {
            _wordsDboAdapter = new WordsDboAdapter();
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

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    _wordsDboAdapter.Insert(new Word
                    {
                        Text = this.TranslatableWord,
                        Transcription = this.Transcription,
                        Translation = this.Translations.ToList(),
                        Examples = this.Examples.ToList(),
                        ExamplesTranslation = this.ExamplesTranslation.ToList()
                    });

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
                   if (!String.IsNullOrWhiteSpace(Translation))
                        Translations.Add(Translation);
                });
            }
        }

        public RelayCommand DeleteTranslationCommand
        {
            get
            {
                return new RelayCommand((selectedTranslation) =>
                {
                    Translations.Remove((string)selectedTranslation);
                });
            }
        }

        public RelayCommand SearchWordCommand
        {
            get
            {
                return new RelayCommand(async (action) =>
                {
                    var langFrom = _langsShortForm.FirstOrDefault(x => x.Value == LangFrom).Key;
                    var langTo = _langsShortForm.FirstOrDefault(x => x.Value == LangTo).Key;

                    var wordInfo = await _yandexService.LookupAsync(TranslatableWord, langFrom, langTo);
                    Word word = wordInfo.DictionaryAnswer?.GetWord();

                    if (word != null)
                    {
                        /*                        if (WordsList.Any(x => x.Text == word.Text && x.Translation == word.Translation))
                                                {
                                                    messageBoxResult = MessageBox.Show("Oops! The word '" + SearchWord + "' has already been added to the dictionary!", "Warning",
                                                                                       MessageBoxButton.OK, MessageBoxImage.Warning,
                                                                                       MessageBoxResult.OK, MessageBoxOptions.None);
                                                    return;
                                                }*/

                        TranslatableWord = word.Text;
                        Transcription = word?.Transcription;
                        Translations = new ObservableCollection<string>(word.Translation);
                        Examples = new ObservableCollection<string>(word.Examples);
                        ExamplesTranslation = new ObservableCollection<string>(word.ExamplesTranslation);
                    }
                });
            }
        }
    }
}
