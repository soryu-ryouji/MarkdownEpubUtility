using EpubBuilder.Core;

/// <summary>
/// 生成Epub电子书所需要的数据
/// </summary>
public struct BuildedData
{
    private string mdPath;
    public string MdPath
    {
        get { return mdPath;}
        set { mdPath = value; }
    }

    private string coverPath;
    public string CoverPath
    {
        get { return coverPath;}
        set { coverPath = value; }
    }

    private string buildPath;
    public string BuildPath
    {
        get { return buildPath;}
        set { buildPath = value; }
    }
    
    private string language;
    public string Language
    {
        get { return language;}
        set { language = value; }
    }

    private string title;
    public string Title
    {
        get { return title;}
        set { title = value; }
    }
    
    private string author;
    public string Author
    {
        get { return author;}
        set { author = value; }
    }
    
    private string uuid;
    public string Uuid
    {
        get { return uuid;}
        set { uuid = value; }
    }

    private int splitLevel;
    public int SplitLevel
    {
        get { return splitLevel; }
        set { splitLevel = value; }
    }

    public BuildedData()
    {
        mdPath = "";
        buildPath = "";
        coverPath = "";
        language = "zh";
        title = "";
        author = "";
        uuid = "";
        splitLevel = 1;
    }

    /// <summary>
    /// 将markdown的父目录路径设置为生成路径
    /// </summary>
    public void SetBuildPathFromMdPath()
    {
        BuildPath = Path.GetDirectoryName(MdPath);
    }

    /// <summary>
    /// 检查 Build Data 的数据是否可用
    /// </summary>
    /// <param name="buildData"></param>
    /// <returns></returns>
    public static bool CheckData(BuildedData buildData)
    {
        bool isPass = true;

        if (!File.Exists(buildData.MdPath))
        {
            Log.AddLog($"CheckData -- No markdown file in the path : 「{buildData.MdPath}」", LogType.Error);
            isPass = false;
        }

        if (!File.Exists(buildData.CoverPath))
        {
            Log.AddLog($"CheckData -- No cover image in the path : 「{buildData.CoverPath}」", LogType.Error);
            isPass = false;
        }

        if (!Directory.Exists(buildData.BuildPath))
        {
            Log.AddLog($"CheckData -- No build directory in the path : 「{buildData.MdPath}」", LogType.Error);
            isPass = false;
        }

        return isPass;
    }
}