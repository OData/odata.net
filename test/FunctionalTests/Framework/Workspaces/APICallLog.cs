//---------------------------------------------------------------------
// <copyright file="APICallLog.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Microsoft.OData.Service;
using System.Data.Test.Astoria.FullTrust;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DSP = Microsoft.OData.Service.Providers;
using System.Data.Objects;

namespace System.Data.Test.Astoria.CallOrder
{
    #region entry
    public class APICallLogEntry
    {
        public static XElement ToXml(APICallLogEntry entry)
        {
            XElement element = new XElement(entry.MethodName);

            XElement arguments = new XElement("arguments");
            element.Add(arguments);
            foreach (KeyValuePair<string, string> pair in entry.Arguments)
                arguments.Add(new XElement(pair.Key, Convert.ToBase64String(Encoding.UTF8.GetBytes(pair.Value))));

            XElement stack = new XElement("stack", entry.StackTrace);
            element.Add(stack);

            return element;
        }

        public static APICallLogEntry FromXml(XElement element)
        {
            APICallLogEntry entry = new APICallLogEntry(element.Name.LocalName,
                    element.Element("arguments").Elements().Select(argument => new KeyValuePair<string, string>(argument.Name.LocalName,
                        Encoding.UTF8.GetString(Convert.FromBase64String(argument.Value)))));
        
            entry.StackTrace = element.Element("stack").Value;

            return entry;
        }

        public APICallLogEntry(string methodName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            Init(methodName, arguments.ToArray());
        }

        public APICallLogEntry(string methodName, params KeyValuePair<string, string>[] arguments)
        {
            Init(methodName, arguments);
        }

        public APICallLogEntry(string methodName, params string[] arguments)
        {
            if (arguments.Length % 2 != 0)
                throw new ArgumentException("Number of arguments not even, cannot infer key/value pairs");

            Init(methodName, arguments.Where((a, i) => i % 2 == 0).Select((a, i) => new KeyValuePair<string, string>(arguments[2 * i], arguments[2 * i + 1])).ToArray());
        }

        private void Init(string methodName, KeyValuePair<string, string>[] arguments)
        {
            this.MethodName = methodName;
            this.Arguments = arguments;
            this.StackTrace = new StackTrace().ToString();
        }

        public string MethodName
        {
            get;
            private set;
        }

        public KeyValuePair<string, string>[] Arguments
        {
            get;
            private set;
        }

