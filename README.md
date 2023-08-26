![logo](./Docs/Images/logo.png)

# Epub Builder Library

Epub Builder 是一个将 Markdown 文档转换为 Epub 电子书的跨平台的库。

通过 `SplitLevel` 参数，可以实现电子书内容的分割显示，避免了所有内容挤在一张 `html`中造成了加载迟缓。


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
var buildPath = @"D:\太原之恋.epub";

var buildMetadata = new BuildMetadata(mdPath, coverPath, pageSplitLevel:1);

var epub = new Epub(epubMetadata, buildMetadata);

var zip = epub.Generate();
zip.Save(buildPath);
```


# EpubBuilder CLI

`-m` / `--markdown` : markdown 文件路径（必填）

`-c` / `--cover` : cover 文件路径

`-b` / `--build` : epub 文件生成路径

`-l` / `--language` : epub 语言

`-t` / `--title` : epub 电子书名称

`-a` / `--author` : 作者姓名

`-u` / `--uuid` : epub 唯一标识符

`-s` / `--split` : 页文件分割等级


**Example**

```shell
eb -m E:\天之炽\天之炽.md -c E:\天之炽\cover.jpg -b E:\天之炽\天之炽.epub -s 2
```


## Docs

[Epub 电子书格式介绍](./Docs/Epub电子书格式.md)


## LICENSE

![MIT](./Docs/Images/MIT.png)

EpubBuilder is released under the MIT