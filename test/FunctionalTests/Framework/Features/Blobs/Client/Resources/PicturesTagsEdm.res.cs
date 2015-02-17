//---------------------------------------------------------------------
// <copyright file="PicturesTagsEdm.res.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

[Sniffer]
// copied from the BlobsEdm.res.cs
public partial class DataServiceClass : IServiceProvider 
{
    public static void InitializeService(IDataServiceConfiguration config) 
    {
        //entities
        config.SetEntitySetAccessRule("Pictures", EntitySetRights.All);
        config.SetEntitySetAccessRule("Tags", EntitySetRights.WriteAppend | EntitySetRights.AllRead);
        config.UseVerboseErrors = true;

        //operations
        config.SetServiceOperationAccessRule("*", ServiceOperationRights.ReadSingle);
    }

    object IServiceProvider.GetService(Type serviceType) 
    {
        if (serviceType == typeof(IDataServiceStreamProvider)) return new PictureStreamProvider(this.CurrentDataSource);
        return null;
    }

    [WebGet]
    [SingleResult]
    public int ObtainMRELock() 
    {
        lock (BlockedFileStream.lockDictionary) 
        {
            BlockedFileStream.globalLockCounter++;
            BlockedFileStream.lockDictionary.Add(BlockedFileStream.globalLockCounter, new ManualResetEvent(false));
            return BlockedFileStream.globalLockCounter;
        }
    }

    [WebGet]
    [SingleResult]
    public int SignalMRELock(int lockId) 
    {
        lock (BlockedFileStream.lockDictionary) 
        {
            BlockedFileStream.lockDictionary[lockId].Set();
        }
        return lockId;
    }

    private int GetIdFromHeaders() 
    {
        int id;
        WebOperationContext wctx = WebOperationContext.Current;
        bool success = Int32.TryParse(wctx.IncomingRequest.Headers["LogIndex"], out id);
        return success ? id : -1;
    }

    [WebGet]
    [SingleResult]
    public int IncrementLog(string desc) 
    {
        TestOperation to = new TestOperation { Date = DateTime.Now, Name = desc };
        this.CurrentDataSource.AddObject("TestOperations", to);
        this.CurrentDataSource.SaveChanges();
        return to.Id;
    }

    [WebGet]
    [SingleResult]
    public XElement GetLog() 
    {
        int id = GetIdFromHeaders();
        PTEntities ctx = this.CurrentDataSource;
        TestOperation t = ctx.TestOperations.Include("LogEntries.LogEntryHeaders").FirstOrDefault(to => (to.Id == id));
        if (t == null) return null;
        XElement retVal = new XElement("TestOperation", new XAttribute("id", id), new XAttribute("desc", t.Name));
        foreach (var le in t.LogEntries) 
        {
            XElement xle = new XElement(le.Verb, new XAttribute("Uri", le.URI));
            retVal.Add(xle);
            foreach (var leh in le.LogEntryHeaders) 
            {
                xle.Add(new XElement("Header", new XAttribute("key", leh.Header), new XAttribute("value", leh.Value)));
            }
        }
        return retVal;
    }
}

class BlockedFileStream : System.Data.Test.Astoria.FullTrust.TrustedFileStream 
{
    private readonly int bufferSize, lockId;
    private int counter = 0;
    private ManualResetEvent mre;

    private static int multiplayer = 3;
    public static Dictionary<int, ManualResetEvent> lockDictionary = new Dictionary<int, ManualResetEvent>();
    public static int globalLockCounter = 0;

    public BlockedFileStream(string fileName, FileMode mode, FileAccess access, FileShare share, int serviceBufferSize, int lockId)
        : base(fileName, mode, access, share) 
    {
        this.bufferSize = serviceBufferSize;
        this.lockId = lockId;
        this.mre = lockDictionary[lockId];
        if (mre == null) 
            throw new ArgumentException("Did not find lock in the dictionary", "lockId");
        if (bufferSize * multiplayer > this.Length) 
            throw new ArgumentException("File too small for buffer!", "serviceBufferSize");
    }

