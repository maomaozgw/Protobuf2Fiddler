﻿using System.Diagnostics;
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

        public static FileDescriptorSet LoadFilesAsFileDescription(params string[] files)
        {
            FileDescriptorSet set = new FileDescriptorSet();

            foreach (string inPath in files)
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
            if (proc != null)
            {
                BinaryWriter binaryWriter = new BinaryWriter(proc.StandardInput.BaseStream);
                binaryWriter.Write(data);
                binaryWriter.Flush();
                binaryWriter.Close();
                retval = proc.StandardOutput.ReadToEnd();
            }
            

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
            if (proc != null)
            {
                BinaryWriter binaryWriter = new BinaryWriter(proc.StandardInput.BaseStream);
                binaryWriter.Write(data);
                binaryWriter.Flush();
                binaryWriter.Close();
                retval = proc.StandardOutput.ReadToEnd();
            }
            

            return retval;
        }

        public static byte[] EncodeWithProto(string strProtobuf, string messageType, string protoFile)
        {
            byte[] retval = new byte[0];

            ProcessStartInfo procStartInfo = GetProtocStartInfo();
            procStartInfo.Arguments = string.Format(@"--encode={0} --proto_path={1} {2}", messageType, Path.GetDirectoryName(protoFile), protoFile);
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;

            //
            // write the decoded protobuf string to protoc for it to comiple into protobuf binary format.
            //
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(procStartInfo);
            if (proc != null)
            {
                StreamWriter streamWriter = new StreamWriter(proc.StandardInput.BaseStream);
                streamWriter.Write(strProtobuf);
                streamWriter.Flush();
                streamWriter.Close(); BinaryReader binaryReader = new BinaryReader(proc.StandardOutput.BaseStream);
                byte[] buf = new byte[4096];
                while (true)
                {
                    int protoBufBytesRead = binaryReader.Read(buf, 0, 4096);

                    if (protoBufBytesRead > 0)
                    {
                        Array.Resize(ref retval, retval.Length + protoBufBytesRead);
                        Array.Copy(buf, 0, retval, retval.Length - protoBufBytesRead, protoBufBytesRead);

                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Now, read off it's standard output for the binary stream.

           
            return retval;
        }

        private static ProcessStartInfo GetProtocStartInfo()
        {
            string workingDirectory;
            string protoFile = InputFileLoader.GetProtocPath(out workingDirectory);
            ProcessStartInfo procStartInfo = new ProcessStartInfo
            {
                WorkingDirectory = workingDirectory,
                FileName = protoFile,
                CreateNoWindow = true
            };
            return procStartInfo;
        }
    }
}
