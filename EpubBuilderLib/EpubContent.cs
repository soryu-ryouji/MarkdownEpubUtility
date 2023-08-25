using System.Data;

namespace EpubBuilder;

/// <summary>
/// Epub 电子书内容的类型
/// </summary>
public enum EpubContentType
{
    Jpg,
    Png,
    Html,
    Ncx,
    Mimetype,
    Container,
    Opf,
    Css,
}

/// <summary>
/// Epub 电子书的内容
/// </summary>
public class EpubContent
{
    public readonly EpubContentType Type;
    public string FileName;
    public string Content;

    public EpubContent(EpubContentType type, string fileName, string content)
    {
        Type = type;
        FileName = fileName;
        Content = content;
    }

    public string GenerateManifestItem()
    {
        string item = Type switch
        {
            EpubContentType.Html =>  $"""<item href = "Text/{FileName}" id = "{FileName}" media-type="application/xhtml+xml"/>""",
            EpubContentType.Jpg => $"""<item href="Image/{FileName}" id="{FileName}" media-type="image/jpeg"/>""",
            EpubContentType.Ncx => $"""<item href="{FileName}" id="ncx" media-type="application/x-dtbncx+xml"/>""",
            EpubContentType.Css => $"""<item href="Styles/{FileName}" id="stylesheet"  media-type="text/css"/>""",
            _ => ""
        };

        return item;
    }

    public string GenerateOpfSpineItem()
    {
        string item = "";
        if (Type == EpubContentType.Html) item = $"<itemref idref=\"{FileName}\"/>";
        else if (Type == EpubContentType.Ncx) item = "<spine toc = \"ncx\">";

        return item;
    }

    public void PrintEpubContent()
    {
        Console.WriteLine($"Type: {Type}");
        Console.WriteLine($"FileName: {FileName}");
        Console.WriteLine($"Content: {Content}");
    }
}

public class EpubContentList
{
    public List<EpubContent> Contents = new();

    /// <summary>
    /// 根据 PageList 和 SplitLevel 提取出 EpubContent
    /// </summary>
    /// <param name="htmlPages"></param>
    /// <param name="splitLevel"></param>
    /// <returns></returns>
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
        
        Contents.Add(new EpubContent(fileExtension switch
        {
            ".jpg" => EpubContentType.Jpg,
            ".png" => EpubContentType.Png,
            _ => throw new DataException("Only jpg and png images can be used")
        }, fileName + $"{fileExtension}" , imagePath));
    }

    private int ExtractSubPage(PageElem pageElem, int chapterNum, int splitLevel)
    {
        // 将当前页面添加进 epubContentList
        var epubContent = new EpubContent(EpubContentType.Html, $"chapter_{chapterNum}.xhtml",
            ParseMd.Md2Html(pageElem.Content));
        Contents.Add(epubContent);
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
                Contents.Last().Content = PageElem.RenderPage(pageElem);
            }
        }

        return chapterNum;
    }

    public void PrintEpubContentList()
    {
        foreach (var content in Contents)
        {
            Console.WriteLine(content.GenerateManifestItem());
        }
    }
}