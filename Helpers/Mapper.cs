﻿using MFDictionary.MVVM.Model;
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
            return new Word
            {
                Text = yandexDictionary.def.First().text,
                Translation = yandexDictionary.def.First().tr.First().text,
                Ex1 = yandexDictionary.def.First().tr.First().ex.ElementAtOrDefault(0)?.text,
                Ex2 = yandexDictionary.def.First().tr.First().ex.ElementAtOrDefault(1)?.text,
                Ex3 = yandexDictionary.def.First().tr.First().ex.ElementAtOrDefault(2)?.text
            };
        }
    }
}
