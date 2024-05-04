using System.Text;
using Ionic.Zlib;
using Ionic.Zip;

namespace MarkdownEpubUtility;

public class Epub
{
    public EpubMetadata EpubData;
    public BuildMetadata BuildData;
    public EpubContent Content = [];

    public Epub(EpubMetadata epubData, BuildMetadata buildData)
    {
        this.EpubData = epubData;
        this.BuildData = buildData;

        Content.Init();
    }

    private void GenerateContent()
    {
        EpubConvert.ConvertMdAbsolutePath(BuildData.MdLines, BuildData.MdPath, Content);

        var pages = EpubConvert.ToHtmlPages(BuildData.MdLines, BuildData.SplitLevel);
        Content.AddRange(EpubConvert.HtmlPagesToEpubContentList(pages, BuildData.SplitLevel));

        if (BuildData.CoverPath != "") Content.AddImage("cover", BuildData.CoverPath);

        // Toc needs to be generated before the opf file is generated
        // otherwise it won't be added to the list
        Content.Add(new(EpubContentType.Ncx, "toc.ncx", EpubConvert.GenerateToc(pages, BuildData.SplitLevel)));
        Content.Add(new(EpubContentType.Opf, "content.opf", EpubConvert.GenerateOpf(Content, EpubData)));
    }

    private ZipFile PackContent()
    {
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

        foreach (var item in Content)
        {
            switch (item.Type)
            {
                case EpubContentType.Mimetype: zip.AddEntry($"{item.FileName}", item.GetData()); break;
                case EpubContentType.Container: zip.AddEntry($"META-INF/{item.FileName}", item.GetData()); break;
                case EpubContentType.Image: zip.AddEntry($"OEBPS/Image/{item.FileName}", item.GetData()); break;
                case EpubContentType.Css: zip.AddEntry($"OEBPS/Styles/{item.FileName}", item.GetData()); break;
                case EpubContentType.Html: zip.AddEntry($"OEBPS/Text/{item.FileName}", item.GetData()); break;
                case EpubContentType.Opf: zip.AddEntry($"OEBPS/{item.FileName}", item.GetData()); break;
                case EpubContentType.Ncx: zip.AddEntry($"OEBPS/{item.FileName}", item.GetData()); break;
                default: throw new ArgumentOutOfRangeException($"{item.Type} cannot be added to epub zip");
            }
        }

        return zip;
    }

    public ZipFile CreateEpub()
    {
        GenerateContent();
        var epubFile = PackContent();
        return epubFile;
    }
}