        public string StackTrace
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return MethodName + "(" + string.Join(", ", Arguments.Select(a => a.Key + " = " + a.Value).ToArray()) + ")";
        }
    }
    #endregion

    public class APICallLog
    {
        public const string DirectoryName = "APICallLog";
        public const string MarkerFileName = "EnableLogging.txt";

        public static APICallLog Current = new APICallLog();
        
        private readonly string directoryPath;
        private readonly string markerFilePath;
        
        private uint depth = 0;
        private uint entryCount = 0;
        
        private APICallLog()
        {
            if (Current != null)
            {
                throw new InvalidOperationException("Cannot instantiate twice");
            }

            Updatable = new IUpdatableCallLog(this);
            ServiceProvider = new IServiceProviderCallLog(this);
            DataService = new DataServiceAPICallLog(this);
            ExpandProvider = new IExpandProviderCallLog(this);
            Stream = new StreamCallLog(this);

#if !ASTORIA_PRE_V2
            UpdateProvider = new IDataServiceUpdateProviderCallLog(this);
            MetadataProvider = new IDataServiceMetadataProviderCallLog(this);
            QueryProvider = new IDataServiceQueryProviderCallLog(this);
            StreamProvider = new IDataServiceStreamProviderCallLog(this);
            ProcessingPipeline = new DataServiceProcessingPipelineCallLog(this);
            PagingProvider = new IDataServicePagingProviderCallLog(this);
            IDSH = new IDataServiceHostCallLog(this);
            IDSH2 = new IDataServiceHost2CallLog(this);
#endif

#if !ASTORIA_PRE_V3
            StreamProvider2 = new IDataServiceStreamProvider2CallLog(this);
#endif

#if !ClientSKUFramework
            if (System.Web.HttpContext.Current != null)
            {
                // IIS
                directoryPath = System.Web.HttpContext.Current.Server.MapPath(DirectoryName);
            }
            else
#endif
            {
                // WebServiceHost
                directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DirectoryName);
            }

            markerFilePath = Path.Combine(directoryPath, MarkerFileName);
        }

        public bool InCall
        {
            get
            {
                return depth != 0;
            }
        }

        public bool Enabled
        {
            get
            {
                return Directory.Exists(directoryPath) && File.Exists(markerFilePath);
            }
        }

        private void Write(APICallLogEntry entry)
        {
            string fileName = string.Format("{0:0000000000000000000000000}.xml", entryCount++);
            string entryPath = Path.Combine(directoryPath, fileName);
            string xml = APICallLogEntry.ToXml(entry).ToString();
            TrustedMethods.WriteAllText(entryPath, xml);
        }
        
        internal void Add(APICallLogEntry entry)
        {
            bool inCall = InCall;

            // need to increase depth even if we do nothing, as the caller will pop either way
            Push();

            if (inCall || !Enabled)
                return;

            Write(entry);
        }

        public void Add(string methodName, params string[] arguments)
        {
            Add(new APICallLogEntry(methodName, arguments));
        }

        public void Push()
        {
            depth++;
        }

        public void Pop()
        {
            depth--;
        }

        internal void Add(MethodInfo method, params string[] argumentValues)
        {
            List<KeyValuePair<string, string>> arguments = new List<KeyValuePair<string, string>>();
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length < argumentValues.Length)
                throw new ArgumentException("Too many argument values specified");

            for (int i = 0; i < argumentValues.Length; i++)
            {
                arguments.Add(new KeyValuePair<string, string>(parameters[i].Name, argumentValues[i]));
            }

            Add(new APICallLogEntry(method.DeclaringType.Name + "." + method.Name, arguments));
        }

        public string Serialize(object value)
        {
            if (value == null)
                return "null";

            Type t = value.GetType();

            if (t == typeof(byte[]))
                return Convert.ToBase64String((byte[])value);

            if (t == typeof(DateTime))
            {
                return ((DateTime)value).ToString("o", Globalization.CultureInfo.InvariantCulture.NumberFormat);
            }

            Globalization.CultureInfo realCulture = Globalization.CultureInfo.CurrentCulture;
            Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.InvariantCulture;
            try
            {
                if (t.IsPrimitive || t == typeof(string) || t.IsValueType)
                    return value.ToString();

                t = ObjectContext.GetObjectType(t);

                return t.ToString();
            }
            finally
            {
                Threading.Thread.CurrentThread.CurrentCulture = realCulture;
            }
        }

        #region interfaces
        public IUpdatableCallLog Updatable
        {
            get;
            private set;
        }

        public IServiceProviderCallLog ServiceProvider
        {
            get;
            private set;
        }

        public DataServiceAPICallLog DataService
        {
            get;
            private set;
        }

        public IExpandProviderCallLog ExpandProvider
        {
            get;
            private set;
        }

        public IDataServiceHostCallLog IDSH
        {
            get;
            private set;
        }

        public StreamCallLog Stream
        {
            get;
            private set;
        }

#if !ASTORIA_PRE_V2
        public IDataServiceUpdateProviderCallLog UpdateProvider
        {
            get;
            private set;
        }

        public IDataServiceMetadataProviderCallLog MetadataProvider
        {
            get;
            private set;
        }

        public IDataServiceQueryProviderCallLog QueryProvider
        {
            get;
            private set;
        }

        public IDataServiceStreamProviderCallLog StreamProvider
        {
            get;
            private set;
        }

        public DataServiceProcessingPipelineCallLog ProcessingPipeline
        {
            get;
            private set;
        }

        public IDataServicePagingProviderCallLog PagingProvider
        {
            get;
            private set;
        }

        public IDataServiceHost2CallLog IDSH2
        {
            get;
            private set;
        }
