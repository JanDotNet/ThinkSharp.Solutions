using ThinkSharp.Solutions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkSharp.Solutions.ServiceFromTemplate
{
    public class PlaceholderViewModel : ViewModelBase
    {
        private string myName = string.Empty;
        private string myReplacement = string.Empty;
        private string myDescription = string.Empty;
        private string[] mySuggestions = new string[0];

        public PlaceholderViewModel()
        {
        }

        public PlaceholderViewModel(string name, string replacement)
        {
            Name = name;
            Replacement = replacement;
        }

        public string Name
        {
            get { return myName; }
            set
            {
                myName = value;
                OnPropertyChanged();
            }
        }

        public string Replacement
        {
            get { return myReplacement; }
            set
            {
                myReplacement = value;
                OnPropertyChanged();
            }
        }

        public string[] Suggestions
        {
            get { return mySuggestions; }
            set
            {
                mySuggestions = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return myDescription; }
            set
            {
                myDescription = value;
                OnPropertyChanged();
            }
        }
    }
}
