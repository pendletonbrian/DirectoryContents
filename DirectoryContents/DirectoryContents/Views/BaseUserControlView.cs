using System.Windows.Controls;
using DirectoryContents.ViewModels;

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

        #endregion Protected Methods
    }
}