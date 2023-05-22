using NUnit.Framework;
using EpubBuilder.Core;

namespace EpubBuilder.Tests;

[TestFixture]
public class TestPageElement
{
    [Test]
    public void AddPageElem_WhenAddOneElem()
    {
        PageElement pageElement = new PageElement(level:1,"HeadingOne");
        PageElement childElement = new PageElement(level:1,"HeadingChild");
        pageElement.AddPageElem(childElement,2);

        var target = pageElement.ChildrenPage.Last();
        bool isTrue = target == childElement;
        
        Assert.IsTrue(isTrue);
    }
    
    [Test]
    public void AddPageElem_WhenAddTwoElem_LimitLevelIsThree()
    {
        // 将页面分割等级限制为3
        PageElement pageElement = new PageElement(level:1,"HeadingOne");
        PageElement childElement_1 = new PageElement(level:1,"HeadingChild_1");
        PageElement childElement_2 = new PageElement(level:3,"HeadingChild_2");
        childElement_2.Level = 3;
        pageElement.AddPageElem(childElement_1,3);
        pageElement.AddPageElem(childElement_2,3);

        var target_1 = pageElement.ChildrenPage.Last();
        var target_2 = pageElement.ChildrenPage.Last().ChildrenPage.Last();
        bool isTrue = (target_1 == childElement_1) && (target_2 == childElement_2);
        
        Assert.IsTrue(isTrue);
    }
    
    [Test]
    public void AddPageElem_WhenAddThreeElem()
    {
        // 将页面分割等级限制为3
        PageElement pageElement = new PageElement(level:1,"HeadingOne");
        PageElement childElement_1 = new PageElement(level:1,"HeadingChild_1");
        PageElement childElement_2 = new PageElement(level:3,"HeadingChild_2");
        PageElement childElement_3 = new PageElement(level:4,"HeadingChild_3");

        // childElement_1会因为当前pageElement没有任何子元素而被作为第一个子元素，Level被强制设置为2
        pageElement.AddPageElem(childElement_1,2);
        // childElement_2会因为Level大于childElement_1被添加到childElement_1的子元素，pageLevel达到2
        pageElement.AddPageElem(childElement_2,2);
        // childElement_3会因为pageLevel而被限制在childElement_1的子元素中
        pageElement.AddPageElem(childElement_3,2);

        var target_1 = pageElement.ChildrenPage.Last();
        var target_2 = pageElement.ChildrenPage.Last().ChildrenPage.First();
        var target_3 = pageElement.ChildrenPage.Last().ChildrenPage.Last();

        bool isTrue = (target_1 == childElement_1) && (target_2 == childElement_2) && (target_3 == childElement_3);
        
        Assert.IsTrue(isTrue);
    }
}
