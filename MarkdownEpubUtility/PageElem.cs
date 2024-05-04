using System.Text;

namespace MarkdownEpubUtility;

public class PageElem(string url, int level, string heading)
{
    public int Level = level;
    public string Url = url;
    public readonly string Heading = heading;
    public readonly List<string> Content = [];
    public readonly List<PageElem> Children = [];

    public void AddElem(PageElem newPageElement, int limitLevel)
    {
        if (Children.Count == 0)
        {
            // If there is no element in the current page's ChildrenPage
            // the first element is set as its child by default, and its level is reset to the current page level +1.
            newPageElement.Level = Level + 1;
            Children.Add(newPageElement);
        }
        else if (Level == limitLevel)
        {
            // If the current limit level has been reached, all new pages will stay on the current page.
            Children.Add(newPageElement);
        }
        else if (Level + 1 == newPageElement.Level)
        {
            // If the level of the added page is a child of the current page, add it to the ChildrenPage
            Children.Add(newPageElement);
        }
        else if (Level + 1 < newPageElement.Level)
        {
            // If the level of the new page is greater than that of the current page's children
            // the AddPageElem method of the last element of the current page's ChildrenPage is called,
            // and the page is automatically inserted into the appropriate position.
            Children.Last().AddElem(newPageElement, limitLevel);
        }
    }

    public static string Render(PageElem pageElem)
    {
        var sb = new StringBuilder();
        sb.Append(EpubConvert.Md2Html(string.Join("\n", pageElem.Content)));

        if (pageElem.Children.Count != 0)
        {
            foreach (var unit in pageElem.Children)
            {
                sb.Append(Render(unit));
            }
        }

        return sb.ToString();
    }
}