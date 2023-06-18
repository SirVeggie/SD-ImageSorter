
using Apprentice.Personal.Tools;

int arguments = args.Length;
FileInfo regexFile = new($@"{new FileInfo(Environment.ProcessPath!).DirectoryName}\default_regex.txt");
string regex = regexFile.Exists ? File.ReadAllText(regexFile.FullName) : @"(nsfw|nude)(?<!Negative prompt[\s\S]*)";

if (arguments == 0) {
    string source = Directory.GetCurrentDirectory();
    string target = $@"{source}\sorted";

    Console.WriteLine("You can also use command line arguments: source_dir target_dir regex");
    Console.Write($"Give source directory ({source}):\n > ");
    string sourceInput = Console.ReadLine() ?? "";
    source = string.IsNullOrWhiteSpace(sourceInput) ? source : sourceInput;
    Console.WriteLine();

    Console.Write($"Give target directory ({target}):\n > ");
    string targetInput = Console.ReadLine() ?? "";
    target = string.IsNullOrWhiteSpace(targetInput) ? target : targetInput;
    if (!target.Contains('\\'))
        target = $@"{source}\{target}";
    Console.WriteLine();

    DirectoryInfo targetInfo = new(target);
    if (!targetInfo.Exists) {
        if (string.IsNullOrWhiteSpace(targetInput) || target.Contains(source))
            targetInfo.Create();
        else {
            Console.WriteLine("Target folder doesn't exist, create it manually for safety reasons");
            return;
        }
    }

    Console.Write($"Give metadata regex ({regex}):\n > ");
    string regexInput = Console.ReadLine() ?? "";
    regex = string.IsNullOrWhiteSpace(regexInput) ? regex : regexInput;

    ImageSorter.SortFiles(source, target, regex);
    return;
}

if (2 <= arguments && arguments <= 3) {
    Console.WriteLine($"Running image sorting with values\nSource: {args[0]}\nTarget: {args[1]}Regex: {args[2]}");
    string source = args[0];
    string target = args[1];
    if (!target.Contains('\\'))
        target = $@"{source}\{target}";
    if (arguments == 3) {
        regex = args[2];
    }
    ImageSorter.SortFiles(source, target, regex);
    return;
}

Console.WriteLine("Invalid amount of arguments");
