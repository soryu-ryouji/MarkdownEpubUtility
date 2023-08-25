![logo](./Docs/Images/logo.png)

# Epub Builder Library

**CLI Example**

```shell
eb -m E:\天之炽\天之炽.md -c E:\天之炽\cover.jpg -b E:\天之炽\天之炽.epub -s 2
```

**Example**

```c#
var epubMetadata = new EpubMetadata
{
    Title = "太原之恋",
    Language = "zh",
    Author = "刘慈欣",
};

var mdPath = @"D:\Books\Novel\太原之恋\太原之恋.md";
var coverPath = @"D:\Books\Novel\太原之恋\cover.jpg";
var buildMetadata = new BuildMetadata(mdPath, coverPath, pageSplitLevel:1);
var epub = new Epub(epubMetadata, buildMetadata);

var zip = epub.Generate();
zip.Save(@"D:\太原之恋.epub");
```

## Docs

[Epub 电子书格式介绍](./Docs/Epub电子书格式.md)


## LICENSE

![MIT](./Docs/Images/MIT.png)

Epub is released under the MIT