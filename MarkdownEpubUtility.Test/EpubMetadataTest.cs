using System.Text;
using System.Xml;
using Xunit.Abstractions;

namespace MarkdownEpubUtility.Test;

public class EpubMetadataTest
{
    private readonly ITestOutputHelper _output;

    public EpubMetadataTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test_ParseXml()
    {
        // var text = File.ReadAllText("../../../res/test.xml");
        // var metadata = EpubMetadata.ParseXml(text);

        // _output.WriteLine(metadata.ToString());
    }
}