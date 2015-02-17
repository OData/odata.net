//---------------------------------------------------------------------
// <copyright file="ConstructedInterfaces.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace System.Data.Test.Astoria
{
    public class ConstructedInterface
    {
        private ConstructedFile _file;

        public ConstructedInterface(ConstructedFile file)
        {
            _file = file;
            this.Implemented = true;
        }

        public bool Implemented
        {
            get;
            set;
        }

        public ConstructedFile ImplementingFile
        {
            get { return _file; }
            set { _file = value; }
        }
    }

    public class IUpdatableInterface : ConstructedInterface
    {
        private string _resetResource;
        private string _createResource;
        private string _getResource;
        private string _setValue;
        private string _getValue;
        private string _setReference;
        private string _addReferenceToCollection;
        private string _removeReferenceFromCollection;
        private string _deleteResource;
        private string _saveChanges;
        private string _resolveResource;
        private string _clearChanges;

        public IUpdatableInterface()
            : base(null)
        { }

        public IUpdatableInterface(ConstructedFile file)
            : base(file)
        { }

        public string ResetResource
        {
            get { return _resetResource; }
            set
            {
                _resetResource = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public object ResetResource(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("ResetResource", methodSig);
            }
        }

        public string CreateResource
        {
            get { return _createResource; }
            set
            {
                _createResource = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public object CreateResource(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("CreateResource", methodSig);
            }
        }

        public string GetResource
        {
            get { return _getResource; }
            set
            {
                _getResource = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public object GetResource(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetResource", methodSig);
            }
        }

        public string SetValue
        {
            get { return _setValue; }
            set
            {
                _setValue = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public void SetValue(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("SetValue", methodSig);
            }
        }

        public string GetValue
        {
            get { return _getValue; }
            set
            {
                _getValue = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public object GetValue(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetValue", methodSig);
            }
        }

        public string SetReference
        {
            get { return _setReference; }
            set
            {
                _setReference = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public void SetReference(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("SetReference", methodSig);
            }
        }

        public string AddReferenceToCollection
        {
            get { return _addReferenceToCollection; }
            set
            {
                _addReferenceToCollection = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public void AddReferenceToCollection(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("AddReferenceToCollection", methodSig);
            }
        }

        public string RemoveReferenceFromCollection
        {
            get { return _removeReferenceFromCollection; }
            set
            {
                _removeReferenceFromCollection = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public void RemoveReferenceFromCollection(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("RemoveReferenceFromCollection", methodSig);
            }
        }

        public string DeleteResource
        {
            get { return _deleteResource; }
            set
            {
                _deleteResource = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public void DeleteResource(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("DeleteResource", methodSig);
            }
        }

        public string SaveChanges
        {
            get { return _saveChanges; }
            set
            {
                _saveChanges = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public void SaveChanges(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("SaveChanges", methodSig);
            }
        }

        public string ResolveResource
        {
            get { return _resolveResource; }
            set
            {
                _resolveResource = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public object ResolveResource(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("ResolveResource", methodSig);
            }
        }

        public string ClearChanges
        {
            get { return _clearChanges; }
            set
            {
                _clearChanges = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public void ClearChanges(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("ClearChanges", methodSig);
            }
        }
    }

    public class IDataServiceQueryProviderInterface : ConstructedInterface
    {
        private string _currentDataSource;
        private string _nullPropagationRequired;
        private string _getQueryRootForResourceSet;
        private string _getResourceType;
        private string _getPropertyOpenValue;
        private string _getPropertyValue;
        private string _getOpenPropertyValues;
        private string _invokeServiceOperation;

        public IDataServiceQueryProviderInterface()
            : base(null)
        { }

        public IDataServiceQueryProviderInterface(ConstructedFile file)
            : base(file)
        { }

        public string CurrentDataSource
        {
            get { return _currentDataSource; }
            set
            {
                _currentDataSource = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public object CurrentDataSource";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("CurrentDataSource", methodSig);
            }
        }

        public string InvokeServiceOperation
        {
            get { return _invokeServiceOperation; }
            set
            {
                _invokeServiceOperation = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public object InvokeServiceOperation(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("InvokeServiceOperation", methodSig);
            }
        }

        public string IsNullPropagationRequired
        {
            get { return _nullPropagationRequired; }
            set
            {
                _nullPropagationRequired = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public bool IsNullPropagationRequired";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("IsNullPropagationRequired", methodSig);
            }
        }

        public string GetQueryRootForResourceSet
        {
            get { return _getQueryRootForResourceSet; }
            set
            {
                _getQueryRootForResourceSet = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public IQueryable GetQueryRootForResourceSet(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetQueryRootForResourceSet", methodSig);
            }
        }

        public string GetResourceType
        {
            get { return _getResourceType; }
            set
            {
                _getResourceType = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public DSP.ResourceType GetResourceType(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetResourceType", methodSig);
            }
        }

        public string GetOpenPropertyValue
        {
            get { return _getPropertyOpenValue; }
            set
            {
                _getPropertyOpenValue = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public object GetOpenPropertyValue(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetPropertyValue_ResourceProperty", methodSig);
            }
        }

        public string GetPropertyValue
        {
            get { return _getPropertyValue; }
            set
            {
                _getPropertyValue = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public object GetPropertyValue(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetPropertyValue", methodSig);
            }
        }

        public string GetOpenPropertyValues
        {
            get { return _getOpenPropertyValues; }
            set
            {
                _getOpenPropertyValues = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetOpenPropertyValues", methodSig);
            }
        }
    }

    public class IDataServiceMetadataProviderInterface : ConstructedInterface
    {
        private string _containerNamespace;
        private string _containerName;
        private string _resourceSets;
        private string _types;
        private string _serviceOperations;
        private string _tryResolveResourceSet;
        private string _getSetForResourceType;
        private string _getDerivedTypes;
        private string _hasDerivedTypes;
        private string _tryResolveResourceType;
        private string _tryResolveServiceOperation;
        private string _getResourceAssociationSet;

        public IDataServiceMetadataProviderInterface()
            : base(null)
        { }

        public IDataServiceMetadataProviderInterface(ConstructedFile file)
            : base(file)
        { }

        public string ContainerNamespace
        {
            get { return _containerNamespace; }
            set
            {
                _containerNamespace = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public string ContainerNamespace";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("ContainerNamespace", methodSig);
            }
        }

        public string ContainerName
        {
            get { return _containerName; }
            set
            {
                _containerName = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public string ContainerName";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("ContainerName", methodSig);
            }
        }

        public string ResourceSets
        {
            get { return _resourceSets; }
            set
            {
                _resourceSets = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public IEnumerable<DSP.ResourceSet> ResourceSets";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("ResourceSets", methodSig);
            }
        }

        public string Types
        {
            get { return _types; }
            set
            {
                _types = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public IEnumerable<DSP.ResourceType> Types";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("Types", methodSig);
            }
        }

        public string ServiceOperations
        {
            get { return _serviceOperations; }
            set
            {
                _serviceOperations = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public IEnumerable<DSP.ServiceOperation> ServiceOperations";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("ServiceOperations", methodSig);
            }
        }

        public string TryResolveResourceSet
        {
            get { return _tryResolveResourceSet; }
            set
            {
                _tryResolveResourceSet = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public bool TryResolveResourceSet(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("TryResolveResourceSet", methodSig);
            }
        }

        public string GetSetForResourceType
        {
            get { return _getSetForResourceType; }
            set
            {
                _getSetForResourceType = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public DSP.ResourceSet GetSetForResourceType(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetSetForResourceType", methodSig);
            }
        }

        public string TryResolveResourceType
        {
            get { return _tryResolveResourceType; }
            set
            {
                _tryResolveResourceType = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public bool TryResolveResourceType(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("TryResolveResourceType", methodSig);
            }
        }

        public string GetDerivedTypes
        {
            get { return _getDerivedTypes; }
            set
            {
                _getDerivedTypes = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public IEnumerable<DSP.ResourceType> GetDerivedTypes(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetDerivedTypes", methodSig);
            }
        }

        public string HasDerivedTypes
        {
            get { return _hasDerivedTypes; }
            set
            {
                _hasDerivedTypes = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public bool HasDerivedTypes(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("HasDerivedTypes", methodSig);
            }
        }

        public string TryResolveServiceOperation
        {
            get { return _tryResolveServiceOperation; }
            set
            {
                _tryResolveServiceOperation = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public bool TryResolveServiceOperation(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("TryResolveServiceOperation", methodSig);
            }
        }

        public string GetResourceAssociationSet
        {
            get { return _getResourceAssociationSet; }
            set
            {
                _getResourceAssociationSet = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public DSP.ResourceAssociationSet GetResourceAssociationSet(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("GetResourceAssociationSet", methodSig);
            }
        }
    }

    public class IDataServiceUpdateProviderInterface : ConstructedInterface
    {
        private string _setConcurrencyValues;

        public IDataServiceUpdateProviderInterface()
            : base(null)
        { }

        public IDataServiceUpdateProviderInterface(ConstructedFile file)
            : base(file)
        { }

        public string SetConcurrencyValues
        {
            get { return _setConcurrencyValues; }
            set
            {
                _setConcurrencyValues = value;

                NewMethodInfo methodSig = new NewMethodInfo();
                methodSig.MethodSignature = "public void SetConcurrencyValues(";
                methodSig.BodyText = value;

                base.ImplementingFile.AddMethod("SetConcurrencyValues", methodSig);
            }
        }
    }

    public class IServiceProviderInterface
    {
        public IServiceProviderInterface()
        {
            Services = new Dictionary<Type, string>();
        }

        public Dictionary<Type, string> Services
        {
            get;
            private set;
        }

        public string GetMethodDeclaration()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("      public object GetService(Type serviceType)");
            builder.AppendLine("      {");
            builder.AppendLine("          APICallLog.Current.ServiceProvider.GetService(serviceType);");
            builder.AppendLine("          try");
            builder.AppendLine("          {");
            foreach (var pair in Services)
            {
                builder.AppendLine("                if(serviceType == typeof(" + pair.Key.FullName + "))");
                builder.AppendLine("                    return " + pair.Value + ";");
            }
            builder.AppendLine("              return null;");
            builder.AppendLine("          }");
            builder.AppendLine("          finally");
            builder.AppendLine("          {");
            builder.AppendLine("              APICallLog.Current.Pop();");
            builder.AppendLine("          }");
            builder.AppendLine("      }");

            return builder.ToString();
        }
    }
}