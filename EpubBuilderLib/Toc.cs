using System.Text;

namespace EpubBuilder;

/// <summary>
/// Table of Contents
/// Epub 电子书 目录
/// </summary>
public class Toc
{
    public readonly List<TocElem> ElemList = new();

    /// <summary>
    /// 将元素添加到所有子元素的最后
    /// 如果该元素比最后的元素的Level小，则将其与最后的元素的子元素的Level进行比较。
    /// 如果等级相同，或者是大于末尾的元素，则将其放在该元素的后面
    /// </summary>
    public void AddElem(TocElem tocElem)
    {
        // 如果当前 _elements 列表为零时，将元素直接添加到 _elements 列表中
        if (ElemList.Count == 0)
        {
            tocElem.Level = 1;
            ElemList.Add(tocElem);
        }
        // 如果当前tocElement的Level为1，则将其添加到tocElemList中
        else if (tocElem.Level == 1) ElemList.Add(tocElem);
        // 如果当前 _elements 元素不为空，则将其添加到当前 _elements 列表最末尾的元素中
        else ElemList.Last().AddElem(tocElem);
    }

    /// <summary>
    /// 渲染 Toc 元素当前的目录
    /// </summary>
    public string RenderToc()
    {
        var renderText = new List<string>();
        var offset = 0;
        foreach (var tocTuple in ElemList.Select(elem => elem.RenderToc(offset)))
        {
            offset = tocTuple.offset;
            renderText.Add(tocTuple.renderText);
        }

        return string.Join("", renderText);
    }

    /// <summary>
    /// 使用 PageList 生成 Toc
    /// </summary>
    public void GenerateTocFromPageList(PageList pageList, int splitLevel)
    {
        // 遍历 pageList.PageElemList 中的元素生成Toc
        // PageElemList 的子元素 AddTocElemFromPageElem 会自动递归添加到Toc中
        var offset = 0;
        foreach (var pageElem in pageList.ElemList)
        {
            offset = AddElemFromPageElem(pageElem, offset, splitLevel);
        }
    }

    /// <summary>
    /// 使用 PageElem 向 Toc 中添加 TocElem
    /// </summary>
    private int AddElemFromPageElem(PageElem pageElem, int offset, int splitLevel)
    {
        // 将当前页面添加进 Toc
        var tocElem = new TocElem($"Text/chapter_{offset}.xhtml", pageElem.Heading, pageElem.Level);
        AddElem(tocElem);

        // 这里判断子元素是否需要继续递归
        if (splitLevel < pageElem.Level)
        {
            // 当 splitLevel 等于当前 pageElem 的 Level 时，说明 其 ChildrenPage 里所有的元素都是当前 pageElem 的 子标题
            // 因此将该 pageElem 的 ChildrenPage 里所有的元素标记为 subChapter

            for (var i = 0; i < pageElem.ChildrenPage.Count; i++)
            {
                AddElem(new TocElem($"Text/chapter_{offset}.xhtml#subChapter_{i}",
                    pageElem.ChildrenPage[i].Heading,
                    pageElem.Level + 1)
                );
            }
        }
        else if (splitLevel >= pageElem.Level)
        {
            // 当splitLevel大于Page页面的等级的时候，说明当前Page的ChildrenPage是会生成单独xhtml文件的
            // 因此递归调用AddTocElemFromPageElem方法将其添加到Toc，直到ChildrenPage的Level与SplitLevel相等

            foreach (var elem in pageElem.ChildrenPage)
            {
                offset = AddElemFromPageElem(elem, offset, splitLevel);
            }
        }

        return offset += 1;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (var n = 0; n < ElemList.Count; n++)
        {
            var elem = ElemList[n];
            var indent = (n == ElemList.Count - 1) ?  "    " : "\u2502   ";
            
            sb.Append(elem.Title + Environment.NewLine);
            
            var childCount = elem.Children.Count;
            for (var i = 0; i < childCount; i++)
            {
                PrintTree(elem.Children[i], indent, (i == childCount - 1), sb);
            }
        }

        return sb.ToString();
    }

    private void PrintTree(TocElem elem, string indent, bool isLast, StringBuilder sb)
    {
        string curLine;
        if (isLast)
        {
            curLine = indent + "└─ " + elem.Title + Environment.NewLine;
            indent += "    ";
        }
        else
        {
            curLine = indent + "├─ " + elem.Title + Environment.NewLine;
            indent += "|   ";
        }

        sb.Append(curLine);
        var childCount = elem.Children.Count;

        for (var i = 0; i < childCount; i++)
        {
            PrintTree(elem.Children[i], indent, (i == childCount - 1), sb);
        }
    }
}