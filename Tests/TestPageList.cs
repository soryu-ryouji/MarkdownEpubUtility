using NUnit.Framework;
using EpubBuilder.Core;

namespace EpubBuilder.Tests;

[TestFixture]
public class TestPageList
{
    [Test]
    public void AddPageElem_WhenAddOneElem()
    {
        PageList pageList = new PageList();
        PageElement pageElement = new PageElement(1, "1_level_heading");
        pageList.AddPageElem(pageElement,2);
        var target = pageList.PageElemList.Last();

        bool isTrue = target == pageElement;
        
        Assert.IsTrue(isTrue);
    }
    
    [Test]
    public void AddPageElem_WhenAddTwoElem()
    {
        PageList pageList = new PageList();
        PageElement pageElement_1 = new PageElement(1, "1_level_heading_1");
        PageElement pageElement_2 = new PageElement(1, "1_level_heading_2");
        pageList.AddPageElem(pageElement_1,2);
        pageList.AddPageElem(pageElement_2,2);
        var target_1 = pageList.PageElemList[0];
        var target_2 = pageList.PageElemList[1];

        bool isTrue = (target_1 == pageElement_1) && (target_2 == pageElement_2);
        
        Assert.IsTrue(isTrue);
    }

    [Test]
    public void AddPageElem_WhenAddOne1LevelElemAnd2LevelElem()
    {
        PageList pageList = new PageList();
        PageElement pageElement_1 = new PageElement(1, "1_level_heading_1");
        PageElement pageElement_2 = new PageElement(2, "1_level_heading_2");
        PageElement pageElement_3 = new PageElement(2, "1_level_heading_2");
        pageList.AddPageElem(pageElement_1,2);
        pageList.AddPageElem(pageElement_2,2);
        pageList.AddPageElem(pageElement_3,2);

        var target_1 = pageList.PageElemList.Last();
        var target_2 = pageList.PageElemList.Last().ChildrenPage.First();
        var target_3 = pageList.PageElemList.Last().ChildrenPage.Last();

        bool isTrue = (target_1 == pageElement_1) && (target_2 == pageElement_2) && (target_3 == pageElement_3);
        
        Assert.IsTrue(isTrue);
    }
    
    [Test]
    public void AddPageElem_WhenAddOne1LevelElemAnd2LevelElem_LimitTwoLevel()
    {
        PageList pageList = new PageList();
        PageElement pageElement_1 = new PageElement(1, "1_level_heading_1");
        PageElement pageElement_2 = new PageElement(2, "1_level_heading_2");
        PageElement pageElement_3 = new PageElement(2, "1_level_heading_2");
        PageElement pageElement_4 = new PageElement(3, "1_level_heading_2");
        pageList.AddPageElem(pageElement_1,2);
        pageList.AddPageElem(pageElement_2,2);
        pageList.AddPageElem(pageElement_3,2);
        pageList.AddPageElem(pageElement_4,2);

        var target_1 = pageList.PageElemList.Last();
        var target_2 = pageList.PageElemList.Last().ChildrenPage[0];
        var target_3 = pageList.PageElemList.Last().ChildrenPage[1];
        var target_4 = pageList.PageElemList.Last().ChildrenPage[1].ChildrenPage.Last();

        bool isTrue = (target_1 == pageElement_1) && (target_2 == pageElement_2) && (target_3 == pageElement_3) &&
                      (target_4 == pageElement_4);
        
        Assert.IsTrue(isTrue);
    }
}