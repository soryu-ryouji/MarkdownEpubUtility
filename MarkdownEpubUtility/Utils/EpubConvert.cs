using Markdig;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownEpubUtility;

public static class EpubConvert
{
    public static byte[] ToBytes(this string self)
    {
        return Encoding.UTF8.GetBytes(self);
    }

    # region EpubMetadata
    public static string ToOpf(this EpubMetadata epubMetadata)
    {
        var metadataList = new List<string>
        {
            epubMetadata.Title != ""
                ? $"<dc:title>{epubMetadata.Title}</dc:title>"
                : "<dc:title>EpubBuilder</dc:title>",
            epubMetadata.Author != ""
                ? $"<dc:creator>{epubMetadata.Author}</dc:creator>"
                : "<dc:creator>Anonymous</dc:creator>",
            epubMetadata.Language != ""
                ? $"<dc:language>{epubMetadata.Language}</dc:language>"
                : "<dc:language>zh</dc:language>",
            epubMetadata.Generator != ""
                ? $"<dc:publisher>{epubMetadata.Generator}</dc:publisher>"
                : "<dc:publisher>EpubBuilder</dc:publisher>"
        };

        if (epubMetadata.Description != "")
            metadataList.Add($"<dc:description>{epubMetadata.Description}</dc:description>");
        if (epubMetadata.License != "")
            metadataList.Add($"<dc:rights>{epubMetadata.License}</dc:rights>");
        if (epubMetadata.PublishedDate != "")
            metadataList.Add($"<dc:date>{epubMetadata.PublishedDate}</dc:date>");
        if (epubMetadata.ModifiedDate != "")
            metadataList.Add($"<dc:date>{epubMetadata.ModifiedDate}</dc:date>");
        if (epubMetadata.Subject != "")
            metadataList.Add($"<dc:subject>{epubMetadata.Subject}</dc:subject>");
        if (epubMetadata.Uuid != "")
            metadataList.Add($"""<dc:identifier id="uuid_id" opf:scheme="uuid">{epubMetadata.Uuid}</dc:identifier>""");

        return string.Join(Environment.NewLine, metadataList);
    }

    #endregion

    #region EpubToc

    public static string ToTree(this EpubToc self)
    {
        var sb = new StringBuilder();

        void PrintTree(EpubTocItem elem, string indent, bool isLast)
        {
            sb.Append(indent + (isLast ? "└─ " : "├─ ") + elem.Title + Environment.NewLine);
            var newIndent = indent + (isLast ? "    " : "|   ");

            for (var i = 0; i < elem.Children.Count; i++)
            {
                PrintTree(elem.Children[i], newIndent, i == elem.Children.Count - 1);
            }
        }

        for (var n = 0; n < self.Count; n++)
        {
            var elem = self.ElementAt(n);
            var indent = (n == self.Count - 1) ? "    " : "\u2502   ";

            sb.Append(elem.Title + Environment.NewLine);

            var childCount = elem.Children.Count;
            for (var i = 0; i < childCount; i++)
            {
                PrintTree(elem.Children[i], indent, i == childCount - 1);
            }
        }

        return sb.ToString();
    }

    public static string ToFileString(this EpubToc self)
    {
        string result = RemoveExtraBlankLines(
            $"""
            <?xml version = "1.0" encoding = "utf-8"?>
            <ncx xmlns = "http://www.daisy.org/z3986/2005/ncx/" version = "2005-1">
            <head>
            <meta content="3" name="dtb:depth" />
            </head>
            <docTitle><text></text></docTitle>
            <docAuthor><text></text></docAuthor>
            <navMap>
            {self.Render()}
            </navMap>
            </ncx>
            """);

        return result;
    }

    #endregion

    # region EpubPage
    /// <summary>
    /// Generating a Toc with EpubPage
    /// </summary>
    public static EpubToc ToToc(this EpubPage self, int splitLevel)
    {
        var toc = new EpubToc();
        foreach (var pageElem in self.ElemList)
        {
            toc.Add(pageElem, splitLevel);
        }

        return toc;
    }

