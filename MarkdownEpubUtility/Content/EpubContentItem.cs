namespace MarkdownEpubUtility;

public class EpubContentItem
{
    public EpubContentItem(EpubContentType type, string fileName, byte[] content)
    {
        Type = type;
        FileName = fileName;
        Content = content;
    }

    public EpubContentItem(EpubContentType type, string fileName, string content)
    {
        Type = type;
        FileName = fileName;
        Content = content.ToBytes();
    }

    public readonly EpubContentType Type;
    public string FileName;
    public byte[] Content;

    public override string ToString()
    {
        return
        $"""
        Type: {Type}
        FileName: {FileName}
        Content: {Content}
        """;
    }
}