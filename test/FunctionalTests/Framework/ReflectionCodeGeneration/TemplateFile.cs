//---------------------------------------------------------------------
// <copyright file="TemplateFile.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Test.Astoria;
using System.Reflection;

namespace System.Data.Test.Astoria.ReflectionProvider
{
    public class TemplateFile
    {
        private Dictionary<string, string> _tokens;
        private string _fileText;
        private string _destinationPath;

        public Action<TemplateFile> ReplaceCustomTokens;

        public TemplateFile(string resourceName, string destinationPath)
        {
            _tokens = new Dictionary<string, string>();
            _destinationPath = destinationPath;

            Assembly testFrameworkAssembly = typeof(Workspace).Assembly;
            _fileText = IOUtil.ReadResourceText(testFrameworkAssembly, resourceName);
        }

        public void AddDefaultWorkspaceTokens(Workspace workspace)
        {
            _tokens.Add("[[ContextTypeName]]", workspace.ContextTypeName);
            _tokens.Add("[[ContextNamespace]]", workspace.ContextNamespace);
            _tokens.Add("[[WorkspaceName]]", workspace.Name);

            switch (AstoriaTestProperties.Host)
            {
                case Host.WebServiceHost:
                case Host.WebServiceHostRemote:
                case Host.IDSH:
                case Host.IDSH2:
                    _tokens.Add("[[ServiceClassName]]", "TestServiceHost." + workspace.ServiceClassName);
                    break;

                default:
                    _tokens.Add("[[ServiceClassName]]", workspace.ServiceClassName);
                    break;
            }
        }

        public string FileText
        {
            get { return _fileText; }
            set { _fileText = value; }
        }

        public Dictionary<string, string> Tokens
        {
            get { return _tokens; }
        }

        public void Build()
        {
            this.ReplaceCustomTokens(this);

            foreach (string token in _tokens.Keys)
                _fileText = _fileText.Replace(token, _tokens[token]);

            using (StreamWriter writer = new StreamWriter(_destinationPath))
            {
                writer.Write(_fileText);
            }
        }
    }

    
}
