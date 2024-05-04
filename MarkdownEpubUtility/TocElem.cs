namespace MarkdownEpubUtility;

public class TocElem(string url, string title, int level = 1)
{
    public readonly List<TocElem> Children = [];
    public int Level { get; set; } = level;
    private string Url { get; } = url;
    public string Title { get; } = title;


    /// <summary>
    /// Raises the Level of both the current element and its children, leaving its overall rank order unchanged
    /// </summary>
    private void UpLevel(int level)
    {
        // Tips : The reason it is level+1 is because 1 represents the first level heading
        // and 2 represents the second level heading.
        Level = level;
        foreach (var child in Children.Where(child => child.Level <= this.Level))
        {
            child.UpLevel(this.Level + 1);
        }
    }

    public void AddChild(TocElem tocElement)
    {
        if (tocElement.Level <= this.Level)
        {
            tocElement.UpLevel(this.Level + 1);
        }

        Children.Add(tocElement);
    }

    public void AddElem(TocElem tocElement)
    {
        if (Children.Count == 0)
        {
            Children.Add(tocElement);
        }
        else if (tocElement.Level <= Children.Last().Level)
        {
            Children.Add(tocElement);
        }
        else if (tocElement.Level > Children.Last().Level)
        {
            Children.Last().AddElem(tocElement);
        }
    }

    /// <summary>
    /// Generate a catalog of the TocElement itself and its children.
    /// </summary>
    public (int offset, string renderText) Render(int offset)
    {
        offset += 1;
        var id = offset;
        string childrenToc;

        if (Children.Count == 0) childrenToc = "";
        else
        {
            var childTocList = new List<string>();
            foreach (var childTuple in Children.Select(child => child.Render(offset)))
            {
                offset = childTuple.offset;
                childTocList.Add(childTuple.renderText);
            }

            childrenToc = string.Join("", childTocList);
        }

        string renderText =
            $"""
            <navPoint id = "navPoint-{id}">
                <navLabel><text>{Title}</text></navLabel>
                <content src = "{Url}"/>
                {childrenToc}
            </navPoint>
            """;

        return (offset, renderText);
    }
}