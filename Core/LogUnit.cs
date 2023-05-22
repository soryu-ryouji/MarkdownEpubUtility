namespace EpubBuilder.Core;

public struct LogUnit
{
    public LogType Type;
    public string Content;

    public void Print()
    {
        Console.WriteLine($"{Type} : {Content}");
    }

    public LogUnit(LogType type, string content)
    {
        Type = type;
        Content = content;
    }
}