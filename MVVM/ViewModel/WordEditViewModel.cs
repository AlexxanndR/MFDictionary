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

        public WordEditViewModel()
        {
            _translations = new ObservableCollection<string>()
            {
                "adsas", "asdasasdasdaasdd", "adasd", "asdasasdasdaasdd", "asdasasdasdaasdd",
                "asdasasdasdaasdd", "asdasasdasdaasdd", "asdasasdasdaasdd", "asdasasdasdaasdd",
                "asdasasdasdaasdd", "asdasasdasdaasdd", "asdasasdasdaasdd"
            };
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
                return new RelayCommand((change) =>
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
                return new RelayCommand((change) =>
                {
                    if (LangFrom != null)
                        LangsTo = _langsRatio[LangFrom];
                });
            }
        }


    }
}
