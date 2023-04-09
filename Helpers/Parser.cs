using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.Helpers
{
    internal class Parser
    {
        public static List<string> ParseExamplesToList(string examples)
        {
            if (String.IsNullOrEmpty(examples))
                return null;

            return new List<string>(examples.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static List<string> ParseExamplesForTest(List<string> examples, string word)
        {
            return examples.Select(exm => exm.Replace(word, new string('_', word.Length))).ToList();
        }
    }
}
