using System.Collections;
using System.Text;

namespace MarkdownEpubUtility;

public class EpubPage : IEnumerable<EpubPageItem>
{
    public readonly List<EpubPageItem> ElemList = [];

    public void Add(EpubPageItem pageElement, int limitLevel)
    {
        if (ElemList.Count == 0)
        {
            pageElement.Level = 1;
            ElemList.Add(pageElement);
        }
        else if (pageElement.Level == 1)
        {
            ElemList.Add(pageElement);
        }
        else if (pageElement.Level > 1)
        {
            ElemList.Last().Add(pageElement, limitLevel);
        }
    }

    public int Count => ElemList.Count;

    public IEnumerator<EpubPageItem> GetEnumerator() => ElemList.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}