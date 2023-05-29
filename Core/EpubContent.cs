namespace EpubBuilder.Core;

public class EpubContent
{
    public EpubContentType Type;
    public string FileName;
    public string Content;

    public EpubContent(EpubContentType type, string fileName, string content)
    {
        Type = type;
        FileName = fileName;
        Content = content;
    }
}