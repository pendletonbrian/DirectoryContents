using System.Collections.Generic;
using System.Windows.Input;
using DirectoryContents.Classes;

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

        #region Private Members

        private readonly List<KeyValuePair<string, string>> m_AlgorithimList = new List<KeyValuePair<string, string>>();
        private Enumerations.ChecksumAlgorithim m_SelectedAlgorithim = Enumerations.ChecksumAlgorithim.None;

        #endregion Private Members

        #region Public Properties

        public List<KeyValuePair<string, string>> AlgorithimList
        {
            get { return m_AlgorithimList; }

            private set { }
        }

        public Enumerations.ChecksumAlgorithim SelectedAlgorithim
        {
            get { return m_SelectedAlgorithim; }

            set
            {
                if (m_SelectedAlgorithim.Equals(value) == false)
                {
                    Log($"{nameof(SettingsViewModel)}.{nameof(SelectedAlgorithim)} is changing from {m_SelectedAlgorithim} to {value}.");

                    m_SelectedAlgorithim = value;

                    RaisePropertyChanged(nameof(SelectedAlgorithim));
                }
            }
        }

        #endregion Public Properties

        #region constructor

        public SettingsViewModel(MainWindowViewModel viewModel) : base(viewModel)
        {
            m_AlgorithimList = Enumerations.GetEnumValueDescriptionPairs(typeof(Enumerations.ChecksumAlgorithim));
        }

        #endregion constructor
    }
}