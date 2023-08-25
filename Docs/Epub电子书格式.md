# Epub 电子书格式介绍

一个未经加密处理的epub电子书由以下三部分组成：

- META-INF（文件夹，有一个文件container.xml）
- OEBPS（文件夹，包含images文件夹、很多xhtml文件、*.css文件和content.opf文件）
- mimetype

## mimetype

每一个epub电子书均包含一个名为mimtype的文件，且内容不变，用以说明epub的文件格式。文件内容为

```
application/epub+zip
```

## 文件夹: META-INF

META-INF用于存放容器信息，默认情况下改目录包含一个文件，即container.xml，文件内容如下

```xml
<?xml version="1.0"?>
<container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
    <rootfiles>
        <rootfile full-path="OEBPS/content.opf" media-type="application/oebps-package+xml"/>
    </rootfiles>
</container>
```

container.xml文件的主要功能用于告诉阅读器，电子书的根文件（rootfile）的路径和打开格式，一般来说，该containerxml文件也不需要任何修改，除非改变了根文件的路径和文件名称

除了container.xml文件之外，OCF还规定了以下几个文件：

- manifest.xml 文件列表
- metadata.xml 元数据
- signatures.xml 数字签名
- encryption.xml 加密
- rights.xml 权限管理

这些目录是可选的

> Tips:
>
> OCF代表Open Container Format（开放式容器格式）。
>
> 它是一个开放标准，用于定义和组织容器化应用程序的文件结构和元数据。

## 文件夹: OEBPS

OEPBS目录用于存放OPF文档、CSS文件、NCX文档。

### 文件: OPF

OPF文件的全称是Open Packaging Format文件。它是EPUB（电子出版物）格式中的一个关键文件，用于描述EPUB的元数据和内容组织。

OPF文档是epub的核心文件，且是一个标准的xml文件，依据OPF规范，此文件的根元素为<package>。

其内容主要由五部分组成

#### 1、metadata

`<dc-metadata>`，其元素构成采用dubline core(DC)的15项核心元素

- `<dc-title>` : 标题
- `<dc-creator>` : 责任者
- `<dc-subject>` : 主题词或关键词
- `<dc-descributor>` : 内容描述
- `<dc-date>` : 日期
- `<dc-type>` : 类型
- `<dc-publisher>` : 出版者
- `<dc-contributor>` : 发行者
- `<dc-format>` : 格式
- `<dc-identifier>` : 标识信息
- `<dc-source>` : 来源信息
- `<dc-language>` : 语言
- `<dc-relation>` : 相关资料
- `<dc-coverage>` : 覆盖范围
- `<dc-rights>` : 权限描述

`<x-metadata>`,该metadata为扩展元素。如果有些信息在上述元素中无法描述，则在此元素中进行扩展。

