using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DirectoryContents.Classes;
using DirectoryContents.Models;
using DirectoryContents.ViewModels;

namespace DirectoryContents.Views
{
    /// <summary>
    /// Interaction logic for TreeChecksumView.xaml
    /// </summary>
    public partial class TreeChecksumView : BaseUserControlView
    {
        #region Private Members

        private readonly TreeChecksumViewModel m_ViewModel;

        #endregion Private Members

        #region constructor

        public TreeChecksumView(MainWindowViewModel viewModel, DirectoryItem rootNode) : base(viewModel)
        {
            InitializeComponent();

            m_ViewModel = new TreeChecksumViewModel(viewModel, rootNode);

            DataContext = m_ViewModel;
        }

        #endregion constructor

        #region Private Methods

        private void CancelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = m_ViewModel.IsGenerationInProgress();
            }

            e.Handled = true;
        }

        private void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.CancelGeneration();
        }

        private void GenerateCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
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

        private async void GenerateCommand_ExecutedAsync(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ShowProgressBar(true);

                Mouse.OverrideCursor = Cursors.Wait;

                ShowStatusMessage($"Generating {m_ViewModel.SelectedAlgorithim.GetDescription()} hash.");

                Stopwatch timer = Stopwatch.StartNew();

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(() => m_ViewModel.GenerateChecksumsAsync(cancellationTokenSource));

                timer.Stop();

                bool wasCancelled = m_ViewModel.WasCancelled;

                ShowPreviousPage(m_ViewModel.SelectedAlgorithim);

                if (wasCancelled)
                {
                    ShowStatusMessage($"Hash generation was cancelled after: {timer.Elapsed.GetTimeFromTimeSpan()}");
                }
                else
                {
                    ShowStatusMessage($"Time to generate hashes: {timer.Elapsed.GetTimeFromTimeSpan()}");
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;

                ShowProgressBar(false);
            }
        }

        #endregion Private Methods
    }
}
