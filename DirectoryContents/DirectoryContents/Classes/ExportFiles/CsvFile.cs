using System.IO;
using System.Text;
using DirectoryContents.Models;

namespace DirectoryContents.Classes.ExportFiles
{
    internal class CsvFile : IFileExport
    {
        private static string GetLine(DirectoryItem node)
        {
            if (string.IsNullOrWhiteSpace(node.Checksum))
            {
                return $"\"{node.Filepath}\",\"{node.ItemName}\",";
            }
            else
            {
                return $"\"{node.Filepath}\",\"{node.ItemName}\",\"{node.Checksum}\"";
            }
        }

        private void ExportNode(StringBuilder sb, DirectoryItem node)
        {
            foreach (DirectoryItem childNode in node.Items)
            {
                sb.AppendLine(GetLine(childNode));

                if (childNode.HasChildren)
                {
                    ExportNode(sb, childNode);

                    continue;
                }
            }
        }

        public void Export(DirectoryItem rootNode, string fullyQualifiedFilepath, StringBuilder sb)
        {
            sb.AppendLine(rootNode.ItemName);

            sb.AppendLine("\"File path\",\"File/Directory name\",Checksum");

            foreach (DirectoryItem node in rootNode.Items)
            {
                sb.AppendLine(GetLine(node));

                if (node.HasChildren)
                {
                    ExportNode(sb, node);
                }
            }

            using (StreamWriter writer = new StreamWriter(fullyQualifiedFilepath))
            {
                writer.Write(sb.ToString());
                writer.Flush();
            }
        }
    }
}
