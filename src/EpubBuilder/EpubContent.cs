namespace EpubBuilder;

public class EpubContent(EpubContentType type, string fileName, string content)
{
    public readonly EpubContentType Type = type;
    public string FileName = fileName;
    public string Content = content;

    public string GenerateManifestItem()
    {
        string item = Type switch
        {
            EpubContentType.Html =>
                $"""<item href = "Text/{FileName}" id = "{FileName}" media-type="application/xhtml+xml"/>""",
            EpubContentType.Jpg => $"""<item href="Image/{FileName}" id="{FileName}" media-type="image/jpeg"/>""",
            EpubContentType.Png => $"""<item href="Image/{FileName}" id="{FileName}" media-type="image/png"/>""",
            EpubContentType.Ncx => $"""<item href="{FileName}" id="ncx" media-type="application/x-dtbncx+xml"/>""",
            EpubContentType.Css => $"""<item href="Styles/{FileName}" id="stylesheet"  media-type="text/css"/>""",
            _ => string.Empty
        };

        return item;
    }

    public string GenerateOpfSpineItem()
    {
        return (Type is EpubContentType.Html) ? $"""<itemref idref = "{FileName}"/>""" : string.Empty;
    }

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