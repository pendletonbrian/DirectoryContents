﻿using System;
using System.IO;
using System.Text;
using System.Windows;
using DirectoryContents.Classes;
using DirectoryContents.Classes.Checksums;
using DirectoryContents.Classes.ExportFiles;
using DirectoryContents.ViewModels;

namespace DirectoryContents
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private Members

        private const int FAILURE = -1;
        private const string m_Algorithim_Md5 = "-MD5";
        private const string m_Algorithim_Sha_1 = "-SHA1";
        private const string m_Algorithim_Sha_256 = "-SHA256";
        private const string m_Algorithim_Sha_384 = "-SHA384";
        private const string m_Algorithim_Sha_512 = "-SHA512";
        private const string m_Directory_Path = "directory_path";
        private const string m_Help_1 = "/?";
        private const string m_Help_2 = "--help";
        private const string m_Help_3 = "-help";
        private const string m_Results_File = "results_file";
        private const int SUCCESS = 0;
        private const string m_Export_TextFlat = "-TFlat";
        private const string m_Export_TextFile = "-TFile";
        private const string m_Export_CsvFile = "-TCsv";
        private const int NumberOfRequiredArguments = 4;

        #endregion Private Members

        private static IHashAlgorithim GetAlgorithim(string flag)
        {
            if (string.IsNullOrWhiteSpace(flag))
            {
                throw new ArgumentException($"The checksum algorithim flag is empty/null.");
            }

            Enumerations.ChecksumAlgorithim algorithim = Enumerations.ChecksumAlgorithim.None;

            if (flag.Equals(m_Algorithim_Md5, StringComparison.OrdinalIgnoreCase))
            {
                Log("Algorithim is MD5.");

                algorithim = Enumerations.ChecksumAlgorithim.MD5;
            }
            else if (flag.Equals(m_Algorithim_Sha_1, StringComparison.OrdinalIgnoreCase))
            {
                Log("Algorithim is SHA-1.");

                algorithim = Enumerations.ChecksumAlgorithim.SHA1;
            }
            else if (flag.Equals(m_Algorithim_Sha_256, StringComparison.OrdinalIgnoreCase))
            {
                Log("Algorithim is SHA-256.");

                algorithim = Enumerations.ChecksumAlgorithim.SHA256;
            }
            else if (flag.Equals(m_Algorithim_Sha_384, StringComparison.OrdinalIgnoreCase))
            {
                Log("Algorithim is SHA384.");

                algorithim = Enumerations.ChecksumAlgorithim.SHA384;
            }
            else if (flag.Equals(m_Algorithim_Sha_512, StringComparison.OrdinalIgnoreCase))
            {
                Log("Algorithim is SHA-512.");

                algorithim = Enumerations.ChecksumAlgorithim.SHA512;
            }
            else
            {
                throw new ArgumentException($"The checksum algorithim flag is unhandled: \"{flag}\".");
            }

            return HashAlgorithimFactory.Get(algorithim);
        }

        private static IFileExport GetExport(string flag)
        {
            Enumerations.ExportFileStructure export = Enumerations.ExportFileStructure.None;

            if (flag.Equals(m_Export_TextFile, StringComparison.OrdinalIgnoreCase))
            {
                export = Enumerations.ExportFileStructure.TextFile;
            }
            else if (flag.Equals(m_Export_TextFlat, StringComparison.OrdinalIgnoreCase))
            {
                export = Enumerations.ExportFileStructure.TextFlat;
            }
            else if (flag.Equals(m_Export_CsvFile, StringComparison.OrdinalIgnoreCase))
            {
                export = Enumerations.ExportFileStructure.CSV;
            }
            else
            {
                throw new ArgumentException($"The export flag is unhandled: \"{flag}\".");
            }

            Log($"Export is {export}");

            return FileExporterFactory.Get(export);
        }

        private static void Log(string msg)
        {
            Console.Out.WriteLine($"{msg}");
        }

        private static void LogError(string msg)
        {
            Console.Error.WriteLine($"{msg}");
        }

        private static void ShowUsage()
        {
            var val = TreeChecksumViewModel.IsFIPSEnabled();
            bool isFipsEnabled = val ?? false;

            string startString = $"Usage: { AppDomain.CurrentDomain.FriendlyName}";

            // 32 is the ASCII code for a space.
            string spacing = new string(Convert.ToChar(32), startString.Length);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Empty);
            sb.AppendLine($"{startString} [{m_Directory_Path}]");
            sb.AppendLine($"{spacing} [{m_Results_File}]");

            if (isFipsEnabled)
            {
                sb.AppendLine($"{spacing} [{m_Algorithim_Md5} (Disabled) | {m_Algorithim_Sha_1} | {m_Algorithim_Sha_256} | {m_Algorithim_Sha_384} | {m_Algorithim_Sha_512}]");
            }
            else
            {
                sb.AppendLine($"{spacing} [{m_Algorithim_Md5} | {m_Algorithim_Sha_1} | {m_Algorithim_Sha_256} | {m_Algorithim_Sha_384} | {m_Algorithim_Sha_512}]");
            }

            sb.AppendLine($"{spacing} [{m_Export_TextFile} | {m_Export_TextFlat} | {m_Export_CsvFile}]");

            sb.AppendLine(string.Empty);
            sb.AppendLine("Where");
            sb.AppendLine($"\t{m_Directory_Path}\tThe fully qualified path of the directory to parse.");
            sb.AppendLine($"\t{m_Results_File}\tThe fully qualified path of the file in which to save the results.");

            sb.AppendLine(string.Empty);
            sb.AppendLine("Options (flags are not case-sensitive)");

            if (isFipsEnabled)
            {
                sb.AppendLine($"\t{m_Algorithim_Md5}\t\tOption is disabled: FIPS is enabled on this machine.");
            }
            else
            {
                // RFC 1321
                sb.AppendLine($"\t{m_Algorithim_Md5}\t\tCalculate the 128-bit/16 byte MD5 message digest, as defined in RFC 1321.");
            }

            // FIPS-180-1
            sb.AppendLine($"\t{m_Algorithim_Sha_1}\t\tCalculate the 160-bit/20 byte SHA-1 message digest, as defined in FIPS-180-1.");

            // FIPS-180-2
            sb.AppendLine($"\t{m_Algorithim_Sha_256}\t\tCalculate the 256-bit/32 byte SHA-256 message digest, as defined in FIPS-180-2.");

            // FIPS-180-2
            sb.AppendLine($"\t{m_Algorithim_Sha_384}\t\tCalculate the 384-bit/48 byte SHA-384 message digest, as defined in FIPS-180-2.");

            // FIPS-180-2
            sb.AppendLine($"\t{m_Algorithim_Sha_512}\t\tCalculate the 512-bit/64 byte SHA-512 message digest, as defined in FIPS-180-2.");

            sb.AppendLine(string.Empty);
            sb.AppendLine($"\t{ m_Export_TextFile}\t\tExport into a text file, keeping the directory structure.");
            sb.AppendLine($"\t{ m_Export_TextFlat}\t\tExport into a text file, listing every file and folder with the fully qualified path.");
            sb.AppendLine($"\t{ m_Export_CsvFile}\t\tExport into a CSV file, listing every file and folder with the fully qualified path.");

            sb.AppendLine(string.Empty);
            sb.AppendLine($"NOTE: All algorithims generate the same value for a given file as their Linux counterparts (md5sum, sha512sum, etc).");

            LogError(sb.ToString());
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            Log($"Application name: \"{AppDomain.CurrentDomain.FriendlyName}\".");

            if (e.Args.Length > 0)
            {
                // To start off, let's print all given arguments for
                // debugging's sake.
                int count = 0;

                foreach (string arg in e.Args)
                {
                    sb.AppendLine($"  Argument {++count}: \"{arg}\".");
                }

                Log(sb.ToString());

                // If the user just wants to see the help, print and exit.
                if (e.Args[0].Equals(m_Help_1, StringComparison.OrdinalIgnoreCase) ||
                    e.Args[0].Equals(m_Help_2, StringComparison.OrdinalIgnoreCase) ||
                    e.Args[0].Equals(m_Help_3, StringComparison.OrdinalIgnoreCase))
                {
                    ShowUsage();

                    Shutdown(SUCCESS);

                    return;
                }

                // The correct number of arguments were not supplied. Print help
                // and exit.
                if (e.Args.Length != NumberOfRequiredArguments)
                {
                    ShowUsage();

                    Shutdown(FAILURE);

                    return;
                }

                // First argument should be the source directory.
                string directoryToParse = e.Args[0];

                if (Directory.Exists(directoryToParse) == false)
                {
                    LogError($"The directory \"{directoryToParse}\" does not exist.");

                    ShowUsage();

                    Shutdown(FAILURE);

                    return;
                }

                // Second argument should be the results file.
                string resultsFile = e.Args[1];

                // Third argument should be which algorithim to use for the checksum.
                IHashAlgorithim hashAlgorithim = null;

                try
                {
                    hashAlgorithim = GetAlgorithim(e.Args[2]);

                    var val = TreeChecksumViewModel.IsFIPSEnabled();
                    bool isFipsEnabled = val ?? false;

                    if (isFipsEnabled &&
                        hashAlgorithim.Equals(Enumerations.ChecksumAlgorithim.MD5))
                    {
                        LogError("FIPS is enabled, cannot use MD5.");

                        ShowUsage();

                        Shutdown(FAILURE);
                    }

                }
                catch (Exception ex)
                {
                    LogError($"The checksum algorithim could not be determined from the argument \"{e.Args[2]}\"..");
                    LogError(ex.Message);

                    ShowUsage();

                    Shutdown(FAILURE);

                    return;
                }

                // Fourth argument is the export format.
                IFileExport fileExport = null;
                try
                {
                    fileExport = GetExport(e.Args[3]);
                }
                catch (Exception ex)
                {
                    LogError($"The export format could not be determined from the argument \"{e.Args[3]}\"..");
                    LogError(ex.Message);

                    ShowUsage();

                    Shutdown(FAILURE);

                    return;
                }

                DirectoryViewModel vm = new DirectoryViewModel(null)
                {
                    DirectoryToParse = directoryToParse
                };

                await vm.ParseAsync();

                await vm.ExportAsync(resultsFile);

                Shutdown(SUCCESS);

                return;
            }
            else
            {
                ShowUsage();

                Views.MainWindow mainWindow = new Views.MainWindow();

                mainWindow.Show();
            }
        }
    }
}
