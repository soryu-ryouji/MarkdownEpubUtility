namespace EpubBuilder.Core;

public class Common
{
    /// <summary>
    /// 从指定路径中读入所有文本行
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static List<string> ReadLines(string filePath)
    {
        List<string> lines = new List<string>(File.ReadLines(filePath));

        return lines;
    }
}