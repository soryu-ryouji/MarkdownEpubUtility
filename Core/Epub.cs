using System.Text;
using CompressionLevel = Ionic.Zlib.CompressionLevel;
using ZipFile = Ionic.Zip.ZipFile;
namespace EpubBuilder.Core;

public class Epub
{
    private EpubVersion _version = EpubVersion.V30;
    private Metadata _metadata = new Metadata();

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
    public void Generate(BuildedData buildedData, int splitLevel)
    {
        // 读入markdown文本行
        List<string> mdLines = Common.ReadLines(buildedData.MdPath);
        // 将markdown文本行切分为PageList
        PageList pageList = ParseMd.SplitPage(mdLines, splitLevel);
        // 使用 pageList 生成 Toc 文档内容
        string tocNcx = GenerateToc(pageList);
        // 使用 pageList 生成 opf 文档内容
        string opf = GenerateOpf(pageList, splitLevel);

        List<EpubContent> epubContents = new List<EpubContent>();

        // 将 pageList 内容添加进 epubContents
        epubContents.AddRange(GenerateEpubContentListFromPageList(pageList, splitLevel));
        // 将 toc.ncx 内容添加进 epubContents
        epubContents.Add(new EpubContent(EpubContentType.Ncx, "toc.ncx", tocNcx));
        // 将 mimetype 内容添加进 epubContents
        epubContents.Add(new EpubContent(EpubContentType.Mimetype,"mimetype","application/epub+zip"));
        // 将 container.xml 内容添加进 epubContents
        epubContents.Add(new EpubContent(EpubContentType.Container, "container.xml", "<?xml version=\"1.0\"?>\n" +
            "<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">" +
            "<rootfiles>\n" +
            "<rootfile full-path=\"OEBPS/content.opf\" media-type=\"application/oebps-package+xml\"/>\n" +
            "</rootfiles>\n" +
            "</container>"));
        // 将 opf 内容添加进 epubContents
        epubContents.Add(new EpubContent(EpubContentType.Opf,"content.opf",opf));
        
        // 创建存储文件的基础目录
        string epubDir = Path.Combine(buildedData.BuildPath, _metadata.Title);

        // 导出 Epub 文件内容
        OutputEpubContent(epubDir,epubContents);
        
        // 将文件目录压缩成电子书形式
        using (var epubFile = new ZipFile(Path.Combine(buildedData.BuildPath,$"{_metadata.Title}.epub"))) {
            epubFile.EmitTimesInWindowsFormatWhenSaving = false;
            epubFile.CompressionLevel = CompressionLevel.None;
            epubFile.AddDirectory(epubDir);
            epubFile.Save();
        }
    }

    /// <summary>
    /// 导出 EpubContent 中的所有内容
    /// </summary>
    /// <param name="epubDir"></param>
    public void OutputEpubContent(string epubDir,List<EpubContent> epubContents)
    {
        Directory.CreateDirectory(epubDir);
        // 创建 META-INF, OEBPS, OEBPS/Text/, OEBPS/Images/, OEBPS/Styles文件夹
        Directory.CreateDirectory(Path.Combine(epubDir, "META-INF"));
        Directory.CreateDirectory(Path.Combine(epubDir, "OEBPS", "Text"));
        Directory.CreateDirectory(Path.Combine(epubDir, "OEBPS", "Images"));
        Directory.CreateDirectory(Path.Combine(epubDir, "OEBPS", "Styles"));

         // 导出 epubContents 文件
         foreach (var unit in epubContents)
         {
             // 导出所有 html 文件
             if (unit.Type == EpubContentType.Html)
             {
                 string html = "<?xml version='1.0' encoding='utf-8'?>\n" +
                               "<html xmlns=\"http://www.w3.org/1999/xhtml\">\n" +
                               "<head>\n" +
                               $"<title>{_metadata.Title}</title>\n" +
                               "</head>\n" +
                               "<body>\n" +
                               $"{unit.Content}" +
                               "</body>\n" +
                               "</html>";
                 
                 Common.OutputText(Path.Combine(epubDir,"OEBPS","Text",unit.FileName),html);
             }
 
             // 导出 toc.ncx文件
             if (unit.Type == EpubContentType.Ncx)
             {
                 Common.OutputText(Path.Combine(epubDir,"OEBPS","toc.ncx"),unit.Content);
             }
 
             // 导出 mimetype 文件
             if (unit.Type == EpubContentType.Mimetype)
             {
                 Common.OutputText(Path.Combine(epubDir,unit.FileName),unit.Content);
             }
 
             // 导出 META-INF/container.xml文件
             if (unit.Type == EpubContentType.Container)
             {
                 Common.OutputText(Path.Combine(epubDir,"META-INF",unit.FileName),unit.Content);
             }
             
             // 导出 content.opf 文件
             if (unit.Type == EpubContentType.Opf)
             {
                 Common.OutputText(Path.Combine(epubDir,"OEBPS",unit.FileName),unit.Content);
             }
         }
    }

