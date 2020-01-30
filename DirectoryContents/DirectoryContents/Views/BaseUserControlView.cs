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

        #region Public Properties

        public string TitleText

        {
            get { return m_ViewModel.TitleText; }
        }

        #endregion

        #region constructor

        public BaseUserControlView(MainWindowViewModel viewModel)
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
        /// Writes the message to the log file, prepending a timestamp (if 
        /// desired) and appending a new line. Calls flush after logging
        /// the message.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prependTimeStamp"></param>
        protected void Log(string msg, bool prependTimeStamp = true)
        {
            MainWindowViewModel.Log(msg, prependTimeStamp);
        }

        #endregion
    }
}
