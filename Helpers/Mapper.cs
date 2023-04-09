using MFDictionary.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MFDictionary.Helpers
{
    public static class Mapper
    {
        public static Word GetWord(this YandexDictionary yandexDictionary)
        {
            if (yandexDictionary.def.Length == 0)
                return null;

            return new Word
            {
                Text = yandexDictionary.def.First().text,
                Transcription = yandexDictionary.def.First().ts,
                Translation = yandexDictionary.def.First().tr.Select(x => x.text).ToList(),
                Examples = yandexDictionary.def.First().tr.Where(x => x.ex != null).Select(y => y.text).ToList(),
                ExamplesTranslation = yandexDictionary.def.First().tr.Where(x => x.ex != null).SelectMany(y => y.ex.SelectMany(z => z.tr.Select(i => i.text))).ToList(),
            };
        }
    }
}
