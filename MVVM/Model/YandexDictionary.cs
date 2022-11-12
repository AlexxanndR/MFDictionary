using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.MVVM.Model
{
    public class YandexDictionary
    {
        public Head head { get; set; }
        public Def[] def { get; set; }
    }

    public class Head
    {
    }

    public class Def
    {
        public string text { get; set; }
        public string pos { get; set; }
        public string gen { get; set; }
        public string anm { get; set; }
        public Tr[] tr { get; set; }
    }

    public class Tr
    {
        public string text { get; set; }
        public string pos { get; set; }
        public Syn[] syn { get; set; }
        public Mean[] mean { get; set; }
        public Ex[] ex { get; set; }
    }

    public class Syn
    {
        public string text { get; set; }
        public string pos { get; set; }
    }

    public class Mean
    {
        public string text { get; set; }
    }

    public class Ex
    {
        public string text { get; set; }
        public Tr1[] tr { get; set; }
    }

    public class Tr1
    {
        public string text { get; set; }
    }
}
