using System.Collections.ObjectModel;

namespace DirectoryContents.Models
{
    public class DirectoryItem
    {
        #region Public Properties

        public string ItemName { get; private set; }

        public bool IsHidden { get; private set; }

        public ObservableCollection<DirectoryItem> Items { get; private set; }

        #endregion Public Properties

        #region constructors

        public DirectoryItem()
        {
            Items = new ObservableCollection<DirectoryItem>();
        }

        public DirectoryItem(string itemName) : this()
        {
            ItemName = itemName;
        }

        #endregion constructors

    }
}
