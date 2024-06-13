﻿using System.Text;
using Ionic.Zlib;
using Ionic.Zip;

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

    private ZipFile PackContent()
    {
        var zip = new ZipFile(Encoding.UTF8)
        {
            CompressionLevel = CompressionLevel.Level0,
            Name = $"{Metadata.Title}.epub"
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
                case EpubContentType.Mimetype: zip.AddEntry($"{item.FileName}", item.Content); break;
                case EpubContentType.Container: zip.AddEntry($"META-INF/{item.FileName}", item.Content); break;
                case EpubContentType.Image: zip.AddEntry($"OEBPS/Image/{item.FileName}", item.Content); break;
                case EpubContentType.Css: zip.AddEntry($"OEBPS/Styles/{item.FileName}", item.Content); break;
                case EpubContentType.Html: zip.AddEntry($"OEBPS/Text/{item.FileName}", item.Content); break;
                case EpubContentType.Opf: zip.AddEntry($"OEBPS/{item.FileName}", item.Content); break;
                case EpubContentType.Ncx: zip.AddEntry($"OEBPS/{item.FileName}", item.Content); break;
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