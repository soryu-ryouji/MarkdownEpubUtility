using System.Text;
using Ionic.Zlib;
using Ionic.Zip;

namespace MarkdownEpubUtility;

public class Epub
{
    public EpubMetadata EpubData;
    public BuildMetadata BuildData;
    public EpubContents Contents = [];

    public Epub(EpubMetadata epubData, BuildMetadata buildData)
    {
        this.EpubData = epubData;
        this.BuildData = buildData;

        // Init Contents
        Contents.Add(new EpubContent(EpubContentType.Mimetype, "mimetype", "application/epub+zip"));
        Contents.Add(new EpubContent(EpubContentType.Container, "container.xml",
            """
            <?xml version="1.0"?>
            <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
                <rootfiles>
                    <rootfile full-path="OEBPS/content.opf" media-type="application/oebps-package+xml"/>
                </rootfiles>
            </container>
            """));
        Contents.Add(new(EpubContentType.Css, "stylesheet.css", CssCreator.GenerateStyleSheet()));
    }

    private void GenerateContent()
    {
        EpubConvert.ConvertMdAbsolutePath(BuildData.MdLines, BuildData.MdPath, Contents);

        var pages = EpubConvert.ToHtmlPages(BuildData.MdLines, BuildData.SplitLevel);
        Contents.AddRange(EpubConvert.HtmlPagesToEpubContentList(pages, BuildData.SplitLevel));

        if (BuildData.CoverPath != "") Contents.AddImage("cover", BuildData.CoverPath);

        // Toc needs to be generated before the opf file is generated
        // otherwise it won't be added to the list
        Contents.Add(new(EpubContentType.Ncx, "toc.ncx", EpubConvert.GenerateToc(pages, BuildData.SplitLevel)));
        Contents.Add(new(EpubContentType.Opf, "content.opf", EpubConvert.GenerateOpf(Contents, EpubData)));
    }

    public ZipFile CreateEpub()
    {
        GenerateContent();

        var zip = new ZipFile(Encoding.UTF8)
        {
            CompressionLevel = CompressionLevel.Level0,
            Name = $"{EpubData.Title}.epub"
        };

        zip.AddDirectoryByName("META-INF");
        zip.AddDirectoryByName("OEBPS");
        zip.AddDirectoryByName("OEBPS/Text");
        zip.AddDirectoryByName("OEBPS/Image");
        zip.AddDirectoryByName("OEBPS/Styles");

        foreach (var content in Contents)
        {
            switch (content.Type)
            {
                case EpubContentType.Mimetype: zip.AddEntry($"{content.FileName}", content.GetData()); break;
                case EpubContentType.Container: zip.AddEntry($"META-INF/{content.FileName}", content.GetData()); break;
                case EpubContentType.Image: zip.AddEntry($"OEBPS/Image/{content.FileName}", content.GetData()); break;
                case EpubContentType.Css: zip.AddEntry($"OEBPS/Styles/{content.FileName}", content.GetData()); break;
                case EpubContentType.Html: zip.AddEntry($"OEBPS/Text/{content.FileName}", content.GetData()); break;
                case EpubContentType.Opf: zip.AddEntry($"OEBPS/{content.FileName}", content.GetData()); break;
                case EpubContentType.Ncx: zip.AddEntry($"OEBPS/{content.FileName}", content.GetData()); break;
                default: throw new ArgumentOutOfRangeException($"{content.Type} cannot be added to epub zip");
            }
        }

        return zip;
    }
}