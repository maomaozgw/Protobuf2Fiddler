using System.Diagnostics;
using google.protobuf;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;



namespace ProtoBuf.CodeGenerator
{
    public class ProtoTransformer
    {

        public static bool Transform(List<string> fileList, string outPutFilePath)
        {
            StringBuilder errors = new StringBuilder();

            try
            {
                string xml = LoadFilesAsXml(fileList);
                string code = ApplyTransform(new XsltArgumentList(), xml);
                CSharpCodeProvider objProvider = new CSharpCodeProvider();
                ICodeCompiler objCompiler = objProvider.CreateCompiler();

                CompilerParameters objCompilerParameters = new CompilerParameters();
                objCompilerParameters.ReferencedAssemblies.Add("System.dll");
                objCompilerParameters.ReferencedAssemblies.Add("protobuf-net.dll");
                objCompilerParameters.GenerateExecutable = false;
                objCompilerParameters.GenerateInMemory = false;
                objCompilerParameters.OutputAssembly = Path.GetFileName(outPutFilePath);
                CompilerResults result = objCompiler.CompileAssemblyFromSource(objCompilerParameters, code);
                if (result.Errors.HasErrors)
                {
                    Console.WriteLine("编译错误：");
                    foreach (CompilerError err in result.Errors)
                    {
                        Console.WriteLine(err.ErrorText);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }


        private static string LoadFilesAsXml(List<string> fileList)
        {
            FileDescriptorSet set = new FileDescriptorSet();

            foreach (string inPath in fileList)
            {
                InputFileLoader.Merge(set, inPath, Console.Error);
            }

            XmlSerializer xser = new XmlSerializer(typeof(FileDescriptorSet));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineHandling = NewLineHandling.Entitize;
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                xser.Serialize(writer, set);
            }
            return sb.ToString();
        }

        public static FileDescriptorSet LoadFilesAsFileDescription(List<string> fileList)
        {
            FileDescriptorSet set = new FileDescriptorSet();

            foreach (string inPath in fileList)
            {
                InputFileLoader.Merge(set, inPath, Console.Error);
            }
            return set;
        }

        private static string ApplyTransform(XsltArgumentList xsltArgs, string xml)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.CheckCharacters = false;

            StringBuilder sb = new StringBuilder();
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            using (TextWriter writer = new StringWriter(sb))
            {
                XslCompiledTransform xslt = new XslCompiledTransform();
                string xsltTemplate = "csharp.xslt";
                if (!File.Exists(xsltTemplate))
                {
                    string localXslt = InputFileLoader.CombinePathFromAppRoot(xsltTemplate);
                    if (File.Exists(localXslt))
                        xsltTemplate = localXslt;
                }
                try
                {
                    xslt.Load(xsltTemplate);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Unable to load tranform: " + "CSharp.xslt", ex);
                }

                xsltArgs.AddParam("defaultNamespace", "", "FiddlerProtos");

                xslt.Transform(reader, xsltArgs, writer);
            }
            return sb.ToString();
        }

        public static string DecodeRaw(byte[] data)
        {
            string retval = string.Empty;
            ProcessStartInfo procStartInfo = GetProtocStartInfo();
            procStartInfo.Arguments = @"--decode_raw";
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;

            Process proc = Process.Start(procStartInfo);

            // proc.StandardInput.BaseStream.Write(protobufBytes, 0, protobufBytes.Length);
            BinaryWriter binaryWriter = new BinaryWriter(proc.StandardInput.BaseStream);
            binaryWriter.Write(data);
            binaryWriter.Flush();
            binaryWriter.Close();
            retval = proc.StandardOutput.ReadToEnd();

            return retval;
        }

        public static string DecodeWithProtoFile(byte[] data, string protoFilePath, string messageType)
        {
            string retval = string.Empty;

            ProcessStartInfo procStartInfo = GetProtocStartInfo();
            procStartInfo.Arguments = string.Format(@"--decode={0} --proto_path={1} {2}", messageType, Path.GetDirectoryName(protoFilePath), protoFilePath);
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;

            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(procStartInfo);
            Debug.WriteLine(procStartInfo.Arguments);
            // proc.StandardInput.BaseStream.Write(protobufBytes, 0, protobufBytes.Length);
            BinaryWriter binaryWriter = new BinaryWriter(proc.StandardInput.BaseStream);
            binaryWriter.Write(data);
            binaryWriter.Flush();
            binaryWriter.Close();
            retval = proc.StandardOutput.ReadToEnd();

            return retval;
        }

        private static ProcessStartInfo GetProtocStartInfo()
        {
            string workingDirectory = string.Empty;
            string protoFile = InputFileLoader.GetProtocPath(out workingDirectory);
            ProcessStartInfo procStartInfo = new ProcessStartInfo();
            procStartInfo.WorkingDirectory = workingDirectory;
            procStartInfo.FileName = protoFile;
            return procStartInfo;
        }
    }
}