    /// <summary>
    /// 根据 pageList 来生成 toc 文件内容
    /// </summary>
    /// <param name="pageList"></param>
    /// <returns></returns>
    public string GenerateToc(PageList pageList)
    {
        Toc toc = new Toc();
        toc.GenerateTocFromPageList(pageList,1);
        
        // 生成 toc.ncx 内容
        string tocNcx = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<ncx xmlns=\"http://www.daisy.org/z3986/2005/ncx/\" version=\"2005-1\">\n" +
                         $"\t<docTitle><text>{_metadata.Title}</text></docTitle>\n" +
                         $"\t<docAuthor><text>{_metadata.Author}</text></docAuthor>\n" +
                         "<navMap>\n" +
                         $"{toc.RenderToc()}\n" +
                         "</navMap>\n" +
                         "</ncx>";

        return tocNcx;
    }

    /// <summary>
    /// 根据 pageList 生成 opf 文件内容
    /// </summary>
    /// <param name="pageList"></param>
    /// <returns></returns>
    public string GenerateOpf(PageList pageList,int splitLevel)
    {
        string opf = "<?xml version=\"1.0\"  encoding=\"UTF-8\"?>\n" +
                     "<package xmlns=\"http://www.idpf.org/2007/opf\" unique-identifier=\"uuid_id\" version=\"2.0\">\n" +
                     "<metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\">\n" +
                     $"{GenerateOpfMetadata()}\n" +
                     "</metadata>\n" +
                     "<manifest>\n" +
                     $"{GenerateOpfManifest(pageList, splitLevel)}\n" +
                     "<item href=\"toc.ncx\" id=\"ncx\" media-type=\"application/x-dtbncx+xml\"/>\n" +
                     "</manifest>\n" +
                     "<spine toc = \"ncx\">\n" +
                     $"{GenerateOpfSpine(pageList, splitLevel)}\n" +
                     "</spine>\n" +
                     "</package>";
        return opf;
    }

    public string GenerateOpfMetadata()
    {
        List<string> metadataList = new List<string>();
        // 标题元数据
        if (_metadata.Title != "") metadataList.Add($"<dc:title>{_metadata.Title}</dc:title>");
        else metadataList.Add("<dc:title>EpubBuilder</dc:title>");
        
        // UUID元数据
        if (_metadata.Uuid != "")
            metadataList.Add($"<dc:identifier id=\"uuid_id\" opf:scheme=\"uuid\">{_metadata.Uuid}</dc:identifier>");
        else
            metadataList.Add("<dc:identifier id=\"uuid_id\" opf:scheme=\"uuid\">af96c033-08f7-4de9-96a7-eb05eff019c0</dc:identifier>");
        
        // 语言元数据
        if (_metadata.Lang != "")
            metadataList.Add($"<dc:language>{_metadata.Lang}</dc:language>");
        else metadataList.Add("<dc:language>zh</dc:language>");
        
        // 主题或关键字
        if (_metadata.Subject.Count != 0)
            metadataList.Add($"<dc:subject>{String.Join(",",_metadata.Subject)}</dc:subject>");

        return String.Join("\n",metadataList);
    }

