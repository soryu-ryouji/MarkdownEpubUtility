using System.Collections;

namespace MarkdownEpubUtility;

/// <summary>
/// Table of Contents
/// </summary>
public class EpubToc : IEnumerable<EpubTocItem>
{
    private readonly List<EpubTocItem> _tocItems = [];

    /// <summary>
    /// Add the element to the end of all child elements
    /// If the element is smaller than the Level of the last element, compare it to the Levels of the children of the last element.
    /// If the level is the same, or if it is greater than the end element, put it after that element
    /// </summary>
    public void Add(EpubTocItem tocElem)
    {
        if (_tocItems.Count == 0)
        {
            tocElem.Level = 1;
            _tocItems.Add(tocElem);
        }
        else if (tocElem.Level == 1) _tocItems.Add(tocElem);
        else _tocItems.Last().Add(tocElem);
    }

    /// <summary>
    /// Adding a TocItem to a Toc using a EpubPageItem
    /// </summary>
    public void Add(EpubPageItem pageItem, int splitLevel)
    {
        var tocElem = new EpubTocItem(pageItem.Url, pageItem.Heading, pageItem.Level);
        Add(tocElem);

        if (pageItem.Level > splitLevel)
        {
            foreach (var t in pageItem.Children)
            {
                Add(new EpubTocItem(pageItem.Url, t.Heading, pageItem.Level + 1));
            }
        }
        else if (pageItem.Level <= splitLevel)
        {
            foreach (var elem in pageItem.Children)
            {
                Add(elem, splitLevel);
            }
        }
    }

    /// <summary>
    /// Renders the current catelog of the Toc element
    /// </summary>
    public string Render()
    {
        var renderText = new List<string>();
        var offset = 0;
        foreach (var tocTuple in _tocItems.Select(elem => elem.Render(offset)))
        {
            offset = tocTuple.offset;
            renderText.Add(tocTuple.renderText);
        }

        return string.Join("", renderText);
    }

    public int Count => _tocItems.Count;

    public IEnumerator<EpubTocItem> GetEnumerator() => _tocItems.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}