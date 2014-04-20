﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Xsl;
using google.protobuf;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics;

namespace ProtoBuf.CodeGenerator
{
    public sealed class CommandLineOptions
    {
        private TextWriter errorWriter = Console.Error;
        private string workingDirectory = Environment.CurrentDirectory;

        /// <summary>
        /// Root directory for the session
        /// </summary>
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set { workingDirectory = value; }
        }

        /// <summary>
        /// Nominates a writer for error messages (else stderr is used)
        /// </summary>
        public TextWriter ErrorWriter
        {
            get { return errorWriter; }
            set { errorWriter = value; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int Main(params string[] args)
        {
            CommandLineOptions opt = null;
            try
            {
                opt = Parse(Console.Out, args);
                opt.Execute();
                return opt.ShowHelp ? 1 : 0; // count help as a non-success (we didn't generate code)
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.Message);
                return 1;
            }
        }

        private string template = TemplateCSharp, outPath = "", defaultNamespace;
        private bool showLogo = true, showHelp, writeErrorsToFile;
        private readonly List<string> inPaths = new List<string>();
        private readonly List<string> args = new List<string>();

        private int messageCount;
        public int MessageCount { get { return messageCount; } }
        public bool WriteErrorsToFile { get { return writeErrorsToFile; } set { writeErrorsToFile = value; } }
        public string Template { get { return template; } set { template = value; } }
        public string DefaultNamespace { get { return defaultNamespace; } set { defaultNamespace = value; } }
        public bool ShowLogo { get { return showLogo; } set { showLogo = value; } }
        public string OutPath { get { return outPath; } set { outPath = value; } }
        public bool ShowHelp { get { return showHelp; } set { showHelp = value; } }
        private readonly XsltArgumentList xsltOptions = new XsltArgumentList();
        public XsltArgumentList XsltOptions { get { return xsltOptions; } }
        public List<string> InPaths { get { return inPaths; } }
        public List<string> Arguments { get { return args; } }

        private readonly TextWriter messageOutput;

        public static CommandLineOptions Parse(TextWriter messageOutput, params string[] args)
        {
            CommandLineOptions options = new CommandLineOptions(messageOutput);

            string key, value;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].Trim();
                if (arg.StartsWith("-o:"))
                {
                    if (!string.IsNullOrEmpty(options.OutPath)) options.ShowHelp = true;
                    options.OutPath = arg.Substring(3).Trim();
                }
                else if (arg.StartsWith("-p:"))
                {
                    Split(arg.Substring(3), out key, out value);
                    options.XsltOptions.AddParam(key, "", value ?? "true");
                }
                else if (arg.StartsWith("-t:"))
                {
                    options.Template = arg.Substring(3).Trim();
                }
                else if (arg.StartsWith("-ns:"))
                {
                    options.DefaultNamespace = arg.Substring(4).Trim();
                }
                else if (arg == "/?" || arg == "-h")
                {
                    options.ShowHelp = true;
                }
                else if (arg == "-q") // quiet
                {
                    options.ShowLogo = false;
                }
                else if (arg == "-d")
                {
                    options.Arguments.Add("--include_imports");
                }
                else if (arg.StartsWith("-i:"))
                {
                    options.InPaths.Add(arg.Substring(3).Trim());
                }
                else if (arg == "-writeErrors")
                {
                    options.WriteErrorsToFile = true;
                }
                else if (arg.StartsWith("-w:"))
                {
                    options.WorkingDirectory = arg.Substring(3).Trim();
                }
                else
                {
                    options.ShowHelp = true;
                }
            }
            if (options.InPaths.Count == 0)
            {
                options.ShowHelp = (string)options.XsltOptions.GetParam("help", "") != "true";
            }
            return options;

        }

        static readonly char[] SplitTokens = { '=' };
        private static void Split(string arg, out string key, out string value)
        {
            string[] parts = arg.Trim().Split(SplitTokens, 2);
            key = parts[0].Trim();
            value = parts.Length > 1 ? parts[1].Trim() : null;
        }

        public CommandLineOptions(TextWriter messageOutput)
        {
            if (messageOutput == null) throw new ArgumentNullException("messageOutput");
            this.messageOutput = messageOutput;

            // handling this (even trivially) suppresses the default write;
            // we'll also use it to track any messages that are generated
            XsltOptions.XsltMessageEncountered += delegate { messageCount++; };
        }

        public const string TemplateCSharp = "csharp";

        private string code;
        public string Code { get { return code; } private set { code = value; } }

    }
}
