using Markdig;
using System.Text.RegularExpressions;

namespace MarkdownEpubUtility;

class EpubConvert
{

    public static string RemoveExtraBlankLines(string input)
    {
        var pattern = @"^\s*$\n|\r";
        var replacement = "";
        var regex = new Regex(pattern, RegexOptions.Multiline);
        var result = regex.Replace(input, replacement);

        return result;
    }

    # region Convert Html To List<EpubContent>
    public static List<EpubContent> HtmlPagesToEpubContentList(HtmlPages htmlPages, int splitLevel)
    {
        var contents = new List<EpubContent>();

        int chapterNum = 0;
        foreach (var subPage in htmlPages.ElemList)
        {
            chapterNum = ExtractHtmlSubPage(subPage, chapterNum, splitLevel, contents);
        }

        return contents;
    }

    private static int ExtractHtmlSubPage(PageElem pageElem, int chapterNum, int splitLevel, List<EpubContent> contents)
    {
        var epubContent = new EpubContent(EpubContentType.Html, $"chapter_{chapterNum}.xhtml", Md2Html(pageElem.Content));
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
                contents.Last().Content = PageElem.Render(pageElem);
            }
        }

        return chapterNum;
    }
    # endregion

    # region Pages To Toc
    public static string GenerateToc(HtmlPages htmlPages, int splitLevel)
    {
        var toc = new Toc();
        toc.GenerateTocFromPageList(htmlPages, splitLevel);

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
            {toc.Render()}
            </navMap>
            </ncx>
            """);

        return result;
    }

    # endregion

    #region Opf

    public static string GenerateOpf(EpubContents epubContents, EpubMetadata epubMetadata)
    {
        var coverMetadata = "";
        if (epubContents.SearchContent("cover.jpg")) coverMetadata = """<meta name="cover" content="cover.jpg"/>""";

        string opf = RemoveExtraBlankLines(
                $"""
                <?xml version = "1.0"  encoding = "UTF-8"?>
                <package xmlns = "http://www.idpf.org/2007/opf" unique-identifier = "uuid_id" version = "2.0">
                <metadata xmlns:dc = "https://purl.org/dc/elements/1.1/">
                {coverMetadata}
                {epubMetadata.GenerateOpfMetadata()}
                </metadata>
                <manifest>
                {GenerateOpfManifest(epubContents)}
                </manifest>
                <spine toc = "ncx">
                {GenerateOpfSpine(epubContents)}
                </spine>
                </package>
                """);

        return opf;
    }

    public static string GenerateOpfManifest(EpubContents epubContents)
    {
        return string.Join(Environment.NewLine, epubContents.Select(content => content.ManifestItem));
    }

    public static string GenerateOpfSpine(EpubContents epubContents)
    {
        return string.Join(Environment.NewLine, epubContents.Select(content => content.SpineItem));
    }

    #endregion

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
    public static HtmlPages ToHtmlPages(List<string> mdLines, int splitLevel)
    {
        // First read the first line of the markdownList
        // if the first line is not a title, it will report an error and exit.

        // When adding a PageElem to a PageList using AddPageElem
        // AddPageElem automatically sorts the paragraphs into the appropriate position.

        // When the split level is reached
        // All pages larger than the split level will be added to the parent's ChildrenPage list.

        var pageList = new HtmlPages();
        if (GetHeadingLevel(mdLines[0]) == 0) throw new Exception("First line of Markdown must have a heading");

        var chapterIndex = 0;
        var subChapterIndex = 0;
        var curPage = new PageElem("", 1, "");

        foreach (var line in mdLines)
        {
            if (line.Trim() == "") continue;

            var level = GetHeadingLevel(line);
            if (level != 0)
            {
                var page = new PageElem("", level: level, heading: GetHeadingText(line));
                curPage = page;

                pageList.AddElem(page, splitLevel);
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