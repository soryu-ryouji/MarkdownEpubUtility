namespace EpubBuilder.Core;

public class Epub
{
    private EpubVersion _version = EpubVersion.V30;
    private Metadata _metadata = new Metadata();
    private Toc _toc = new Toc();
    private List<PageElement> _pageList = new List<PageElement>();

    /// <summary>
    /// 向 epub 的 metadata 中添加数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="content"></param>
    public void AddMetadata(MetadataType type, string content)
    {
        switch (type)
        {
            case MetadataType.Title : _metadata.Title = content; break;
            case MetadataType.Author : _metadata.Author = content; break;
            case MetadataType.Lang : _metadata.Lang = content; break;
            case MetadataType.Generator : _metadata.Generator = content; break;
            case MetadataType.TocName : _metadata.TocName = content; break;
            case MetadataType.License : _metadata.License = content; break;
            case MetadataType.PublishedDate : _metadata.PublishedDate = content; break;
            case MetadataType.ModifiedDate : _metadata.ModifiedDate = content; break;
            case MetadataType.Uuid : _metadata.Uuid = content; break;
        }
    }

    /// <summary>
    /// 向 epub 的 metadata 中添加数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="content"></param>
    public void AddMetadata(MetadataType type, List<string> content)
    {
        switch (type)
        {
            case MetadataType.Description : _metadata.Description = content; break;
            case MetadataType.Subject : _metadata.Subject = content; break;
        }
    }

    /// <summary>
    /// 依照当前Epub数据，生成Epub电子书
    /// </summary>
    public void Generate(BuildedData buildedData)
    {
        // TODO: ncx.toc
        // TODO: content.opf
        // TODO: OEBPS/Text/chapter.xhtml
        
        // 读入markdown文本行
        List<string> mdLines = Common.ReadLines(buildedData.MdPath);
        // 将markdown文本行切分为PageList
        var pageList = ParseMd.SplitPage(mdLines);
        // 使用 pageList 生成 Toc
        Toc toc = new Toc();
        toc.GenerateTocFromPageList(pageList,1);

        throw new NotImplementedException();
    }
}