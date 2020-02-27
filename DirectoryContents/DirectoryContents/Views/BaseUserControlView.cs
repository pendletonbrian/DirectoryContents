using DirectoryContents.ViewModels;
using System.Windows.Controls;

namespace DirectoryContents.Views
{
    public class BaseUserControlView : UserControl
    {
        #region Private Members

        private readonly MainWindowViewModel m_ViewModel;

        #endregion Private Members

        #region Public Properties

        public string TitleText

        {
            get { return m_ViewModel.TitleText; }
        }

        #endregion Public Properties

        #region constructor

        public BaseUserControlView(MainWindowViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        #endregion constructor

        #region Private Methods

        private void BaseUserControlView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        #endregion Private Methods

        #region Protected Methods

        protected virtual void BackCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            m_ViewModel.ShowPreviousPage();
        }

        internal void ShowPreviousPage()
        {
            m_ViewModel.ShowPreviousPage();
        }

        internal void ShowProgressBar(bool isVisible)
        {
            if (m_ViewModel is null)
            {
                return;
            }

            m_ViewModel.ShowProgressBar = isVisible;
        }

        internal void ShowStatusMessage(string msg, bool autoRemove = true)
        {
            m_ViewModel?.ShowStatusMessage(msg, autoRemove);
        }

        #endregion Protected Methods
    }
}