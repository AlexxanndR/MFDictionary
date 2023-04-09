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

        public static TestType.Type SelectedTestType { get; set; }
    }
}
