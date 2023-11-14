using System.Text;
using System.Text.RegularExpressions;
using Ionic.Zlib;
using Ionic.Zip;

namespace EpubBuilder;

public class Epub
{
    private readonly EpubMetadata _epubMetadata;
    private readonly BuildMetadata _buildMetadata;

    public Epub(EpubMetadata epubMetadata, BuildMetadata buildMetadata)
    {
        _epubMetadata = epubMetadata;
        _buildMetadata = buildMetadata;
    }

    public ZipFile CreateEpub()
    {
        var contents = CreateEpubContents();
        var zipFile = ToZip(contents);

        return zipFile;
    }

    public EpubContents CreateEpubContents()
    {
        var contents = new EpubContents();

        ConvertMdAbsolutePath(_buildMetadata.MdLines, _buildMetadata.MdPath, contents);

        var pageList = ParseMd.ToHtmlPages(_buildMetadata.MdLines, _buildMetadata.PageSplitLevel);
        contents.ExtractPages(pageList, _buildMetadata.PageSplitLevel);

        if (_buildMetadata.CoverPath != "") contents.AddImage("cover", _buildMetadata.CoverPath);
        contents.Add(new (EpubContentType.Ncx, "toc.ncx", GenerateToc(pageList)));
        contents.Add(new (EpubContentType.Mimetype, "mimetype", "application/epub+zip"));
        contents.Add(new (EpubContentType.Container, "container.xml",
            """
            <?xml version="1.0"?>
            <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
                <rootfiles>
                    <rootfile full-path="OEBPS/content.opf" media-type="application/oebps-package+xml"/>
                </rootfiles>
            </container>
            """));
        contents.Add(new (EpubContentType.Css, "stylesheet.css", CssCreator.GenerateStyleSheet()));
        contents.Add(new (EpubContentType.Opf, "content.opf", GenerateOpf(contents, _buildMetadata.CoverPath != "")));

        return contents;
    }

    public ZipFile ToZip(EpubContents contents)
    {
        var zip = new ZipFile(Encoding.UTF8)
        {
            CompressionLevel = CompressionLevel.Level0,
            Name = $"{_epubMetadata.Title}.epub"
        };

        zip.AddDirectoryByName("META-INF");
        zip.AddDirectoryByName("OEBPS");
        zip.AddDirectoryByName("OEBPS/Text");
        zip.AddDirectoryByName("OEBPS/Image");
        zip.AddDirectoryByName("OEBPS/Styles");

        foreach (var content in contents)
        {
            switch (content.Type)
            {
                case EpubContentType.Html: HandleHtmlContent(zip, content); break;
                case EpubContentType.Css: HandleCssContent(zip, content); break;
                case EpubContentType.Jpg: HandleImageContent(zip, content); break;
                case EpubContentType.Png: HandleImageContent(zip, content); break;
                case EpubContentType.Container: HandleContainerContent(zip, content); break;
                case EpubContentType.Mimetype: HandleMimeTypeContent(zip, content); break;
                case EpubContentType.Opf: HandleOpfContent(zip, content); break;
                case EpubContentType.Ncx: HandleNcxContent(zip, content); break;
                default: throw new ArgumentOutOfRangeException($"{content.Type} cannot be added to epub zip");
            }
        }

        return zip;
    }

    #region ToZipHandle

    private void HandleHtmlContent(ZipFile zip, EpubContent content)
    {
        var html =
        $"""
        <?xml version='1.0' encoding='utf-8'?>
        <html xmlns='http://www.w3.org/1999/xhtml'>
        <head>
        <title>{_epubMetadata.Title}</title>
        <link href='../Styles/stylesheet.css' rel='stylesheet' type='text/css'/>
        </head>
        <body>
        {content.Content}
        </body>
        </html>
        """;

        zip.AddEntry($"OEBPS/Text/{content.FileName}", html);
    }

    private void HandleCssContent(ZipFile zip, EpubContent content)
    {
        zip.AddEntry($"OEBPS/Styles/{content.FileName}", content.Content);
    }

