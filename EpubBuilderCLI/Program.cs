using EpubBuilder;
using CommandLine;

namespace EpubBuilderCLI;

class Program
{
    public static void Main(string[] args)
    {
        bool isRun = true;
        var unit = ParseCLI.ParseCommandLineArgs(args, ref isRun);
        if (isRun is not true) return;

        var epubMetadata = unit.epubMetadata;
        var buildMetadata = unit.buildMetadata;
        var buildPath = unit.buildPath;

        var epub = new Epub(epubMetadata, buildMetadata);
        var zip = epub.Generate();
        zip.Save(buildPath);
    }
}

/// <summary>
/// 命令行参数
/// </summary>
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
    public static (EpubMetadata epubMetadata, BuildMetadata buildMetadata, string buildPath) ParseCommandLineArgs(string[] args, ref bool isRun)
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
            buildPath = options.BuildPath ?? Path.Combine(Path.GetDirectoryName(mdPath)!, $"{Path.GetFileNameWithoutExtension(mdPath)}.epub");
            splitLevel = options.SplitLevel;

            title = options.Title ?? Path.GetFileNameWithoutExtension(mdPath);
            author = options.Author ?? "EpubBuilder";
            language = options.Language ?? "zh";
            uuid = options.Uuid ?? "";
        });

        if (mdPath is "") isRun = false;


        var epubMetadata = new EpubMetadata{
            Title = title,
            Author = author,
            Language = language,
            Uuid = uuid
            };
        var buildMetadata = new BuildMetadata(mdPath, coverPath, splitLevel);
        
        return (epubMetadata, buildMetadata ,buildPath);
    }
}