using System.Collections.Generic;
using DirectoryContents.Classes;
using DirectoryContents.Models;

namespace DirectoryContents.ViewModels
{
    public class FileChecksumViewModel : BaseUserControlViewModel
    {
        #region Private Members

        private readonly List<KeyValuePair<string, string>> m_AlgorithimList = new List<KeyValuePair<string, string>>();
        private readonly DirectoryItem m_Item = null;
        private Enumerations.ChecksumAlgorithim m_SelectedAlgorithim = Enumerations.ChecksumAlgorithim.None;

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

        #endregion Public Properties

        #region constructor

        public FileChecksumViewModel(MainWindowViewModel viewModel, DirectoryItem item) : base(viewModel)
        {
            m_AlgorithimList = Enumerations.GetEnumValueDescriptionPairs(typeof(Enumerations.ChecksumAlgorithim));

            m_Item = item;
        }

        #endregion constructor
    }
}