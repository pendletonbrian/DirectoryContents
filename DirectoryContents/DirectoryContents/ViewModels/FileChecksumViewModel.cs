using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DirectoryContents.Classes;
using DirectoryContents.Classes.Checksums;
using DirectoryContents.Models;

namespace DirectoryContents.ViewModels
{
    /// <summary>
    /// Handle the settings for hashing a single file.
    /// </summary>
    public class FileChecksumViewModel : BaseUserControlViewModel
    {
        #region Public Members

        public static RoutedCommand GenerateCommand = new RoutedCommand();

        #endregion Public Members

        #region Private Members

        private readonly List<KeyValuePair<string, string>> m_AlgorithimList = new List<KeyValuePair<string, string>>();
        private string m_ComputedChecksum = string.Empty;
        private readonly ObservableCollection<string> m_ComputedChecksumList = new ObservableCollection<string>();
        private Enumerations.ChecksumAlgorithim m_SelectedAlgorithim = Enumerations.ChecksumAlgorithim.None;
        private DirectoryItem m_SelectedItem = null;

        #endregion Private Members

        #region Public Properties

        public List<KeyValuePair<string, string>> AlgorithimList
        {
            get { return m_AlgorithimList; }

            // Set in the constructor, and not editable after that, as it's just
            // a lookup.
            private set { }
        }

        public string ComputedChecksum
        {
            get { return m_ComputedChecksum; }

            set
            {
                if (string.IsNullOrWhiteSpace(m_ComputedChecksum) ||
                    m_ComputedChecksum.Equals(value, StringComparison.OrdinalIgnoreCase) == false)
                {
                    m_ComputedChecksum = value;

                    RaisePropertyChanged(nameof(ComputedChecksum));
                }
            }
        }

        public ObservableCollection<string> ComputedChecksumList
        {
            get { return m_ComputedChecksumList; }
        }

        public Enumerations.ChecksumAlgorithim SelectedAlgorithim
        {
            get { return m_SelectedAlgorithim; }

            set
            {
                if (m_SelectedAlgorithim.Equals(value) == false)
                {
                    Log($"{nameof(FileChecksumViewModel)}.{nameof(SelectedAlgorithim)} is changing from {m_SelectedAlgorithim} to {value}.");

                    m_SelectedAlgorithim = value;

                    RaisePropertyChanged(nameof(SelectedAlgorithim));
                }
            }
        }

        public DirectoryItem SelectedItem
        {
            get { return m_SelectedItem; }

            private set
            {
                if (m_SelectedItem is null ||
                    m_SelectedItem.Equals(value) == false)
                {
                    m_SelectedItem = value;

                    RaisePropertyChanged(nameof(SelectedItem));
                }
            }
        }

        #endregion Public Properties

        #region constructor

        public FileChecksumViewModel(MainWindowViewModel viewModel, DirectoryItem item) : base(viewModel)
        {
            m_AlgorithimList = Enumerations.GetEnumValueDescriptionPairs(typeof(Enumerations.ChecksumAlgorithim));
            RaisePropertyChanged(nameof(AlgorithimList));

            SelectedItem = item;
        }

        #endregion constructor

        #region Public Methods

        internal async Task ComputeChecksumAsync()
        {
            Log($"{nameof(FileChecksumViewModel)}.{nameof(ComputeChecksumAsync)}: Start");

            IHashAlgorithim algorithim = HashAlgorithimFactory.Get(SelectedAlgorithim);
            Hasher hasher = new Hasher(algorithim);

            string checksum = string.Empty;

            bool? result = await Task.Run(() => hasher.TryGetFileChecksum(SelectedItem.FullyQualifiedFilename, out checksum));

            if (result.HasValue &&
                result.Value)
            {
                ComputedChecksum = checksum;
            }
            else
            {
                Log($"    There was no result from the hashing.");
            }

            Log($"{nameof(FileChecksumViewModel)}.{nameof(ComputeChecksumAsync)}: End");
        }

        internal bool IsAlgorithimSelected()
        {
            return SelectedAlgorithim.Equals(Enumerations.ChecksumAlgorithim.None) == false;
        }

        #endregion Public Methods
    }
}
