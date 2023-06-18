using MetadataExtractor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Apprentice.Tools.OS {
    public class MetadataReader {

        public string FilePath { get; private set; }
        public FileInfo FileInfo { get; private set; }
        public IReadOnlyList<MetadataExtractor.Directory> Metadata { get; private set; }
        public string AllInfo { get; private set; } = string.Empty;
        public Size ImageSize { get; private set; }
        public string TextData { get; private set; } = string.Empty;

        /// <summary>Filename</summary>
        public string Name => FileInfo.Name;
        /// <summary>File size in bytes</summary>
        public long Filesize => FileInfo.Length;
        public DateTime CreateDate => FileInfo.CreationTime;
        public DateTime ModifiedDate => FileInfo.LastWriteTime;

        public MetadataReader(string imageFile) {
            FilePath = imageFile;
            FileInfo = new FileInfo(imageFile);

            Metadata = ImageMetadataReader.ReadMetadata(imageFile);
            int width = 0;
            int height = 0;
            foreach (var item in Metadata) {
                foreach (var tag in item.Tags) {
                    if (tag.Name.ToLower() == "image width") width = int.Parse(tag.Description ?? "0");
                    if (tag.Name.ToLower() == "image height") height = int.Parse(tag.Description ?? "0");
                    if (tag.Name.ToLower() == "textual data") TextData += (TextData.Length > 0 ? "\n" : "") + tag.Description;
                    AllInfo += (AllInfo.Length > 0 ? "\n" : "") + $"{item.Name}:{tag.Name} = {tag.Description}";
                }
            }
        }

        public string GetPositivePrompt() {
            Match m = Regex.Match(TextData, @"parameters:\s*([\s\S]*?)(\nNegative prompt:(?![\s\S]*?\nNegative prompt:)|\n.*$)");
            if (m.Success)
                return m.Groups[1].Value;
            return string.Empty;
        }

        public string GetNegativePrompt() {
            Match m = Regex.Match(TextData, @"[\s\S]*Negative prompt:\s*([\s\S]*?)\n.*$");
            if (m.Success)
                return m.Groups[1].Value;
            return string.Empty;
        }

        public string GetPromptInfo() {
            Match m = Regex.Match(TextData, @"\n(.*)$");
            if (m.Success)
                return m.Groups[1].Value;
            return string.Empty;
        }
    }
}