#endif
#if !ASTORIA_PRE_V3
        public IDataServiceStreamProvider2CallLog StreamProvider2
        {
            get;
            private set;
        }
#endif
        #endregion
    }

    #region Interfaces
    public class InterfaceCallLog<T>
    {
        internal InterfaceCallLog(APICallLog parent)
        {
            Parent = parent;
        }

        protected APICallLog Parent
        {
            get;
            private set;
        }

        protected void Add(string methodName, params string[] argumentValues)
        {
            MethodInfo method = typeof(T).GetMethod(methodName);
            if (method == null)
                throw new ArgumentException("Could not find method '" + methodName + "' on interface '" + typeof(T).ToString() + "'");

            Parent.Add(method, argumentValues);
        }

        protected void AddGetProperty(string propertyName)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("Could not find property '" + propertyName + "' on interface '" + typeof(T).ToString() + "'");

            Parent.Add(property.GetGetMethod());
        }

        protected void AddSetProperty(string propertyName, object value)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("Could not find property '" + propertyName + "' on interface '" + typeof(T).ToString() + "'");

            Parent.Add(property.GetSetMethod(), Parent.Serialize(value));
        }
    }

    #region IUpdatable
    public class IUpdatableCallLog : InterfaceCallLog<IUpdatable>
    {
        internal IUpdatableCallLog(APICallLog parent)
            : base(parent)
        {
        }

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            Add("AddReferenceToCollection", Parent.Serialize(targetResource), propertyName, Parent.Serialize(resourceToBeAdded));
        }

        public void ClearChanges()
        {
            Add("ClearChanges");
        }

        public void CreateResource(string containerName, string fullTypeName)
        {
            if (containerName == null)
                containerName = "null";
            Add("CreateResource", containerName, fullTypeName);
        }

        public void DeleteResource(object targetResource)
        {
            Add("DeleteResource", Parent.Serialize(targetResource));
        }

        public void GetResource(IQueryable query, string fullTypeName)
        {
            if (fullTypeName == null)
                fullTypeName = "null";
            Add("GetResource", query.GetType().ToString(), fullTypeName);
        }

        public void GetValue(object targetResource, string propertyName)
        {
            Add("GetValue", Parent.Serialize(targetResource), propertyName);
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            Add("RemoveReferenceFromCollection", Parent.Serialize(targetResource), propertyName, Parent.Serialize(resourceToBeRemoved));
        }

        public void ResetResource(object resource)
        {
            Add("ResetResource", Parent.Serialize(resource));
        }

        public void ResolveResource(object resource)
        {
            Add("ResolveResource", Parent.Serialize(resource));
        }

        public void SaveChanges()
        {
            Add("SaveChanges");
        }

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            Add("SetReference", Parent.Serialize(targetResource), propertyName, Parent.Serialize(propertyValue));
        }

        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            Add("SetValue", Parent.Serialize(targetResource), propertyName, Parent.Serialize(propertyValue));
        }
    }
    #endregion

    #region IServiceProvider
    public class IServiceProviderCallLog : InterfaceCallLog<IServiceProvider>
    {
        internal IServiceProviderCallLog(APICallLog parent)
            : base(parent)
        {
        }

        public void GetService(Type serviceType)
        {
            Add("GetService", serviceType.ToString());
        }
    }
    #endregion

    #region DataService<T>
    public class DataServiceAPICallLog
    {
        private APICallLog Parent;

        internal DataServiceAPICallLog(APICallLog parent)
        {
            Parent = parent;
        }

        public void CreateDataSource<T>(DataService<T> service)
        {
            // non-public, don't reflect
            Parent.Add(new APICallLogEntry(service.GetType().Name + ".CreateDataSource", new KeyValuePair<string, string>[0]));
        }

        public void HandleException<T>(DataService<T> service, HandleExceptionArgs args)
        {
            Parent.Add(new APICallLogEntry(service.GetType().Name + "." + "HandleException",
                new Dictionary<string, string>()
                {
                    { "ExceptionType", args.Exception.GetType().ToString() },
                    { "ExceptionMessage", args.Exception.Message },
                    { "ResponseContentType", args.ResponseContentType.ToString() },
                    { "ResponseStatusCode", args.ResponseStatusCode.ToString() },
                    { "ResponseWritten", args.ResponseWritten.ToString() },
                    { "UseVerboseErrors", args.UseVerboseErrors.ToString() }
                }));
        }

        public void OnStartProcessingRequest<T>(DataService<T> service, ProcessRequestArgs args)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "IsBatchOperation", args.IsBatchOperation.ToString() },
                { "RequestUri", args.RequestUri.ToString() }
            };
