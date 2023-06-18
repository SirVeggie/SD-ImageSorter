using Apprentice.Tools.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Apprentice.Personal.Tools {
    public static class ImageSorter {

        public static void SortFiles(string folder, string dest, string regex) {
            DirectoryInfo source = new(folder);
            DirectoryInfo destination = new(dest);
            if (!source.Exists || !destination.Exists) {
                Console.WriteLine(!source.Exists ? "Source folder doesn't exist" : "Destination folder doesn't exist");
            }

            Regex r = new(regex, RegexOptions.IgnoreCase);
            var files = source.GetFiles();
            int counter = 0;
            int max = files.Length;
            Console.WriteLine();
            foreach (FileInfo file in files) {
                Console.CursorLeft = 0;
                Console.Write($"Progress: {++counter}/{max}".PadRight(25));
                MoveFile(file, destination, r);
            }
            Console.WriteLine();
            Console.WriteLine("Finished");
        }

        public static void MoveFile(FileInfo file, DirectoryInfo destDir, Regex regex) {
            if (!CheckMatch(file.FullName, regex))
                return;
            string newPath = Increment($@"{destDir.FullName}\{file.Name}");
            if (!file.Exists) {
                Console.WriteLine("Failed to move image: File not found");
                return;
            }

            if (File.Exists(newPath)) {
                Console.WriteLine("Failed to move image: filename already exists");
                return;
            }

            try {
                file.MoveTo(newPath);
            } catch (Exception e) {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        private static string Increment(string filepath) {
            FileInfo file = new(filepath);
            string filename = Path.GetFileNameWithoutExtension(file.FullName);
            int counter = 1;
            while (file.Exists) {
                file = new($@"{file.DirectoryName}\{filename} ({++counter}){file.Extension}");
            }
            return file.FullName;
        }

        public static bool CheckMatch(string imageFile, Regex regex) {
            MetadataReader meta = new(imageFile);
            return regex.IsMatch(meta.TextData);
        }

        public static void NameContentsByDate(string directory) {
            DirectoryInfo dir = new(directory);
            if (!dir.Exists)
                throw new ArgumentException("Invalid directory");
            var files = dir.GetFiles();
            int counter = 0;
            int max = files.Length;

            string prev = string.Empty;
            int duplicates = 1;
            int errors = 0;
            files = files.OrderBy(x => GetDateName(x)).ToArray();
            foreach (var file in files) {
                counter++;
                string newName = GetDateName(file);
                if (newName == prev)
                    duplicates++;
                else
                    duplicates = 1;
                try {
                    file.MoveTo($@"{file.DirectoryName}\{newName}-{duplicates}{file.Extension}");
                    prev = newName;
                } catch {
                    errors++;
                }
            }

            if (errors > 0)
                Console.WriteLine($"Encountered {errors} errors, ({errors} files were not moved)");
        }

        public static string GetDateName(FileInfo file) {
            int year = file.LastWriteTime.Year;
            int month = file.LastWriteTime.Month;
            int day = file.LastWriteTime.Day;
            long timestamp = new DateTimeOffset(file.LastWriteTimeUtc).ToUnixTimeSeconds();
            long daytimestamp = new DateTimeOffset(new DateTime(year, month, day, 0, 0, 0)).ToUnixTimeSeconds();
            long seconds = timestamp - daytimestamp;
            return $"{year}-{month}-{day}-{seconds}";
        }

        public static string GetDateNameFull(FileInfo file) {
            FileInfo newFile = new($@"{file.DirectoryName}\{GetDateName(file)}-1{file.Extension}");

            int counter = 1;
            while (newFile.Exists) {
                newFile = new($@"{file.DirectoryName}\{GetDateName(file)}-{++counter}{file.Extension}");
            }

            return newFile.Name;
        }
    }
}
