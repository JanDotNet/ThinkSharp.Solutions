using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThinkSharp.Solutions.Infrastructure
{
    public static class IOHelper
    {
        private static Regex NewGuidRegex = new Regex("00000000-1111-0000-1111-000000000000", RegexOptions.Compiled);
        private static Regex NewCachedGuidRegex = new Regex("00000000-1111-0000-1111-[0-9a-f]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

        public static void ReplaceGuids(string directoryPath)
        {
            var guidMap = new Dictionary<string, string>();
            foreach (var file in Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories))
            {
                var fileContent = File.ReadAllText(file);
                var foundMatch = false;
                var newGuidMatchEvaluator = new MatchEvaluator(match =>
                {
                    foundMatch = true;
                    return Guid.NewGuid().ToString();
                });
                var newCachedGuidMatchEvaluator = new MatchEvaluator(input =>
                {
                    foundMatch = true;
                    string cachedGuid;
                    if (!guidMap.TryGetValue(input.Value, out cachedGuid))
                    {
                        cachedGuid = Guid.NewGuid().ToString();
                        guidMap.Add(input.Value, cachedGuid);
                    }
                    return cachedGuid;
                });

                fileContent = NewGuidRegex.Replace(fileContent, newGuidMatchEvaluator);
                fileContent = NewCachedGuidRegex.Replace(fileContent, newCachedGuidMatchEvaluator);

                if (foundMatch)
                {
                    File.WriteAllText(file, fileContent);
                }
            }
        }
    }
}
