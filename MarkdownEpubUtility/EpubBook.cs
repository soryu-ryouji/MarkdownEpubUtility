using System.IO.Compression;

namespace MarkdownEpubUtility;

public class EpubBook
{
    public EpubMetadata Metadata;
    public BuildMetadata BuildData;
    public EpubContent Content = [];

    public EpubBook(EpubMetadata epubData, BuildMetadata buildData)
    {
        Metadata = epubData;
        BuildData = buildData;

        Content.Init();
    }

    private void GenerateContent()
    {
        EpubConvert.ConvertMdAbsolutePath(BuildData.MdLines, BuildData.MdPath, Content);

        var pages = EpubConvert.MdToEpubPage(BuildData.MdLines, BuildData.SplitLevel);
        Content.AddRange(EpubConvert.PageToContent(pages, BuildData.SplitLevel));

        if (BuildData.CoverPath != "") Content.AddImage("cover", BuildData.CoverPath);

        // Toc needs to be generated before the opf file is generated
        // otherwise it won't be added to the list
        Content.Add(new(EpubContentType.Ncx, "toc.ncx", pages.ToToc(BuildData.SplitLevel).ToFileString()));
        Content.Add(new(EpubContentType.Opf, "content.opf", Content.ToOpf(Metadata)));
    }

    private MemoryStream PackContent()
    {
        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            // Add other EPUB content
            foreach (var item in Content)
            {
                string entryName = item.Type switch
                {
                    EpubContentType.Mimetype => $"{item.FileName}",
                    EpubContentType.Container => $"META-INF/{item.FileName}",
                    EpubContentType.Image => $"OEBPS/Image/{item.FileName}",
                    EpubContentType.Css => $"OEBPS/Styles/{item.FileName}",
                    EpubContentType.Html => $"OEBPS/Text/{item.FileName}",
                    EpubContentType.Opf => $"OEBPS/{item.FileName}",
                    EpubContentType.Ncx => $"OEBPS/{item.FileName}",
                    _ => throw new ArgumentOutOfRangeException($"{item.Type} cannot be added to epub zip")
                };

                var entry = archive.CreateEntry(entryName);
                using (var entryStream = entry.Open())
                {
                    entryStream.Write(item.Content, 0, item.Content.Length);
                }
            }
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public void CreateEpub(string filePath)
    {
        GenerateContent();
        using (var epubStream = PackContent())
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                epubStream.CopyTo(fileStream);
            }
        }
    }
}