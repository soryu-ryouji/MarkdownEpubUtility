using System.Collections;
using System.Data;

namespace MarkdownEpubUtility;

public class EpubContent : IEnumerable<EpubContentItem>
{
    private List<EpubContentItem> _content = [];

    public void Init()
    {
        _content.Add(new EpubContentItem(EpubContentType.Mimetype, "mimetype", "application/epub+zip"));
        _content.Add(new EpubContentItem(EpubContentType.Container, "container.xml",
            """
            <?xml version="1.0"?>
            <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
                <rootfiles>
                    <rootfile full-path="OEBPS/content.opf" media-type="application/oebps-package+xml"/>
                </rootfiles>
            </container>
            """));
        _content.Add(new(EpubContentType.Css, "stylesheet.css", CssCreator.GenerateStyleSheet()));
    }

    public void AddImage(string fileName, string imagePath)
    {
        var fileExtension = Path.GetExtension(imagePath);

        EpubContentType contentType = fileExtension switch
        {
            ".jpg" => EpubContentType.Image,
            ".png" => EpubContentType.Image,
            _ => throw new DataException("Only jpg and png images can be used")
        };

        _content.Add(new EpubContentItem(contentType, $"{fileName}{fileExtension}", File.ReadAllBytes(imagePath)));
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, _content.Select(content => content.SpineItem));
    }

    public bool Search(string fileName)
    {
        foreach (var temp in _content)
        {
            if (temp.FileName == fileName) return true;
        }

        return false;
    }

    public void Add(EpubContentItem content) => _content.Add(content);
    public void AddRange(IEnumerable<EpubContentItem> contents) => _content.AddRange(contents);

    public IEnumerator<EpubContentItem> GetEnumerator() => _content.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}