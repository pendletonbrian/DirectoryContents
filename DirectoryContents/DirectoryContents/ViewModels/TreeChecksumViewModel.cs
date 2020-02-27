using DirectoryContents.Classes;
using DirectoryContents.Classes.Checksums;
using DirectoryContents.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DirectoryContents.ViewModels
{
    public class TreeChecksumViewModel : BaseUserControlViewModel
    {
        #region Public Members

        public static RoutedCommand CancelCommand = new RoutedCommand();
        public static RoutedCommand GenerateCommand = new RoutedCommand();

        #endregion Public Members

        #region Private Members

        private readonly List<KeyValuePair<string, string>> m_AlgorithimList = new List<KeyValuePair<string, string>>();
        private CancellationTokenSource m_CancellationTokenSource = null;
        private volatile bool m_GenerationInProgress = false;
        private DirectoryItem m_RootNode = null;
        private Enumerations.ChecksumAlgorithim m_SelectedAlgorithim = Enumerations.ChecksumAlgorithim.None;

        #endregion Private Members

        #region Public Properties

        public List<KeyValuePair<string, string>> AlgorithimList
        {
            get { return m_AlgorithimList; }

            // Set in the constructor, and not editable after that, as it's just
            // a lookup.
            private set { }
        }

        public DirectoryItem RootNode
        {
            get { return m_RootNode; }

            private set
            {
                if (m_RootNode is null ||
                    m_RootNode.Equals(value) == false)
                {
                    m_RootNode = value;

                    RaisePropertyChanged(nameof(RootNode));
                }
            }
        }

        public Enumerations.ChecksumAlgorithim SelectedAlgorithim
        {
            get { return m_SelectedAlgorithim; }

            set
            {
                if (m_SelectedAlgorithim.Equals(value) == false)
                {
                    Log($"{nameof(FileChecksumViewModel)}.{nameof(SelectedAlgorithim)} is changing from {m_SelectedAlgorithim} to {value}.");

                    m_SelectedAlgorithim = value;

                    RaisePropertyChanged(nameof(SelectedAlgorithim));
                }
            }
        }

        public bool WasCancelled { get; private set; }

        #endregion Public Properties

        #region constructor

        public TreeChecksumViewModel(MainWindowViewModel viewModel, DirectoryItem rootNode) : base(viewModel)
        {
            m_AlgorithimList = Enumerations.GetEnumValueDescriptionPairs(typeof(Enumerations.ChecksumAlgorithim));
            RaisePropertyChanged(nameof(AlgorithimList));

            RootNode = rootNode;
        }

        #endregion constructor

        #region Public Methods

        internal void CancelGeneration()
        {
            m_CancellationTokenSource.Cancel(true);
            WasCancelled = true;
        }

        internal async Task GenerateChecksumsAsync(CancellationTokenSource tokenSource)
        {
            try
            {
                Log($"{nameof(TreeChecksumViewModel)}.{nameof(GenerateChecksumsAsync)}: Start");

                IHashAlgorithim algorithim = HashAlgorithimFactory.Get(SelectedAlgorithim);
                Hasher hasher = new Hasher(algorithim);

                m_CancellationTokenSource = tokenSource;
                CancellationToken token = tokenSource.Token;
                token.ThrowIfCancellationRequested();

                m_GenerationInProgress = true;

                foreach (DirectoryItem childNode in RootNode.Items)
                {
                    await GenerateChecksumAsync(childNode, hasher, token);
                }
            }
            finally
            {
                Log($"{nameof(TreeChecksumViewModel)}.{nameof(GenerateChecksumsAsync)}: End");

                m_GenerationInProgress = false;

                m_CancellationTokenSource.Dispose();
            }
        }

        internal bool IsAlgorithimSelected()
        {
            return SelectedAlgorithim.Equals(Enumerations.ChecksumAlgorithim.None) == false;
        }

        internal bool IsGenerationInProgress()
        {
            return m_GenerationInProgress;
        }

        #endregion Public Methods

        #region Private Methods

        private async Task GenerateChecksumAsync(DirectoryItem node, Hasher hasher, CancellationToken token)
        {
            if (node.IsDirectory.Equals(false))
            {
                Log($"{nameof(TreeChecksumViewModel)}.{nameof(GenerateChecksumAsync)} => File: {node.FullyQualifiedFilename}");

                if (token.IsCancellationRequested)
                {
                    Log("    Cancellation requested.");

                    return;
                }

                string checksum = string.Empty;

                ShowStatusMessage($"Hashing {node.FullyQualifiedFilename}");

                bool? result = await Task.Run(() => hasher.TryGetFileChecksum(node.FullyQualifiedFilename, out checksum));

                if (result.HasValue &&
                    result.Value)
                {
                    node.Checksum = checksum;

                    Log($"    Checksum: {checksum}");
                }
                else
                {
                    Log($"    There was no result from the hashing.");
                }

                return;
            }
            else
            {
                Log($"{nameof(TreeChecksumViewModel)}.{nameof(GenerateChecksumAsync)} => Directory: {node.FullyQualifiedFilename}");
            }

            if (token.IsCancellationRequested)
            {
                Log($"{nameof(TreeChecksumViewModel)}.{nameof(GenerateChecksumAsync)}: cancellation requested.");

                return;
            }

            foreach (DirectoryItem childNode in node.Items)
            {
                await GenerateChecksumAsync(childNode, hasher, token);
            }
        }

        #endregion Private Methods
    }
}