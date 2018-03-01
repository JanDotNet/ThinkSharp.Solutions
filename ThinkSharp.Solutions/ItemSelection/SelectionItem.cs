using ThinkSharp.Solutions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkSharp.Solutions.ItemSelection
{
    public class SelectionItem<TItem> : ViewModelBase
    {
        private bool myIsChecked;
        public SelectionItem(TItem item, string name, string tooltip)
        {
            Item = item;
            Name = name;
            ToolTip = tooltip;
        }

        public string Name { get; }
        public string ToolTip { get; }
        public bool IsChecked
        {
            get { return myIsChecked; }
            set
            {
                if (myIsChecked == value)
                    return;
                myIsChecked = value;
                OnPropertyChanged();
            }
        }

        public TItem Item { get; }
    }
}
