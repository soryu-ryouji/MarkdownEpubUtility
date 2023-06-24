namespace EpubBuilder;

/// <summary>
/// Table of Contents
/// Epub 电子书 目录
/// </summary>
public class Toc
{
    List<TocElem> _tocElemList = new();

    public List<TocElem> TocElemList
    {
        get { return _tocElemList; }
    }

    /// <summary>
    /// 判断当前Toc的子元素列表是否为空
    /// </summary>
    public bool ChildrenIsEmpty()
    {
        return _tocElemList.Count == 0;
    }

    /// <summary>
    /// 将元素添加到所有子元素的最后
    /// 如果该元素比最后的元素的Level小，则将其与最后的元素的子元素的Level进行比较。
    /// 如果等级相同，或者是大于末尾的元素，则将其放在该元素的后面
    /// </summary>
    public void AddElem(TocElem tocElement)
    {
        if (_tocElemList.Count == 0)
        {
            // 如果当前 _elements 列表为零时，将元素直接添加到 _elements 列表中
            _tocElemList.Add(tocElement);
        }
        else if (tocElement.Level == 1)
        {
            // 如果当前tocElement的Level为1，则将其添加到tocElemList中
            _tocElemList.Add(tocElement);
        }
        else
        {
            // 如果当前 _elements 元素不为空，则将其添加到当前 _elements 列表最末尾的元素中
            _tocElemList.Last().AddElem(tocElement);
        }
    }

    /// <summary>
    /// 渲染 Toc 元素当前的目录
    /// </summary>
    public string RenderToc()
    {
        var output = new List<string>();
        int offset = 0;
        foreach (var elem in _tocElemList)
        {
            (int, string) tocTruple = elem.RenderToc(offset);
            offset = tocTruple.Item1;
            output.Add(tocTruple.Item2);
        }

        return string.Join("", output);
    }

    /// <summary>
    /// 使用 PageList 生成 Toc
    /// </summary>
    /// <param name="pageList"></param>
    /// <param name="splitLevel"></param>
    public void GenerateTocFromPageList(PageList pageList, int splitLevel)
    {
        // 遍历 pageList.PageElemList 中的元素生成Toc
        // PageElemList 的子元素 AddTocElemFromPageElem 会自动递归添加到Toc中
        int chapterNum = 0;
        foreach (var unit in pageList.PageElemList)
        {
            chapterNum = AddTocElemFromPageElem(unit, chapterNum, splitLevel);
        }
    }

    /// <summary>
    /// 使用 PageElem 向 Toc 中添加 TocElem
    /// </summary>
    /// <param name="pageElem"></param>
    /// <param name="chapterNum"></param>
    /// <param name="splitLevel"></param>
    /// <returns></returns>
    public int AddTocElemFromPageElem(PageElem pageElem, int chapterNum, int splitLevel)
    {
        // 在实例化Toc类后，该方法会根据 PageElem 的 Level 和 splitLevel 自动向 Toc 中添加 TocElem
        // Example:
        // FirstPage
        // |—— SecondPage_1
        // |   |—— ThirdPage_1
        // |   |—— ThirdPage_2
        // |—— SecondPage_2
        // 
        // 当 splitLevel 为2时，其 Toc 结构为
        // FirstPage
        // |—— SecondPage_1
        // |—— SecondPage_2

        // 将当前页面添加进 Toc
        var tocElem = new TocElem($"Text/chapter_{chapterNum}.xhtml", pageElem.Heading, pageElem.Level);
        AddElem(tocElem);

        // 这里判断子元素是否需要继续递归
        if (splitLevel < pageElem.Level)
        {
            // 当 splitLevel 等于当前 pageElem 的 Level 时，说明 其 ChildrenPage 里所有的元素都是当前 pageElem 的 子标题
            // 因此将该 pageElem 的 ChildrenPage 里所有的元素标记为 subChapter

            for (int i = 0; i < pageElem.ChildrenPage.Count; i++)
            {
                AddElem(new TocElem($"Text/chapter_{chapterNum}.xhtml#subChapter_{i}",
                    pageElem.ChildrenPage[i].Heading,
                    pageElem.Level + 1)
                );
            }
        }
        else if (splitLevel >= pageElem.Level)
        {
            // 当splitLevel大于Page页面的等级的时候，说明当前Page的ChildrenPage是会生成单独xhtml文件的
            // 因此递归调用AddTocElemFromPageElem方法将其添加到Toc，直到ChildrenPage的Level与SplitLevel相等

            for (int i = 0; i < pageElem.ChildrenPage.Count; i++)
            {
                // 递归当前PageElem的子元素
                chapterNum = AddTocElemFromPageElem(pageElem.ChildrenPage[i], chapterNum, splitLevel);
            }
        }

        return chapterNum += 1;
    }

    public void PrintTocListStruct()
    {
        foreach (var unit in _tocElemList)
        {
            PrintTocStruct(unit);
        }
    }

    public static void PrintTocStruct(TocElem tocElem)
    {
        string space = "——";
        Console.WriteLine(RepeatString(space, tocElem.Level - 1) + tocElem.Title);

        if (tocElem.Children.Count != 0)
        {
            foreach (var subToc in tocElem.Children)
            {
                PrintTocStruct(subToc);
            }
        }
    }

    private static string RepeatString(string str, int times)
    {
        if (times == 0) return "";

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < times; i++)
        {
            sb.Append(str);
        }

        return sb.ToString();
    }
}