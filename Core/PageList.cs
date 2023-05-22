namespace EpubBuilder.Core;

public class PageList
{
    public List<PageElement> Pages = new List<PageElement>();

    public void AddPageElem(PageElement pageElement, int limitLevel)
    {
        if (Pages.Count == 0)
        {
            // 如果当前页面列表中一个元素都没有，则默认将第一个元素设置为其子元素，并将其等级设置为1
            pageElement.Level = 1;
            Pages.Add(pageElement);
        }
        else if (pageElement.Level == 1)
        {
            // 如果待添加页面的等级为1，则将其添加到当前子元素列表中
            Pages.Add(pageElement);
        }
        else if (pageElement.Level > 1)
        {
            // 如果待添加页面的等级大于2，则将其添加到当前子元素的最末尾元素中，使用AddPageElem进行自动插入
            Pages.Last().AddPageElem(pageElement, limitLevel);
        }
    }
}