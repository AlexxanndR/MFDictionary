using BespokeFusion;
using MessageBox.Avalonia.Enums;
using MFDictionary.Core;
using MFDictionary.Helpers;
using MFDictionary.MVVM.Model;
using MFDictionary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MFDictionary.MVVM.ViewModel
{
    internal class WordEditViewModel : ObservableObject
    {
        private YandexService _yandexService;

        private WordsDboAdapter _wordsDboAdapter;

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

        private string _examples;

        public string Examples
        {
            get { return _examples; }
            set
            {
                _examples = value;
                OnPropertyChanged();
            }
        }

        private string _examplesTranslation;

        public string ExamplesTranslation
        {
            get { return _examplesTranslation; ; }
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
            _translations = new ObservableCollection<string>();
        }

        private async Task Init()
        {
            var langs = await _yandexService.GetLangsAsync();

            await Task.Run(() =>
            {
                try
                {
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
                catch (Exception ex) 
                {
                    //TO DO
                }
            });

            if (ApplicationContext.SelectedId >= 0)
            {
                Word word = await _wordsDboAdapter.GetByIdAsync(ApplicationContext.SelectedId);
                TranslatableWord = word.Text;
                Transcription = word?.Transcription;
                Translations = new ObservableCollection<string>(word.Translation);
                Examples = word.Examples.Count != 0 ? String.Join("\n", word.Examples) : String.Empty;
                ExamplesTranslation = word.ExamplesTranslation.Count != 0 ? String.Join("\n", word.ExamplesTranslation) : String.Empty;
            }
        }

        private List<string> ParseExamples()
        {
            if (String.IsNullOrEmpty(Examples))
                return null;

            return new List<string>(Examples.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
        }

        private List<string> ParseExamplesTranslation()
        {
            if (String.IsNullOrEmpty(ExamplesTranslation))
                return null;

            return new List<string>(ExamplesTranslation.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
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
                            CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                            {
                                FontFamily = new FontFamily("Oswald Light"),
                                TxtMessage = 
                                {
                                    Text = "No internet connection!", 
                                    Foreground = Brushes.Black, FontSize = 20,
                                    VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center
                                },
                                TxtTitle = { Text = "Warning", Foreground = Brushes.Black },
                                BtnOk = { Content = "Ok", Background = Brushes.Transparent, Foreground = Brushes.Black },
                                BtnCancel = { Content = "Cancel", Background = Brushes.Transparent, Foreground = Brushes.Black },
                                MainContentControl = { Background = Brushes.White },
                                TitleBackgroundPanel = { Background = Brushes.MistyRose },
                                BorderThickness = new Thickness(0),

                                WindowStyle = WindowStyle.None
                            };
                            msg.Show();
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
                    CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                    {
                        FontFamily = new FontFamily("Oswald Light"),
                        TxtMessage = { 
                            Text = "Data won't be save! Are you sure?", 
                            Foreground = Brushes.Black, FontSize = 20,
                            VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center
                        },
                        TxtTitle = { Text = "Warning", Foreground = Brushes.Black },
                        BtnOk = { Content = "Ok", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black},
                        BtnCancel = { Content = "Cancel", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black },
                        MainContentControl = { Background = Brushes.White },
                        TitleBackgroundPanel = { Background = Brushes.MistyRose },
                        BorderThickness = new Thickness(0),
                        
                        WindowStyle = WindowStyle.None
                    };
                    msg.Show();

                    if (msg.Result == MessageBoxResult.OK)
                    {
                        foreach (Window window in Application.Current.Windows)
                            if (window.GetType() == typeof(MainWindow))
                                (window as MainWindow).MainWindowFrame.Navigate(new Uri(string.Format("MVVM/View/DictionaryView.xaml"), UriKind.RelativeOrAbsolute));
                    }
                });
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(async (obj) =>
                {
                    if (String.IsNullOrWhiteSpace(TranslatableWord) || Translations.Count == 0)
                    {
                        CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                        {
                            TxtMessage = { Text = "Oops! There is no word to add!", Foreground = Brushes.Black, FontSize = 20 },
                            TxtTitle = { Text = "Warning", Foreground = Brushes.Black },
                            BtnOk = { Content = "Ok", Background = Brushes.Transparent, Foreground = Brushes.Black },
                            BtnCancel = { Content = "Cancel", Background = Brushes.Transparent, Foreground = Brushes.Black },
                            MainContentControl = { Background = Brushes.White },
                            TitleBackgroundPanel = { Background = Brushes.MistyRose },
                            BorderThickness = new Thickness(0),
                            FontFamily = new FontFamily("Oswald Light"),
                            WindowStyle = WindowStyle.None,
                        };
                        msg.Show();
                        return;
                    }

                    Word word = new Word
                    {
                        Text = this.TranslatableWord,
                        Transcription = this.Transcription,
                        Translation = this.Translations.ToList(),
                        Examples = this.ParseExamples(),
                        ExamplesTranslation = this.ParseExamplesTranslation()
                    };


                    if (ApplicationContext.SelectedId >= 0)
                    {
                        word.Id = ApplicationContext.SelectedId;
                        await _wordsDboAdapter.UpdateAsync(word);
                    }
                    else if (ApplicationContext.SelectedId == -1)
                    {
                        bool isWordExist = await _wordsDboAdapter.IsWordExist(word);
                        if (isWordExist)
                        {
                            CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                            {
                                FontFamily = new FontFamily("Oswald Light"),
                                TxtMessage = { Text = "The word already exists in the dictionary!", Foreground = Brushes.Black, FontSize = 20 },
                                TxtTitle = { Text = "Warning", Foreground = Brushes.Black },
                                BtnOk = { Content = "Ok", Background = Brushes.Transparent, Foreground = Brushes.Black },
                                BtnCancel = { Content = "Cancel", Background = Brushes.Transparent, Foreground = Brushes.Black },
                                MainContentControl = { Background = Brushes.White },
                                TitleBackgroundPanel = { Background = Brushes.MistyRose },
                                BorderThickness = new Thickness(0),
                                WindowStyle = WindowStyle.None
                            };
                            msg.Show();
                            return;
                        }

                        await _wordsDboAdapter.InsertAsync(word);
                    }

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

                    Word word = null;

                    try
                    {
                        var wordInfo = await _yandexService.LookupAsync(TranslatableWord, langFrom, langTo);
                        word = wordInfo.DictionaryAnswer?.GetWord();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is HttpRequestException)
                        {
                            CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                            {
                                FontFamily = new FontFamily("Oswald Light"),
                                TxtMessage =
                                {
                                    Text = "No internet connection!",
                                    Foreground = Brushes.Black, FontSize = 20,
                                    VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center
                                },
                                TxtTitle = { Text = "Warning", Foreground = Brushes.Black },
                                BtnOk = { Content = "Ok", Background = Brushes.Transparent, Foreground = Brushes.Black },
                                BtnCancel = { Content = "Cancel", Background = Brushes.Transparent, Foreground = Brushes.Black },
                                MainContentControl = { Background = Brushes.White },
                                TitleBackgroundPanel = { Background = Brushes.MistyRose },
                                BorderThickness = new Thickness(0),

                                WindowStyle = WindowStyle.None
                            };
                            msg.Show();
                        }
                    }

                    if (word != null)
                    {
                        TranslatableWord = word.Text;
                        Transcription = word.Transcription;
                        Translations = new ObservableCollection<string>(word.Translation);
                        Examples = word.Examples.Count != 0 ? String.Join("\n", word.Examples) : String.Empty;
                        ExamplesTranslation = word.ExamplesTranslation.Count != 0 ? String.Join("\n", word.ExamplesTranslation) : String.Empty;
                    }
                });
            }
        }
    }
}
