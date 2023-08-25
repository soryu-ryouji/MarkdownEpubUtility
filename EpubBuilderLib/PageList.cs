namespace EpubBuilder;

public class PageList
{
    public List<PageElem> ElemList = new();

    public void AddPageElem(PageElem pageElement, int limitLevel)
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
            ElemList.Last().AddPageElem(pageElement, limitLevel);
        }
    }

    /// <summary>
    /// 打印当前PageList的元素
    /// </summary>
    public void PrintPageListStruct()
    {
        foreach (var unit in ElemList)
        {
            PrintPageChildrenStruct(unit);
        }
    }

    public static void PrintPageChildrenStruct(PageElem pageElement)
    {
        string space = "——";
        Console.WriteLine(RepeatString(space, pageElement.Level - 1) + pageElement.Heading);

        if (pageElement.ChildrenPage.Count != 0)
        {
            foreach (var subPage in pageElement.ChildrenPage)
            {
                PrintPageChildrenStruct(subPage);
            }
        }
    }

    public static string RepeatString(string str, int times)
    {
        if (times == 0)
        {
            return "";
        }

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < times; i++)
        {
            sb.Append(str);
        }

        return sb.ToString();
    }
}