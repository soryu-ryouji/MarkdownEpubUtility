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
    public string SpineItem => Type is EpubContentType.Html ? $"""<itemref idref = "{FileName}"/>""" : string.Empty;

    public string ManifestItem => Type switch
    {
        EpubContentType.Html =>
            $"""<item href = "Text/{FileName}" id = "{FileName}" media-type="application/xhtml+xml"/>""",
        EpubContentType.Image => $"""<item href="Image/{FileName}" id="{FileName}" media-type="image/jpeg"/>""",
        EpubContentType.Ncx => $"""<item href="{FileName}" id="ncx" media-type="application/x-dtbncx+xml"/>""",
        EpubContentType.Css => $"""<item href="Styles/{FileName}" id="stylesheet"  media-type="text/css"/>""",
        _ => string.Empty
    };

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