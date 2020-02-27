using DirectoryContents.Models;
using System.Text;

namespace DirectoryContents.Classes.ExportFiles
{
    public interface IFileExport
    {
        void Export(DirectoryItem rootNode, string fullyQualifiedFilepath, StringBuilder stringBuilder);
    }
}