    public override int Read(byte[] array, int offset, int count) 
    {
        int read = base.Read(array, offset, count < bufferSize ? count : bufferSize);
        counter += read;
        if (counter > (multiplayer * bufferSize) && mre != null) 
        {
            if (mre.WaitOne()) 
            {
                lock (BlockedFileStream.lockDictionary) 
                {
                    lockDictionary.Remove(lockId);
                }
                (mre as IDisposable).Dispose();
                mre = null;
            }
        }
        return read;
    }
}

public class PictureStreamProvider : IDataServiceStreamProvider 
{
    private readonly string DataDirName;
    private readonly PTEntities entityContext;

    public PictureStreamProvider(PTEntities entityContext) 
    {
        this.entityContext = entityContext;

        string assemblyPath = System.Data.Test.Astoria.FullTrust.TrustedMethods.GetAssemblyLocation(Assembly.GetExecutingAssembly());
        assemblyPath = Path.GetDirectoryName(assemblyPath);
        string appName = assemblyPath.Split('/', '\\').Last();
        DataDirName = Path.Combine(System.Data.Test.Astoria.FullTrust.TrustedMethods.GetTempPath(), Path.Combine("BlobClientTestFiles", appName));

        // copy the static data files into temp directory 
        string sourceDir = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/BlobClientTestFiles/") ?? 
            Path.Combine(assemblyPath, @"App_Data\BlobClientTestFiles");

        System.Data.Test.Astoria.FullTrust.TrustedMethods.EnsureDirectoryExists(DataDirName);
        System.Data.Test.Astoria.FullTrust.TrustedMethods.CopyDirectoryRecursively(sourceDir, DataDirName);
    }

    #region Helpers
    private string DataPath(string fileName) 
    {
        return Path.Combine(DataDirName, fileName);
    }

