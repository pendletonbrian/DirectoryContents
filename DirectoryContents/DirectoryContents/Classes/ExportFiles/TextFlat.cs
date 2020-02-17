﻿using DirectoryContents.Models;
using System.IO;
using System.Text;

namespace DirectoryContents.Classes.ExportFiles
{
    internal class TextFlat : IFileExport
    {
        public void Export(DirectoryItem rootNode, string fullyQualifiedFilepath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(rootNode.ItemName);

            foreach (DirectoryItem node in rootNode.Items)
            {
                if (string.IsNullOrWhiteSpace(node.Checksum))
                {
                    sb.AppendLine($"{node.FullyQualifiedFilename}");                    
                }
                else
                {
                    sb.AppendLine($"{node.Checksum} : {node.FullyQualifiedFilename}");
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
            foreach (DirectoryItem childNode in node.Items)
            {
                if (string.IsNullOrWhiteSpace(childNode.Checksum))
                {
                    sb.AppendLine($"{childNode.FullyQualifiedFilename}");
                }
                else
                {
                    sb.AppendLine($"{childNode.Checksum} : {childNode.FullyQualifiedFilename}");
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
