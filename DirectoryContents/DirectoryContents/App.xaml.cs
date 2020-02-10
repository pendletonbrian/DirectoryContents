using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DirectoryContents
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private Members

        private const string m_Help_1 = "/?";
        private const string m_Help_2 = "--help";
        private const string m_Help_3 = "-help";

        private const string m_DIRECTORY_PATH = "directory_path";
        private const string m_Algorithim_Crc = "-CRC";
        private const string m_Algorithim_Md5 = "-MD5";
        private const string m_Algorithim_Sha_1 = "-SHA1";
        private const string m_Algorithim_Sha_256 = "-SHA256";
        private const string m_Algorithim_Sha_384 = "-SHA384";
        private const string m_Algorithim_Sha_512 = "-SHA512";

        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Application name: \"{AppDomain.CurrentDomain.FriendlyName}\".");

            Console.WriteLine(sb.ToString());

            if (e.Args.Length > 0)
            {
                int count = 0;

                foreach (string arg in e.Args)
                {
                    sb.AppendLine($"  Argument {++count}: \"{arg}\".");
                }

                Console.WriteLine(sb.ToString());

                if (e.Args[0].Equals(m_Help_1, StringComparison.OrdinalIgnoreCase) ||
                    e.Args[0].Equals(m_Help_2, StringComparison.OrdinalIgnoreCase) ||
                    e.Args[0].Equals(m_Help_3, StringComparison.OrdinalIgnoreCase))
                {
                    ShowUsage();

                    Shutdown(0);
                }

                string directoryPath = e.Args[0];

                if (Directory.Exists(directoryPath) == false)
                {
                    Console.Error.WriteLine($"The directory \"{directoryPath}\" does not exist.");

                    Shutdown(-1);
                }

                ViewModels.DirectoryViewModel vm = new ViewModels.DirectoryViewModel(null)
                {
                    DirectoryToParse = directoryPath
                };

                vm.Parse();

                ShowUsage();

                Shutdown(0);
            }
            else
            {
                Views.MainWindow mainWindow = new Views.MainWindow();

                mainWindow.Show();
            }
        }

        private static void ShowUsage()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Empty);
            sb.AppendLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} [{m_DIRECTORY_PATH}] {m_Algorithim_Crc}");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Where");
            sb.AppendLine($"\t{m_DIRECTORY_PATH}\tThe fully qualified path of the directory to parse.");

            sb.AppendLine(string.Empty);
            sb.AppendLine("Options (flags are not case-sensitive)");
            sb.AppendLine($"\t{m_Algorithim_Crc}\t\tUse the CRC-32 (32 bit cyclic redundancy check) checksum.");
            sb.AppendLine($"\t{m_Algorithim_Md5}\t\tUse the MD5 checksum.");
            sb.AppendLine($"\t{m_Algorithim_Sha_1}\t\tUse the SHA-1 checksum.");
            sb.AppendLine($"\t{m_Algorithim_Sha_256}\t\tUse the SHA-256 checksum.");
            sb.AppendLine($"\t{m_Algorithim_Sha_384}\t\tUse the SHA-384 checksum.");
            sb.AppendLine($"\t{m_Algorithim_Sha_512}\t\tUse the SHA-512 checksum.");

            Console.Error.WriteLine(sb.ToString());
        }

    }
}
