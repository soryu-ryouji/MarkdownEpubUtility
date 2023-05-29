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

    /// <summary>
    /// 向指定路径导出指定文本
    /// </summary>
    /// <param name="targetPath"></param>
    /// <param name="content"></param>
    public static void OutputText(string targetPath, string content)
    {
        FileInfo fl = new FileInfo(targetPath);
        FileStream fs = fl.Create();
        fs.Close();
        fl.Delete();

        using (StreamWriter sw = new StreamWriter(targetPath)) {
            sw.Write(targetPath);
        }
    }
}