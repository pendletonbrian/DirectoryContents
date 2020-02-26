using DirectoryContents.Classes;
using DirectoryContents.Models;
using DirectoryContents.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

        public TreeChecksumView(MainWindowViewModel viewModel, DirectoryItem rootNode) : base(viewModel)
        {
            InitializeComponent();

            m_ViewModel = new TreeChecksumViewModel(viewModel, rootNode);

            DataContext = m_ViewModel;
        }

        #endregion

        #region Private Methods

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

                ShowPreviousPage();

                ShowStatusMessage($"Time to generate hashes: {timer.Elapsed.GetTimeFromTimeSpan()}");
            }
            finally
            {
                Mouse.OverrideCursor = null;

                ShowProgressBar(false);
            }
        }

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

        #endregion

    }
}
