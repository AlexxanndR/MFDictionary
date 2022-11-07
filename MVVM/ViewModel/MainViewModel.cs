using MFDictionary.Core;
using MFDictionary.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        DictionaryAPI api = new DictionaryAPI();

        private Dictionary<string, List<string>> fromToLangsDict = new Dictionary<string, List<string>>();

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

        public MainViewModel()
        {
            LangsFrom = new List<string>();
            LangsTo = new List<string>();

            Init();
        }

        private async Task Init()
        {
            var langs = await api.GetLangs();

            foreach (var lan in langs)
            {
                var key = lan.Split('-')[0];
                var value = lan.Split('-')[1];

                if (fromToLangsDict.ContainsKey(key) == true)
                    fromToLangsDict[key].Add(value);
                else
                {
                    LangsFrom.Add(key);
                    fromToLangsDict.Add(key, new List<string> { value });
                }
            }
        }

        public RelayCommand LangFromChangedCommand
        {
            get
            {
                return new RelayCommand((change) =>
                {
                    LangsTo = fromToLangsDict[LangFrom];
                });
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

        public RelayCommand FindWordCommand
        {
            get
            {
                return new RelayCommand((change) =>
                {
                    DictionaryAPI dictionaryAPI = new DictionaryAPI();
                    var a   =  dictionaryAPI.Request("привет").Result;

                    var s = a.ToString();
                    
                });
            }
        }
    }
}
