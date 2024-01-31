using System.Text;
using Ionic.Zlib;
using Ionic.Zip;

namespace EpubBuilder;

public class Epub
{
    public EpubMetadata epubMetadata;
    public BuildMetadata buildMetadata;
    public EpubContents contents = [];

    public Epub(EpubMetadata epubMetadata, BuildMetadata buildMetadata)
    {
        this.epubMetadata = epubMetadata;
        this.buildMetadata = buildMetadata;

        // Init Contents
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
    }

    private void GenerateContent()
    {
        EpubConvert.ConvertMdAbsolutePath(buildMetadata.MdLines, buildMetadata.MdPath, contents);

        var pages = EpubConvert.ToHtmlPages(buildMetadata.MdLines, buildMetadata.SplitLevel);
        contents.AddRange(EpubConvert.HtmlPagesToEpubContentList(pages, buildMetadata.SplitLevel));

        if (buildMetadata.CoverPath != "") contents.AddImage("cover", buildMetadata.CoverPath);
        // Tips: Toc文件需要在opf文件生成前生成，否则不会被添加进列表中
        contents.Add(new (EpubContentType.Ncx, "toc.ncx", EpubConvert.GenerateToc(pages, buildMetadata.SplitLevel)));
        contents.Add(new (EpubContentType.Opf, "content.opf", EpubConvert.GenerateOpf(contents, epubMetadata)));
    }

    public ZipFile CreateEpub()
    {
        GenerateContent();

        var zip = new ZipFile(Encoding.UTF8)
        {
            CompressionLevel = CompressionLevel.Level0,
            Name = $"{epubMetadata.Title}.epub"
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