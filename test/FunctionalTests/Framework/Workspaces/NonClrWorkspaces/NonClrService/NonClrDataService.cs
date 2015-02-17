//---------------------------------------------------------------------
// <copyright file="NonClrDataService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria.NonClr
{
    using System.Linq;
    using System.ServiceModel.Web;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria.NonClr;
    using Microsoft.OData.Service.Providers;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Data.Test.Astoria.LateBound;
    using System.Data.Test.Astoria.CallOrder;

    public abstract class NonClrTestWebService<T> : TestDataWebService<T> where T : NonClrContext
    {
        protected T ContextInstance;

        protected NonClrTestWebService()
        {
            ContextInstance = (T)Activator.CreateInstance(typeof(T), this);
        }

        protected override T CreateDataSource()
        {
            APICallLog.Current.DataService.CreateDataSource(this);
            try
            {
                return ContextInstance;
            }
            finally
            {
                APICallLog.Current.Pop();   
            }
        }

        public override IQueryable<EntityType> GetEntitySet<EntityType>(string entitySetName)
        {
            IDataServiceMetadataProvider idsmp = this.CurrentDataSource;
            ResourceSet set;
             if(!idsmp.TryResolveResourceSet(entitySetName, out set))
                 throw new DataServiceException("Could not find resource set with name '" + entitySetName + "'");

             IDataServiceQueryProvider idsqp = this.CurrentDataSource;
             return idsqp.GetQueryRootForResourceSet(set).OfType<EntityType>();
        }
 	
        [WebGet]
        public void RestoreData()
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            context.RestoreData();
        }

        [WebGet]
        public void ResourceTypeComplete(string name)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            context.ResourceTypeComplete(name);
        }

        [WebGet]
        public void AddResourceType(string name, string typeName, string resourceTypeKind, bool isOpenType, string baseType, string namespaceName, bool isAbstract)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            context.AddResourceType(name, typeName, resourceTypeKind, isOpenType, baseType, namespaceName, isAbstract);
        }

        [WebGet]
        public void RemoveResourceType(string name)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            context.RemoveResourceType(name);
        }

        [WebGet]
        public void AddResourceSet(string name, string typeName)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            context.AddResourceSet(name, typeName);
        }

        [WebGet]
        public void RemoveResourceContainer(string name)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            context.RemoveResourceSet(name);
        }

        [WebGet]
        public void AddResourceProperty(string propertyName, string addToResourceType, string resourcePropertyKind, 
                                        string propertyType, string containerName, bool isClrProperty)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            context.AddResourceProperty(propertyName, addToResourceType, resourcePropertyKind, propertyType, containerName, isClrProperty);
        }

        [WebGet]
        public void AddServiceOperation(string name, string serviceOperationResultKind, string typeName)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;

            ServiceOperationResultKind operationResult = context.ConvertServiceOperationResultKind(serviceOperationResultKind);
            context.AddServiceOperation(name, operationResult, typeName, "GET");
        }

        [WebGet]
        public void RemoveServiceOperation(string name)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            context.RemoveServiceOperation(name);
        }

        [WebGet]
        public void ResetMetadata(string name)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            context.ResetMetadata();
        }

        [WebGet]
        public void SetOpenTypeMethodsImplementation(string name, bool lazyEvaluation)
        {
            OpenTypeMethodsImplementations value = (OpenTypeMethodsImplementations)Enum.Parse(typeof(OpenTypeMethodsImplementations), name);
            LateBoundToClrConverter.OpenTypeMethodsImplementation = OpenTypeMethodsImplementation.GetImplementation(value, lazyEvaluation);
        }

        [WebGet]
        public void SetIsOpenType(string resourceTypeName, bool value)
        {
            NonClrContext context = (NonClrContext)this.CurrentDataSource;
            ResourceType type;

            CallOrder.APICallLog.Current.Push();
            try
            {
                if (!context.TryResolveResourceType(resourceTypeName, out type))
                    throw new DataServiceException(500, "Could not find type '" + resourceTypeName + "'");
            }
            finally
            {
                CallOrder.APICallLog.Current.Pop();
            }

            if (!type.IsReadOnly)
                type.IsOpenType = value;
            else
            {
                FieldInfo field = type.GetType().GetField("isOpenType", BindingFlags.NonPublic | BindingFlags.Instance);
                field.SetValue(type, value);
            }
        }
    }
}