每一个`metadata`部分至少需要包含一个`language`
元素，其值需要符合[[RFC5646]](https://datatracker.ietf.org/doc/html/rfc5646)

`<dc-identifier>`对于一个且仅是一个特定的 EPUB 出版物是唯一的。此唯一标识符，无论是选择还是分配，都必须存储在包元数据中的
dc:identifier 元素中，并在包元素唯一标识符属性中作为唯一标识符引用。尽管不是静态的，但应尽可能少地更改出版物的唯一标识符。

在更新元数据、修复勘误表或对出版物进行其他微小更改时，不应发布新的标识符。而是使用`modified`属性

```xml
<metadata xmlns:dc="http://purl.org/dc/elements/1.1/">
    <dc:identifier id="pub-id">urn:uuid:A1B0D67E-2E81-4DF5-9E67-A64CBE366809</dc:identifier>
    <meta property="dcterms:modified">2011-01-01T12:00:00Z</meta>
</metadata>
```

#### 2、manifest

文件列表，列出书籍出版的所有文件，但是不包括 : mimetype、container.xml、content.opf，由一个子元素构成

每行列表的格式如下

```
<item id="" href="" media-type="">
```

- `id` : 元素的id号，在文档范围内必须是唯一的
- `href` : 元素的相对路径
- `media-type` : 文件的媒体类型，其类型需遵守[[RFC2046]](https://datatracker.ietf.org/doc/html/rfc2046)

```xml
<manifest>
	<item href="toc.ncx" id="ncx" media-type="application/x-dtbncx+xml" />
	<item href="cover.xhtml" id="cover" media-type="application/xhtml+xml"/>
	<item href="copyright.xhtml" id="copyright" media-type="application/xhtml+xml"/>
	<item href="catalog.xhtml" id="catalog" media-type="application/xhtml+xml"/>
	<item href="chap0.xhtml" id="chap0" media-type="application/xhtml+xml"/>
</manifest>
```

#### 3、spine

脊骨，其主要功能是提供书籍的线性阅读次序。

```xml
<itemref idref="copyright"/>
```

- idref : 在manifest中列出的文件的id

```xml
<spine toc="ncx">
    <itemref idref="cover" />
    <itemref idref="copyright" />
</spine>
```

#### 4、guide（V3已废弃）

> The `guide` element [[OPF2\]](https://idpf.org/epub/30/spec/epub30-publications.html#refOPF2) is deprecated in favor
> of the `landmarks` feature in
> the [EPUB Navigation Document](https://idpf.org/epub/30/spec/epub30-publications.html#gloss-content-document-epub-nav).
> Refer
> to [The landmarks nav Element](https://idpf.org/epub/30/spec/epub30-contentdocs.html#sec-xhtml-nav-def-types-landmarks) [[ContentDocs30\]](https://idpf.org/epub/30/spec/epub30-publications.html#refContentDocs3)
> for more information.
>
> Authors may include the `guide` element in the Package Document for EPUB 2 Reading System forwards compatibility
> purposes. EPUB 3 Reading Systems must ignore the `guide` element when provided in EPUB 3 Publications
> whose [EPUB Navigation Document](https://idpf.org/epub/30/spec/epub30-publications.html#gloss-content-document-epub-nav)
> includes the `landmarks` feature.

一次列出电子书的特定页面，例如封面、目录、序言等，属性值指向文件保存地址。一般情况下，epub电子书可以不用该元素。

#### 5、tour（V3已废弃）

导读，可以根据不同的读者水平或阅读目的，按一定的次序，选择电子书中的部分页面组成导读。一般情况下，epub电子书可以不用该元素。

### 文件: NCX

NCX (Navigation Center eXtended) 文件是epub电子书的又一个核心文件，用于制作电子书的目录，其文件的命名通常为toc.ncx。

ncx文件中最主要的节点是navMap。navMap节点是由许多navPoint节点组成的。而navPoint节点则是由navLabel、content两个子节点组成。

- navPoint节点中，playOrder属性定义当前项在目录中显示的次序。navLabel子节点中的text节点定义了每个目录的名字。

- content子节点的src属性定义了对应每个章节的文件的具体位置

nvaPoint节点可以嵌套，嵌套的层次就是目录的层次

```xml
<?xml version="1.0" encoding="utf-8"?>
<ncx xmlns="http://www.daisy.org/z3986/2005/ncx/" version="2005-1">
    <head>
        <meta content="178_0" name="dtb:uid"/>
        <meta content="2" name="dtb:depth"/>
        <meta content="0" name="dtb:totalPageCount"/>
        <meta content="0" name="dtb:maxPageNumber"/>
    </head>
    <docTitle>
        <text>1984</text>
    </docTitle>
    <docAuthor>
        <text>[英] 乔治·奥威尔</text>
    </docAuthor>
    
    <navMap>
        <navPoint id="catalog" playOrder="0">
            <navLabel>
                <text>目录</text>
            </navLabel>
            <content src="catalog.xhtml"/>
        </navPoint>
        <navPoint id="chap0" playOrder="1">
            <navLabel>
                <text>前言</text>
            </navLabel>
            <content src="chap0.xhtml"/>
        </navPoint>
        <navPoint id="chap1" playOrder="2">
            <navLabel>
                <text>第一部</text>
            </navLabel>
            <content src="chap1.xhtml"/>
        </navPoint>
        <navPoint id="chap2" playOrder="3">
            <navLabel>
                <text>第1节</text>
            </navLabel>
            <content src="chap2.xhtml"/>
        </navPoint>
        <navPoint id="chap3" playOrder="4">
            <navLabel>
                <text>第2节</text>
            </navLabel>
            <content src="chap3.xhtml"/>
        </navPoint>
        <navPoint id="chap4" playOrder="5">
            <navLabel>
                <text>第3节</text>
            </navLabel>
            <content src="chap4.xhtml"/>
        </navPoint>
    </navMap>
</ncx>
```

在navPoint中，`id`就是`manifest`中定义的文件id，而`playOrder`则用于控制目录的显示顺序，但是Epub格式规范中并不对此强制要求。

可能会有有人觉得.opf文件与.ncx文件有一点重复：.opf文件的item节点中的href属性描述了各个章节文件的位置与顺序，.ncx文件中的content节点中的src属性也描述了各个章节文件的位置与顺序。

其实他们还是有区别的，区别在于，.opf文件定义的是读者在顺序阅读时会用到的章节文件与它们的顺序，.ncx文件则定义的是目录中会用到的章节文件与它们的顺序。

如果存在某些附件性质的内容被希望在目录中出现，但却不希望在读者顺序阅读的时候出现时，那么就可以通过对.opf文件和.ncx文件进行不同的设置来达到这个目的。

当然，大部分的时候，.opf与.ncx这两个文件的内容基本是重合的。

## 参考资料

[vernlium的Epub格式解析](https://vernlium.github.io/2015/06/10/epub%E6%A0%BC%E5%BC%8F%E8%A7%A3%E6%9E%90/)

[Epub官方文档](https://idpf.org/epub/30/spec/epub30-publications.html#sec-package-elem)