#if !ASTORIA_PRE_V2
            parameters.Add("AbsoluteRequestUri", args.OperationContext.AbsoluteRequestUri.ToString());
            parameters.Add("AbsoluteServiceUri", args.OperationContext.AbsoluteServiceUri.ToString());
            parameters.Add("IsBatchRequest", args.OperationContext.IsBatchRequest.ToString());
            parameters.Add("RequestMethod", args.OperationContext.RequestMethod.ToUpperInvariant());
            // TODO: headers?
#endif
            Parent.Add(new APICallLogEntry(service.GetType().Name + "." + "OnStartProcessingRequest", parameters));
        }

        public void InitializeService(string typeName)
        {
            Parent.Add(typeName + ".InitializeService");
        }

        public void QueryInterceptor<T>(DataService<T> service, string methodName)
        {
            MethodInfo method = service.GetType().GetMethod(methodName);
            if (method == null)
                throw new ArgumentException("Could not find method '" + methodName + "' on service of type '" + service.GetType().FullName + "'");
            Parent.Add(service.GetType().GetMethod(methodName));
        }

        public void ChangeInterceptor<T>(DataService<T> service, string methodName, object o, UpdateOperations action)
        {
            MethodInfo method = service.GetType().GetMethod(methodName);
            if (method == null)
                throw new ArgumentException("Could not find method '" + methodName + "' on service of type '" + service.GetType().FullName + "'");
            Parent.Add(method, Parent.Serialize(o), action.ToString());
        }
    }
    #endregion

    #region IExpandProvider
#pragma warning disable 618 // Disable "obsolete" warning for the IExpandProvider interface. Used for backwards compatibilty.
    public class IExpandProviderCallLog : InterfaceCallLog<IExpandProvider>
    {
        internal IExpandProviderCallLog(APICallLog parent)
            : base(parent)
        {
        }

        public void ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
        {
            Add("ApplyExpansions", Parent.Serialize(queryable), string.Join(",", expandPaths.Select(esc => string.Join("/", esc.Select(es => es.Name).ToArray())).ToArray()));
        }
    }
