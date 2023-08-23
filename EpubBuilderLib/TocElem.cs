namespace EpubBuilder;

public class TocElem
{
    private int _level;
    private string _url;
    private string _title;
    public List<TocElem> Children;

    public int Level
    {
        get { return _level; }
        set { _level = value; }
    }

    public string Url
    {
        get { return _url; }
    }

    public string Title
    {
        get { return _title; }
    }

    public TocElem(string url, string title)
    {
        _level = 1;
        _url = url;
        _title = title;
        Children = new();
    }

    public TocElem(string url, string title, int level)
    {
        _level = level;
        _url = url;
        _title = title;
        Children = new();
    }

    /// <summary>
    /// 判断当前元素的子元素是否为空
    /// </summary>
    public bool ChildrenIsEmpty()
    {
        if (Children.Count == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 将当前元素和其子元素的Level都提升，其整体等级秩序保持不变
    /// </summary>
    public void UpLevel(int level)
    {
        // Tips : 之所以是 level+1 ，是因为1代表一级标题，2代表二级标题
        _level = level;
        foreach (var child in Children)
        {
            if (child.Level <= this._level)
            {
                child.UpLevel(this.Level + 1);
            }
        }
    }

    /// <summary>
    /// 为当前的元素添加子元素
    /// </summary>
    public void AddChild(TocElem tocElement)
    {
        // Tips : 之所以是 level+1 ，是因为1代表一级标题，2代表二级标题
        // 若被添加元素的level小于或等于当前元素的level，则将其元素等级设为比当前元素的level大1的状态
        if (tocElement.Level <= this._level)
        {
            tocElement.UpLevel(this._level + 1);
        }

        Children.Add(tocElement);
    }

    /// <summary>
    /// 为当前元素或者为其子元素添加元素
    /// </summary>
    public void AddElem(TocElem tocElement)
    {
        if (Children.Count == 0)
        {
            // 若当前子元素的个数为0时，直接插入到子元素中
            Children.Add(tocElement);
        }
        else if (tocElement.Level <= Children.Last().Level)
        {
            // 若当前元素的等级小于或者等于当前元素的子元素，则将其作为自己的子元素插入
            Children.Add(tocElement);
        }
        else if (tocElement.Level > Children.Last().Level)
        {
            // 若当前元素的 Level 大于 当前元素最后一个子元素的 Level，则让其再与子元素的最后一个子元素进行比较，直到将其插入到合适位置
            Children.Last().AddElem(tocElement);
        }
    }

    /// <summary>
    /// 生成TocElement自身和其子元素的目录
    /// </summary>
    public (int, string) RenderToc(int offset)
    {
        offset += 1;
        int id = offset;
        string childrenToc;
        if (ChildrenIsEmpty())
        {
            childrenToc = "";
        }
        else
        {
            var childTocList = new List<string>();
            foreach (var child in Children)
            {
                (int, string) childTuple = child.RenderToc(offset);
                offset = childTuple.Item1;
                childTocList.Add(childTuple.Item2);
            }

            childrenToc = string.Join("", childTocList);
        }

        string output = string.Format(
            "<navPoint id=\"navPoint-{0}\">\n" +
            "<navLabel>" +
            "<text>{1}</text>" +
            "</navLabel>\n" +
            "<content src=\"{2}\"/>\n" +
            "{3}" +
            "</navPoint>\n"
            , id.ToString()
            , _title
            , _url
            , childrenToc);

        return (offset, output);
    }
}