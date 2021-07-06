using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DirectoryContents.Classes.Checksums;
using DirectoryContents.Models;
using DirectoryContents.ViewModels;
using static DirectoryContents.Classes.Enumerations;

namespace DirectoryContents.Views
{
    /// <summary>
    /// Interaction logic for DirectoryView.xaml
    /// </summary>
    public partial class DirectoryView : BaseUserControlView
    {
        #region Private Members

        private readonly DirectoryViewModel m_ViewModel;

        #endregion Private Members

        #region constructor

        public DirectoryView(MainWindowViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();

            m_ViewModel = new DirectoryViewModel(viewModel);

            DataContext = m_ViewModel;
        }

        #endregion constructor

        #region Private Methods

        private void BrowseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private async void BrowseCommand_ExecutedAsync(object sender, ExecutedRoutedEventArgs e)
        {
            string directoryPath = null;

            using (System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog())
            {
                diag.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                diag.Description = "Select the directory:";

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    directoryPath = diag.SelectedPath;
                }
            }

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                return;
            }

            await LoadDirectoryAsync(directoryPath);
        }

        private void CollapseAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }

            e.Handled = true;
        }

        private void CollapseAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.CollapseAll();

            treeView.UpdateLayout();
        }

        private void CopyChecksumCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                // I want there to be an item selected that is a file.

                e.CanExecute = m_ViewModel.IsItemSelected() &&
                               m_ViewModel.SelectedItem.IsDirectory.Equals(false);
            }

            e.Handled = true;
        }

        private void CopyChecksumCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetDataObject(m_ViewModel.SelectedItem.Checksum, false);

            ShowStatusMessage($"Copied {m_ViewModel.SelectedItem.Checksum} to the clipboard");
        }

        private void ExpandAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }

            e.Handled = true;
        }

        private void ExpandAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.ExpandAll();

            treeView.UpdateLayout();
        }

        private void ExportCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = m_ViewModel.IsLoaded();
            }

            e.Handled = true;
        }

        private async void ExportCommand_ExecutedAsync(object sender, ExecutedRoutedEventArgs e)
        {
            string filepath;

            using (System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog())
            {
                diag.Title = "Export file as:";
                diag.OverwritePrompt = true;
                diag.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                diag.AddExtension = true;
                diag.FileName = m_ViewModel.RootNode.ItemName;

                switch (m_ViewModel.SelectedExportStructure)
                {
                    case ExportFileStructure.TextFile:
                    case ExportFileStructure.TextFlat:

                        diag.Filter = "Text Files | *.txt";
                        diag.DefaultExt = "txt";

                        break;

                    case ExportFileStructure.CSV:

                        diag.Filter = "Comma Separated Files | *.csv";
                        diag.DefaultExt = "csv";

                        break;

                    default:
                        throw new ArgumentException("There is no export type selected.");
                }

                diag.FilterIndex = 0;

                if (diag.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                filepath = diag.FileName;
            }

            if (string.IsNullOrWhiteSpace(filepath))
            {
                return;
            }

            try
            {
                await m_ViewModel.ExportAsync(filepath);

                MessageBoxResult result = MessageBox.Show(
                    "File exported!  Would you like to view the result?",
                    TitleText,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(filepath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", TitleText, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateAllChecksumsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = m_ViewModel.IsLoaded();
            }

            e.Handled = true;
        }

        private void GenerateAllChecksumsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.ShowNextPage(PageControl.TreeChecksum,
                additionalData: m_ViewModel.RootNode,
                transitionType: PageTransitionType.SlideAndFade);
        }

        private void GenerateFileHashCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = false;

                bool? result = m_ViewModel.IsSelectedItemFile();

                if (result.HasValue)
                {
                    if (result.Value)
                    {
                        e.CanExecute = true;

                        ShowStatusMessage(string.Empty, false);
                    }
                    else
                    {
                        ShowStatusMessage($"Cannot generate checksum: \"{m_ViewModel.SelectedItem.ItemName}\" is a directory.");
                    }
                }
            }

            e.Handled = true;
        }

        private void GenerateFileHashCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.ShowNextPage(PageControl.FileChecksum,
                additionalData: m_ViewModel.SelectedItem,
                transitionType: PageTransitionType.SlideAndFade);
        }

        private async Task LoadDirectoryAsync(string fullyQualifiedDirectoryPath)
        {
            try
            {
                ShowProgressBar(true);

                Mouse.OverrideCursor = Cursors.Wait;

                treeView.Items.Clear();

                treeView.UpdateLayout();

                m_ViewModel.DirectoryToParse = fullyQualifiedDirectoryPath;

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                CancellationToken token = cancellationTokenSource.Token;

                await Task.Run(() => m_ViewModel.ParseAsync(),
                    token);

                treeView.Items.Add(m_ViewModel.RootNode);

                treeView.UpdateLayout();
            }
            catch
            {
            }
            finally
            {
                Mouse.OverrideCursor = null;

                ShowProgressBar(false);
            }
        }

        private void SearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = m_ViewModel.IsLoaded();
            }

            e.Handled = true;
        }

        private void SearchCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.Search();
        }

        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            bool isValidData = e.Data.GetDataPresent(DataFormats.FileDrop);

            if (isValidData == false)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private async void TreeView_DropAsync(object sender, DragEventArgs e)
        {
            string[] filenameList = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (filenameList is null ||
                filenameList.Length.Equals(0))
            {
                return;
            }

            // If the luser is dragging a file, in direct opposition of the name
            // of the app, then go directly to the File Checksum page.
            if (DirectoryViewModel.IsItemFile(filenameList[0]))
            {
                FileInfo fileInfo = new FileInfo(filenameList[0]);

                m_ViewModel.ShowNextPage(PageControl.FileChecksum,
                    additionalData: new DirectoryItem(fileInfo),
                    transitionType: PageTransitionType.SlideAndFade);

                return;
            }

            await LoadDirectoryAsync(filenameList[0]);

            // This is a bit of a hack. Setting the focus makes the application
            // reevaluate the XXX_CanExecute methods. Otherwise, the "Export"
            // menu option is not enabled after parsing the directory. This only
            // occurs with the drag and drop. Using the browse command every-
            // thing works as expected.
            treeView.Focus();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DirectoryItem item = e.NewValue as DirectoryItem;

            if (item is null)
            {
                return;
            }

            m_ViewModel.SelectedItem = item;
        }

        private void ViewInFileExplorerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = m_ViewModel.IsItemSelected();
            }

            e.Handled = true;
        }

        private void ViewInFileExplorerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                m_ViewModel.ShowSelectedItemInFileExplorer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", TitleText, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion Private Methods

        public void SetHashAlgorithim(IHashAlgorithim hashAlgorithim)
        {
            m_ViewModel.HashAlgorithim = hashAlgorithim;
        }
    }
}
