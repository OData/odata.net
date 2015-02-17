//---------------------------------------------------------------------
// <copyright file="ConstructedFile.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria
{
    public struct NewMethodInfo
    {
        public string MethodSignature;
        public string BodyText;
    }

    public class ConstructedFile
    {
        private Dictionary<string, NewMethodInfo> _methods;

        public ConstructedFile(string fileName, string destinationFolder)
        {
            this.FileName = fileName;
            this.DestinationFolder = destinationFolder;
            
            _methods = new Dictionary<string, NewMethodInfo>();
        }

        public ConstructedFile(string fileName)
            : this(fileName, null)
        { }

        public string DestinationFolder
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public void AddMethod(string name, NewMethodInfo method)
        {
            if (_methods.Keys.Contains(name))
                _methods[name] = method;
            else
                _methods.Add(name, method);
        }

        public void ApplyChanges()
        {
            string[] files = Directory.GetFiles(this.DestinationFolder, "*" + this.FileName, SearchOption.AllDirectories);

            if (files.Count() > 0)
            {
                string filePath = files.FirstOrDefault();
                string fileText = File.ReadAllText(filePath);

                foreach (string methodName in _methods.Keys)
                {
                    NewMethodInfo methodSig = _methods[methodName];

                    int startPos = fileText.IndexOf(methodSig.MethodSignature);
                    if (startPos == -1)
                        throw new TestFailedException(String.Format("Could not find method body to replace. file: {0}, method: {1}", this.FileName, methodName));

                    startPos = fileText.IndexOf("{", startPos + 1) + 1;

                    int endPos = fileText.IndexOf("// " + methodName);
                    if( endPos == -1 )
                        endPos = fileText.IndexOf("//" + methodName); // try without space

                    if (endPos == -1)
                        throw new TestFailedException(String.Format("Could not find method body to replace. file: {0}, method: {1}", this.FileName, methodName));

                    endPos = fileText.LastIndexOf("}", endPos, endPos - startPos) - 1;

                    fileText = fileText.Substring(0, startPos) + "\r\n" + methodSig.BodyText + "\r\n" + fileText.Substring(endPos);
                }

                File.WriteAllText(filePath, fileText);
            }
        }
    }

    public class InterfaceImplementations
    {
        private IUpdatableInterface iupdatableInterface;
        private IDataServiceQueryProviderInterface idataServiceQueryProvider;
        private IDataServiceMetadataProviderInterface idataServiceMetadataProvider;
        private IDataServiceUpdateProviderInterface iupdateProvider;
        private IServiceProviderInterface iserviceProvider;

        public InterfaceImplementations()
        {
        }

        public IUpdatableInterface IUpdatable
        {
            get 
            { 
                if( iupdatableInterface == null )
                    iupdatableInterface = new IUpdatableInterface();

                return iupdatableInterface;  
            }
        }

        public IDataServiceUpdateProviderInterface IDataServiceUpdateProvider
        {
            get
            {
                if (iupdateProvider == null)
                    iupdateProvider = new IDataServiceUpdateProviderInterface();

                return iupdateProvider;
            }
        }

        public IDataServiceQueryProviderInterface IDataServiceQueryProvider
        {
            get
            {
                if (idataServiceQueryProvider == null)
                    idataServiceQueryProvider = new IDataServiceQueryProviderInterface();

                return idataServiceQueryProvider;
            }
        }

        public IDataServiceMetadataProviderInterface IDataServiceMetadataProvider
        {
            get
            {
                if (idataServiceMetadataProvider == null)
                    idataServiceMetadataProvider = new IDataServiceMetadataProviderInterface();

                return idataServiceMetadataProvider;
            }
        }

        public IServiceProviderInterface IServiceProvider
        {
            get
            {
                if (iserviceProvider == null)
                    iserviceProvider = new IServiceProviderInterface();

                return iserviceProvider;
            }
        }
    }

    
    public class ServiceModifications
    {
        private InterfaceImplementations _interfaceImplementations;
        private List<ConstructedFile> _files;

        public ServiceModifications(Workspace workspace)
        {
            _interfaceImplementations = new InterfaceImplementations();
            _files = new List<ConstructedFile>();

            this.Workspace = workspace;
        }

        public List<ConstructedFile> Files
        {
            get { return _files; }
        }

        public InterfaceImplementations Interfaces
        {
            get { return _interfaceImplementations; }
        }

        public Workspace Workspace
        {
            get; set;
        }

        public void ReplaceMethod(string fileName, string methodName, NewMethodInfo newMethodInfo)
        {
            ConstructedFile file;

            if (_files.Where(cf => cf.FileName == fileName).Count() != 0)
                file = _files.Where(cf => cf.FileName == fileName).First();
            else
                file = new ConstructedFile(fileName);

            file.AddMethod(methodName, newMethodInfo);
        }

        public void ApplyChanges(string destinationFolder)
        {
            foreach (ConstructedFile file in _files)
            {
                file.DestinationFolder = destinationFolder;
                file.ApplyChanges();
            }
        }
    }
}
