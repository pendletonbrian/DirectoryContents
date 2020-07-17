using System;

namespace DirectoryContents.Classes.ExportFiles
{
    public static class FileExporterFactory
    {
        public static IFileExport Get(Enumerations.ExportFileStructure exportFileStructure)
        {
            IFileExport exportType = new Empty();

            switch (exportFileStructure)
            {
                case Enumerations.ExportFileStructure.None:

                    // Do nothing. Keep the case so that we know it's being
                    // handled and it's not a mistake.
                    break;

                case Enumerations.ExportFileStructure.TextFile:
                    exportType = new TextFile();
                    break;

                case Enumerations.ExportFileStructure.TextFlat:
                    exportType = new TextFlat();
                    break;

                case Enumerations.ExportFileStructure.CSV:
                    exportType = new CsvFile();
                    break;

                default:
                    throw new ArgumentException($"Unhandled {nameof(Enumerations.ExportFileStructure)}: {exportFileStructure}");
            }

            return exportType;
        }
    }
}