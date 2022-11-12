using MFDictionary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.MVVM.ViewModel
{
    internal class DictionaryViewModel : ObservableObject
    {
        YandexService yandexService;

        private Dictionary<string, List<string>> langsRatio;

        private Dictionary<string, string> langsShortForm = new Dictionary<string, string>()
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

        private async Task Init()
        {
            var langs = await yandexService.GetLangsAsync();

            foreach (var lan in langs)
            {
                var from = langsShortForm[lan.Split('-')[0]];
                var to = langsShortForm[lan.Split('-')[1]];

                if (langsRatio.ContainsKey(from) == true)
                    langsRatio[from].Add(to);
                else
                {
                    LangsFrom.Add(from);
                    langsRatio.Add(from, new List<string> { to });
                }
            }
        }

        public DictionaryViewModel()
        {
            yandexService = new YandexService();
            langsRatio = new Dictionary<string, List<string>>();
            LangsFrom = new List<string>();
            LangsTo = new List<string>();
        }

        public RelayCommand WindowLoadedCommand
        {
            get
            {
                return new RelayCommand(async (loaded) =>
                {
                    await Init();
                });
            }
        }

        public RelayCommand LangFromChangedCommand
        {
            get
            {
                return new RelayCommand((change) =>
                {
                    LangsTo = langsRatio[LangFrom];
                });
            }
        }

        public RelayCommand FindWordCommand
        {
            get
            {
                return new RelayCommand(async (action) =>
                {
                    var langFrom = langsShortForm.FirstOrDefault(x => x.Value == LangFrom).Key;
                    var langTo   = langsShortForm.FirstOrDefault(x => x.Value == LangTo).Key;


                    var wordInfo = await yandexService.LookupAsync(SearchWord, langFrom, langTo);


                });
            }
        }


    }
}
