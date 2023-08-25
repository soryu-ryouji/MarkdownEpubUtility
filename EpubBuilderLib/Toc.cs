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
    public void GenerateTocFromPageList(HtmlPages htmlPages, int splitLevel)
    {
        foreach (var pageElem in htmlPages.ElemList)
        {
            AddElemFromPageElem(pageElem, splitLevel);
        }
    }

    /// <summary>
    /// 使用 PageElem 向 Toc 中添加 TocElem
    /// </summary>
    private void AddElemFromPageElem(PageElem pageElem, int splitLevel)
    {
        var tocElem = new TocElem(pageElem.Url, pageElem.Heading, pageElem.Level);
        AddElem(tocElem);
        
        // 这里判断子元素是否需要继续递归
        if (pageElem.Level > splitLevel)
        {
            foreach (var t in pageElem.Children)
            {
                AddElem(new TocElem(pageElem.Url, t.Heading, pageElem.Level + 1));
            }
        }
        else if (pageElem.Level <= splitLevel)
        {
            foreach (var elem in pageElem.Children)
            {
                AddElemFromPageElem(elem, splitLevel);
            }
        }
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