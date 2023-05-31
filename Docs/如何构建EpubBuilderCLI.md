# 如何构建 Epub Builder CLI

## Dependency

- dotnet-sdk-6.0

微软官方安装文档 : [MS Dotnet Docs](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Publish

在 Windows 上将应用作为独立的单一文件应用程序发布

```shell
dotnet publish -r win-x64
```

在 Linux 上将应用作为依赖框架的单一文件应用程序发布

```shell
dotnet publish -r linux-x64 --self-contained false
```

打包后的程序在项目的 `bin/Debug/net6.0/平台名称/publish/` 目录下