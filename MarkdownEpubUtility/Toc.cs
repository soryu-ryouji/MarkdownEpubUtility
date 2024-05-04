using System.Text;

namespace MarkdownEpubUtility;

/// <summary>
/// Table of Contents
/// </summary>
public class Toc
{
    public readonly List<TocElem> ElemList = [];

    /// <summary>
    /// Add the element to the end of all child elements
    /// If the element is smaller than the Level of the last element, compare it to the Levels of the children of the last element.
    /// If the level is the same, or if it is greater than the end element, put it after that element
    /// </summary>
    public void AddElem(TocElem tocElem)
    {
        if (ElemList.Count == 0)
        {
            tocElem.Level = 1;
            ElemList.Add(tocElem);
        }
        else if (tocElem.Level == 1) ElemList.Add(tocElem);
        else ElemList.Last().AddElem(tocElem);
    }

    /// <summary>
    /// Renders the current catelog of the Toc element
    /// </summary>
    public string Render()
    {
        var renderText = new List<string>();
        var offset = 0;
        foreach (var tocTuple in ElemList.Select(elem => elem.Render(offset)))
        {
            offset = tocTuple.offset;
            renderText.Add(tocTuple.renderText);
        }

        return string.Join("", renderText);
    }

    /// <summary>
    /// Generating a Toc with PageList
    /// </summary>
    public void GenerateTocFromPageList(HtmlPages htmlPages, int splitLevel)
    {
        foreach (var pageElem in htmlPages.ElemList)
        {
            AddElemFromPageElem(pageElem, splitLevel);
        }
    }

    /// <summary>
    /// Adding a TocElem to a Toc using a PageElem
    /// </summary>
    private void AddElemFromPageElem(PageElem pageElem, int splitLevel)
    {
        var tocElem = new TocElem(pageElem.Url, pageElem.Heading, pageElem.Level);
        AddElem(tocElem);

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
            var indent = (n == ElemList.Count - 1) ? "    " : "\u2502   ";

            sb.Append(elem.Title + Environment.NewLine);

            var childCount = elem.Children.Count;
            for (var i = 0; i < childCount; i++)
            {
                PrintTree(elem.Children[i], indent, (i == childCount - 1), sb);
            }
        }

        return sb.ToString();
    }

    private static void PrintTree(TocElem elem, string indent, bool isLast, StringBuilder sb)
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