#pragma warning restore 618
    #endregion

    #region IDataServiceHost
    public class IDataServiceHostCallLog : InterfaceCallLog<IDataServiceHost>
    {
        internal IDataServiceHostCallLog(APICallLog parent)
            : base(parent)
        {
        }

        #region IDataServiceHost Members
        public void AbsoluteRequestUri()
        {
            this.AddGetProperty("AbsoluteRequestUri");
        }

        public void AbsoluteServiceUri()
        {
            this.AddGetProperty("AbsoluteServiceUri");
        }

        public void GetQueryStringItem(string item)
        {
            this.Add("GetQueryStringItem", item);
        }

        public void ProcessException(HandleExceptionArgs args)
        {
            Parent.Add(new APICallLogEntry("IDataServiceHost.ProcessException",
                new Dictionary<string, string>()
                {
                    { "ExceptionType", args.Exception.GetType().ToString() },
                    { "ExceptionMessage", args.Exception.Message },
                    { "ResponseContentType", args.ResponseContentType.ToString() },
                    { "ResponseStatusCode", args.ResponseStatusCode.ToString() },
                    { "ResponseWritten", args.ResponseWritten.ToString() },
                    { "UseVerboseErrors", args.UseVerboseErrors.ToString() }
                }));
        }

        public void RequestAccept()
        {
            this.AddGetProperty("RequestAccept");
        }

        public void RequestAcceptCharSet()
        {
            this.AddGetProperty("RequestAcceptCharSet");
        }

        public void RequestContentType()
        {
            this.AddGetProperty("RequestContentType");
        }

        public void RequestHttpMethod()
        {
            this.AddGetProperty("RequestHttpMethod");
        }

        public void RequestIfMatch()
        {
            this.AddGetProperty("RequestIfMatch");
        }

        public void RequestIfNoneMatch()
        {
            this.AddGetProperty("RequestIfNoneMatch");
        }

        public void RequestMaxVersion()
        {
               this.AddGetProperty("RequestMaxVersion");
        }

        public void RequestStream()
        {
               this.AddGetProperty("RequestStream");
        }

        public void RequestVersion()
        {
               this.AddGetProperty("RequestVersion");
        }

        public string ResponseCacheControl
        {
            get
            {
                   this.AddGetProperty("ResponseCacheControl");
                return null;
            }
            set
            {
                   this.AddSetProperty("ResponseCacheControl", value);
            }
        }

        public string ResponseContentType
        {
            get
            {
                   this.AddGetProperty("ResponseContentType");
                return null;
            }
            set
            {
                   this.AddSetProperty("ResponseContentType", value);
            }
        }

        public string ResponseETag
        {
            get
            {
                this.AddGetProperty("ResponseETag");
                return null;
            }
            set
            {
                this.AddSetProperty("ResponseETag", value);
            }
        }

        public string ResponseLocation
        {
            get
            {
                this.AddGetProperty("ResponseLocation");
                return null;
            }
            set
            {
                this.AddSetProperty("ResponseLocation", value);
            }
        }

        public int ResponseStatusCode
        {
            get
            {
                this.AddGetProperty("ResponseStatusCode");
                return 0;
            }
            set
            {
                this.AddSetProperty("ResponseStatusCode", value);
            }
        }

        public void ResponseStream()
        {
            this.AddGetProperty("ResponseStream");
        }

        public string ResponseVersion
        {
            get
            {
                this.AddGetProperty("ResponseVersion");
                return null;
            }
            set
            {
                this.AddSetProperty("ResponseVersion", value);
            }
        }

        #endregion
    }
    #endregion

    #region Stream
    public class StreamCallLog
    {
        private APICallLog parent;
        internal StreamCallLog(APICallLog parent)
        {
            this.parent = parent;
        }

        public void CanRead()
        {
            parent.Add("System.IO.Stream.CanRead");
        }

        public void CanWrite()
        {
            parent.Add("System.IO.Stream.CanWrite");
        }

        public void Read(byte[] buffer, int offset, int count)
        {
            parent.Add("System.IO.Stream.Read",
                "buffer", "byte[" + buffer.Length + "]",
                "offset", offset.ToString(),
                "count", count.ToString());
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            parent.Add("System.IO.Stream.Write",
                "buffer", Convert.ToBase64String(buffer, offset, count),
                "offset", offset.ToString(),
                "count", count.ToString());
        }

        public void Close()
        {
            parent.Add("System.IO.Stream.Close");
        }

        public void Dispose()
        {
            parent.Add("System.IO.Stream.Dispose");            
        }
    }
    #endregion

