using System.Text;
using DirectoryContents.Models;

namespace DirectoryContents.Classes.ExportFiles
{
    public class Empty : IFileExport
    {
        public void Export(DirectoryItem rootNode, string fullyQualifiedFilepath, StringBuilder sb)
        {
        }
    }
}