    private IDictionary<string, string> GetParamsTable( DataServiceOperationContext host) 
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        string absServicePath = host.AbsoluteServiceUri.ToString().TrimEnd(new char[] { ' ', '/' });
        string absServiceFolder = absServicePath.Substring(0, absServicePath.LastIndexOf('/') + 1);
        dict.Add("${AbsServicePath}", absServicePath);
        dict.Add("${AbsServiceFolder}", absServiceFolder);
        return dict;
    }
    #endregion

    #region IDataServiceStreamProvider Members
    void IDataServiceStreamProvider.DeleteStream(object entity,  DataServiceOperationContext host) 
    {
        //Debug.Assert(entity is InternalPicture || entity is ExternalPicture, "Unexpected entity type!");
        if (entity is InternalPicture) 
        {
            InternalPicture pict = entity as InternalPicture;

            FileStorage fs = pict.FileStorage ?? entityContext.FileStorage.FirstOrDefault(f => f.Picture.Id == pict.Id);

            if (fs != null) 
            {
                System.Data.Test.Astoria.FullTrust.TrustedMethods.DeleteFileIfExists(DataPath(fs.Location));
                entityContext.DeleteObject(fs);
            }
        }
    }

    Stream IDataServiceStreamProvider.GetReadStream(object entity, string Etag, bool? checkETagForEquality,  DataServiceOperationContext host) 
    {
        //Debug.Assert(entity is InternalPicture || entity is ExternalPicture, "Unexpected entity type!");
        if (entity is InternalPicture) 
        {
            InternalPicture pict = entity as InternalPicture;
            this.LoadFileStorageIfItsNotLoaded(pict, "PT.FK_FileStorage_InternalPictures1", "FileStorage");

            if (pict.FileStorage.ContentDisposition != null) host.ResponseHeaders.Add("Content-Disposition", pict.FileStorage.ContentDisposition);
            if (host.RequestHeaders.AllKeys.Contains("B4C-Sleep")) 
            {
                string[] slData = host.RequestHeaders["B4C-Sleep"].Split(',', ';', ' ');
                int lockId = Int32.Parse(slData[0]);
                int buffSize = Int32.Parse(slData[1]);
                return new BlockedFileStream(DataPath(pict.FileStorage.Location), FileMode.Open, FileAccess.Read, FileShare.Read, buffSize, lockId);
            }
            return new System.Data.Test.Astoria.FullTrust.TrustedFileStream(DataPath(pict.FileStorage.Location), FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        else if (entity is ExternalPicture) 
        {
            return new MemoryStream(Encoding.UTF8.GetBytes("This is a replacement for external entity link."), false);
        }
        else 
        {
            throw new NotSupportedException("Unexpected entity type!");
        }
    }

    private string replaceParams(string data, IDictionary<string, string> paramsTable) 
    {
        foreach (var p in paramsTable) 
        {
            data = data.Replace(p.Key, p.Value);
        }
        return data;
    }

    Uri IDataServiceStreamProvider.GetReadStreamUri(object entity,  DataServiceOperationContext host) 
    {
        Uri uri = null;
        if (entity is ExternalPicture) 
        {
            ExternalPicture pict = entity as ExternalPicture;
            if (!string.IsNullOrEmpty(pict.URL)) 
            {
                string url = pict.URL;
                if (url.Contains("${")) 
                {
                    url = replaceParams(url, GetParamsTable(host));
                }
                uri = new Uri(url, UriKind.RelativeOrAbsolute);
            }
        }
        return uri;
    }

    string IDataServiceStreamProvider.GetStreamContentType(object entity,  DataServiceOperationContext host) 
    {
        if (entity is InternalPicture)
        {
            InternalPicture pict = entity as InternalPicture;

            this.LoadFileStorageIfItsNotLoaded(pict, "PT.FK_FileStorage_InternalPictures1", "FileStorage");
            
            return pict.FileStorage.ContentType;
        }
        else if (entity is ExternalPicture) 
        {
            ExternalPicture pict = entity as ExternalPicture;
            return string.IsNullOrEmpty(pict.ContentType) ? "text/plain" : pict.ContentType;
        }
        else 
        {
            throw new NotSupportedException("Unexpected entity type!");
        }
    }

    private void ThrowExceptionIfNotPocoWithProxy(InternalPicture source, Exception exception)
    {
        if (typeof(InternalPicture) == source.GetType())
        {
            throw exception;
        }
    }

    private void LoadFileStorageIfItsNotLoaded(InternalPicture source, string relationshipName, string targetRoleName)
    {
        if (source is System.Data.Objects.DataClasses.IEntityWithRelationships)
        {
            System.Data.Objects.DataClasses.RelationshipManager relationshipManager = ((System.Data.Objects.DataClasses.IEntityWithRelationships)source).RelationshipManager;
            var relatedReference = relationshipManager.GetRelatedReference<FileStorage>(relationshipName, targetRoleName);
            if (!relatedReference.IsLoaded)
            {
                try
                {
                    relatedReference.Load();
                }
                catch (System.Data.EntityException ex)
                {
                    // expects a first chance exception for PocoWithProxy
                    this.ThrowExceptionIfNotPocoWithProxy(source, ex);
                }
                catch (System.InvalidOperationException ex)
                {
                    // expects a different exception for PocoWithProxy if NoTracking
                    this.ThrowExceptionIfNotPocoWithProxy(source, ex);
                }
            }
        }
        else
        {
            // find FileStorage for PocoWithoutProxy
            foreach (FileStorage fs in this.entityContext.FileStorage)
            {
                if (fs.Id == source.Id)
                {
                    source.FileStorage = fs;
                    return;
                }
            }
        }
    }

    string IDataServiceStreamProvider.GetStreamETag(object entity, DataServiceOperationContext host) 
    {
        if (entity is ExternalPicture) 
        {
            ExternalPicture pict = entity as ExternalPicture;
            return pict.BlobETag;
        }
        if (entity is InternalPicture) 
        {
            InternalPicture pict = entity as InternalPicture;
            // ETag value is timestamp of the storage file
            if (this.GetEntityState(pict) != EntityState.Added)
            {
                this.LoadFileStorageIfItsNotLoaded(pict, "PT.FK_FileStorage_InternalPictures1", "FileStorage");

                if (pict.FileStorage == null)
                    return null;

                var creationTime = System.Data.Test.Astoria.FullTrust.TrustedMethods.GetFileCreationTime(DataPath(pict.FileStorage.Location));
                return string.Concat("\"", creationTime.ToString("dd MMMM yyyy hh:mm:ss.ffffff", System.Globalization.DateTimeFormatInfo.InvariantInfo), "\"");
            }
        }
        return null;
    }

    private EntityState GetEntityState(object entity)
    {
        System.Data.Objects.ObjectStateEntry ose;
        if (this.entityContext.ObjectStateManager.TryGetObjectStateEntry(entity, out ose))
        {
            return ose.State;
        }
        else
        {
            return EntityState.Detached;
        }
    }

    Stream IDataServiceStreamProvider.GetWriteStream(object entity, string Etag, bool? checkETagForEquality,  DataServiceOperationContext host) 
    {
        //Debug.Assert(entity is InternalPicture, "Unexpected entity type!");
        InternalPicture pict = entity as InternalPicture;
        if (pict != null) 
        {
            string filename = host.RequestHeaders.AllKeys.Contains("Slug") ? host.RequestHeaders["Slug"] : string.Format("{0}.txt", Guid.NewGuid());
            if (filename.Contains(@"FAIL")) 
                throw new InvalidOperationException("'FAIL' in Slug :: Test hook for exception!");

            string contentType = host.RequestHeaders.AllKeys.Contains("Content-Type") ? host.RequestHeaders["Content-Type"] : null;

            if (this.GetEntityState(pict) != EntityState.Added)
            {
                this.LoadFileStorageIfItsNotLoaded(pict, "PT.FK_FileStorage_InternalPictures1", "FileStorage");
                if (pict.FileStorage != null) {

                    if (checkETagForEquality != null)
                    {
                        // the value of checkETagForEquality should be "True" (if-match header). This code does not understand "False" value (if-none-match header).
                        if (!(bool)checkETagForEquality) 
                            throw new NotSupportedException("The service does not support if-none-match header !");

                        // if etag does not match, return 412 -> Precondition failed
                        // ETag value is timestamp of the storage file
                        var creationTime = System.Data.Test.Astoria.FullTrust.TrustedMethods.GetFileCreationTime(DataPath(pict.FileStorage.Location));
                        string fileCreationTimeStamp = string.Concat("\"", creationTime.ToString("dd MMMM yyyy hh:mm:ss.ffffff", System.Globalization.DateTimeFormatInfo.InvariantInfo), "\"");
                        if (fileCreationTimeStamp != Etag) 
                        {
                            throw new DataServiceException(412, string.Format("Etag values does not match, expected: {0}, actual: {1}", Etag, fileCreationTimeStamp));
                        }
                    }

                    pict.FileStorage.ContentType = contentType;
                    pict.FileStorage.Location = filename;
                }
                else 
                {
                    // else - trouble, incosistent data - 500(internal error)
                    throw new DataServiceException("Inconsistent data found, giving up!");
                }
            }
            else
            {
                FileStorage fs = new FileStorage { ContentType = contentType, Location = filename, Picture = pict };
                entityContext.AddObject("FileStorage", fs);
            }

            return new System.Data.Test.Astoria.FullTrust.TrustedFileStream(DataPath(filename), FileMode.Create, FileAccess.Write, FileShare.Write);
        }
        else 
        {
            throw new NotSupportedException("Unexpected entity type!");
        }
    }

    string IDataServiceStreamProvider.ResolveType(string entitySetName, DataServiceOperationContext host) 
    {
        return typeof(InternalPicture).FullName;
    }

    int IDataServiceStreamProvider.StreamBufferSize 
    {
        get { return 0; }
    }

    #endregion
}

class SnifferInspector : IDispatchMessageInspector {

    public static readonly string LogIndexHeader = "LogIndex";
    public static readonly string[] ignorePoints = new string[] { ".svc", "LogEntry", "LogEntryHeader", "TestOperation", "IncrementLog", "$metadata", "GetLog" };

    public class MyCorrelation {
        public Uri Uri { get; set; }
        public string LogIndex { get; set; }
    }

    #region IDispatchMessageInspector Members
    public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext) {
        Uri originalURI = null;
        if (request.Properties.ContainsKey("OriginalHttpRequestUri"))
        {
            originalURI = request.Properties["OriginalHttpRequestUri"] as Uri;
        }
        else
        {
            //When running in WCF ServiceHost mode , the OriginalHttpRequestUri is inaccissible as the host does not support it 
            originalURI = request.Properties.Via;
        }

        #region Log only interesting operations
        string lastSegment = originalURI.Segments.Last();
        if (ignorePoints.Any(fo => lastSegment.Contains(fo))) return null;
        #endregion

        HttpRequestMessageProperty httpmsg = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
        if (httpmsg != null && httpmsg.Headers != null && httpmsg.Headers.AllKeys.Contains(LogIndexHeader)) {


            PTEntities ctx = new PTEntities();
            int LogIndex;
            if (!Int32.TryParse(httpmsg.Headers[LogIndexHeader], out LogIndex)) return null;

            TestOperation t = ctx.TestOperations.FirstOrDefault(to => (to.Id == LogIndex));
            if (t == null) return null;

            LogEntry le = new LogEntry { TestOperation = t, URI = originalURI.ToString(), Verb = httpmsg.Method };
            ctx.AddObject("LogEntries", le);

            // skip loging of the logIndex header
            foreach (var key in httpmsg.Headers.AllKeys.Where(k => !k.Equals(LogIndexHeader, StringComparison.OrdinalIgnoreCase))) {
                ctx.AddObject("LogEntryHeaders", new LogEntryHeader { LogEntry = le, Header = key, Value = httpmsg.Headers[key] });
            }
            ctx.SaveChanges();
            return new MyCorrelation { Uri = originalURI, LogIndex = httpmsg.Headers[LogIndexHeader] };
        }
        return null;
    }

    public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState) {
        if (correlationState != null && correlationState is MyCorrelation) {
            MyCorrelation myCor = correlationState as MyCorrelation;
            HttpResponseMessageProperty httpmsg = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];
            if (httpmsg != null && httpmsg.Headers != null) {
                PTEntities ctx = new PTEntities();
                int LogIndex;
                if (!Int32.TryParse(myCor.LogIndex, out LogIndex)) return;

                TestOperation t = ctx.TestOperations.FirstOrDefault(to => (to.Id == LogIndex));
                if (t == null) return;

                LogEntry le = new LogEntry { TestOperation = t, URI = myCor.Uri.ToString(), Verb = "RESPONSE" };
                ctx.AddObject("LogEntries", le);

                // skip loging of the logIndex header
                foreach (var key in httpmsg.Headers.AllKeys.Where(k => !k.Equals(LogIndexHeader, StringComparison.OrdinalIgnoreCase))) {
                    ctx.AddObject("LogEntryHeaders", new LogEntryHeader { LogEntry = le, Header = key, Value = httpmsg.Headers[key] });
                }
                ctx.SaveChanges();
            }
        }
    }
    #endregion
}

[AttributeUsage(AttributeTargets.Class)]
public class SnifferAttribute : Attribute, IServiceBehavior {
    #region IServiceBehavior Members
    void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) {
    }

    void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
        foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers) {
            foreach (EndpointDispatcher ed in cd.Endpoints) {
                ed.DispatchRuntime.MessageInspectors.Add(new SnifferInspector());
            }
        }
    }

    void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
    }
    #endregion
}
