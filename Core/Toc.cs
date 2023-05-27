namespace EpubBuilder.Core;

/// <summary>
/// Table of Contents
/// Epub 电子书 目录
/// </summary>
public class Toc
{
    List<TocElement> _elements = new List<TocElement>();

    public List<TocElement> Elements
    {
        get { return _elements; }
    }

    /// <summary>
    /// 判断当前Toc的子元素列表是否为空
    /// </summary>
    public bool ChildrenIsEmpty()
    {
        if (_elements.Count == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 将元素添加到所有子元素的最后
    /// 如果该元素比最后的元素的Level小，则将其与最后的元素的子元素的Level进行比较。
    /// 如果等级相同，或者是大于末尾的元素，则将其放在该元素的后面
    /// </summary>
    public void AddElem(TocElement tocElement)
    {
        if (_elements.Count == 0)
        {
            // 如果当前 _elements 列表为零时，将元素直接添加到 _elements 列表中
            _elements.Add(tocElement);
        }
        else
        {
            // 如果当前 _elements 元素不为空，则将其添加到当前 _elements 列表最末尾的元素中
            _elements.Last().AddElem(tocElement);
        }
    }

    /// <summary>
    /// 渲染Toc元素当前的目录
    /// </summary>
    public string RenderToc()
    {
        List<string> output = new List<string>();
        int offset = 0;
        foreach (var elem in _elements)
        {
            (int, string) tocTruple = elem.RenderToc(offset);
            offset = tocTruple.Item1;
            output.Add(tocTruple.Item2);
        }

        return String.Join("", output);
    }

    /// <summary>
    /// 从PageList中生成Toc
    /// </summary>
    /// <param name="pageList"></param>
    /// <param name="splitLevel"></param>
    public void GenerateTocFromPageList(PageList pageList, int splitLevel)
    {
        // 使用深度优先挨个访问pageList的所有节点
        throw new NotImplementedException();
    }

    /// <summary>
    /// 使用 PageElem 向 Toc 中添加 TocElem
    /// </summary>
    /// <param name="pageElem"></param>
    /// <param name="chapterNum"></param>
    /// <param name="splitLevel"></param>
    /// <returns></returns>
    public int AddTocElemFromPageElem(PageElement pageElem, int chapterNum, int splitLevel)
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
        TocElement tocElem = new TocElement($"Text/Chapter{chapterNum}", pageElem.Heading);
        tocElem.Level = pageElem.Level;
        AddElem(tocElem);

        if (splitLevel == pageElem.Level)
        {
            // 当 splitLevel 等于当前 pageElem 的 Level 时，说明 其 ChildrenPage 里所有的元素都是当前 pageElem 的 子标题
            // 因此将该 pageElem 的 ChildrenPage 里所有的元素标记为 subChapter
 
            for (int i = 0; i < pageElem.ChildrenPage.Count; i++)
            {
                AddElem(new TocElement($"Text/Chapter{chapterNum}#subChapter{i}", pageElem.ChildrenPage[i].Heading,pageElem.Level+1));
            }
        }
        else if (splitLevel > pageElem.Level)
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
        foreach (var unit in _elements)
        {
            PrintTocStruct(unit);
        }
    }

    public static void PrintTocStruct(TocElement tocElem)
    {
        string space = "——";
        Console.WriteLine(RepeatString(space,tocElem.Level-1) + tocElem.Title);
        
        if (tocElem.Children.Count != 0)
        {
            foreach (var subToc in tocElem.Children)
            {
                PrintTocStruct(subToc);
            }
        }
    }
    
    public static string RepeatString(string str, int times)
    {
        if (times == 0)
        {
            return "";
        }
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < times; i++)
        {
            sb.Append(str);
        }

        return sb.ToString();
    }
}