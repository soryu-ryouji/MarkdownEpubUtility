using Markdig;
using System.Text.RegularExpressions;

namespace EpubBuilder;

class EpubConvert
{

    public static string RemoveExtraBlankLines(string input)
    {
        // 使用正则表达式替换多余的空行
        string pattern = @"^\s*$\n|\r";
        string replacement = "";
        Regex regex = new Regex(pattern, RegexOptions.Multiline);
        string result = regex.Replace(input, replacement);

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

        // 若 pageElem.ChildrenPage 的子节点数量为 0 ，则直接忽略 splitLevel
        if (pageElem.Children.Count != 0)
        {
            // 当 splitLevel 大于 pageElem.Level 时，说明当前 pageElem 的子节点还可以继续细分为更小的 EpubContent
            if (splitLevel > pageElem.Level)
            {
                foreach (var subPage in pageElem.Children)
                {
                    chapterNum = ExtractHtmlSubPage(subPage, chapterNum, splitLevel, contents);
                }
            }
            else
            {
                // 当 splitLevel 等于 pageElem.Level时
                // 将当前 pageElem 下所有子元素的 Content 添加到 pageElem 中
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

    # region Markdown To Html

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
    /// 根据 split level 将 markdown 文档拆分成一个或多个特定页面
    /// 默认用户会遵守规范，在文章的最开头会以#作为开始符
    /// （默认 split level 为 1）
    /// </summary>
    public static HtmlPages ToHtmlPages(List<string> mdLines, int splitLevel)
    {
        // 首先会读取 markdownList 的第一行，若第一行不为标题，则直接报错退出
        // 使用 AddPageElem 添加到 PageElem 到 PageList 中时，AddPageElem 会自动将段落排序到合适的位置
        // 当到达 split level 的等级时，所有的大于 split level 的页面，都会被添加其父 Page 的 ChildrenPage 列表中

        var pageList = new HtmlPages();
        // 第一行必须是标题，如果不是，则直接报错退出
        if (GetHeadingLevel(mdLines[0]) == 0) throw new Exception("First line of Markdown must have a heading");

        var chapterIndex = 0;
        var subChapterIndex = 0;
        var curPage = new PageElem("",1,"");

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