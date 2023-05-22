using CommandLine;

namespace EpubBuilder.Core;


/// <summary>
/// 命令行参数
/// </summary>
class Options
{
    // Required
    [Option('m', "markdown", Required = true, HelpText = "Markdown Path")]
    public string ParameterM { get; set; }

    // Option
    [Option('c', "cover", Required = false, HelpText = "Cover Path ")]
    public string ParameterC { get; set; }

    [Option('b', "build", Required = false, HelpText = "Build Path")]
    public string ParameterB { get; set; }

    [Option('d', "debug", Required = false, HelpText = "Enable debug mode.")]
    public bool DebugMode { get; set; }
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
        BuildedData makeData = new BuildedData();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(options =>
            {
                // Requested
                makeData.MdPath = options.ParameterM;;

                // Options
                makeData.CoverPath = options.ParameterC;
                makeData.BuildPath = options.ParameterB;

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
            if (makeData.BuildPath == null)
            {
                makeData.SetBuildPathFromMdPath();
            }

            Log.AddLog($"Parse CommandLine -- "+
                $"MdPath : {makeData.MdPath}   |   " +
                $"BuildPath : {makeData.BuildPath}   |   " +
                $"CoverPath : {makeData.CoverPath}   |   ");
        
        return makeData;
    }
}