using DirectoryContents.Models;
using System.IO;
using System.Text;

namespace DirectoryContents.Classes.ExportFiles
{
    internal class TextFile : IFileExport
    {
        public void Export(DirectoryItem rootNode, string fullyQualifiedFilepath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(rootNode.ItemName);

            foreach (DirectoryItem node in rootNode.Items)
            {
                if (string.IsNullOrWhiteSpace(node.Checksum))
                {
                    sb.AppendLine($"|\t{node.ItemName}");
                }
                else
                {
                    sb.AppendLine($"|\t{node.ItemName}  : {node.Checksum}");
                }

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

        private void ExportNode(StringBuilder sb, DirectoryItem node)
        {
            string tabs = new string('\t', node.Depth);

            foreach (DirectoryItem childNode in node.Items)
            {
                if (string.IsNullOrWhiteSpace(childNode.Checksum))
                {
                    sb.AppendLine($"{tabs}|\t{childNode.ItemName}");
                }
                else
                {
                    sb.AppendLine($"{tabs}|\t{childNode.ItemName} ({childNode.Checksum})");
                }

                if (childNode.HasChildren)
                {
                    ExportNode(sb, childNode);

                    continue;
                }
            }
        }
    }
}
