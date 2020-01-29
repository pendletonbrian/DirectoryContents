using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DirectoryContents.ViewModels;

namespace DirectoryContents.Views
{
    public class BaseUserControlView : UserControl
    {
        #region Private Members

        private readonly MainWindowViewModel m_ViewModel;

        #endregion

        #region constructors

        public BaseUserControlView() : base()
        {
            Loaded += BaseUserControlView_Loaded;
        }

        public BaseUserControlView(MainWindowViewModel viewModel) : this()
        {
            m_ViewModel = viewModel;
        }

        #endregion

        #region Private Methods

        private void BaseUserControlView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        #endregion

        #region Protected Methods

        protected virtual void BackCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
        }

        /// <summary>
        /// Log the given message with a timestamp.
        /// </summary>
        /// <param name="msg"></param>
        protected void Log(string msg)
        { }

        #endregion
    }
}
