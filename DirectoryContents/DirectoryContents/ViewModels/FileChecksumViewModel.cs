using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectoryContents.Classes;
using DirectoryContents.Models;

namespace DirectoryContents.ViewModels
{
    public class FileChecksumViewModel : BaseUserControlViewModel
    {
        #region Private Members

        private Enumerations.ChecksumAlgorithim m_SelectedAlgorithim = Enumerations.ChecksumAlgorithim.None;
        private readonly List<KeyValuePair<string, string>> m_AlgorithimList = new List<KeyValuePair<string, string>>();
        private readonly DirectoryItem m_Item = null;

        #endregion

        #region Public Properties

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

        public List<KeyValuePair<string, string>> AlgorithimList
        {
            get { return m_AlgorithimList; }

            private set { }
        }

        public DirectoryItem SelectedItem
        {
            get { return m_Item; }

            private set { }
        }

        #endregion

        #region constructor

        public FileChecksumViewModel(MainWindowViewModel viewModel, DirectoryItem item) : base(viewModel)
        {
            m_AlgorithimList = Enumerations.GetEnumValueDescriptionPairs(typeof(Enumerations.ChecksumAlgorithim));

            m_Item = item;
        }

        #endregion
    }
}
