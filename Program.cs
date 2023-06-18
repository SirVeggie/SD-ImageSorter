
using Apprentice.Personal.Tools;

int arguments = args.Length;

if (arguments == 0) {
    string source = Directory.GetCurrentDirectory();
    string target = $@"{source}\sorted";
    string regex = "nude|sex|pussy|cum|fellatio|blow ?job";

    Console.WriteLine("You can also use command line arguments: source_dir target_dir regex");
    Console.Write($"Give source directory ({source}):\n > ");
    string sourceInput = Console.ReadLine() ?? "";
    source = string.IsNullOrWhiteSpace(sourceInput) ? source : sourceInput;
    Console.WriteLine();

    Console.Write($"Give target directory ({target}):\n > ");
    string targetInput = Console.ReadLine() ?? "";
    target = string.IsNullOrWhiteSpace(targetInput) ? target : targetInput;
    Console.WriteLine();

    DirectoryInfo targetInfo = new DirectoryInfo(target);
    if (!targetInfo.Exists) {
        if (string.IsNullOrWhiteSpace(targetInput))
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

if (arguments != 3) {
    Console.WriteLine("Invalid amount of arguments");
    return;
}

Console.WriteLine($"Running image sorting with values\nSource: {args[0]}\nTarget: {args[1]}Regex: {args[2]}");
ImageSorter.SortFiles(args[0], args[1], args[2]);
