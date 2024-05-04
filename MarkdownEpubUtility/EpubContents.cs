using System.Collections;
using System.Data;

namespace MarkdownEpubUtility;

public class EpubContents : IEnumerable<EpubContent>
{
    private List<EpubContent> _contents = [];

    public void AddImage(string fileName, string imagePath)
    {
        var fileExtension = Path.GetExtension(imagePath);

        EpubContentType contentType = fileExtension switch
        {
            ".jpg" => EpubContentType.Image,
            ".png" => EpubContentType.Image,
            _ => throw new DataException("Only jpg and png images can be used")
        };

        _contents.Add(new EpubContent(contentType, $"{fileName}{fileExtension}", imagePath));
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, _contents.Select(content => content.SpineItem));
    }

    public bool SearchContent(string fileName)
    {
        foreach (var temp in _contents)
        {
            if (temp.FileName == fileName) return true;
        }

        return false;
    }

    public void Add(EpubContent content) => _contents.Add(content);
    public void AddRange(IEnumerable<EpubContent> contents) => _contents.AddRange(contents);

    public IEnumerator<EpubContent> GetEnumerator() => _contents.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}