#if !ASTORIA_PRE_V2
    #region IDataServiceUpdateProvider
    public class IDataServiceUpdateProviderCallLog : InterfaceCallLog<Microsoft.OData.Service.Providers.IDataServiceUpdateProvider>
    {
        internal IDataServiceUpdateProviderCallLog(APICallLog parent)
            : base(parent)
        {
        }

        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            string check = (checkForEquality.HasValue) ? checkForEquality.Value.ToString() : "null";
            
            if (concurrencyValues != null)
            {
                List<string> arguments = new List<string>() 
                { 
                    "resourceCookie", resourceCookie.GetType().ToString(), 
                    "checkForEquality", check 
                };
                arguments.AddRange(concurrencyValues.SelectMany(p => new string[] { p.Key, p.Value == null ? "null" : p.Value.ToString() }));
                Parent.Add(new APICallLogEntry("IDataServiceUpdateProvider.SetConcurrencyValues", arguments.ToArray()));
            }
            else
            {
                Add("SetConcurrencyValues", resourceCookie.GetType().ToString(), check, "null");
            }
        }
    }
    #endregion

    #region IDataServiceMetadataProvider
    public class IDataServiceMetadataProviderCallLog : InterfaceCallLog<DSP.IDataServiceMetadataProvider>
    {
        internal IDataServiceMetadataProviderCallLog(APICallLog parent)
            : base(parent)
        {
        }

        private string Serialize(DSP.ResourceSet set)
        {
            return set.Name;
        }

        private string Serialize(DSP.ResourceType type)
        {
            return type.Namespace + "." + type.Name;
        }

        private string Serialize(DSP.ResourceProperty property)
        {
            return property.Name;
        }

        private string Serialize(DSP.ServiceOperation serviceOp)
        {
            return serviceOp.Name;
        }

        #region IDataServiceMetadataProvider Members

        public void ContainerName()
        {
            AddGetProperty("ContainerName");
        }

        public void ContainerNamespace()
        {
            AddGetProperty("ContainerNamespace");
        }

        public void GetDerivedTypes(DSP.ResourceType resourceType)
        {
            Add("GetDerivedTypes", Serialize(resourceType));
        }

        public void GetResourceAssociationSet(DSP.ResourceSet resourceSet, DSP.ResourceType resourceType, DSP.ResourceProperty resourceProperty)
        {
            Add("GetResourceAssociationSet", Serialize(resourceSet), Serialize(resourceType), Serialize(resourceProperty));
        }

        public void HasDerivedTypes(DSP.ResourceType resourceType)
        {
            Add("HasDerivedTypes", Serialize(resourceType));
        }

        public void ResourceSets()
        {
            AddGetProperty("ResourceSets");
        }

        public void ServiceOperations()
        {
            AddGetProperty("ServiceOperations");
        }

        public void TryResolveResourceSet(string name)
        {
            Add("TryResolveResourceSet", name);
        }

        public void TryResolveResourceType(string name)
        {
            Add("TryResolveResourceType", name);
        }

        public void TryResolveServiceOperation(string name)
        {
            Add("TryResolveServiceOperation", name);
        }

        public void Types()
        {
            AddGetProperty("Types");
        }

        #endregion
    }
    #endregion

    #region IDataServiceQueryProvider
    public class IDataServiceQueryProviderCallLog : InterfaceCallLog<DSP.IDataServiceQueryProvider>
    {
        internal IDataServiceQueryProviderCallLog(APICallLog parent)
            : base(parent)
        {
        }

        private string Serialize(DSP.ResourceSet set)
        {
            return set.Name;
        }

        private string Serialize(DSP.ResourceType type)
        {
            return type.Namespace + "." + type.Name;
        }

        private string Serialize(DSP.ResourceProperty property)
        {
            return property.Name;
        }

        private string Serialize(DSP.ServiceOperation serviceOp)
        {
            return serviceOp.Name;
        }

        #region IDataServiceQueryProvider Members

        public object CurrentDataSource
        {
            get
            {
                AddGetProperty("CurrentDataSource");
                return null;
            }
            set
            {
                AddSetProperty("CurrentDataSource", value);
            }
        }

        public void GetOpenPropertyValue(object target, string propertyName)
        {
            Add("GetOpenPropertyValue", Parent.Serialize(target), propertyName);
        }

        public void GetOpenPropertyValues(object target)
        {
            Add("GetOpenPropertyValues", Parent.Serialize(target));
        }

        public void GetPropertyValue(object target, DSP.ResourceProperty resourceProperty)
        {
            Add("GetPropertyValue", Parent.Serialize(target), Serialize(resourceProperty));
        }

        public void GetQueryRootForResourceSet(DSP.ResourceSet resourceSet)
        {
            Add("GetQueryRootForResourceSet", Serialize(resourceSet));
        }

        public void GetResourceType(object target)
        {
            Add("GetResourceType", Parent.Serialize(target));
        }

        public void InvokeServiceOperation(DSP.ServiceOperation serviceOperation, object[] parameters)
        {
            Add("InvokeServiceOperation", Serialize(serviceOperation), Parent.Serialize(parameters));
        }

        public void IsNullPropagationRequired()
        {
            AddGetProperty("IsNullPropagationRequired");
        }

        #endregion
    }
    #endregion

    #region IDataServiceStreamProvider
    public class IDataServiceStreamProviderCallLog : InterfaceCallLog<DSP.IDataServiceStreamProvider>
    {
        internal IDataServiceStreamProviderCallLog(APICallLog parent)
            : base(parent)
        { }

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.DeleteStream",
                "entity", this.Parent.Serialize(entity),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetReadStream",
                "entity", this.Parent.Serialize(entity),
                "etag", (etag == null ? "null" : etag),
                "checkETagForEquality", (checkETagForEquality.HasValue ? checkETagForEquality.Value.ToString() : "null"),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetReadStreamUri",
                "entity", this.Parent.Serialize(entity),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetStreamContentType",
                "entity", this.Parent.Serialize(entity),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetStreamETag",
                "entity", this.Parent.Serialize(entity),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetWriteStream",
                "entity", this.Parent.Serialize(entity),
                "etag", (etag == null ? "null" : etag),
                "checkETagForEquality", (checkETagForEquality.HasValue ? checkETagForEquality.Value.ToString() : "null"),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.ResolveType",
                "entitySetName", (entitySetName == null ? "null" : entitySetName),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void StreamBufferSize()
        {
            AddGetProperty("StreamBufferSize");
        }

        public void Dispose()
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.Dispose", new KeyValuePair<string, string>[0]));
        }
    }
    #endregion

    #region DataServiceProcessingPipeline
    public class DataServiceProcessingPipelineCallLog
    {
        private APICallLog Parent;

        internal DataServiceProcessingPipelineCallLog(APICallLog parent)
        {
            Parent = parent;
        }

        public void ProcessingRequest(object sender, DataServiceProcessingPipelineEventArgs e)
        {
            this.Parent.Add("DataServiceProcessingPipeline.ProcessingRequest",
                "sender", sender.GetType().Name,
                "AbsoluteRequestUri", e.OperationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", e.OperationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", e.OperationContext.IsBatchRequest.ToString(),
                "RequestMethod", e.OperationContext.RequestMethod.ToUpperInvariant());
            // TODO: headers?
        }

        public void ProcessingChangeset(object sender, EventArgs e)
        {
            this.Parent.Add("DataServiceProcessingPipeline.ProcessingChangeset",
                "sender", sender.GetType().Name,
                "e", e.GetType().ToString());
        }

        public void ProcessedRequest(object sender, DataServiceProcessingPipelineEventArgs e)
        {
            this.Parent.Add("DataServiceProcessingPipeline.ProcessedRequest",
                "sender", sender.GetType().Name,
                "AbsoluteRequestUri", e.OperationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", e.OperationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", e.OperationContext.IsBatchRequest.ToString(),
                "RequestMethod", e.OperationContext.RequestMethod.ToUpperInvariant());
            // TODO: headers?
        }

        public void ProcessedChangeset(object sender, EventArgs e)
        {
            this.Parent.Add("DataServiceProcessingPipeline.ProcessedChangeset",
                "sender", sender.GetType().Name,
                "e", e.GetType().ToString());
        }
    }
    #endregion

    #region IDataServicePagingProvider
    public class IDataServicePagingProviderCallLog : InterfaceCallLog<DSP.IDataServicePagingProvider>
    {
        internal IDataServicePagingProviderCallLog(APICallLog parent)
            : base(parent)
        { }

        #region IDataServicePagingProvider Members
        public void GetContinuationToken(IEnumerator enumerator)
        {
            //For now, the type of the enumerator is too hard to predict, so we won't track it
            //Add("GetContinuationToken", enumerator.GetType().ToString());
            Parent.Add("GetContinuationToken");
        }

        public void SetContinuationToken(IQueryable query, DSP.ResourceType resourceType, object[] continuationToken)
        {
            if (continuationToken == null)
                Add("SetContinuationToken", query.GetType().ToString(), resourceType.Namespace + "." + resourceType.Name, "null");
            else
            {
                Add("SetContinuationToken", query.GetType().ToString(), resourceType.Namespace + "." + resourceType.Name,
                    string.Join(", ", continuationToken.Select(o => Parent.Serialize(o)).ToArray()));
            }
        }
        #endregion
    }
    #endregion

    #region IDataServiceHost2
    public class IDataServiceHost2CallLog : InterfaceCallLog<IDataServiceHost2>
    {
        internal IDataServiceHost2CallLog(APICallLog parent)
            : base(parent)
        {
        }

        public void RequestHeaders()
        {
            this.AddGetProperty("RequestHeaders");
        }

        public void ResponseHeaders()
        {
            this.AddGetProperty("ResponseHeaders");
        }
    }
    #endregion
