using System.Xml;

namespace MarkdownEpubUtility;

public class EpubMetadata
{
    public string Title = "";
    public string Author = "";
    public string Creator = "";
    public string Language = "";
    public string Generator = "";
    public string Publisher = "";
    public string Description = "";
    public string Subject = "";
    public string License = "";
    public string PublishedDate = "";
    public string ModifiedDate = "";
    public string Uuid = "";

    public static EpubMetadata ParseXml(string text)
    {
        var xml = new XmlDocument();

        xml.LoadXml(text);
        var nsmgr = new XmlNamespaceManager(xml.NameTable);
        nsmgr.AddNamespace("opf", "http://www.idpf.org/2007/opf");
        nsmgr.AddNamespace("dc", "https://purl.org/dc/elements/1.1/");

        var packageNode = xml.DocumentElement;
        if (packageNode == null) throw new ArgumentException("content.opf can't be parser.");
        var metadataNode = packageNode.SelectSingleNode("opf:metadata", nsmgr);
        var metadata = new EpubMetadata();

        if (metadata == null) throw new ArgumentException("metadata of content.opf is null");
        metadata.Title = metadataNode.SelectSingleNode("dc:title", nsmgr)?.InnerText;
        metadata.Author = metadataNode.SelectSingleNode("dc:author", nsmgr)?.InnerText;
        metadata.Creator = metadataNode.SelectSingleNode("dc:creator", nsmgr)?.InnerText;
        metadata.Language = metadataNode.SelectSingleNode("dc:language", nsmgr)?.InnerText;
        metadata.Generator = metadataNode.SelectSingleNode("dc:generator", nsmgr)?.InnerText;
        metadata.Publisher = metadataNode.SelectSingleNode("dc:publisher", nsmgr)?.InnerText;
        metadata.Description = metadataNode.SelectSingleNode("dc:description", nsmgr)?.InnerText;
        metadata.Subject = metadataNode.SelectSingleNode("dc:subject", nsmgr)?.InnerText;
        metadata.License = metadataNode.SelectSingleNode("dc:rights", nsmgr)?.InnerText;
        metadata.PublishedDate = metadataNode.SelectSingleNode("dc:date[@event='publication']", nsmgr)?.InnerText;
        metadata.ModifiedDate = metadataNode.SelectSingleNode("dc:date[@event='modification']", nsmgr)?.InnerText;
        metadata.Uuid = packageNode.SelectSingleNode("opf:package/@unique-identifier", nsmgr)?.Value;

        return metadata;
    }

    public override string ToString()
    {
        return
        $"""
        Title: {Title}
        Author: {Author}
        Language: {Language}
        Generator: {Generator}
        Description: {Description}
        Subject: {Subject}
        License: {License}
        PublishedDate: {PublishedDate}
        ModifiedDate: {ModifiedDate}
        Uuid: {Uuid}
        """;
    }
}