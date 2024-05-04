using System.Text;

namespace MarkdownEpubUtility;

public class HtmlPages
{
    public readonly List<PageElem> ElemList = [];

    public void AddElem(PageElem pageElement, int limitLevel)
    {
        if (ElemList.Count == 0)
        {
            // 如果当前页面列表中一个元素都没有，则默认将第一个元素设置为其子元素，并将其等级设置为1
            pageElement.Level = 1;
            ElemList.Add(pageElement);
        }
        else if (pageElement.Level == 1)
        {
            // 如果待添加页面的等级为1，则将其添加到当前子元素列表中
            ElemList.Add(pageElement);
        }
        else if (pageElement.Level > 1)
        {
            // 如果待添加页面的等级大于2，则将其添加到当前子元素的最末尾元素中，使用AddPageElem进行自动插入
            ElemList.Last().AddElem(pageElement, limitLevel);
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

    private void PrintTree(PageElem elem, string indent, bool isLast, StringBuilder sb)
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