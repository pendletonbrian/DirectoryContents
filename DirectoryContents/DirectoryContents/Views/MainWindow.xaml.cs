using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DirectoryContents.ViewModels;

namespace DirectoryContents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members

        private MainWindowViewModel m_ViewModel;

        #endregion Private Members

        #region constructor

        public MainWindow()
        {
            InitializeComponent();

            m_ViewModel = new MainWindowViewModel();
            DataContext = m_ViewModel;
        }

        #endregion constructor

        #region Private Methods

        private void BrowseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void BrowseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                diag.Description = "Select the folder to parse.";

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    m_ViewModel.DirectoryToParse = diag.SelectedPath;
                }
            }

            treeView.Items.Clear();

            treeView.Items.Add(m_ViewModel.RootNode);

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
                e.CanExecute = m_ViewModel.CanGenerate();
            }

            e.Handled = true;
        }

        private void ExportCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string filepath;

            using (SaveFileDialog diag = new SaveFileDialog())
            {
                diag.Title = "Export file as:";
                diag.OverwritePrompt = true;
                diag.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

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
                m_ViewModel.Export(treeView, filepath);

                System.Windows.MessageBox.Show(this, "File exported!", Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(this, $"Exception: {ex.Message}", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion Private Methods
    }
}