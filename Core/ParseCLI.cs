using CommandLine;

namespace EpubBuilder.Core;


/// <summary>
/// 命令行参数
/// </summary>
class Options
{
    // Required
    [Option('m', "markdown", Required = true, HelpText = "Markdown Path")]
    public string ParameterMarkdownPath { get; set; }

    // Option
    [Option('c', "cover", Required = false, HelpText = "Cover Path ")]
    public string ParameterCoverPath { get; set; }

    [Option('b', "build", Required = false, HelpText = "Build Path")]
    public string ParameterBuildPath { get; set; }

    [Option('d', "debug", Required = false, HelpText = "Enable debug mode")]
    public bool DebugMode { get; set; }
    
    [Option('l',"language",Required = false,HelpText = "Epub Language")]
    public string ParameterLanguage { get; set; }
    
    [Option('t',"title",Required = false,HelpText = "Epub Title")]
    public string ParameterTitle { get; set; }
    
    [Option('a',"author",Required = false,HelpText = "Epub Author")]
    public string ParameterAuthor { get; set; }
    
    [Option('u',"uuid",Required = false,HelpText = "Epub universally unique identifier")]
    public string ParameterUuid { get; set; }
    
    [Option('s',"split",Required = false,HelpText = "Split Level")]
    public int ParameterSplitLevel { get; set; }
}

/// <summary>
/// 使用命令行创建Epub电子书
/// </summary>
class ParseCLI
{
    /// <summary>
    /// 解析命令行参数
    /// </summary>
    public static BuildedData ParseCommandLineArgs(string[] args)
    {
        BuildedData buildedData = new BuildedData();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(options =>
            {
                // Requested
                buildedData.MdPath = options.ParameterMarkdownPath;;

                // Options
                buildedData.CoverPath = options.ParameterCoverPath;
                buildedData.BuildPath = options.ParameterBuildPath;
                buildedData.Language = options.ParameterLanguage;
                buildedData.Title = options.ParameterTitle;
                buildedData.Author = options.ParameterAuthor;
                buildedData.Uuid = options.ParameterUuid;
                buildedData.SplitLevel = options.ParameterSplitLevel;

                // 判断是否要打开Debug模式
                if (options.DebugMode)
                {
                    Log.DebugMode = true;
                }
            })
            .WithNotParsed(errors =>
            {
                Log.AddLog($"Incorrect parameter used : {String.Join(" ", args)}", LogType.Error);
            });

            // 若 build path 为空，则将markdown所在的父目录作为其生成目录
            if (buildedData.BuildPath == null)
            {
                buildedData.SetBuildPathFromMdPath();
            }

            if (buildedData.Title == null)
            {
                buildedData.Title = Path.GetFileNameWithoutExtension(buildedData.MdPath);
            }

            Log.AddLog($"Parse CommandLine -- "+
                $"MdPath : {buildedData.MdPath}   |   " +
                $"BuildPath : {buildedData.BuildPath}   |   " +
                $"CoverPath : {buildedData.CoverPath}   |   " +
                $"Title : {buildedData.Title}");
        
        return buildedData;
    }
}