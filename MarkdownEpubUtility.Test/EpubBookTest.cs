using System.Text;
using Xunit.Abstractions;

namespace MarkdownEpubUtility.Test;

public class EpubBookTest
{
    private readonly ITestOutputHelper _output;

    public EpubBookTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void OpenBook_Test()
    {
        var book = EpubBook.OpenBook("../../../res/动物化的后现代.epub");
        foreach (var content in book.Content)
        {
            if (content.Type == EpubContentType.Html) _output.WriteLine(Encoding.UTF8.GetString(content.Content));
        }
    }

    [Fact]
    public void Test_ExtractImage()
    {
        // Given
        var book = EpubBook.OpenBook("../../../res/动物化的后现代.epub");

        // When

        var items = book.ExtractImage();

        // Then

        if (!Directory.Exists("../../../res/image/"))
        {
            Directory.CreateDirectory("../../../res/image/");
        }
        foreach (var image in items)
        {
            var filePath = Path.Combine("../../../res/image/", image.FileName);
            File.WriteAllBytes(filePath, image.Content);
        }
    }
}