    public static string ToTree(this EpubPage self)
    {
        var sb = new StringBuilder();

        void PrintTree(EpubPageItem elem, string indent, bool isLast)
        {
            string curLine;
            if (isLast)
            {
                curLine = indent + "└─ " + elem.Heading + Environment.NewLine;
                indent += "    ";
            }
            else
            {
                curLine = indent + "├─ " + elem.Heading + Environment.NewLine;
                indent += "|   ";
            }

            sb.Append(curLine);
            var childCount = elem.Children.Count;

            for (var i = 0; i < childCount; i++)
            {
                PrintTree(elem.Children[i], indent, i == childCount - 1);
            }
        }

        for (var i = 0; i < self.Count; i++)
        {
            var elem = self.ElementAt(i);
            var indent = (i == self.Count - 1) ? "    " : "\u2502   ";

            sb.Append(elem.Heading + Environment.NewLine);

            var childCount = elem.Children.Count;
            for (var n = 0; n < childCount; n++)
            {
                PrintTree(elem.Children[n], indent, (n == childCount - 1));
            }
        }

        return sb.ToString();
    }
    # endregion

    public static string RemoveExtraBlankLines(string input)
    {
        var pattern = @"^\s*$\n|\r";
        var replacement = "";
        var regex = new Regex(pattern, RegexOptions.Multiline);
        var result = regex.Replace(input, replacement);

        return result;
    }

    # region Convert EpubPage To List<EpubContent>
    public static List<EpubContentItem> PageToContent(EpubPage htmlPages, int splitLevel)
    {
        var contents = new List<EpubContentItem>();

        int chapterNum = 0;
        foreach (var subPage in htmlPages.ElemList)
        {
            chapterNum = ExtractHtmlSubPage(subPage, chapterNum, splitLevel, contents);
        }

        return contents;
    }

    private static int ExtractHtmlSubPage(EpubPageItem pageElem, int chapterNum, int splitLevel,
        List<EpubContentItem> contents)
    {
        var epubContent = new EpubContentItem(EpubContentType.Html, $"chapter_{chapterNum}.xhtml", Md2Html(pageElem.Content));
        contents.Add(epubContent);
        chapterNum++;

        // If pageElem.ChildrenPage has 0 children, splitLevel is ignored.
        if (pageElem.Children.Count != 0)
        {
            // When splitLevel is greater than pageElem.Level
            // it means that the children of the current pageElem can be further subdivided into smaller EpubContents.
            if (splitLevel > pageElem.Level)
            {
                foreach (var subPage in pageElem.Children)
                {
                    chapterNum = ExtractHtmlSubPage(subPage, chapterNum, splitLevel, contents);
                }
            }
            else
            {
                // When splitLevel is equal to pageElem.Level,
                // Add the Content of all children of the current pageElem to the pageElem.
                contents.Last().Content = EpubPageItem.Render(pageElem).ToBytes();
            }
        }

        return chapterNum;
    }
    # endregion

    # region EpubContent

    public static string ToOpf(this EpubContent self, EpubMetadata epubMetadata)
    {
        var coverMetadata = "";
        if (self.Search("cover.jpg")) coverMetadata = """<meta name="cover" content="cover.jpg"/>""";

        string opf = RemoveExtraBlankLines(
                $"""
                <?xml version = "1.0"  encoding = "UTF-8"?>
                <package xmlns = "http://www.idpf.org/2007/opf" unique-identifier = "uuid_id" version = "2.0">
                <metadata xmlns:dc = "https://purl.org/dc/elements/1.1/">
                {coverMetadata}
                {epubMetadata.ToOpf()}
                </metadata>
                <manifest>
                {self.ToOpfManifest()}
                </manifest>
                <spine toc = "ncx">
                {self.ToOpfSpine()}
                </spine>
                </package>
                """);

        return opf;
    }

    public static string ToOpfManifest(this EpubContent self)
    {
        return string.Join(Environment.NewLine, self.Select(ToOpfManifestItem));
    }

    public static string ToOpfSpine(this EpubContent epubContents)
    {
        return string.Join(Environment.NewLine, epubContents.Select(ToOpfSpineItem));
    }
    # endregion

