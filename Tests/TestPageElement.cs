using System.Text;
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

    [Test]
    public void RenderSelfAndAllSubPage()
    {
        PageList pageList = new PageList();
        
        PageElement page1 = new PageElement(1, "page1");
        page1.Content.Add("# 1-1 “御宅族系文化”所展现的后现代主义姿态");
        page1.Content.Add("大概沒有人不知道“御宅族”这个词吧？");
        page1.Content.Add("简单来说，那是和漫画、动画、电子游戏、个人电脑、科幻（SF）、特摄片、手办模型相互之间有着深刻关联、沉溺在次文化里的一群人的总称。");
        page1.Content.Add("本书将这一群人的次文化称之为“御宅族系文化”");
        PageElement page2 = new PageElement(2,"page2");
        page2.Content.Add("## 御宅族的三个世代");
        page2.Content.Add("我想在此简单叙述一下，为何本书不说“御宅族文化”，而使用“御宅族系文化”这种曖昧表现的理由。");
        page2.Content.Add("由于前述那些复杂的的状况，九〇年代对于“何谓御宅族”、“御宅族化的东西是什么”、“谁是御宅族，谁又不是御宅族”等问题，在御宅族之间累积了莫大的争议。");
        PageElement page3 = new PageElement(2,"page3");
        page3.Content.Add("## 御宅族系文化具有的日本印象");
        page3.Content.Add("关于御宅族系文化的特征，从过去到现在经常被拿来和日本传统文化做比较。");
        page3.Content.Add("例如评论家大塚英志于一九八九年出版的《故事消费论》，便将八〇年代急速增加的二次创作的存在意义，以歌舞伎与人形净琉璃中所用的“世界”和“志趣”的概念来进行分析。");
        PageElement page4 = new PageElement(3,"page4");
        page4.Content.Add("### 美国是御宅族系文化的源流");
        page4.Content.Add("不过如果是这样，反过来说，为什么御宅族一直对“日本的事物”抱持上述的执着呢？");
        page4.Content.Add("在此我们必须要想到，御宅族系文化的起源无论是动画、特摄、科幻、或是电脑游戏，乃至于支撑上述全体的杂志文化，事实上都是战后五〇到七〇年代从美国进口而来的次文化。");
        PageElement page5 = new PageElement(3,"page5");
        page5.Content.Add("### 使日本动画蓬勃的独特美学");
        page5.Content.Add("有趣的是接下来的七〇年代，日本动画业积极接纳了这种贫乏，反而将其独特的美学蓬勃发展。");
        page5.Content.Add("一般来说，七〇年代的动画创作者们大致分为表现主义与故事主义两种类型。");
        PageElement page6 = new PageElement(2,"page6");
        page6.Content.Add("## 日本文化背景里的战败伤痕");
        page6.Content.Add("御宅族系文化对于日本的执着，並不是成立在传统上面，而是成立于传统被消灭以后。");
        page6.Content.Add("换句话说，御宅族系文化存在的背后，是叫做战败的心灵外伤，隐藏着日本人决定性地失去了传统自我认同的残酷事实。");
        PageElement page7 = new PageElement(1,"page7");
        page7.Content.Add("# 2-1 御宅族与后现代");
        page7.Content.Add("如果只是主张御宅族系文化的本质，与后现代社会结构间有着深厚关联，並无新意。以下两点，早已被指出是御宅族系文化在后现代的特征。");
        PageElement page8 = new PageElement(2,"page8");
        page8.Content.Add("## 拟像的增殖");
        page8.Content.Add("一个是“二次创作”的存在。所谓二次创作，是指将原作于的动画和漫画、电玩游戏以性的角度进行阅读及转换、再加以制作及贩售的同人志或同人游戏、同人手办模型等的总称。k");

        int splitLevel = 2;
        pageList.AddPageElem(page1,splitLevel);
        pageList.AddPageElem(page2,splitLevel);
        pageList.AddPageElem(page3,splitLevel);
        pageList.AddPageElem(page4,splitLevel);
        pageList.AddPageElem(page5,splitLevel);
        pageList.AddPageElem(page6,splitLevel);
        pageList.AddPageElem(page7,splitLevel);
        pageList.AddPageElem(page8,splitLevel);
        var str = PageElement.RenderSelfAndAllSubPageContent(page1);

        Console.WriteLine(str);
    }
}
