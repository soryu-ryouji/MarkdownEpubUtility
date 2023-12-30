# Epub Builder 执行流程

1. 传入 EpubMetadata 和 BuildMetadata 结构体

2. EpubBuilder 根据 BuildMetadata 的参数对 Markdown 文档进行预处理，例如提取出Markdown 文档中所有图片引用的绝对路径

3. 根据 Markdown 文档内容生成 HtmlPages

4. 根据 HtmlPages 生成 EpubContents

5. 根据 EpubContents 生成 opf 和 toc 文件

6. 导出 EpubContnets 保存的内容，生成epub文件