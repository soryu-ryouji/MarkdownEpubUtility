using System.Text;
using System.Text.RegularExpressions;
using CompressionLevel = Ionic.Zlib.CompressionLevel;
using ZipFile = Ionic.Zip.ZipFile;

namespace EpubBuilder;

public class Epub
{
    // epub 电子书元数据
    private readonly EpubMetadata _epubMetadata;

    // 构建 epub 电子书的元数据
    private readonly BuildMetadata _buildMetadata;

    public Epub(EpubMetadata epubMetadata, BuildMetadata buildMetadata)
    {
        _epubMetadata = epubMetadata;
        _buildMetadata = buildMetadata;
    }

    public EpubContentList ContentList = new();

    /// <summary>
    /// 依照当前 Epub 数据，生成 Epub 电子书
    /// </summary>
    public ZipFile Generate()
    {
        // 获取 markdown 的所有行
        var mdLines = new List<string>(File.ReadAllLines(_buildMetadata.MdPath));
        // 将 markdown文件中的相对路径转换为绝对路径
        ConvertMdAbsolutePath(mdLines, _buildMetadata.MdPath);

        var pageList = ParseMd.SplitPage(mdLines, _buildMetadata.PageSplitLevel);
        ContentList.ExtractPages(pageList, _buildMetadata.PageSplitLevel);

        var tocNcx = GenerateToc(pageList);
        var opf = GenerateOpf(ContentList);


        // 将 toc.ncx 内容添加进 epubContents
        ContentList.Contents.Add(new EpubContent(EpubContentType.Ncx, "toc.ncx", tocNcx));
        // 将 mimetype 内容添加进 epubContents
        ContentList.Contents.Add(new EpubContent(EpubContentType.Mimetype, "mimetype", "application/epub+zip"));
        // 将 container.xml 内容添加进 epubContents
        ContentList.Contents.Add(new EpubContent(EpubContentType.Container, "container.xml",
            "<?xml version=\"1.0\"?>\n" +
            "<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">" +
            "<rootfiles>\n" +
            "<rootfile full-path=\"OEBPS/content.opf\" media-type=\"application/oebps-package+xml\"/>\n" +
            "</rootfiles>\n" +
            "</container>"));

        // 将 opf 内容添加进 epubContents
        ContentList.Contents.Add(new EpubContent(EpubContentType.Opf, "content.opf", opf));
        ContentList.Contents.Add(
            new EpubContent(EpubContentType.Css, "stylesheet.css", CssCreator.GenerateStyleSheet()));

        // 若有封面，则将封面添加进 epubContents
        if (_buildMetadata.CoverPath != "")
        {
            ContentList.Contents.Add(new EpubContent(EpubContentType.Image, "cover.jpg", _buildMetadata.CoverPath));
        }

        // 创建电子书压缩包
        var zip = new ZipFile(Encoding.UTF8);
        // 设置压缩等级
        zip.CompressionLevel = CompressionLevel.Level0;
        // 设置压缩包的名称
        zip.Name = $"{_epubMetadata.Title}.epub";
        // 设置压缩包的保存路径
        zip.AddDirectoryByName("META-INF");
        zip.AddDirectoryByName("OEBPS");
        zip.AddDirectoryByName("OEBPS/Text");
        zip.AddDirectoryByName("OEBPS/Image");
        zip.AddDirectoryByName("OEBPS/Styles");

        foreach (var content in ContentList.Contents)
        {
            if (content.Type == EpubContentType.Html)
            {
                string html = "<?xml version='1.0' encoding='utf-8'?>\n" +
                              "<html xmlns=\"http://www.w3.org/1999/xhtml\">\n" +
                              "<head>\n" +
                              $"<title>{_epubMetadata.Title}</title>\n" +
                              "<link href=\"../Styles/stylesheet.css\" rel=\"stylesheet\" type=\"text/css\"/>" +
                              "</head>\n" +
                              "<body>\n" +
                              $"{content.Content}" +
                              "</body>\n" +
                              "</html>";
                zip.AddEntry($"OEBPS/Text/{content.FileName}", html);
            }
            else if (content.Type == EpubContentType.Css)
            {
                zip.AddEntry($"OEBPS/Styles/{content.FileName}", content.Content);
            }
            else if (content.Type == EpubContentType.Image)
            {
                zip.AddEntry($"OEBPS/Image/{content.FileName}.jpg", File.ReadAllBytes(content.Content));
            }
            else if (content.Type == EpubContentType.Container)
            {
                zip.AddEntry($"META-INF/{content.FileName}", content.Content);
            }
            else if (content.Type == EpubContentType.Mimetype)
            {
                zip.AddEntry($"{content.FileName}", content.Content);
            }
            else if (content.Type == EpubContentType.Opf)
            {
                zip.AddEntry($"OEBPS/{content.FileName}", content.Content);
            }
            else if (content.Type == EpubContentType.Ncx)
            {
                zip.AddEntry($"OEBPS/{content.FileName}", content.Content);
            }
        }

        return zip;
    }


