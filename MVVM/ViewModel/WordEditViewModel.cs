using MFDictionary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.MVVM.ViewModel
{
    internal class WordEditViewModel
    {
        public RelayCommand WindowLoadedCommand
        {
            get
            {
                return new RelayCommand((parameter) =>
                {
                    var a = parameter;
                });
            }
        }

    }
}
