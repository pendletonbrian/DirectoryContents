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

        #endregion Private Members

        #region constructor

        public FileChecksumView(MainWindowViewModel viewModel, DirectoryItem item) : base(viewModel)
        {
            InitializeComponent();

            m_ViewModel = new FileChecksumViewModel(viewModel, item);

            DataContext = m_ViewModel;
        }

        #endregion constructor

        #region Private Methods

        private void GenerateCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = m_ViewModel.IsAlgorithimSelected();
            }

            e.Handled = true;
        }

        private void GenerateCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void BackCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

    }
}