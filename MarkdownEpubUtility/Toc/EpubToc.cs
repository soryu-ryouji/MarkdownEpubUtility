using System.Text;

namespace MarkdownEpubUtility;

/// <summary>
/// Table of Contents
/// </summary>
public class EpubToc
{
    public readonly List<EpubTocItem> Toc = [];

    /// <summary>
    /// Add the element to the end of all child elements
    /// If the element is smaller than the Level of the last element, compare it to the Levels of the children of the last element.
    /// If the level is the same, or if it is greater than the end element, put it after that element
    /// </summary>
    public void Add(EpubTocItem tocElem)
    {
        if (Toc.Count == 0)
        {
            tocElem.Level = 1;
            Toc.Add(tocElem);
        }
        else if (tocElem.Level == 1) Toc.Add(tocElem);
        else Toc.Last().Add(tocElem);
    }

    /// <summary>
    /// Adding a TocItem to a Toc using a EpubPageItem
    /// </summary>
    private void Add(EpubPageItem pageItem, int splitLevel)
    {
        var tocElem = new EpubTocItem(pageItem.Url, pageItem.Heading, pageItem.Level);
        Add(tocElem);

        if (pageItem.Level > splitLevel)
        {
            foreach (var t in pageItem.Children)
            {
                Add(new EpubTocItem(pageItem.Url, t.Heading, pageItem.Level + 1));
            }
        }
        else if (pageItem.Level <= splitLevel)
        {
            foreach (var elem in pageItem.Children)
            {
                Add(elem, splitLevel);
            }
        }
    }

    /// <summary>
    /// Generating a Toc with PageList
    /// </summary>
    public void GenerateFromPage(EpubPage htmlPages, int splitLevel)
    {
        foreach (var pageElem in htmlPages.ElemList)
        {
            Add(pageElem, splitLevel);
        }
    }

    /// <summary>
    /// Renders the current catelog of the Toc element
    /// </summary>
    public string Render()
    {
        var renderText = new List<string>();
        var offset = 0;
        foreach (var tocTuple in Toc.Select(elem => elem.Render(offset)))
        {
            offset = tocTuple.offset;
            renderText.Add(tocTuple.renderText);
        }

        return string.Join("", renderText);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (var n = 0; n < Toc.Count; n++)
        {
            var elem = Toc[n];
            var indent = (n == Toc.Count - 1) ? "    " : "\u2502   ";

            sb.Append(elem.Title + Environment.NewLine);

            var childCount = elem.Children.Count;
            for (var i = 0; i < childCount; i++)
            {
                PrintTree(elem.Children[i], indent, (i == childCount - 1), sb);
            }
        }

        return sb.ToString();
    }

    private static void PrintTree(EpubTocItem elem, string indent, bool isLast, StringBuilder sb)
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