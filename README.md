#Protobuf to fiddler
一个基于Fiddler的Proto部分解包工具
##开发环境
需求： .Net Framework 4 、Visual Studio 2010 +

##项目说明

* [Protobuf-net](https://code.google.com/p/protobuf-net/):一个基于.Net的Protobuf转换工具，用于Protobuf 的序列化和反序列化。在本项目中的用途是用于解析Proto文件，生成Proto文件中的Message的列表

* ProtobufGen:Protobuf .Net项目中带的模块，用于根据Proto文件生成对应的.Net 代码。本项目被修改，主要增加了直接根据Proto文件生成.Net DLL，可以动态在.Net 语言中加载用于带类型的解包(早期方案)，通过Protoc.exe进行Protobuf文件的Raw解,Key-Value解(现在方案)。已经加入扩展方法，支持根据字符串生成Protobuf。预备增加新方法可以直接根据Json生成Protobuf


* Protobuf2Fiddler:Fiddler的扩展，继承了Inspectors2、IRequestInspector和IResponseInspector，支持Protobuf的解包并且在Fiddler中展示 

##使用说明
1. 下载安装Fiddler2 for .Net Framework4
2. 将项目使用Visual Studio 编译后，Protobuf2Fiddler的生成文件放置到 `Documents\Fiddler2\Inspectors`文件夹或者`Program Files\Fiddler2\Inspectors` 32bit系统 `Program Files (x86)\Fiddler2\Inspectors` 64bit系统
3. 启动Fiddler，选择一个Session，在Inspectors下的Protobuf中首先选择一个放置Protos的文件夹(如果Proto文件中有引用的Proto文件，请放在一个文件夹)
4. 在Fiddler中选择一个需要解析的Session，在Inspectors->Protobuf中可以看到Raw解的结果，如果需要Key-Value解析，请配置该URL对应的Message Type (如果有嵌套的Message Type，请选择根Message Type)，如果数据与Proto文件匹配即可解析出Key-Value结果