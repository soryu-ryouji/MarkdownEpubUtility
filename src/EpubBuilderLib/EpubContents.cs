using System.Collections;
using System.Data;

namespace EpubBuilder;

public class EpubContents: IEnumerable<EpubContent>
{
    private List<EpubContent> _contents;

    public EpubContents()
    {
        _contents = new List<EpubContent>();
    }

    public void ExtractPages(HtmlPages htmlPages, int splitLevel)
    {
        int chapterNum = 0;
        foreach (var subPage in htmlPages.ElemList)
        {
            chapterNum = ExtractSubPage(subPage, chapterNum, splitLevel);
        }
    }

    public void AddImage(string fileName, string imagePath)
    {
        var fileExtension = Path.GetExtension(imagePath);

        EpubContentType contentType = fileExtension switch
        {
            ".jpg" => EpubContentType.Jpg,
            ".png" => EpubContentType.Png,
            _ => throw new DataException("Only jpg and png images can be used")
        };

        _contents.Add(new EpubContent(contentType, $"{fileName}{fileExtension}", imagePath));
    }

    private int ExtractSubPage(PageElem pageElem, int chapterNum, int splitLevel)
    {
        var epubContent = new EpubContent(EpubContentType.Html, $"chapter_{chapterNum}.xhtml",
            ParseMd.Md2Html(pageElem.Content));
        _contents.Add(epubContent);
        chapterNum++;

        // 若 pageElem.ChildrenPage 的子节点数量为 0 ，则直接忽略 splitLevel
        if (pageElem.Children.Count != 0)
        {
            // 当 splitLevel 大于 pageElem.Level 时，说明当前 pageElem 的子节点还可以继续细分为更小的 EpubContent
            if (splitLevel > pageElem.Level)
            {
                foreach (var subPage in pageElem.Children)
                {
                    chapterNum = ExtractSubPage(subPage, chapterNum, splitLevel);
                }
            }
            else
            {
                // 当 splitLevel 等于 pageElem.Level时
                // 将当前 pageElem 下所有子元素的 Content 添加到 pageElem 中
                _contents.Last().Content = PageElem.RenderPage(pageElem);
            }
        }

        return chapterNum;
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, _contents.Select(content => content.GenerateManifestItem()));
    }

    public void Add(EpubContent content) => _contents.Add(content);

    public IEnumerator<EpubContent> GetEnumerator() => _contents.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}