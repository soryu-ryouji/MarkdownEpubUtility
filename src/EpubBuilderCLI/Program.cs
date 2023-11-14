using EpubBuilder;
using CommandLine;

namespace EpubBuilderCLI;

class Program
{
    public static void Main(string[] args)
    {
        var (epubMetadata, buildMetadata, buildPath) = ParseCLI.ParseCommandLineArgs(args);

        var epub = new Epub(epubMetadata, buildMetadata);
        epub.CreateEpub().Save(buildPath);
    }
}

class Options
{
    // Required
    [Option('m', "markdown", Required = false, HelpText = "Markdown Path")]
    public string? MdPath { get; set; }
    // Option
    [Option('c', "cover", Required = false, HelpText = "Cover Path ")]
    public string? CoverPath { get; set; }

    [Option('b', "build", Required = false, HelpText = "Build Path")]
    public string? BuildPath { get; set; }

    [Option('l',"language",Required = false,HelpText = "Epub Language")]
    public string? Language { get; set; }

    [Option('t',"title",Required = false,HelpText = "Epub Title")]
    public string? Title { get; set; }

    [Option('a',"author",Required = false,HelpText = "Epub Author")]
    public string? Author { get; set; }

    [Option('u',"uuid",Required = false,HelpText = "Epub universally unique identifier")]
    public string? Uuid { get; set; }

    [Option('s',"split",Required = false,HelpText = "Split Level")]
    public int SplitLevel { get; set; }
}

/// <summary>
/// 使用命令行创建Epub电子书
/// </summary>
class ParseCLI
{
    /// <summary>
    /// 解析命令行参数
    /// </summary>
    public static (EpubMetadata epubMetadata, BuildMetadata buildMetadata, string buildPath) ParseCommandLineArgs(string[] args)
    {
        string title = "";
        string author = "";
        string language = "";
        string uuid = "";

        string mdPath = "";
        string coverPath = "";
        string buildPath = "";
        int splitLevel = 1;

        Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(options => {
            mdPath = options.MdPath ?? "";
            coverPath = options.CoverPath ?? "";
            buildPath = options.BuildPath ??
                Path.Combine(Path.GetDirectoryName(mdPath)!, $"{Path.GetFileNameWithoutExtension(mdPath)}.epub");
            splitLevel = options.SplitLevel;

            title = options.Title ?? Path.GetFileNameWithoutExtension(mdPath);
            author = options.Author ?? "EpubBuilder";
            language = options.Language ?? "zh";
            uuid = options.Uuid ?? "";
        });

        var epubMetadata = new EpubMetadata{
            Title = title,
            Author = author,
            Language = language,
            Uuid = uuid
            };

        if (File.Exists(mdPath) is not true)
            throw new ArgumentException($"Not markdown file found in the 「{mdPath}」");

        var mdLines = File.ReadAllLines(mdPath).ToList();
        var buildMetadata = new BuildMetadata(mdLines, mdPath, coverPath, splitLevel);

        return (epubMetadata, buildMetadata ,buildPath);
    }
}