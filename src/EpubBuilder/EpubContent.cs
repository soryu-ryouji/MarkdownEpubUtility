using System.Text;

namespace EpubBuilder;

public enum EpubContentType
{
    Image,
    Html,
    Ncx,
    Mimetype,
    Container,
    Opf,
    Css,
}

public class EpubContent(EpubContentType type, string fileName, string content)
{
    public readonly EpubContentType Type = type;
    public string FileName = fileName;
    // When Type is Image, Content is a string of the image path
    // TODO: Load image Texture into content
    public string Content = content;

    // Just Html Content can be created as SpineItem
    public string SpineItem
    {
        get
        {
            if (Type is EpubContentType.Html)
            {
                return $"""<itemref idref = "{FileName}"/>""";
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public string ManifestItem
    {
        get
        {
            string item = Type switch
            {
                EpubContentType.Html => $"""<item href = "Text/{FileName}" id = "{FileName}" media-type="application/xhtml+xml"/>""",
                EpubContentType.Image => $"""<item href="Image/{FileName}" id="{FileName}" media-type="image/jpeg"/>""",
                EpubContentType.Ncx => $"""<item href="{FileName}" id="ncx" media-type="application/x-dtbncx+xml"/>""",
                EpubContentType.Css => $"""<item href="Styles/{FileName}" id="stylesheet"  media-type="text/css"/>""",
                _ => string.Empty
            };

            return item;
        }
    }

    public byte[] GetData()
    {
        switch (Type)
        {
            case EpubContentType.Image:
            {
                // When Type is Image, Content is a string of the image path
                return File.ReadAllBytes(Content);
            }
            case EpubContentType.Html:
            {
                return Encoding.UTF8.GetBytes(
                $"""
                <?xml version='1.0' encoding='utf-8'?>
                <html xmlns='http://www.w3.org/1999/xhtml'>
                <head><title></title>
                <link href='../Styles/stylesheet.css' rel='stylesheet' type='text/css'/>
                </head>
                <body>
                {Content}
                </body>
                </html>
                """);
            }
            case EpubContentType.Mimetype:
            case EpubContentType.Container:
            case EpubContentType.Css:
            case EpubContentType.Ncx:
            case EpubContentType.Opf:
            {
                return Encoding.UTF8.GetBytes(Content);
            }

            default: throw new InvalidOperationException($"Cannot get data from {Type}");
        }

        throw new InvalidOperationException($"Unsupported content type: {Type}");
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