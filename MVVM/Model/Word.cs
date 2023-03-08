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

        public string Translation { get; set; }

        public string Example1 { get; set; }

        public string Example2 { get; set; }

        public string Example3 { get; set; }
    }
}
