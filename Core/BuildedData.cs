using EpubBuilder.Core;

/// <summary>
/// 生成Epub电子书所需要的数据
/// </summary>
public struct BuildedData
{
    public string MdPath { get; set; }

    public string CoverPath { get; set; }

    public string BuildPath { get; set; }

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