using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkSharp.Solutions.Infrastructure;
using ThinkSharp.Solutions.TemplateDefinition;

namespace ThinkSharp.Solutions.ServiceFromTemplate.PlaceholderReplacement
{
    public class ExampleViewModel : ViewModelBase
    {
        private readonly Example myExample;

        private string myText;

        public ExampleViewModel(Example example)
        {
            myExample = example;
            myText = myExample.Text;
        }

        public string Header => myExample.Header;

        public string Text
        {
            get { return myText; }
            set
            {
                if (myText == value)
                    return;
                myText = value;
                OnPropertyChanged();
            }
        }

        internal void UpdateText(IEnumerable<PlaceholderViewModel> placeHolders)
        {
            var text = myExample.Text;
            foreach (var placeHolder in placeHolders)
            {
                if (!string.IsNullOrEmpty(placeHolder.Replacement))
                {
                    text = text.Replace(placeHolder.TextToReplace, placeHolder.Replacement);
                }
            }
            Text = text;
        }
    }
}
