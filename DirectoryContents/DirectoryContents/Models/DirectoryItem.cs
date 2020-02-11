using System;
using System.Collections.ObjectModel;
using System.IO;
using DirectoryContents.Classes;

namespace DirectoryContents.Models
{
    public class DirectoryItem : NotifyObject, IEquatable<DirectoryItem>
    {
        #region Private Members

        private bool m_IsExpanded;
        private bool m_IsSelected;
        private string m_Checksum;

        #endregion Private Members

        #region Public Properties

        internal int Depth { get; set; }

        public string FullyQualifiedFilename { get; private set; }

        public bool HasChildren
        {
            get { return Items.Count > 0; }
        }

        public string IconUri { get; private set; }

        public bool IsDirectory { get; private set; }

        public bool IsExpanded
        {
            get => m_IsExpanded;

            set
            {
                m_IsExpanded = value;

                RaisePropertyChanged(nameof(IsExpanded));
            }
        }

        public bool IsSelected
        {
            get => m_IsSelected;

            set
            {
                m_IsSelected = value;

                RaisePropertyChanged(nameof(IsSelected));
            }
        }

        public string ItemName { get; private set; }

        public ObservableCollection<DirectoryItem> Items { get; private set; }

        public string Checksum 
        {
            get { return m_Checksum; }

            set
            {
                if (string.IsNullOrWhiteSpace(m_Checksum) ||
                    m_Checksum.Equals(value, StringComparison.OrdinalIgnoreCase) == false)
                {
                    m_Checksum = value;

                    RaisePropertyChanged(nameof(Checksum));
                }
            }
        }

        #endregion Public Properties

        #region constructors

        public DirectoryItem()
        {
            Items = new ObservableCollection<DirectoryItem>();
        }

        public DirectoryItem(DirectoryInfo directoryInfo) : this()
        {
            FullyQualifiedFilename = directoryInfo.FullName;
            ItemName = directoryInfo.Name;
            IsDirectory = true;

            IconUri = "/Images/folder-16.png";
        }

        public DirectoryItem(FileInfo fileInfo) : this()
        {
            FullyQualifiedFilename = fileInfo.FullName;
            ItemName = fileInfo.Name;
            IsDirectory = false;

            IconUri = "/Images/file-16.png";
        }

        #endregion constructors

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            DirectoryItem item = obj as DirectoryItem;

            if (item is null)
            {
                return false;
            }

            return Equals(item);
        }

        public bool Equals(DirectoryItem other)
        {
            if (other is null)
            {
                return false;
            }

            return GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            int hashName = string.IsNullOrWhiteSpace(ItemName) ? 0 : ItemName.GetHashCode();

            int hash = 17;

            unchecked
            {
                hash = (hash * 23) + hashName;
                hash = (hash * 23) + Depth.GetHashCode();
            }

            return hash;
        }

        public override string ToString()
        {
            return ItemName ?? "Name not set.";
        }

        #endregion Public Methods
    }
}