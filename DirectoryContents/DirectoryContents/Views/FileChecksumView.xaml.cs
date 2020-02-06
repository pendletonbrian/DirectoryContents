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
using DirectoryContents.Models;
using DirectoryContents.ViewModels;

namespace DirectoryContents.Views
{
    /// <summary>
    /// Interaction logic for FileChecksumView.xaml
    /// </summary>
    public partial class FileChecksumView : BaseUserControlView
    {
        #region Private Members

        private readonly FileChecksumViewModel m_ViewModel;

        #endregion

        #region constructor

        public FileChecksumView(MainWindowViewModel viewModel, DirectoryItem item) : base(viewModel)
        {
            InitializeComponent();

            m_ViewModel = new FileChecksumViewModel(viewModel, item);

            DataContext = m_ViewModel;
        }

        #endregion
    }
}
