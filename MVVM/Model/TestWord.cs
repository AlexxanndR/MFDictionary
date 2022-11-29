using MFDictionary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.MVVM.Model
{
    internal class TestWord : ObservableObject
    {
        public string Text { get; set; }

        public string GivenTranslation { get; set; }

        public string Translation { get; set; }

        private string _resultColor;
        public string ResultColor
        {
            get { return _resultColor; }
            set
            {
                _resultColor = value;
                OnPropertyChanged();
            }
        }

    }
}
