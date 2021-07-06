using System.Text;
using DirectoryContents.Models;

namespace DirectoryContents.Classes.ExportFiles
{
    public interface IFileExport
    {
        void Export(DirectoryItem rootNode, string fullyQualifiedFilepath, StringBuilder stringBuilder);
    }
}
