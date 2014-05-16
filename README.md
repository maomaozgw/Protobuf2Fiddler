# Protobuf to fiddler #
一个基于Fiddler的Proto部分解包工具

## 使用说明 ##
### 安装 ###
1. 下载安装 **[Fiddler2 for .Net Framework4](http://www.telerik.com/download/fiddler)**
![Download Fiddler](http://gitlab.baidu.com/zhaoguowei/documents/raw/master/Protobuf2Fiddler/DownloadFiddler.png)
2. 下载项目源码或者编译好的压缩包

3. 将编译或者下载后的文件放置到 `Documents\Fiddler2\Inspectors`文件夹或者`Program Files\Fiddler2\Inspectors` 32bit系统 `Program Files (x86)\Fiddler2\Inspectors` 64bit系统
![UpzipPlugins](http://gitlab.baidu.com/zhaoguowei/documents/raw/master/Protobuf2Fiddler/unzipplugin.png)
*tip:*
    * 放置在`Documents`下插件只对当前用户有效，放置在`Program Files`中可能导致配置文件写入时的权限问题

### 使用 ### 
1. 安装完毕插件后启动Fiddler，在Fiddler中选择一个Session
2. 在右侧的 Inspectors 选项卡中可以看到 **Protobuf** 功能
![ProtobufTab](http://gitlab.baidu.com/zhaoguowei/documents/raw/master/Protobuf2Fiddler/protobuftab.png)
3. 点击**Browse** ，选择**Proto**文件的文件夹
![ChooseFolder](http://gitlab.baidu.com/zhaoguowei/documents/raw/master/Protobuf2Fiddler/chooseproto.png)
4. 在MessageType 中可以看到文件夹中的Proto文件中定义的Message信息
2. 在Fiddler中选择一个需要解析的Session，在`Inspectors->Protobuf`中可以看到Raw解的结果
![rawDecode](http://gitlab.baidu.com/zhaoguowei/documents/raw/master/Protobuf2Fiddler/RawDecode.png)
3. 如果需要Key-Value解析，配置该URL对应的Message Type (如果有嵌套的Message Type，请选择根Message Type)，如果数据与Proto文件匹配即可解析出Key-Value结果。
![keyDecode](http://gitlab.baidu.com/zhaoguowei/documents/raw/master/Protobuf2Fiddler/keyDecode.png)
*配置的URL与Message Type的映射会自动保存。*
*配置完毕后需要重新选择需要解码的Session才能显示*

## 开发环境 ##

**需求：** .Net Framework 4 、Visual Studio 2010 +、Windows

## 项目说明 ##

* **[Protobuf-net](https://code.google.com/p/protobuf-net/):**一个基于.Net的Protobuf转换工具，用于Protobuf 的序列化和反序列化。在本项目中的用途是用于解析Proto文件，生成Proto文件中的Message的列表

* **ProtobufGen:Protobuf .Net**项目中带的模块，用于根据Proto文件生成对应的.Net 代码。本项目被修改，主要增加了直接根据Proto文件生成.Net DLL，可以动态在.Net 语言中加载用于带类型的解包(早期方案)，通过Protoc.exe进行Protobuf文件的Raw解,Key-Value解(现在方案)。已经加入扩展方法，支持根据字符串生成Protobuf。预备增加新方法可以直接根据Json生成Protobuf


* **Protobuf2Fiddler:**Fiddler的扩展，继承了Inspectors2、IRequestInspector和IResponseInspector，支持Protobuf的解包并且在Fiddler中展示 