    /// <summary>
    /// 根据 PageList 和 SplitLevel 生成 manifest
    /// </summary>
    /// <param name="pageList"></param>
    /// <param name="splitLevel"></param>
    /// <returns></returns>
    public string GenerateOpfManifest(PageList pageList,int splitLevel)
    {
        // 为 pageList 生成 chapter.xhtml 的标识
        int chapterNum = 0;
        List<string> manifestList = new List<string>();
        
        foreach (var unit in pageList.PageElemList)
        {
            chapterNum = GenerateOpfManifestPageItem(manifestList, unit, chapterNum, splitLevel);
        }

        return String.Join("\n",manifestList);
    }

    public int GenerateOpfManifestPageItem(List<string> manifestList, PageElement pageElem,int chapterNum ,int splitLevel)
    {
        // 为当前的 pageElem 添加 item
        manifestList.Add($"<item href=\"Texts/chapter{chapterNum}.xhtml\" id=\"chap{chapterNum}\" media-type=\"application/xhtml+xml\"/>");
        chapterNum++;
        
        // 若 pageElem.ChildrenPage 的子节点数量为 0 ，则直接忽略 splitLevel
        if (pageElem.ChildrenPage.Count != 0)
        {
            // 递归进子节点
            if (splitLevel > pageElem.Level)
            {
                foreach (var unit in pageElem.ChildrenPage)
                {
                    chapterNum = GenerateOpfManifestPageItem(manifestList, unit, chapterNum, splitLevel);
                }
            }
        }

        return chapterNum;
    }

    public string GenerateOpfSpine(PageList pageList, int splitLevel)
    {
        // 为 pageList 生成 chapter.xhtml 的标识
        int chapterNum = 0;
        List<string> spineList = new List<string>();
        
        foreach (var unit in pageList.PageElemList)
        {
            chapterNum = GenerateOpfSpineItem(spineList, unit, chapterNum, splitLevel);
        }

        return String.Join("\n",spineList);
    }

    public int GenerateOpfSpineItem(List<string> spineList, PageElement pageElem, int chapterNum, int splitLevel)
    {
        
        // 为当前的 pageElem 添加 item
        spineList.Add($"<itemref idref=\"chap{chapterNum}\"/>");
        chapterNum++;
        
        // 若 pageElem.ChildrenPage 的子节点数量为 0 ，则直接忽略 splitLevel
        if (pageElem.ChildrenPage.Count != 0)
        {
            // 递归进子节点
            if (splitLevel > pageElem.Level)
            {
                foreach (var unit in pageElem.ChildrenPage)
                {
                    chapterNum = GenerateOpfSpineItem(spineList, unit, chapterNum, splitLevel);
                }
            }
        }

        return chapterNum;
    }
    
    

    public List<EpubContent> GenerateEpubContentListFromPageList(PageList pageList, int splitLevel)
    {
        int chapterNum = 0;
        List<EpubContent> epubContents = new List<EpubContent>();
        
        foreach (var unit in pageList.PageElemList)
        {
            chapterNum = AddEpubContentFromPageElem(epubContents, unit, chapterNum, splitLevel);
        }

        return epubContents;
    }

    public int AddEpubContentFromPageElem(List<EpubContent> epubContents, PageElement pageElem, int chapterNum, int splitLevel)
    {
        // 为当前的 pageElem 添加 item
        epubContents.Add(new EpubContent(EpubContentType.Html,$"chapter_{chapterNum}.xhtml", 
            ParseMd.Md2Html(String.Join("\n",pageElem.Content))));
        chapterNum++;
        
        // 若 pageElem.ChildrenPage 的子节点数量为 0 ，则直接忽略 splitLevel
        if (pageElem.ChildrenPage.Count != 0)
        {
            // 当 splitLevel 大于 pageElem.Level 时，说明当前 pageElem 的子节点还可以继续细分为更小的 EpubContent
            if (splitLevel > pageElem.Level)
            {
                foreach (var unit in pageElem.ChildrenPage)
                {
                    chapterNum = AddEpubContentFromPageElem(epubContents, unit, chapterNum, splitLevel);
                }
            }
            else
            {
                // 当 splitLevel 等于 pageElem.Level时
                // 将当前 pageElem 下所有子元素的 Content 添加到 pageElem 中
                epubContents.Last().Content = PageElement.RenderSelfAndAllSubPageContent(pageElem);
            }
        }

        return chapterNum;
    }
}