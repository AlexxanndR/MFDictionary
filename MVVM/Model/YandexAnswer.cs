using MFDictionary.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.Services
{
    internal class YandexAnswer
    {
        public YandexDictionary DictionaryAnswer { get; set; }

        public string Text { get; set; }

        public string Code { get; set; }
    }
}
