using System.IO.Compression;

namespace MarkdownEpubUtility;

public class EpubBook
{
    public EpubMetadata Metadata;
    public EpubContent Content = [];

    public EpubBook(EpubMetadata epubData)
    {
        Metadata = epubData;
        Content.Init();
    }

    private void GenerateContent(BuildMetadata buildMetadata)
    {
        EpubConvert.ConvertMdAbsolutePath(buildMetadata.MdLines, buildMetadata.MdPath, Content);

        var pages = EpubConvert.MdToEpubPage(buildMetadata.MdLines, buildMetadata.SplitLevel);
        Content.AddRange(EpubConvert.PageToContent(pages, buildMetadata.SplitLevel));

        if (buildMetadata.CoverPath != "") Content.AddImage("cover", buildMetadata.CoverPath);

        // Toc needs to be generated before the opf file is generated
        // otherwise it won't be added to the list
        Content.Add(new(EpubContentType.Ncx, "toc.ncx", pages.ToToc(buildMetadata.SplitLevel).ToFileString()));
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

    public void CreateEpub(string filePath, BuildMetadata buildData)
    {
        GenerateContent(buildData);
        using (var epubStream = PackContent())
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                epubStream.CopyTo(fileStream);
            }
        }
    }

    public static EpubBook OpenBook(string filePath)
    {
        var epubContent = new EpubContent();

        using (var archive = ZipFile.OpenRead(filePath))
        {
            foreach (var entry in archive.Entries)
            {
                using (var entryStream = entry.Open())
                using (var memoryStream = new MemoryStream())
                {
                    entryStream.CopyTo(memoryStream);
                    var contentBytes = memoryStream.ToArray();

                    EpubContentType contentType;
                    if (entry.FullName == "mimetype")
                    {
                        contentType = EpubContentType.Mimetype;
                    }
                    else if (entry.FullName == "META-INF/container.xml")
                    {
                        contentType = EpubContentType.Container;
                    }
                    else if (entry.FullName.StartsWith("OEBPS/Image/"))
                    {
                        contentType = EpubContentType.Image;
                    }
                    else if (entry.FullName.StartsWith("OEBPS/Styles/"))
                    {
                        contentType = EpubContentType.Css;
                    }
                    else if (entry.FullName.StartsWith("OEBPS/Text/"))
                    {
                        contentType = EpubContentType.Html;
                    }
                    else if (entry.FullName.EndsWith(".opf"))
                    {
                        contentType = EpubContentType.Opf;
                    }
                    else if (entry.FullName.EndsWith(".ncx"))
                    {
                        contentType = EpubContentType.Ncx;
                    }
                    else
                    {
                        continue;
                    }

                    var fileName = Path.GetFileName(entry.FullName);
                    epubContent.Add(new EpubContentItem(contentType, fileName, contentBytes));
                }
            }
        }

        var book = new EpubBook(new EpubMetadata())
        {
            Content = epubContent
        };

        return book;
    }
}