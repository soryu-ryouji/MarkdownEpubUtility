using System.Text;

namespace MarkdownEpubUtility;

public class EpubPage
{
    public readonly List<EpubPageItem> ElemList = [];

    public void Add(EpubPageItem pageElement, int limitLevel)
    {
        if (ElemList.Count == 0)
        {
            pageElement.Level = 1;
            ElemList.Add(pageElement);
        }
        else if (pageElement.Level == 1)
        {
            ElemList.Add(pageElement);
        }
        else if (pageElement.Level > 1)
        {
            ElemList.Last().Add(pageElement, limitLevel);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (var i = 0; i < ElemList.Count; i++)
        {
            var elem = ElemList[i];
            var indent = (i == ElemList.Count - 1) ? "    " : "\u2502   ";

            sb.Append(elem.Heading + Environment.NewLine);

            var childCount = elem.Children.Count;
            for (var n = 0; n < childCount; n++)
            {
                PrintTree(elem.Children[n], indent, (n == childCount - 1), sb);
            }
        }

        return sb.ToString();
    }

    private void PrintTree(EpubPageItem elem, string indent, bool isLast, StringBuilder sb)
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
            PrintTree(elem.Children[i], indent, (i == childCount - 1), sb);
        }
    }
}