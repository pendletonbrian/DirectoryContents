using DirectoryContents.Classes;
using DirectoryContents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DirectoryContents.Classes.Enumerations;

namespace DirectoryContents.ViewModels
{
    public class TreeChecksumViewModel : BaseUserControlViewModel
    {
        #region Private Members

        private DirectoryItem m_RootNode = null;
        private ChecksumAlgorithim m_ChecksumAlgorithim = ChecksumAlgorithim.None;

        #endregion

        public TreeChecksumViewModel(MainWindowViewModel viewModel, DirectoryItem node, ChecksumAlgorithim checksumAlgorithim) : base(viewModel)
        {
            m_RootNode = node;
            m_ChecksumAlgorithim = checksumAlgorithim;
        }

        internal async Task GenerateAsync()
        {
        }

    }
}
