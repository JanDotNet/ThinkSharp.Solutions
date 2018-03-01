using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkSharp.Solutions.Infrastructure
{
    public static class IOHelper
    {
        public static void NormalizeAttributes(string directoryPath)
        {
            string[] filePaths = Directory.GetFiles(directoryPath);
            string[] subdirectoryPaths = Directory.GetDirectories(directoryPath);

            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
            foreach (string subdirectoryPath in subdirectoryPaths)
            {
                NormalizeAttributes(subdirectoryPath);
            }
            File.SetAttributes(directoryPath, FileAttributes.Normal);
        }

        public static bool IsEmpty(string directory)
            => !Directory.EnumerateFileSystemEntries(directory).Any();

        public static void ReplacePlaceholdersInFileNamesAndDirectories(string directoryPath, IEnumerable<KeyValuePair<string, string>> placeholders)
        {
            string[] filePaths = Directory.GetFiles(directoryPath);
            string[] subdirectoryPaths = Directory.GetDirectories(directoryPath);

            foreach (string filePath in filePaths)
            {
                var file = new FileInfo(filePath);
                var fileName = file.Name;
                var matchingPlaceholders = placeholders.Where(p => fileName.Contains(p.Key)).ToArray();
                if (matchingPlaceholders.Length > 0)
                {
                    foreach (var placeholder in matchingPlaceholders)
                        fileName = fileName.Replace(placeholder.Key, placeholder.Value);
                    file.MoveTo(Path.Combine(file.DirectoryName, fileName));
                }
            }
            foreach (string subdirectory in subdirectoryPaths)
            {
                var dir = new DirectoryInfo(subdirectory);
                var dirName = dir.Name;
                var matchingPlaceholders = placeholders.Where(p => dirName.Contains(p.Key)).ToArray();
                if (matchingPlaceholders.Length > 0)
                {
                    foreach (var placeholder in matchingPlaceholders)
                        dirName = dirName.Replace(placeholder.Key, placeholder.Value);
                    dir.MoveTo(Path.Combine(dir.Parent.FullName, dirName));
                }
            }

            foreach (string subdirectoryPath in Directory.GetDirectories(directoryPath))
            {
                ReplacePlaceholdersInFileNamesAndDirectories(subdirectoryPath, placeholders);
            }
        }

        public static void ReplacePlaceholdersInFilesContent(string directoryPath, IEnumerable<KeyValuePair<string, string>> placeholders)
        {
            string[] filePaths = Directory.GetFiles(directoryPath);
            string[] subdirectoryPaths = Directory.GetDirectories(directoryPath);

            foreach (string filePath in filePaths)
            {
                var fileContent = File.ReadAllText(filePath);
                var matchingPlaceholders = placeholders.Where(p => fileContent.Contains(p.Key)).ToArray();
                if (matchingPlaceholders.Length > 0)
                {
                    foreach (var placeholder in matchingPlaceholders)
                        fileContent = fileContent.Replace(placeholder.Key, placeholder.Value);
                    File.WriteAllText(filePath, fileContent);
                }
            }

            foreach (string subdirectoryPath in Directory.GetDirectories(directoryPath))
            {
                ReplacePlaceholdersInFilesContent(subdirectoryPath, placeholders);
            }
        }
    }
}
