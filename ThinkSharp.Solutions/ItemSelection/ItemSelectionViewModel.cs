using ThinkSharp.Solutions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkSharp.Solutions.ItemSelection
{
    public class ItemSelectionViewModel<TItem> : DialogViewModel where TItem : class
    {
        public ItemSelectionViewModel() : base("Select Item") { }

        public ObservableCollection<SelectionItem<TItem>> Items { get; } = new ObservableCollection<SelectionItem<TItem>>();

        public TItem SelectedItem => Items.FirstOrDefault(i => i.IsChecked)?.Item;

        public void Add(TItem item, string name, string description)
        {
            var selectionItem = new SelectionItem<TItem>(item, name, description);
            if (Items.Count == 0)
                selectionItem.IsChecked = true;

            Items.Add(selectionItem);
        }
    }
}
