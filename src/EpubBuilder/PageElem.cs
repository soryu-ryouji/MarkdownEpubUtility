using System.Text;

namespace EpubBuilder;

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
            // 如果当前页面的ChildrenPage一个元素都没有，则默认将第一个元素设置为其子元素，并将其等级重置为 当前页面等级+1
            newPageElement.Level = Level + 1;
            Children.Add(newPageElement);
        }
        else if (Level == limitLevel)
        {
            // 如果当前已经到了 limit level，则将所有新增页面停留在当前页面
            Children.Add(newPageElement);
        }
        else if (Level + 1 == newPageElement.Level)
        {
            // 如果新增的页面的等级为当前页面的子页面，则将其加入到ChildrenPage
            Children.Add(newPageElement);
        }
        else if (Level + 1 < newPageElement.Level)
        {
            // 如果新增页面的等级明显大于当前页面的子页面，则调用当前页面ChildrenPage最末尾元素的AddPageElem方法，自动插入到合适位置
            Children.Last().AddElem(newPageElement, limitLevel);
        }
    }

    public static string Render(PageElem pageElem)
    {
        var sb = new StringBuilder();
        sb.Append(ParseMd.Md2Html(string.Join("\n", pageElem.Content)));

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