#endif

#if !ASTORIA_PRE_V3
    #region IDataServiceStreamProvider2
    public class IDataServiceStreamProvider2CallLog : InterfaceCallLog<DSP.IDataServiceStreamProvider2>
    {
        internal IDataServiceStreamProvider2CallLog(APICallLog parent)
            : base(parent)
        { }

        private string Serialize(DSP.ResourceProperty streamProperty)
        {
            return streamProperty == null ? "null ResourceProperty" : streamProperty.Name;
        }

        public void GetReadStream(object entity, DSP.ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetReadStream",
                "entity", this.Parent.Serialize(entity),
                "streamProperty", this.Serialize(streamProperty),
                "etag", (etag == null ? "null" : etag),
                "checkETagForEquality", (checkETagForEquality.HasValue ? checkETagForEquality.Value.ToString() : "null"),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void GetReadStreamUri(object entity, DSP.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetReadStreamUri",
                "entity", this.Parent.Serialize(entity),
                "streamProperty", this.Serialize(streamProperty),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void GetStreamContentType(object entity, DSP.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetStreamContentType",
                "entity", this.Parent.Serialize(entity),
                "streamProperty", this.Serialize(streamProperty),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void GetStreamETag(object entity, DSP.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetStreamETag",
                "entity", this.Parent.Serialize(entity),
                "streamProperty", this.Serialize(streamProperty),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }

        public void GetWriteStream(object entity, DSP.ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetWriteStream",
                "entity", this.Parent.Serialize(entity),
                "streamProperty", this.Serialize(streamProperty),
                "etag", (etag == null ? "null" : etag),
                "checkETagForEquality", (checkETagForEquality.HasValue ? checkETagForEquality.Value.ToString() : "null"),
                "AbsoluteRequestUri", operationContext.AbsoluteRequestUri.ToString(),
                "AbsoluteServiceUri", operationContext.AbsoluteServiceUri.ToString(),
                "IsBatchRequest", operationContext.IsBatchRequest.ToString(),
                "RequestMethod", operationContext.RequestMethod.ToUpperInvariant()));
        }
    }
    #endregion
#endif
    #endregion
}
