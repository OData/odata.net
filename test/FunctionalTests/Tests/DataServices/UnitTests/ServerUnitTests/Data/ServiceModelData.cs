//---------------------------------------------------------------------
// <copyright file="ServiceModelData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Data.Test.Astoria.TestExecutionLayer;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Edm;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using AstoriaUnitTests.Tests;
    using System.Xml;

    #endregion Namespaces

    /// <summary>Provides information about interesting service models.</summary>
    [DebuggerVisualizer("ServiceModelData: {serviceModelType}")]
    public sealed class ServiceModelData
    {
        #region Private fields.

        /// <summary>Custom data, reflection based.</summary>
        private static ServiceModelData customDataValue;

        /// <summary>Custom row based context.</summary>
        private static ServiceModelData customRowBasedValue;

        /// <summary>Custom object context with friendly feeds.</summary>
        private static ServiceModelData epmCustomObjectContextValue;

        /// <summary>ObjectContext-based Northwind model.</summary>
        private static ServiceModelData northwindValue;

        /// <summary>Interesting service models for testing.</summary>
        private static ServiceModelData[] values;

        /// <summary> model which has all the metadata</summary>
        private IEdmModel model;

        /// <summary>Top-level container names.</summary>
        private List<string> containerNames;

        /// <summary>Whether this is a valid model (can be loaded and queries successfully).</summary>
        private bool isValid;

        /// <summary>Type being encapsulated.</summary>
        private Type serviceModelType;

        /// <summary>The Web3S prefix used for test-side XPath expressions.</summary>
        private string web3STestPrefix;

        #endregion Private fields.

        #region Constructors.

        /// <summary>Hidden constructor.</summary>
        private ServiceModelData()
        {
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>Gets a valid service model.</summary>
        public static ServiceModelData AnyValue
        {
            get
            {
                return Values[1];
            }
        }

        /// <summary>Interesting valid values service models for testing.</summary>
        public static ServiceModelData[] ValidValues
        {
            get { return Values; }
        }

        /// <summary>Interesting service models for testing.</summary>
        public static ServiceModelData[] Values
        {
            get
            {
                if (values == null)
                {
                    values = new ServiceModelData[] {
                        Northwind,
                        CustomData,
                        CustomRowBased,
                        ForType(typeof(CustomRowBasedOpenTypesContext)),
                        ForType(typeof(AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext)),
                    };
                }

                return values;
            }
        }

        public static ServiceModelData Northwind
        {
            get 
            {
                if (northwindValue == null)
                {
                    northwindValue = ForType(typeof(NorthwindModel.NorthwindContext));
                }

                return northwindValue;
            }
        }

        public static ServiceModelData CustomData
        {
            get 
            {
                if (customDataValue == null)
                {
                    customDataValue = ForType(typeof(CustomDataContext));
                }

                return customDataValue;
            }
        }

        public static ServiceModelData CustomRowBased
        {
            get 
            {
                if (customRowBasedValue == null)
                {
                    customRowBasedValue = ForType(typeof(CustomRowBasedContext));
                }

                return customRowBasedValue;
            }
        }

        public static ServiceModelData CustomRowBasedOpenType
        {
            get { return Values.Single(v => v.ServiceModelType == typeof(CustomRowBasedOpenTypesContext)); }
        }

        public static ServiceModelData SqlNorthwindData
        {
            get { return Values.Single(v => v.ServiceModelType == typeof(AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext)); }
        }

        public static ServiceModelData EpmCustomObjectContext
        {
            get
            {
                if (epmCustomObjectContextValue == null)
                {
                    epmCustomObjectContextValue = ForType(typeof(AstoriaUnitTests.ObjectContextStubs.EpmCustomObjectContext));
                }

                return epmCustomObjectContextValue;
            }
        }

        #endregion Public properties.

        /// <summary>Initializes and returns the context for the specified service.</summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static object InitializeAndGetContext(object service)
        {
            TestUtil.CheckArgumentNotNull(service, "service");
            Type type = service.GetType();
            Type lookupType = type;
            MethodInfo ensureProviderForRequest = null;
            while (lookupType != null && ensureProviderForRequest == null)
            {
                ensureProviderForRequest = lookupType.GetMethod("EnsureProviderAndConfigForRequest", BindingFlags.NonPublic | BindingFlags.Instance);
                lookupType = lookupType.BaseType;
            }

            if (ensureProviderForRequest == null)
            {
                throw new InvalidOperationException("Unable to find method EnsureProviderAndConfigForRequest on " + type);
            }
            
            ensureProviderForRequest.Invoke(service, new object[0]);

            PropertyInfo dataSource = type.GetProperty("CurrentDataSource", BindingFlags.Instance | BindingFlags.NonPublic);
            if (dataSource == null)
            {
                throw new InvalidOperationException("Unable to find property CurrentDataSource on " + type);
            }

            return dataSource.GetValue(service, new object[0]);
        }

        public static T InitializeAndGetContext<T>(Microsoft.OData.Service.DataService<T> service)
        {
            return (T)InitializeAndGetContext((object)service);
        }

        public static object CreateProvider(object contextInstance, object dataServiceInstance)
        {
            TestUtil.CheckArgumentNotNull(contextInstance, "contextInstance");
            Assembly assembly = typeof(Microsoft.OData.Service.IDataServiceHost).Assembly;

            IServiceProvider isp = dataServiceInstance as IServiceProvider;
            if (isp != null)
            {
                object provider = isp.GetService(typeof(Microsoft.OData.Service.Providers.IDataServiceMetadataProvider));
                if(provider != null)
                {
                    return provider;
                }
            }

            Type providerType;
            Type type = contextInstance.GetType();
            if (typeof(ObjectContext).IsAssignableFrom(type))
            {
                providerType = assembly.GetType("Microsoft.OData.Service.Providers.EntityFrameworkDataServiceProvider", true);
            }
            else
            {
                providerType = assembly.GetType("Microsoft.OData.Service.Providers.ReflectionDataServiceProvider", true);
            }

            ConstructorInfo ctor = providerType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null, new Type[] { typeof(object), typeof(object) }, null);
            object result = ctor.Invoke(new object[] { dataServiceInstance, contextInstance });

            return result;
        }

        /// <summary>Top-level container names.</summary>
        public List<string> ContainerNames
        {
            get
            {
                return this.containerNames;
            }
        }

        /// <summary>Whether this service is known to have friendly feeds.</summary>
        public bool HasFriendlyFeeds
        {
            get
            {
                return this == epmCustomObjectContextValue || this == customDataValue;
            }
        }

        public bool HasOpenTypes
        {
            get
            {
                return this.model.SchemaElements.OfType<IEdmStructuredType>().Any(t => t.IsOpen);
            }
        }

        /// <summary>Whether this is a valid model (can be loaded and queried successfully).</summary>
        public bool IsValid
        {
            get
            {
                return this.isValid;
            }
        }

        /// <summary>Whether the data model uses the ObjectContext (EDM) model.</summary>
        public bool IsObjectContextBased
        {
            get
            {
                return typeof(ObjectContext).IsAssignableFrom(this.serviceModelType);
            }
        }

        public bool IsUnitTestProvider
        {
            get
            {
                return (typeof(CustomDataContext).IsAssignableFrom(this.serviceModelType) ||
                        typeof(CustomRowBasedContext).IsAssignableFrom(this.serviceModelType) ||
                        typeof(CustomRowBasedOpenTypesContext).IsAssignableFrom(this.serviceModelType));
            }
        }

        /// <summary>Gets a sample container name.</summary>
        public string SampleContainer
        {
            get
            {
                if (this.ServiceModelType == typeof(CustomDataContext) ||
                    this.ServiceModelType == typeof(NorthwindModel.NorthwindContext))
                {
                    return "Customers";
                }
                else
                {
                    return this.containerNames[0];
                }
            }
        }

        /// <summary>Whether updates are supported by this model.</summary>
        public bool SupportsUpdates
        {
            get
            {
                return
                    typeof(IUpdatable).IsAssignableFrom(this.ServiceModelType) ||
                    typeof(ObjectContext).IsAssignableFrom(this.ServiceModelType);
            }
        }

        /// <summary>The Web3S prefix used for test-side XPath expressions.</summary>
        public string Web3STestPrefix
        {
            get
            {
                return this.web3STestPrefix;
            }
        }

        /// <summary>Type being encapsulated.</summary>
        public Type ServiceModelType
        {
            get
            {
                return this.serviceModelType;
            }
        }

        /// <summary>Provides a string representation for this object.</summary>
        /// <returns>A string representation for this object.</returns>
        public override string ToString()
        {
            return this.serviceModelType.ToString();
        }

        /// <summary>Ensures everything that the model depends on is accessible.</summary>
        /// <remarks>
        /// This method is called during initialization and populating some of the static 
        /// properties, so it shouldn't rely on any static properties from this type and
        /// can only assume the serviceModelType field has been initialized.
        /// </remarks>
        public void EnsureDependenciesAvailable()
        {
            System.Diagnostics.Debug.Assert(
               this.serviceModelType != null,
               "this.serviceModelType != null -- otherwise EnsureDependenciesAvailable is being called too soon");
            if (this.serviceModelType == typeof(NorthwindModel.NorthwindContext))
            {
                if (NorthwindModel.NorthwindContext.ContextConnectionString == null ||
                    NorthwindModel.NorthwindContext.ContextConnectionString.StartsWith("name"))
                {
                    SetupNorthwindModelDatabase();
                }
            }
            else if (this.serviceModelType == typeof(AstoriaUnitTests.ObjectContextStubs.EpmCustomObjectContext))
            {
                AstoriaUnitTests.ObjectContextStubs.PopulateData.CreateTableAndPopulateDataForEpm();
            }
            else if (this.serviceModelType == typeof(AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext))
            {
                if (AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext.DefaultConnectionString == null)
                {
                    SetupNorthwindModelDatabase();
                }
            }
        }

        /// <summary>Gets the IQueryable container for the specified container name.</summary>
        /// <param name="modelInstance">Model instance, of ServiceModelType type.</param>
        /// <param name="containerName">Name of container to return.</param>
        /// <returns></returns>
        public IQueryable GetContainerQueryable(object modelInstance, string containerName)
        {
            TestUtil.CheckArgumentNotNull(modelInstance, "modelInstance");
            TestUtil.CheckArgumentNotNull(containerName, "containerName");
            return (IQueryable)modelInstance.GetType().GetProperty(containerName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(modelInstance, null);
        }

        /// <summary>Gets the root type for the specified container.</summary>
        /// <param name="containerName">Name of container to look up root type for.</param>
        /// <returns>The root type for the specified container.</returns>
        public string GetContainerRootTypeName(string containerName)
        {
            TestUtil.CheckArgumentNotNull(containerName, "containerName");

            // Simplification: no MEST, no inheritance.
            IEdmEntityContainer container = this.model.EntityContainer;
            IEdmEntitySet entitySet = container.FindEntitySet(containerName);
            return entitySet.EntityType().FullName();
        }

        /// <summary>Enumerates the properties that make up the ETag for the type.</summary>
        public IEnumerable<PropertyInfo> GetEtagProperties(Type type)
        {
            TestUtil.CheckArgumentNotNull(type, "type"); 
            ETagAttribute[] attributes = (ETagAttribute[])type.GetCustomAttributes(typeof(ETagAttribute), true);
            if (attributes.Length > 0)
            {
                foreach (string name in attributes[0].PropertyNames)
                {
                    yield return type.GetProperty(name);
                }
            }
        }

        /// <summary>Enumerates valid properties for the specified container.</summary>
        /// <param name="containerName">Name of container to get properties for.</param>
        /// <returns>An enumerator of valid properties.</returns>
        public IEnumerable<IEdmProperty> GetKeyProperties(string typeName)
        {
            TestUtil.CheckArgumentNotNull(typeName, "typeName");

            IEdmSchemaType edmType = this.model.FindType(typeName);
            if (edmType.TypeKind == EdmTypeKind.Entity)
            {
                foreach (var member in ((IEdmEntityType)edmType).Key())
                {
                    yield return member;
                }
            }
        }

        public IEnumerable<IEdmProperty> GetNonKeyPrimitiveProperties(string typeName)
        {
            TestUtil.CheckArgumentNotNull(typeName, "typeName");
            IEdmProperty[] keys = GetKeyProperties(typeName).ToArray();
            foreach (var info in GetModelProperties(typeName))
            {
                if (!keys.Contains(info) && info.Type.IsPrimitive())
                {
                    yield return info;
                }
            }
        }

        /// <summary>Enumerates valid properties for the specified type.</summary>
        /// <param name="type">Type to get properties for.</param>
        /// <returns>An enumerator of valid properties.</returns>
        public IEnumerable<IEdmProperty> GetModelProperties(string typeName)
        {
            var edmType = this.model.FindType(typeName);
            if (edmType.TypeKind == EdmTypeKind.Complex ||
                edmType.TypeKind == EdmTypeKind.Entity)
            {
                return ((IEdmStructuredType)edmType).Properties();
            }

            return new IEdmProperty[0];
        }

        /// <summary>Gets a formatted key value for an existing resource in the specified container.</summary>
        /// <param name="containerName">Name of container.</param>
        /// <returns>A formatted key value.</returns>
        public string GetSampleKey(string containerName, SerializationFormatData format)
        {
            TestUtil.CheckArgumentNotNull(containerName, "containerName");
            TestUtil.CheckArgumentNotNull(format, "format");
            object[] keys = GetSampleKeyValues(containerName);
            if (keys == null)
            {
                return null;
            }
            
            // Currently supporting all formats as if they had identical literals (XML-based).
            return TestUtil.Join(",", keys.Select((k)=>TypeData.XmlValueFromObject(k)));
        }

        public object[] GetSampleKeyValues(string containerName)
        {
            TestUtil.CheckArgumentNotNull(containerName, "containerName");
            if (this.ServiceModelType == typeof(CustomDataContext))
            {
                if (containerName == "Customers")
                {
                    return new object[] { 1 };
                }
                else
                {
                    throw new NotImplementedException("GetSampleKey not implemented for container " + containerName);
                }
            }
            else if (this.ServiceModelType == typeof(NorthwindModel.NorthwindContext))
            {
                if (containerName == "Customers")
                {
                    return new object[] { "ALFKI" };
                }
                else
                {
                    throw new NotImplementedException("GetSampleKey not implemented for container " + containerName);
                }
            }
            else
            {
                throw new NotImplementedException("GetSampleKey not implemented for service model type " + this.ServiceModelType);
            }
        }

        public string GetSampleKeyForUri(string containerName)
        {
            TestUtil.CheckArgumentNotNull(containerName, "containerName"); 
            object[] values = GetSampleKeyValues(containerName);
            if (values == null)
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();
            foreach (object o in values)
            {
                if (builder.Length > 0)
                {
                    builder.Append(',');
                }
                builder.Append(TypeData.FormatForKey(o, true, false));
            }
            return builder.ToString();
        }

        public IEdmProperty GetSampleNonKeyPrimitiveProperty(string containerName)
        {
            TestUtil.CheckArgumentNotNull(containerName, "containerName");
            return this.GetNonKeyPrimitiveProperties(this.GetContainerRootTypeName(containerName)).First();
        }

        public IEdmProperty GetSamplePrimitiveProperty(string containerName)
        {
            TestUtil.CheckArgumentNotNull(containerName, "containerName");
            return this.GetValidContainerProperties(containerName).Where((x) => x.Type.IsPrimitive()).First();
        }

        /// <summary>Enumerates valid properties for the specified container.</summary>
        /// <param name="containerName">Name of container to get properties for.</param>
        /// <returns>An enumerator of valid properties.</returns>
        public IEnumerable<IEdmProperty> GetValidContainerProperties(string containerName)
        {
            TestUtil.CheckArgumentNotNull(containerName, "containerName");
            string typeName = GetContainerRootTypeName(containerName);
            return GetModelProperties(typeName);
        }

        #region Private methods.

        private static ServiceModelData ForType(Type serviceModelType)
        {
            Debug.Assert(serviceModelType != null, "serviceModelType != null");

            ServiceModelData result = new ServiceModelData();
            result.serviceModelType = serviceModelType;

            result.EnsureDependenciesAvailable();
            result.model = UnitTestsUtil.LoadMetadataFromDataServiceType(serviceModelType, null);
            result.isValid = true;
            result.containerNames = new List<string>();

            // ensure that there is one container
            var container = result.model.EntityContainer;
            foreach (var entitySet in container.EntitySets())
            {
                result.ContainerNames.Add(entitySet.Name);
            }

            // For the time being, we don't have invalid service models in here.
            Debug.Assert(result.containerNames.Count > 0, "result.containerNames.Count > 0");

            result.web3STestPrefix = GetWeb3STestPrefix(serviceModelType);

            return result;
        }

        /// <summary>Generates a mnemonic prefix for XPath queries for the given type.</summary>
        private static string GetWeb3STestPrefix(Type type)
        {
            string result = "";
            string longForm = type.Name;
            for (int i = 0; i < longForm.Length; i++)
            {
                if (Char.IsUpper(longForm, i))
                {
                    result += Char.ToLowerInvariant(longForm[i]);
                }
            }

            return result;
        }

        /// <summary>Sets up the default connection string for the Northwind EDM model.</summary>
        private static void SetupNorthwindModelDatabase()
        {
            AstoriaDatabase database = new AstoriaDatabase("Northwind");

            NorthwindModel.NorthwindContext.ContextConnectionString = DataUtil.BuildEntityConnection(
                TestUtil.NorthwindMetadataDirectory, "Northwind", database.DatabaseConnectionString);
            TestWebRequest.SerializedTestArguments["NorthwindContextConnectionString"] = NorthwindModel.NorthwindContext.ContextConnectionString;
            AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext.DefaultConnectionString = database.DatabaseConnectionString;
        }

        #endregion Private methods.
    }
}
