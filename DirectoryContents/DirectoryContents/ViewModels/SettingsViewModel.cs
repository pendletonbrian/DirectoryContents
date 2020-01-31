using System.Windows.Input;

namespace DirectoryContents.ViewModels
{
    public class SettingsViewModel : BaseUserControlViewModel
    {
        #region Public Members

        /// <summary>
        /// Command to go to the next page.
        /// </summary>
        public static RoutedCommand NextCommand = new RoutedCommand();

        #endregion Public Members

        #region constructor

        public SettingsViewModel(MainWindowViewModel viewModel) : base(viewModel)
        { }

        #endregion constructor
    }
}