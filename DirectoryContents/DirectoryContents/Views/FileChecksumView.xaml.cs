using DirectoryContents.Classes;
using DirectoryContents.Models;
using DirectoryContents.ViewModels;
using System.Diagnostics;
using System.Windows.Input;

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

        private async void GenerateCommand_ExecutedAsync(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                ShowProgressBar(true);

                Mouse.OverrideCursor = Cursors.Wait;

                ShowStatusMessage($"Generating {m_ViewModel.SelectedAlgorithim.GetDescription()} hash.");

                Stopwatch timer = Stopwatch.StartNew();

                await m_ViewModel.ComputeChecksumAsync();

                timer.Stop();

                ShowStatusMessage($"Time to generate hash: {timer.Elapsed.GetTimeFromTimeSpan()}");
            }
            finally
            {
                Mouse.OverrideCursor = null;

                ShowProgressBar(false);
            }
        }

        private void BackCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

    }
}