using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MFDictionary
{
    public static class TestType
    {
        public enum Type
        {
            WordByTranslations,
            TranslationsByWord,
            WordByExamples
        }

        public static List<string> TypesInStrings
        {
            get
            {
                return new List<string>()
                {
                    "Writing words by using translation",
                    "Writing translations by using word",
                    "Writing words in examples"
                };
            }
        }

        public static Dictionary<string, Type> StringToTypeRatio
        {
            get
            {
                return new Dictionary<string, Type>()
                {
                    { "Writing words by using translation", Type.WordByTranslations },
                    { "Writing translations by using word", Type.TranslationsByWord },
                    { "Writing words in examples", Type.WordByExamples }
                };
            }
        }

        public static List<string> ParseExamplesForTest(List<string> examples, string word)
        {
            return examples.Select(exm => exm.Replace(word, new string('-', word.Length))).ToList();
        }
    }
}
