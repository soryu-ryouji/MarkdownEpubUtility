﻿namespace EpubBuilder.Core;

public class PageElement
{
    public int Level;
    public string Heading;
    public List<string> Content = new List<string>();
    public List<PageElement> ChildrenPage = new List<PageElement>();

    public PageElement(int level, string heading)
    {
        Level = level;
        Heading = heading;
    }

    public void AddPageElem(PageElement newPageElement, int limitLevel)
    {
        if (ChildrenPage.Count == 0)
        {
            // 如果当前页面的ChildrenPage一个元素都没有，则默认将第一个元素设置为其子元素，并将其等级重置为 当前页面等级+1
            newPageElement.Level = Level + 1;
            ChildrenPage.Add(newPageElement);
        }
        else if (Level == limitLevel)
        {
            // 如果当前已经到了 limit level，则将所有新增页面停留在当前页面
            ChildrenPage.Add(newPageElement);
        }
        else if (Level + 1  == newPageElement.Level)
        {
            // 如果新增的页面的等级为当前页面的子页面，则将其加入到ChildrenPage
            ChildrenPage.Add(newPageElement);
        }
        else if (Level + 1 < newPageElement.Level)
        {
            // 如果新增页面的等级明显大于当前页面的子页面，则调用当前页面ChildrenPage最末尾元素的AddPageElem方法，自动插入到合适位置
            ChildrenPage.Last().AddPageElem(newPageElement,limitLevel);
        }
    }
}