using Markdig;

namespace EpubBuilder.Core;


class ParseMd
{
    /// <summary>
    /// 将markdown文本转换为html
    /// </summary>
    /// <param name="markdown"></param>
    /// <returns></returns>
    public static string Md2Html(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder().UseEmphasisExtras().UsePipeTables().Build();

        return Markdown.ToHtml(markdown, pipeline);
    }

    /// <summary>
    /// 根据 split level 将 markdown 文档拆分成一个或多个特定页面
    /// 默认用户会遵守规范，在文章的最开头会以#作为开始符
    /// （默认 split level 为 1）
    /// </summary>
    /// <param name="markdownList"></param>
    /// <param name="splitLevel"></param>
    /// <returns></returns>
    public static PageList SplitPage(List<string> markdownList,int splitLevel=1)
    {
        // 首先会读取 markdownList 的第一行，从第一行中拿到第一个页面的名称和等级，创建 Page，添加到 PageList 中
        // 使用 AddPageElem 添加到 PageElem 到 PageList 中时，AddPageElem 会自动将段落排序到合适的位置
        // 当到达 split level 的等级时，所有的大于 split level 的页面，都会被添加其父 Page 的 ChildrenPage 列表中
        
        PageList pageList = new PageList();
        
        // 第一行必须是标题，如果不是，则直接报错退出
        if (GetHeadingLevel(markdownList[0]) == 0)
        {
            Log.AddLog("The markdown first line is not a Heading",LogType.Error);
            Environment.Exit(10);
        }
        
        PageElement newPage = new PageElement(GetHeadingLevel(markdownList.First()),GetHeadingText(markdownList.First()));
        PageElement curPage = newPage;
        pageList.AddPageElem(newPage,splitLevel);
        // 因为提前获取了markdown的第一行，因此将第一行移除，避免之后重复创建
        markdownList.RemoveAt(0);

        foreach (var line in markdownList)
        {
            // 当line为空时，直接跳过
            if (line.Trim() == "") continue;
            
            // 当line的level不等于0时，创建新的 Page
            int level = GetHeadingLevel(line);
            if (level != 0)
            {
                PageElement page = new PageElement(level, GetHeadingText(line));
                curPage = page;
                pageList.AddPageElem(page,splitLevel);
                // 获取标题之后，跳过当前行
                continue;
            }
            
            curPage.Content.Add(line);
        }

        return pageList;
    }

    /// <summary>
    /// 根据句子开头的「#」，判断当前句子的标题级别
    /// 若开头不为「#」，或者「#」数量超过六个，则返回 0，表示该句子不为标题
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static int GetHeadingLevel(string line)
    {
        int level = 0;
        foreach (var word in line)
        {
            if (word == '#') level += 1;
            else break;
        }

        // markdown 的 「#」 标签最多支持到 h6
        // 因此如果 「#」 数量超过6，则将标题等级其归零
        if (level > 6) level = 0;
        
        return level;
    }

    private static string GetHeadingText(string line)
    {
        int level = GetHeadingLevel(line);
        return line[level..].Trim();
    }
}