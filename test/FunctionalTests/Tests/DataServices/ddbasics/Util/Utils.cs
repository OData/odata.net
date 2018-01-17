//---------------------------------------------------------------------
// <copyright file="Utils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.OData.Service;
using Microsoft.OData.Client;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests
{
    public class PopulatingValuesEventArgs<T> : EventArgs
    {
        public PopulatingValuesEventArgs(string name, List<T> values)
        {
            this.Name = name;
            this.Values = values;
        }

        public string Name { get; private set; }
        public List<T> Values { get; set; }
    }

    /// <summary>Use this class as a typed data context with a single 'Values' entity set.</summary>
    /// <typeparam name="T">Element type for the Values entity set.</typeparam>
    public class TypedCustomDataContext<T> : IUpdatable
    {
        private static List<T> values;
        private static bool preserveChanges;
        public static event EventHandler ValuesRequested;
        public static Func<string, Type> TypeResolver;

        public static bool PreserveChanges
        {
            get { return preserveChanges; }
            set { preserveChanges = value; }
        }

        /// <summary>Clears all event handlers.</summary>
        public static void ClearHandlers()
        {
            EventHandler handler = ValuesRequested;
            ValuesRequested = null;
            if (handler != null)
            {
                Trace.WriteLine("Clearing handler " + handler.ToString());
            }
        }

        /// <summary>Clears values in the data context.</summary>
        public static void ClearValues()
        {
            values = null;
            preserveChanges = false;
        }

        public static void CreateChangeScope()
        {
            preserveChanges = true;
        }

        public static void ClearData()
        {
            ClearValues();
            ClearHandlers();
        }

        public void SetValues(object[] newValues)
        {
            values = new List<T>(newValues.Length);
            foreach (object value in newValues)
            {
                values.Add((T)value);
            }
        }

        public static IList<T> CurrentValues
        {
            get
            {
                return values;
            }
        }

        public IQueryable<T> Values
        {
            get
            {
                if (preserveChanges == false || values == null)
                {
                    if (!preserveChanges)
                    {
                        values = null;
                    }

                    if (ValuesRequested != null)
                    {
                        ValuesRequested(this, EventArgs.Empty);
                    }

                    if (values == null)
                    {
                        values = new List<T>();
                    }

                    PopulatingValuesEventArgs<T> args = new PopulatingValuesEventArgs<T>("Values", values);
                    StaticCallbackManager<PopulatingValuesEventArgs<T>>.FireEvent(this, args);
                    if (args.Values != values)
                    {
                        values = args.Values;
                    }
                }

                return values.AsQueryable();
            }
        }

        public static Action<DataServiceConfiguration> CustomInitializeService;

        public static void InitializeService(DataServiceConfiguration config)
        {
            if (CustomInitializeService != null)
            {
                CustomInitializeService(config);
            }
        }

        #region IUpdatable Members

        object IUpdatable.CreateResource(string containerName, string fullTypeName)
        {
            if (values == null)
            {
                values = new List<T>();
            }

            // IUpdatable.CreateResource(containerName, fullTypeName) - the API doesn't give enough details to translate fullTypeName back to a type.
            // We call TypeResolver to help resolve fullTypeName to the actual type we want to create.
            Type typeToCreate = null;
            if (TypeResolver != null)
            {
                typeToCreate = TypeResolver(fullTypeName);
            }

            if (typeToCreate == null)
            {
                typeToCreate = typeof(T);
            }

            // Create the new instance of the type
            ConstructorInfo emptyConstructor = typeToCreate.GetConstructor(Type.EmptyTypes);
            object resource = emptyConstructor.Invoke(null);
            if (typeof(T).IsAssignableFrom(typeToCreate))
            {
                values.Add((T)resource);
            }

            return resource;
        }

        object IUpdatable.GetResource(IQueryable query, string fullTypeName)
        {
            object resource = null;

            foreach (object r in query)
            {
                Debug.Assert(resource == null, "resource != null");
                resource = r;
            }

            return resource;
        }

        object IUpdatable.ResetResource(object resource)
        {
            List<string> keyPropNames = new List<string>();
            foreach (KeyAttribute keyAttr in resource.GetType().GetCustomAttributes(typeof(KeyAttribute), true))
            {
                keyPropNames.AddRange(keyAttr.KeyNames);
            }

            foreach (PropertyInfo propInfo in resource.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                // reset property if not a key
                if (!(keyPropNames.Contains(propInfo.Name) || (keyPropNames.Count == 0 && propInfo.Name.EndsWith("ID"))))
                {
                    object newPropInstance = propInfo.PropertyType.IsValueType ? Activator.CreateInstance(propInfo.PropertyType) : null;
                    resource.GetType().GetMethod("set_" + propInfo.Name).Invoke(resource, new object[] { newPropInstance });
                }
            }

            return resource;
        }

        object IUpdatable.GetValue(object targetResource, string propertyName)
        {
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return property.GetValue(targetResource, null);
        }

        void IUpdatable.SetValue(object targetResource, string propertyName, object propertyValue)
        {
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName);

            if (property == null)
            {
                throw new Exception("Invalid property name specified");
            }
            else
            {
                property.SetValue(targetResource, propertyValue, null);
            }
        }

        void IUpdatable.SetReference(object targetResource, string propertyName, object propertyValue)
        {
            throw new NotImplementedException();
        }

        void IUpdatable.AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            throw new NotImplementedException();
        }

        void IUpdatable.RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            throw new NotImplementedException();
        }

        void IUpdatable.DeleteResource(object resource)
        {
            values.Remove((T)resource);
        }

        void IUpdatable.SaveChanges()
        {
            // Do nothing
        }

        object IUpdatable.ResolveResource(object targetResource)
        {
            return targetResource;
        }

        void IUpdatable.ClearChanges()
        {
            // Can't fail here as some tests verify negative behavior during batch processing, in which case this method will get called and its failure
            // would override the actual failure of the CUD operation in the batch.
            // This will cause "data corruption", but our tests don't rely on the data being reverted anyway.
        }

        #endregion
    }

    public partial class Utils
    {
        /// <summary>Data.Services assembly - access through DataWebAssembly to fault in.</summary>
        private static Assembly dataWebAssembly;

        /// <summary>Microsoft.OData.Service.Caching.ConfigurationCache type - access through ConfigurationCacheType to fault in.</summary>
        private static Type metadataCacheType;

        /// <summary>Cached 'cache' field in metadata cache.</summary>
        private static FieldInfo configurationCacheField;

        /// <summary>Reusable name table for tests.</summary>
        private static XmlNameTable testNameTable;

        /// <summary>Reusable namespace manager for tests.</summary>
        private static XmlNamespaceManager testNamespaceManager;

        /// <summary>
        /// Public method for cleaning up metadata cache before and at the end of a variation
        /// </summary>
        /// <returns>IDisposable to be used for disposing off the object</returns>
        public static IDisposable ConfigurationCacheCleaner()
        {
            return new MetadataCacheResetter();
        }

        /// <summary>
        /// IDisposable implementation class for metadata cache cleanup
        /// </summary>
        private class MetadataCacheResetter : IDisposable
        {
            internal MetadataCacheResetter()
            {
                Utils.ClearConfiguration();
            }

            public void Dispose()
            {
                Utils.ClearConfiguration();
            }
        }

        /// <summary>Returns the Astoria metadata cache in the current AppDomain.</summary>
        public static IDictionary ConfigurationCache
        {
            get
            {
                if (configurationCacheField == null)
                {
                    configurationCacheField = ConfigurationCacheType.GetField("cache", BindingFlags.NonPublic | BindingFlags.Static);
                }

                return (IDictionary)configurationCacheField.GetValue(null);
            }
        }

        /// <summary>Clears the Astoria metadata cache in the current AppDomain.</summary>
        private static void ClearConfiguration()
        {
            ConfigurationCache.Clear();
        }

        /// <summary>Reusable name table for tests.</summary>
        public static XmlNameTable TestNameTable
        {
            get
            {
                if (testNameTable == null)
                {
                    testNameTable = new NameTable();
                }

                return testNameTable;
            }
        }

        /// <summary>Reusable namespace manager for tests.</summary>
        public static XmlNamespaceManager TestNamespaceManager
        {
            get
            {
                if (testNamespaceManager == null)
                {
                    testNamespaceManager = new XmlNamespaceManager(TestNameTable);

                    // Some common namespaces used by legacy tests.
                    testNamespaceManager.AddNamespace("dw", "http://docs.oasis-open.org/odata/ns/data");
                    testNamespaceManager.AddNamespace("ads", "http://docs.oasis-open.org/odata/ns/data");
                    testNamespaceManager.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                    testNamespaceManager.AddNamespace("cy", "http://www.currency.org");
                    testNamespaceManager.AddNamespace("geo", "http://www.georss.org/georss");
                    testNamespaceManager.AddNamespace("gml", "http://www.opengis.net/gml");
                    testNamespaceManager.AddNamespace("ad", "http://www.address.org");
                    testNamespaceManager.AddNamespace("adsm", "http://docs.oasis-open.org/odata/ns/metadata");
                }

                return testNamespaceManager;
            }
        }

        public static readonly string MimeApplicationAtomXml = "application/atom+xml";
        public static readonly string MimeApplicationXml = "application/xml";

        /// <summary>Creates an object that will restore a static value on disposal.</summary>
        /// <param name="type">Type to read static value from.</param>
        /// <param name="propertyName">Name of property to read value from.</param>
        /// <returns>An object that will restore a static value on disposal.</returns>
        /// <remarks>
        /// The usage pattern is:
        /// using (var r = TestUtil.RestoreStaticValueOnDispose(typeof(Type1), "Prop")) { ... }
        /// </remarks>
        public static IDisposable RestoreStaticValueOnDispose(Type type, string propertyName)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
            MemberTypes memberTypes = MemberTypes.Property | MemberTypes.Field;
            MemberInfo propertyInfo = type.GetMember(propertyName, memberTypes, flags).FirstOrDefault();
            if (propertyInfo == null)
            {
                throw new Exception("Unable to find property " + propertyName + " on type " + type + ".");
            }
            return new StaticValueRestorer(propertyInfo);
        }

        public static Stream ProcessWebRequest(Uri requestUri)
        {
            return ProcessWebRequest(requestUri, MimeApplicationAtomXml, "GET");
        }

        public static XmlDocument ProcessWebRequestAsAtom(Uri requestUri, out WebHeaderCollection responseHeaders)
        {
            using (Stream response = ProcessWebRequest(requestUri, MimeApplicationAtomXml, "GET", out responseHeaders))
            {
                XmlDocument atom = new XmlDocument(TestNameTable);
                atom.Load(response);
                return atom;
            }
        }

        public static Stream ProcessWebRequest(Uri requestUri, string mimeType, string method)
        {
            WebHeaderCollection responseHeaders;
            return ProcessWebRequest(requestUri, mimeType, method, out responseHeaders);
        }

        public static Stream ProcessWebRequest(Uri requestUri, string mimeType, string method, out WebHeaderCollection responseHeaders)
        {
            responseHeaders = null;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            request.Accept = mimeType;
            request.Method = method;
            if (String.Compare(method, "POST", true) == 0)
            {
                request.ContentLength = 0;
            }

            bool gotResponse = false;
            bool gotResponseStream = false;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                gotResponse = true;
                Stream result = response.GetResponseStream();
                gotResponseStream = true;
                responseHeaders = response.Headers;
                return result;
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    HttpWebResponse r = (HttpWebResponse)e.Response;
                    StreamReader reader = new StreamReader(r.GetResponseStream());
                    responseHeaders = r.Headers;
                    Trace.WriteLine(reader.ReadToEnd());
                }

                Trace.WriteLine(e.StackTrace);
                Assert.Fail("WebException: " + e.Message + "\tRequest Uri: " + requestUri.ToString());
                return null;
            }
            finally
            {
                if (gotResponse && !gotResponseStream)
                {
                    response.Close();
                }
            }
        }

        public static bool InterfaceTypeFilter(Type type, object parameterizedTypeCriteria)
        {
            Type parameterizedType = (Type)parameterizedTypeCriteria;
            return type.IsGenericType && type.GetGenericTypeDefinition() == parameterizedType;
        }

        public static Type GetTypeParameter(Type type, Type parameterizedType, int parameterIndex)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(parameterizedType != null, "parameterizedType != null");
            Debug.Assert(parameterizedType.IsGenericTypeDefinition);
            Debug.Assert(parameterIndex >= 0, "parameterIndex >= 0");

            if (parameterizedType.IsInterface)
            {
                if (InterfaceTypeFilter(type, parameterizedType))
                {
                    return type.GetGenericArguments()[parameterIndex];
                }

                Type[] interfaces = type.FindInterfaces(InterfaceTypeFilter, parameterizedType);
                if (interfaces.Length == 0 || interfaces.Length > 1)
                {
                    return null;
                }
                return interfaces[0].GetGenericArguments()[parameterIndex];
            }
            else
            {
                while (type != null && (!type.IsGenericType || type.GetGenericTypeDefinition() != parameterizedType))
                {
                    type = type.BaseType;
                }

                if (type == null)
                {
                    return null;
                }

                return type.GetGenericArguments()[parameterIndex];
            }
        }

        public static void IsBatchResponse(DataServiceResponse response)
        {
            Assert.IsTrue(response.IsBatchResponse, "Must be a batch response");
            Assert.AreEqual<int>(response.BatchStatusCode, 202, "batch status code must be 202");
            Assert.IsTrue(response.BatchHeaders.Count != 0, "there must be a response headers");
        }

        public static void IsNotBatchResponse(DataServiceResponse response)
        {
            Assert.IsTrue(response.IsBatchResponse == false, "Must not be a batch response");
            Assert.AreEqual<int>(response.BatchStatusCode, -1, "batch status code must be 0 (uninitialized)");
            Assert.IsTrue(response.BatchHeaders.Count == 0, "there must be no top level headers for non-batch responses");
        }

        public static void IsSuccessResponse(OperationResponse response, HttpStatusCode statusCode)
        {
            Assert.IsTrue(response.Error == null, "expecting no exception");
            Assert.IsTrue(200 <= response.StatusCode && response.StatusCode < 300, String.Format("Expected: {0}, Actual: {1}", (int)statusCode, response.StatusCode));
            Assert.IsTrue(response.Headers.Count != 0, "headers count cannot be zero");
        }

        public static void IsErrorResponse(OperationResponse response, HttpStatusCode statusCode, bool checkForDataServiceExceptionType)
        {
            Assert.IsTrue(response.Error != null, "expecting exception");
            if (checkForDataServiceExceptionType)
            {
                Assert.IsTrue(response.Error.GetType() == typeof(Microsoft.OData.Client.DataServiceClientException), "exception type must be dataservice exception");
            }
            Assert.IsTrue(response.StatusCode == (int)statusCode, String.Format("Expected: {0}, Actual: {1}", (int)statusCode, response.StatusCode));
            Assert.IsTrue(response.Headers.Count != 0, "headers count cannot be zero");
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) evaluate to true.
        /// </summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathExpressionResults(XNode node, object expectedResult, params string[] xpaths)
        {
            VerifyXPathExpressionResults(node.CreateNavigator(TestNameTable), expectedResult, xpaths);
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) evaluate to true.
        /// </summary>
        /// <param name="navigable">Document to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathExpressionResults(IXPathNavigable navigable, object expectedResult, params string[] xpaths)
        {
            XPathNavigator nav = navigable.CreateNavigator();
            foreach (string xpath in xpaths)
            {
                object actualResult = nav.Evaluate(xpath, TestNamespaceManager);
                Assert.AreEqual(expectedResult, actualResult, "Expression: " + xpath + " evaluated to " + actualResult.ToString());
            }
        }

        private struct StaticValueRestorer : IDisposable
        {
            /// <summary>A zero-length object array.</summary>
            private static readonly object[] EmptyObjectArray = new object[0];

            private MemberInfo propertyInfo;
            private object value;

            internal StaticValueRestorer(MemberInfo propertyInfo)
            {
                this.propertyInfo = propertyInfo;
                if (propertyInfo is PropertyInfo)
                {
                    this.value = ((PropertyInfo)propertyInfo).GetValue(null, EmptyObjectArray);
                }
                else
                {
                    Debug.Assert(propertyInfo is FieldInfo, "propertyInfo is FieldInfo");
                    this.value = ((FieldInfo)propertyInfo).GetValue(null);
                }
            }

            public void Dispose()
            {
                if (propertyInfo is PropertyInfo)
                {
                    ((PropertyInfo)propertyInfo).SetValue(null, this.value, EmptyObjectArray);
                }
                else
                {
                    Debug.Assert(propertyInfo is FieldInfo, "propertyInfo is FieldInfo");
                    ((FieldInfo)propertyInfo).SetValue(null, this.value);
                }
            }
        }

        /// <summary>Data.Services assembly.</summary>
        private static Assembly DataWebAssembly
        {
            get
            {
                if (dataWebAssembly == null)
                {
                    dataWebAssembly = typeof(IDataServiceHost).Assembly;
                }
                return dataWebAssembly;
            }
        }

        private static Type ConfigurationCacheType
        {
            get
            {
                if (metadataCacheType == null)
                {
                    metadataCacheType = DataWebAssembly.GetType("Microsoft.OData.Service.Caching.MetadataCache`1", true);
                    metadataCacheType = metadataCacheType.MakeGenericType(DataWebAssembly.GetType("Microsoft.OData.Service.Caching.DataServiceCacheItem", true));
                }
                return metadataCacheType;
            }
        }
    }

    public static class ResourceUtil
    {
        private const string SystemSpatialBaseName = "Microsoft.Spatial";
        private const string SystemDataServicesBaseName = "Microsoft.OData.Service";
        private const string SystemDataServicesClientBaseName = "Microsoft.OData.Client";
        private const string SystemDataServicesDesignBaseName = "Microsoft.OData.Service.Design";
        private const string ODataLibBaseName = "Microsoft.OData.Core";

        public static System.Resources.ResourceManager MicrosoftDataSpatialResourceManager = new System.Resources.ResourceManager(SystemSpatialBaseName, typeof(Microsoft.Spatial.Geography).Assembly);
        public static System.Resources.ResourceManager SystemDataServicesResourceManager = new System.Resources.ResourceManager(SystemDataServicesBaseName, typeof(Microsoft.OData.Service.DataService<>).Assembly);
        public static System.Resources.ResourceManager SystemDataServicesClientResourceManager = new System.Resources.ResourceManager(SystemDataServicesClientBaseName, typeof(Microsoft.OData.Client.DataServiceContext).Assembly);
        public static System.Resources.ResourceManager ODataLibResourceManager = new System.Resources.ResourceManager(ODataLibBaseName, typeof(Microsoft.OData.IODataResponseMessage).Assembly);

        public static string GetStringResource(System.Resources.ResourceManager manager, string name, params object[] args)
        {
            Assert.IsNotNull(manager, "ResourceManager");
            Assert.IsFalse(string.IsNullOrEmpty(name), "resource name parameter is empty");

            string res = manager.GetString(name, null/*use ResourceManager default, CultureInfo.CurrentUICulture*/);
            Assert.IsNotNull(res, "Failed to load resource \"{0}\"", name);

            if ((null != args) && (0 < args.Length))
            {
                res = String.Format(null/*use ResourceManager default, CultureInfo.CurrentUICulture*/, res, args);
            }

            return res;
        }
    }

    public static class DataSpatialResourceUtil
    {
        public static string GetString(string name, params object[] args)
        {
            return ResourceUtil.GetStringResource(ResourceUtil.MicrosoftDataSpatialResourceManager, name, args);
        }
    }

    public static class DataServicesResourceUtil
    {
        public static string GetString(string name, params object[] args)
        {
            return ResourceUtil.GetStringResource(ResourceUtil.SystemDataServicesResourceManager, name, args);
        }
    }

    public static class DataServicesClientResourceUtil
    {
        public static string GetString(string name, params object[] args)
        {
            return ResourceUtil.GetStringResource(ResourceUtil.SystemDataServicesClientResourceManager, name, args);
        }
    }

    public static class ODataLibResourceUtil
    {
        public static string GetString(string name, params object[] args)
        {
            return ResourceUtil.GetStringResource(ResourceUtil.ODataLibResourceManager, name, args);
        }
    }

    public static class ReflectionUtils
    {
        private static ModuleBuilder moduleBuilder = CreateModuleBuilder("ReflectionUtils_Assembly");
        private static int clientTypeIndex = 0;

        private static ModuleBuilder CreateModuleBuilder(string name)
        {
            const DebuggableAttribute.DebuggingModes debuggingModes = DebuggableAttribute.DebuggingModes.DisableOptimizations;
            AssemblyName assemblyName = new AssemblyName(name);
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            // Enable debugging information.
            var attributeConstructor = typeof(DebuggableAttribute).GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
            var attributeBuilder = new CustomAttributeBuilder(attributeConstructor, new object[] { debuggingModes });
            assembly.SetCustomAttribute(attributeBuilder);
            return assembly.DefineDynamicModule(name, true);
        }
    }
}
