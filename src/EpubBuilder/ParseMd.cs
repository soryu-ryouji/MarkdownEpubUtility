using Markdig;

namespace EpubBuilder;

class ParseMd
{
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
}