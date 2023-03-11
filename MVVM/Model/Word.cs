using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.MVVM.Model
{
    public class Word
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string Transcription { get; set; }

        public List<string> Translation { get; set; }

        public List<string> Examples { get; set; }

        public List<string> ExamplesTranslation { get; set; }
    }
}
