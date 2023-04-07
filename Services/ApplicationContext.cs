using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MFDictionary.Services
{
    public static class ApplicationContext
    {
        public static int SelectedId { get; set; }

        public static int TestWordsNum { get; set; }

        public static List<string> TestTypes 
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

        public static string SelectedTestType { get; set; }
    }
}
