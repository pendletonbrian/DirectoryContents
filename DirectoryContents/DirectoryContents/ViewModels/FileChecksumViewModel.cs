using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #endregion

        #region Private Members

        private readonly List<KeyValuePair<string, string>> m_AlgorithimList = new List<KeyValuePair<string, string>>();
        private readonly DirectoryItem m_Item = null;
        private Enumerations.ChecksumAlgorithim m_SelectedAlgorithim = Enumerations.ChecksumAlgorithim.None;
        private ObservableCollection<string> m_ComputedChecksumList = new ObservableCollection<string>();
        private string m_ComputedChecksum = string.Empty;

        #endregion Private Members

        #region Public Properties

        public List<KeyValuePair<string, string>> AlgorithimList
        {
            get { return m_AlgorithimList; }

            private set { }
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
            get { return m_Item; }

            private set { }
        }

        public ObservableCollection<string> ComputedChecksumList
        {
            get { return m_ComputedChecksumList; }
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

        #endregion Public Properties

        #region constructor

        public FileChecksumViewModel(MainWindowViewModel viewModel, DirectoryItem item) : base(viewModel)
        {
            m_AlgorithimList = Enumerations.GetEnumValueDescriptionPairs(typeof(Enumerations.ChecksumAlgorithim));

            m_Item = item;
        }

        #endregion constructor

        #region Public Methods

        internal bool IsAlgorithimSelected()
        {
            return SelectedAlgorithim.Equals(Enumerations.ChecksumAlgorithim.None) == false;
        }

        internal void ComputeChecksum()
        {
            IHashAlgorithim algorithim = HashAlgorithimFactory.Get(SelectedAlgorithim);
            Hasher hasher = new Hasher(algorithim);

            bool? result = hasher.TryGetFileChecksum(SelectedItem.FullyQualifiedFilename, out string checksum);

            if (result.HasValue &&
                result.Value)
            {
                ComputedChecksum = checksum;
            }
        }

        #endregion

    }
}