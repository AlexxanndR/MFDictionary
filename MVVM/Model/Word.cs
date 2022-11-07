using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.MVVM.Model
{
    internal class Word
    {
        public string head { get; set; }
        public Dictionary<string, Dictionary<string,string>> def { get; set; }
    }
}
