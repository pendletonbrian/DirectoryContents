﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DirectoryContents.Classes.Checksums;
using static DirectoryContents.Classes.Enumerations;

namespace DirectoryContents
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private Members

        private const int SUCCESS = 0;
        private const int FAILURE = -1;

        private const string m_Help_1 = "/?";
        private const string m_Help_2 = "--help";
        private const string m_Help_3 = "-help";

        private const string m_Directory_Path = "directory_path";
        private const string m_Results_File = "results_file";
        private const string m_Algorithim_Crc = "-CRC32";
        private const string m_Algorithim_Md5 = "-MD5";
        private const string m_Algorithim_Sha_1 = "-SHA1";
        private const string m_Algorithim_Sha_256 = "-SHA256";
        private const string m_Algorithim_Sha_384 = "-SHA384";
        private const string m_Algorithim_Sha_512 = "-SHA512";

        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            Console.Out.WriteLine($"Application name: \"{AppDomain.CurrentDomain.FriendlyName}\".");

            if (e.Args.Length > 0)
            {
                // To start off, let's print all given arguments,
                // for debugging's sake.
                int count = 0;

                foreach (string arg in e.Args)
                {
                    sb.AppendLine($"  Argument {++count}: \"{arg}\".");
                }

                Console.Out.WriteLine(sb.ToString());

                // If the user just wants to see the help, print and exit.
                if (e.Args[0].Equals(m_Help_1, StringComparison.OrdinalIgnoreCase) ||
                    e.Args[0].Equals(m_Help_2, StringComparison.OrdinalIgnoreCase) ||
                    e.Args[0].Equals(m_Help_3, StringComparison.OrdinalIgnoreCase))
                {
                    ShowUsage();

                    Shutdown(SUCCESS);

                    return;
                }

                // The correct number of arguments were not supplied.
                // Print help and exit.
                if (e.Args.Length != 3)
                {
                    ShowUsage();

                    Shutdown(FAILURE);

                    return;
                }

                // First agrument should be the source directory.
                string directoryToParse = e.Args[0];

                if (Directory.Exists(directoryToParse) == false)
                {
                    Console.Error.WriteLine($"The directory \"{directoryToParse}\" does not exist.");

                    ShowUsage();

                    Shutdown(FAILURE);

                    return;
                }

                // Second argument should be the results file.
                string resultsFile = e.Args[1];

                // Third argument should be which algorithim 
                //  to use for the checksum.
                IHashAlgorithim hashAlgorithim = null;

                try
                {
                    hashAlgorithim = GetAlgorithim(e.Args[2]);
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine($"The checksum algorithim could not be determined from the argument \"{e.Args[2]}\"..");
                    Console.Error.WriteLine(ex.Message);

                    ShowUsage();

                    Shutdown(FAILURE);

                    return;
                }

                ViewModels.DirectoryViewModel vm = new ViewModels.DirectoryViewModel(null, hashAlgorithim)
                {
                    DirectoryToParse = directoryToParse
                };

                vm.Parse();

                vm.Export(resultsFile);

                Shutdown(SUCCESS);

                return;
            }
            else
            {
                Views.MainWindow mainWindow = new Views.MainWindow();

                mainWindow.Show();
            }
        }

        private static void ShowUsage()
        {
            string startString = $"Usage: { AppDomain.CurrentDomain.FriendlyName}";

            // 32 is the ASCII code for a space.
            string spacing = new string(Convert.ToChar(32), startString.Length);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Empty);
            sb.AppendLine($"{startString} [{m_Directory_Path}] [{m_Results_File}]");
            sb.AppendLine($"{spacing.ToString()} [{m_Algorithim_Crc} | {m_Algorithim_Md5} | {m_Algorithim_Sha_1} |");
            sb.AppendLine($"{spacing.ToString()}  {m_Algorithim_Sha_256} | {m_Algorithim_Sha_384} | {m_Algorithim_Sha_512}]");

            sb.AppendLine(string.Empty);
            sb.AppendLine("Where");
            sb.AppendLine($"\t{m_Directory_Path}\tThe fully qualified path of the directory to parse.");
            sb.AppendLine($"\t{m_Results_File}\tThe fully qualified path of the file in which to save the results.");

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

        private static IHashAlgorithim GetAlgorithim(string flag)
        {
            if (string.IsNullOrWhiteSpace(flag))
            {
                throw new ArgumentException($"The checksum algorithim flag is empty/null.");
            }

            IHashAlgorithim algorithim = null;

            if (flag.Equals(m_Algorithim_Crc, StringComparison.OrdinalIgnoreCase))
            {
                algorithim = new CRC32();
            }
            else if (flag.Equals(m_Algorithim_Md5, StringComparison.OrdinalIgnoreCase))
            {
                algorithim = new MD5();
            }
            else if (flag.Equals(m_Algorithim_Sha_1, StringComparison.OrdinalIgnoreCase))
            {
                algorithim = new SHA1();
            }
            else if (flag.Equals(m_Algorithim_Sha_256, StringComparison.OrdinalIgnoreCase))
            {
                algorithim = new SHA256();
            }
            else if (flag.Equals(m_Algorithim_Sha_384, StringComparison.OrdinalIgnoreCase))
            {
                algorithim = new SHA384();
            }
            else if (flag.Equals(m_Algorithim_Sha_512, StringComparison.OrdinalIgnoreCase))
            {
                algorithim = new SHA512();
            }
            else
            {
                throw new ArgumentException($"The checksum algorithim flag is unhandled: \"{flag}\".");
            }

            return algorithim;
        }

    }
}
