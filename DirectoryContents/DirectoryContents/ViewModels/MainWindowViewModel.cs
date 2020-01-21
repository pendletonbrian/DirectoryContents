using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DirectoryContents.Classes;

namespace DirectoryContents.ViewModels
{
    public class MainWindowViewModel : NotifyObject
    {
        #region Public Members

        public static RoutedCommand BrowseCommand = new RoutedCommand();
        public static RoutedCommand ParseCommand = new RoutedCommand();

        #endregion Public Members

        #region Private Members

        private string m_DirectoryToParse = string.Empty;
        private volatile bool m_IsParsing = false;

        #endregion Private Members

        #region Public Properties

        public string DirectoryToParse
        {
            get { return m_DirectoryToParse; }

            set
            {
                if (string.IsNullOrWhiteSpace(m_DirectoryToParse) ||
                    m_DirectoryToParse.Equals(value, StringComparison.OrdinalIgnoreCase) == false)
                {
                    m_DirectoryToParse = value;

                    RaisePropertyChanged(nameof(DirectoryToParse));
                }
            }
        }

        #endregion Public Properties

        #region constructor

        public MainWindowViewModel() { }

        #endregion

        #region Public Methods

        internal bool CanGenerate()
        {
            return string.IsNullOrWhiteSpace(m_DirectoryToParse).Equals(false);
        }

        internal bool IsParsing()
        {
            return m_IsParsing;
        }

        #endregion Public Methods
    }
}
