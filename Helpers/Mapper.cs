using MFDictionary.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.Helpers
{
    internal static class Mapper
    {
        public static Word GetWord(this YandexDictionary yandexDictionary)
        {
            if (yandexDictionary.def.Length == 0)
                return null;

            return new Word
            {
                Text = yandexDictionary.def.First().text,
                Translation = yandexDictionary.def.First().tr.First().text,
                Example1 = yandexDictionary.def.First().tr.First().ex?.ElementAtOrDefault(0)?.text,
                Example2 = yandexDictionary.def.First().tr.First().ex?.ElementAtOrDefault(1)?.text,
                Example3 = yandexDictionary.def.First().tr.First().ex?.ElementAtOrDefault(2)?.text
            };
        }

        public static TestWord GetTestWord(this Word word)
        {
            return new TestWord
            {
                Text = word.Text,
                GivenTranslation = null,
                Translation = word.Translation,
            };
        }
    }
}
