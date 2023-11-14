﻿using EpubBuilder;
using System.CommandLine;

namespace EpubBuilderCLI;

class Program
{
    struct BuildCommandArgs
    {
        public string MdPath;
        public string CoverPath;
        public string BuildPath;
        public string Language;
        public string Title;
        public string Author;
        public string Uuid;
        public int SplitLevel;
    }
    public static void Main(string[] args)
    {
        ParseArgs(args);
    }

    private static void ParseArgs(string[] args)
    {
        var rootCommand = new RootCommand();

        var mdOption =  new Option<string?>(aliases: ["-m", "--markdown"], description: "Markdown Path") { IsRequired = true };
        var coverOption = new Option<string?>(aliases: ["-c", "--cover"], description: "Cover Path", getDefaultValue: () => "");
        var buildOption = new Option<string?>(aliases: ["-b", "--build"], description: "Build Path", getDefaultValue: () => "");
        var languageOption = new Option<string?>(aliases: ["-l", "--language"], description: "Epub Language", getDefaultValue: () => "zh");
        var titleOption = new Option<string?>(aliases: ["-t", "--title"], description: "Epub Title", getDefaultValue: () => "");
        var authorOption = new Option<string?>(aliases: ["-a", "--author"], description: "Epub Author", getDefaultValue: () => "EpubBuilder");
        var uuidOption = new Option<string?>(aliases: ["-u", "--uuid"], description: "Epub universally unique identifier", getDefaultValue: () => "");
        var splitOption = new Option<int>(aliases: ["-s", "--split"], description: "Split Level", getDefaultValue: () => 1);

        var buildCommand = new Command("build", "build epub book")
        {
            mdOption,
            coverOption,
            buildOption,
            languageOption,
            titleOption,
            authorOption,
            uuidOption,
            splitOption
        };

        buildCommand.SetHandler((mdpath, cover, build, language, title, author, uuid, splitLevel) =>
        {
            var buildCommandArgs = new BuildCommandArgs
            {
                MdPath = mdpath,
                CoverPath = cover,
                BuildPath = build,
                Language = language,
                Title = title,
                Author = author,
                Uuid = uuid,
                SplitLevel = splitLevel
            };

            var (epubMetadata, buildMetadata, buildPath) = HandleCommandLine(buildCommandArgs);
            BuildEpub(epubMetadata, buildMetadata, buildPath);

        },mdOption, coverOption, buildOption, languageOption, titleOption, authorOption, uuidOption, splitOption);

        rootCommand.AddCommand(buildCommand);
        rootCommand.Invoke(args);
    }

    private static void BuildEpub(EpubMetadata epubMetadata, BuildMetadata buildMetadata, string buildPath)
    {
        var epub = new Epub(epubMetadata, buildMetadata);
        epub.CreateEpub().Save(buildPath);
    }

    private static (EpubMetadata epubMetadata, BuildMetadata buildMetadata, string buildPath) HandleCommandLine(BuildCommandArgs args)
    {
        var epubMetadata = new EpubMetadata
        {
            Title = args.Title ?? Path.GetFileNameWithoutExtension(args.MdPath),
            Author = args.Author ?? "EpubBuilder",
            Language = args.Language ?? "zh",
            Uuid = args.Uuid ?? ""
        };

        var mdLines = File.ReadAllLines(args.MdPath).ToList();
        var buildMetadata = new BuildMetadata(mdLines, args.MdPath, args.CoverPath, args.SplitLevel);

        if (string.IsNullOrEmpty(args.BuildPath))
        {
            args.BuildPath = Path.Combine(Path.GetDirectoryName(args.MdPath)!, $"{Path.GetFileNameWithoutExtension(args.MdPath)}.epub");
        }
        return (epubMetadata, buildMetadata, args.BuildPath);
    }
}