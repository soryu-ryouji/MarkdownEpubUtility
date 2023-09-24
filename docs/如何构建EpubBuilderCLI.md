# 如何构建 Epub Builder CLI

## Dependency

- dotnet-sdk-7.0

微软官方安装文档 : [MS Dotnet Docs](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Publish

```shell
# 在 Windows 上将应用作为不依赖 `runtime` 的单一文件应用程序发布
dotnet publish -r win-x64 --self-contained
# 在 Windows 上将应用作为依赖 `runtime` 的单一文件应用程序发布
dotnet publish -r win-x64 --no-self-contained

# 在 Linux 上将应用作为不依赖 `runtime` 的单一文件应用程序发布
dotnet publish -r linux-x64 --no-self-contained
# 在 Linux 上将应用作为依赖 `runtime` 的单一文件应用程序发布
dotnet publish -r linux-x64 --self-contained
```

打包后的程序在项目的 `bin/Debug/net7.0/平台名称/publish/` 目录下