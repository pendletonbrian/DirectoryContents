using DirectoryContents.Models;
using DirectoryContents.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DirectoryContents.Views
{
    /// <summary>
    /// Interaction logic for TreeChecksumView.xaml
    /// </summary>
    public partial class TreeChecksumView : BaseUserControlView
    {
        #region Private Members

        private TreeChecksumViewModel m_ViewModel;

        #endregion

        #region constructor

        public TreeChecksumView(MainWindowViewModel viewModel, DirectoryItem item, Classes.Enumerations.ChecksumAlgorithim checksumAlgorithim) : base(viewModel)
        {
            InitializeComponent();

            m_ViewModel = new TreeChecksumViewModel(viewModel, item, checksumAlgorithim);

            DataContext = m_ViewModel;
        }

        #endregion
    }
}
