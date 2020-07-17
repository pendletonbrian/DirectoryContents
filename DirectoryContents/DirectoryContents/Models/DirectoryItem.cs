using DirectoryContents.Classes;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace DirectoryContents.Models
{
    public class DirectoryItem : NotifyObject, IEquatable<DirectoryItem>
    {
        #region Private Members

        private static readonly string DirectorySeparator;
        private string m_Checksum;
        private FontStyle m_FontStyle = FontStyles.Normal;
        private FontWeight m_FontWeight = FontWeights.Normal;
        private bool m_IsExpanded;
        private bool m_IsSelected;

        #endregion Private Members

        #region Public Properties

        internal int Depth { get; set; }

        public string Checksum
        {
            get => m_Checksum;

            set
            {
                if (string.IsNullOrWhiteSpace(m_Checksum) ||
                    m_Checksum.Equals(value, StringComparison.OrdinalIgnoreCase) == false)
                {
                    m_Checksum = value;

                    RaisePropertyChanged(nameof(Checksum));
                    RaisePropertyChanged(nameof(FormattedChecksum));
                }
            }
        }

        public FontStyle FontStyle
        {
            get => m_FontStyle;

            set
            {
                if (m_FontStyle.Equals(value) == false)
                {
                    m_FontStyle = value;

                    RaisePropertyChanged(nameof(FontStyle));
                }
            }
        }

        public FontWeight FontWeight
        {
            get => m_FontWeight;

            set
            {
                if (m_FontWeight.Equals(value) == false)
                {
                    m_FontWeight = value;

                    RaisePropertyChanged(nameof(FontWeight));
                }
            }
        }

        public string FormattedChecksum
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Checksum))
                {
                    return string.Empty;
                }
                else
                {
                    return $" : {Checksum}";
                }
            }
        }

        public string FullyQualifiedFilename { get; private set; }

        /// <summary>
        /// The fully qualified filepath, not including the file name.
        /// </summary>
        public string Filepath { get; private set; }

        public bool HasChildren => Items.Count > 0;

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

        /// <summary>
        /// The name of the file, with extension. For directories,
        /// this is the directory name.
        /// </summary>
        public string ItemName { get; private set; }

        public ObservableCollection<DirectoryItem> Items { get; private set; }

        #endregion Public Properties

        #region constructors

        static DirectoryItem()
        {
            DirectorySeparator = Path.DirectorySeparatorChar.ToString();
        }

        public DirectoryItem()
        {
            Items = new ObservableCollection<DirectoryItem>();
            Checksum = string.Empty;
        }

        public DirectoryItem(DirectoryInfo directoryInfo) : this()
        {
            FullyQualifiedFilename = directoryInfo.FullName;
            Filepath = directoryInfo.FullName;

            if (FullyQualifiedFilename.EndsWith(DirectorySeparator) == false)
            {
                FullyQualifiedFilename += Path.DirectorySeparatorChar;
            }

            ItemName = directoryInfo.Name;
            IsDirectory = true;

            IconUri = "/Images/folder-16.png";
        }

        public DirectoryItem(FileInfo fileInfo) : this()
        {
            FullyQualifiedFilename = fileInfo.FullName;
            Filepath = fileInfo.DirectoryName;
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