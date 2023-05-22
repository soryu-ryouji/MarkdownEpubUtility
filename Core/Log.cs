namespace EpubBuilder.Core;

public class Log
{
    private static List<LogUnit> logList = new List<LogUnit>();
    public static bool DebugMode = false;


    /// <summary>
    /// 添加日志
    ///</summary>
    public static void AddLog(string content, LogType type = LogType.Info)
    {
        // 若开启了DebugMode，则在运行程序时打印出Log
        if (DebugMode) Console.WriteLine($"{type} : {content}");

        logList.Add(new LogUnit(type, content));

        if (type == LogType.Error)
        {
            Console.Error.WriteLine($"{type} : {content}");
        }

        // 将log写入log.txt文件
        WriteLogToFile(content,type);
    }

    /// <summary>
    /// 打印特定标签的日志
    /// </summary>
    public static void PrintLogWith(LogType logType)
    {
        foreach (var log in logList)
        {
            if (log.Type == logType)
            {
                log.Print();
            }
        }
    }

    /// <summary>
    /// 打印出所有的日志
    /// </summary>
    public static void PrintAllLog()
    {
        foreach (var log in logList)
        {
            log.Print();
        }
    }

    /// <summary>
    /// 将log写入到log.txt文件中
    /// </summary>
    /// <param name="content"></param>
    /// <param name="type"></param>
    private static void WriteLogToFile(string content, LogType type)
    {
        string logEntry = $"{DateTime.Now} - {type} - {content}";
        string logFilePath = Environment.CurrentDirectory + "/log.txt";

        using (StreamWriter writer = File.AppendText(logFilePath))
        {
            writer.WriteLine(logEntry);
        }
    }
}