    private void HandleImageContent(ZipFile zip, EpubContent content)
    {
        zip.AddEntry($"OEBPS/Image/{content.FileName}", File.ReadAllBytes(content.Content));
    }

    private void HandleContainerContent(ZipFile zip, EpubContent content)
    {
        zip.AddEntry($"META-INF/{content.FileName}", content.Content);
    }

    private void HandleMimeTypeContent(ZipFile zip, EpubContent content)
    {
        zip.AddEntry($"{content.FileName}", content.Content);
    }

    private void HandleOpfContent(ZipFile zip, EpubContent content)
    {
        zip.AddEntry($"OEBPS/{content.FileName}", content.Content);
    }

    private void HandleNcxContent(ZipFile zip, EpubContent content)
    {
        zip.AddEntry($"OEBPS/{content.FileName}", content.Content);
    }

    #endregion

    public string GenerateToc(HtmlPages htmlPages)
    {
        var toc = new Toc();
        toc.GenerateTocFromPageList(htmlPages, _buildMetadata.PageSplitLevel);

        return
            $"""
            <?xml version = "1.0" encoding = "utf-8"?>
            <ncx xmlns = "http://www.daisy.org/z3986/2005/ncx/" version = "2005-1">
            <head>
            <meta content="3" name="dtb:depth" />
            </head>
            <docTitle><text>{_epubMetadata.Title}</text></docTitle>
            <docAuthor><text>{_epubMetadata.Author}</text></docAuthor>
            <navMap>
            {toc.RenderToc()}
            </navMap>
            </ncx>
            """;
    }

    public string GenerateOpf(EpubContents epubContents, bool coverExist = false)
    {
        var coverMetadata = coverExist ? """<meta name="cover" content="cover.jpg"/>""" : "";

        return
            $"""
            <?xml version = "1.0"  encoding = "UTF-8"?>
            <package xmlns = "http://www.idpf.org/2007/opf" unique-identifier = "uuid_id" version = "2.0">
            <metadata xmlns:dc = "https://purl.org/dc/elements/1.1/">
            {coverMetadata}
            {_epubMetadata.GenerateOpfMetadata()}
            </metadata>
            <manifest>
            {GenerateOpfManifest(epubContents)}
            </manifest>
            <spine toc = "ncx">
            {GenerateOpfSpine(epubContents)}
            </spine>
            </package>
            """;
    }

    public static string GenerateOpfManifest(EpubContents epubContents)
    {
        return string.Join(Environment.NewLine, epubContents.Select(content => content.GenerateManifestItem()));
    }

    public static string GenerateOpfSpine(EpubContents epubContents)
    {
        return string.Join(Environment.NewLine, epubContents.Select(content => content.GenerateOpfSpineItem()));
    }

    public static void ConvertMdAbsolutePath(List<string> lines, string mdFilePath, EpubContents epubContents)
    {
        ConvertMdImagePath(lines, mdFilePath, epubContents);
    }

    public static void ConvertMdImagePath(List<string> lines, string mdPath, EpubContents epubContents)
    {
        string pattern = @"!\[(.*?)\]\((.*?)\)";
        var regex = new Regex(pattern);
        int numCount = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            if (regex.IsMatch(lines[i]))
            {
                Match match = regex.Match(lines[i]);

                string absoluteImagePath = GetAbsolutePath(mdPath, match.Groups[2].Value);
                string fileExtension = Path.GetExtension(match.Groups[2].Value);
                string fileName = $"image_{numCount}";

                epubContents.AddImage(fileName, absoluteImagePath);
                lines[i] = $"""<img alt = "{fileName}" src = "../Image/{fileName}{fileExtension}"/>""";

                numCount++;
            }
        }
    }

    public static string GetAbsolutePath(string filePath, string relativePath)
    {
        string fileDirPath = Path.GetDirectoryName(filePath) ??
            throw new Exception($"{filePath} does not contain a file");
        string absolutePath = Path.GetFullPath(Path.Combine(fileDirPath, relativePath));

        return absolutePath;
    }
}