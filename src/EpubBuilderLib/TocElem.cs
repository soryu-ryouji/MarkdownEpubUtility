using System.Text;

namespace EpubBuilder;

public class TocElem
{
    public readonly List<TocElem> Children;
    public int Level { get; set; }
    private string Url { get; }
    public string Title { get; }

    public TocElem(string url, string title)
    {
        Level = 1;
        Url = url;
        Title = title;
        Children = new List<TocElem>();
    }

    public TocElem(string url, string title, int level)
    {
        Level = level;
        Url = url;
        Title = title;
        Children = new List<TocElem>();
    }

    /// <summary>
    /// 将当前元素和其子元素的Level都提升，其整体等级秩序保持不变
    /// </summary>
    private void UpLevel(int level)
    {
        // Tips : 之所以是 level+1 ，是因为1代表一级标题，2代表二级标题
        Level = level;
        foreach (var child in Children.Where(child => child.Level <= this.Level))
        {
            child.UpLevel(this.Level + 1);
        }
    }

    /// <summary>
    /// 为当前的元素添加子元素
    /// </summary>
    public void AddChild(TocElem tocElement)
    {
        // Tips : 之所以是 level+1 ，是因为1代表一级标题，2代表二级标题
        // 若被添加元素的level小于或等于当前元素的level，则将其元素等级设为比当前元素的level大1的状态
        if (tocElement.Level <= this.Level)
        {
            tocElement.UpLevel(this.Level + 1);
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
    public (int offset, string renderText) RenderToc(int offset)
    {
        offset += 1;
        var id = offset;
        string childrenToc;
        
        if (Children.Count == 0) childrenToc = "";
        else
        {
            var childTocList = new List<string>();
            foreach (var childTuple in Children.Select(child => child.RenderToc(offset)))
            {
                offset = childTuple.offset;
                childTocList.Add(childTuple.renderText);
            }

            childrenToc = string.Join("", childTocList);
        }

        string renderText =
            $"""
            <navPoint id = "navPoint-{id.ToString()}">
                <navLabel><text>{Title}</text></navLabel>
                <content src = "{Url}" />
                {childrenToc}
            </navPoint>
            """;

        return (offset, renderText);
    }
}