    # region EpubContentItem
    public static string ToOpfManifestItem(this EpubContentItem self)
    {
        return self.Type switch
        {
            EpubContentType.Html =>
                $"""<item href = "Text/{self.FileName}" id = "{self.FileName}" media-type="application/xhtml+xml"/>""",
            EpubContentType.Image =>
                $"""<item href="Image/{self.FileName}" id="{self.FileName}" media-type="image/jpeg"/>""",
            EpubContentType.Ncx => $"""<item href="{self.FileName}" id="ncx" media-type="application/x-dtbncx+xml"/>""",
            EpubContentType.Css => $"""<item href="Styles/{self.FileName}" id="stylesheet"  media-type="text/css"/>""",
            _ => string.Empty
        };

    }

    public static string ToOpfSpineItem(this EpubContentItem self)
    {
        return self.Type switch
        {
            EpubContentType.Html => $"""<itemref idref = "{self.FileName}"/>""",
            _ => string.Empty
        };
    }
    # endregion

    #region Markdown To Html

    public static string Md2Html(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder().UseEmphasisExtras().UsePipeTables().Build();

        return Markdown.ToHtml(markdown, pipeline);
    }

    public static string Md2Html(List<string> markdown)
    {
        var md = string.Join("\n", markdown);
        var pipeline = new MarkdownPipelineBuilder().UseEmphasisExtras().UsePipeTables().Build();

        return Markdown.ToHtml(md, pipeline);
    }

    /// <summary>
    /// Splits a markdown document into one or more specific pages based on split level.
    /// By default, users will follow the standard and start their posts with a # at the very beginning.
    /// (default split level is 1)
    /// </summary>
    public static EpubPage MdToEpubPage(List<string> mdLines, int splitLevel)
    {
        // First read the first line of the markdownList
        // if the first line is not a title, it will report an error and exit.

        // When adding a PageElem to a PageList using AddPageElem
        // AddPageElem automatically sorts the paragraphs into the appropriate position.

        // When the split level is reached
        // All pages larger than the split level will be added to the parent's ChildrenPage list.

        var pageList = new EpubPage();
        if (GetHeadingLevel(mdLines[0]) == 0) throw new Exception("First line of Markdown must have a heading");

        var chapterIndex = 0;
        var subChapterIndex = 0;
        var curPage = new EpubPageItem("", 1, "");

        foreach (var line in mdLines)
        {
            if (line.Trim() == "") continue;

            var level = GetHeadingLevel(line);
            if (level != 0)
            {
                var page = new EpubPageItem("", level: level, heading: GetHeadingText(line));
                curPage = page;

                pageList.Add(page, splitLevel);
            }

            if (level > splitLevel)
            {
                curPage.Url = $"Text/chapter_{chapterIndex - 1}.xhtml#subChapter_{subChapterIndex}";
                curPage.Content.Add(
                    $"""
                     <h{level} id = "subChapter_{subChapterIndex}">{GetHeadingText(line)}</h{level}>

                     """);
                subChapterIndex++;
            }
            else if (level != 0)
            {
                curPage.Url = $"Text/chapter_{chapterIndex}.xhtml";
                curPage.Content.Add(
                    $"""
                     <h{level}>{GetHeadingText(line)}</h{level}>

                     """);
                chapterIndex++;
            }
            else
            {
                curPage.Content.Add(
                    $"""

                     {line}
                     """);
            }
        }

        return pageList;
    }

    private static int GetHeadingLevel(string line)
    {
        var level = 0;
        foreach (var word in line)
        {
            if (word == '#') level += 1;
            else break;
        }

        if (level > 6) level = 0;

        return level;
    }

    private static string GetHeadingText(string line)
    {
        var level = GetHeadingLevel(line);
        return line[level..].Trim();
    }

    # endregion


    # region Markdown Image Path To Absolute Path
    public static void ConvertMdAbsolutePath(List<string> lines, string mdFilePath, EpubContent epubContents)
    {
        ConvertMdImagePath(lines, mdFilePath, epubContents);
    }

    public static void ConvertMdImagePath(List<string> lines, string mdPath, EpubContent epubContents)
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
                lines[i] = $"""<img alt = "{fileName}" src = "../Image/{fileName}{fileExtension}" />""";

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

    # endregion
}