    /// <summary>
    /// 根据 pageList 来生成 toc 文件内容
    /// </summary>
    /// <param name="pageList"></param>
    /// <returns></returns>
    public string GenerateToc(PageList pageList)
    {
        var toc = new Toc();
        toc.GenerateTocFromPageList(pageList, _buildMetadata.PageSplitLevel);

        // 生成 toc.ncx 内容
        string tocNcx = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                        "<ncx xmlns=\"http://www.daisy.org/z3986/2005/ncx/\" version=\"2005-1\">\n" +
                        $"\t<docTitle><text>{_epubMetadata.Title}</text></docTitle>\n" +
                        $"\t<docAuthor><text>{_epubMetadata.Author}</text></docAuthor>\n" +
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
    public string GenerateOpf(EpubContentList epubContentList)
    {
        string opf = "<?xml version=\"1.0\"  encoding=\"UTF-8\"?>\n" +
                     "<package xmlns=\"http://www.idpf.org/2007/opf\" unique-identifier=\"uuid_id\" version=\"2.0\">\n" +
                     "<metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\">\n" +
                     $"{_epubMetadata.GenerateOpfMetadata()}\n" +
                     "</metadata>\n" +
                     "<manifest>\n" +
                     $"{GenerateOpfManifest(epubContentList)}" +
                     "</manifest>\n" +
                     "<spine toc=\"ncx\">\n" +
                     $"{GenerateOpfSpine(epubContentList)}\n" +
                     "</spine>\n" +
                     "</package>";
        return opf;
    }

    /// <summary>
    /// 根据 PageList 和 SplitLevel 生成 manifest
    /// </summary>
    /// <param name="pageList"></param>
    /// <param name="splitLevel"></param>
    /// <returns></returns>
    public string GenerateOpfManifest(EpubContentList epubContentList)
    {
        var sb = new StringBuilder();
        // 为 pageList 生成 chapter.xhtml 的标识
        foreach (var unit in epubContentList.Contents)
        {
            sb.AppendLine(unit.GenerateManifestItem());
        }

        return sb.ToString();
    }

    public string GenerateOpfSpine(EpubContentList epubContentList)
    {
        var sb = new StringBuilder();
        // 为 pageList 生成 chapter.xhtml 的标识
        foreach (var unit in epubContentList.Contents)
        {
            var temp = unit.GenerateOpfSpineItem();
            if (temp != "")
            {
                sb.AppendLine(temp);
            }
        }

        return sb.ToString();
    }

    public void ConvertMdAbsolutePath(List<string> lines, string mdFilePath)
    {
        ConvertMdImagePath(lines, mdFilePath);
    }

    public void ConvertMdImagePath(List<string> mdLines, string mdFilePath)
    {
        string pattern = @"!\[(.*?)\]\((.*?)\)";
        var regex = new Regex(pattern);
        int numCount = 0;

        for (int i = 0; i < mdLines.Count; i++)
        {
            if (regex.IsMatch(mdLines[i]))
            {
                // 将资源添加到 EpubContentList 中
                Match match = regex.Match(mdLines[i]);

                string fileName = $"image_{numCount}";
                string absoluteImagePath = GetAbsolutePath(mdFilePath, match.Groups[2].Value);

                // 将图片添加到 EpubContentList 中
                ContentList.Contents.Add(new EpubContent(EpubContentType.Image, fileName, absoluteImagePath));

                numCount++;
                // 将图片语法替换为md语法
                mdLines[i] = $"<img alt=\"{fileName}\" src=\"../Image/{fileName}.jpg\"/>";
            }
        }
    }

    public string GetAbsolutePath(string filePath, string relativeImagePath)
    {
        // 获取文件所在目录的绝对路径
        string fileDirectory = Path.GetDirectoryName(filePath);

        // 解析相对路径得到绝对路径
        string absoluteImagePath = Path.GetFullPath(Path.Combine(fileDirectory, relativeImagePath));

        // 返回图片的绝对路径
        return absoluteImagePath;
    }
}