//---------------------------------------------------------------------
// <copyright file="WebDataContextWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.OData.Client;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using EDMMetadataExtensions;
using Microsoft.Test.ModuleCore;

namespace TestSL
{
    public class DataServiceRequestProxy
    {
        public string ElementType { get; set; }
        public string RequestUri { get; set; }
        public long ActualCount { get; set; }
    }
    public class Message
    {
        public int MessageID { get; set; }
        public int InstanceID { get; set; }
        public string Method { get; set; }
        public string ReturnValue { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public string Parameter3 { get; set; }
        public string Parameter4 { get; set; }
        public string Parameter5 { get; set; }
        public string Parameter6 { get; set; }
        public string Exception { get; set; }
        public string WhoSentMe { get; set; }
        public bool IsResultAsync { get; set; }
        public bool CountMode { get; set; }
        public bool CountAllPages { get; set; }
        public long ActualCount { get; set; }
    }
}

namespace System.Data.Test.Astoria
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using TestSL;
    using System.Security.Permissions;
    using System.Security;

    public enum ContextAction
    {
        CreateContext,
        AddObject,
        UpdateObject,
        DeleteObject,
        Attach,
        Detach,
        SaveChanges
    }
    public class ActionBag
    {
        public delegate void ActionAddedDelegate(ActionBag addedAction);

        public static event ActionAddedDelegate ActionAdded;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual bool ISSameAction(ActionBag otherAction)
        {
            return otherAction.Before == this.Before
                && otherAction.After == this.After
                && otherAction.Action == this.Action;
        }
        public override bool Equals(object obj)
        {
            ActionBag temp = obj as ActionBag;
            return temp.Before == this.Before
                && temp.After == this.After
                && temp.Action == this.Action
                && temp.Parameters.Equals(this.Parameters);

        }
        public List<ActionBag> _allActions;
        public ActionBag LastAction
        {
            get
            {
                return _allActions[_allActions.Count - 1];
            }
        }
        public List<ActionBag> AllActions
        {
            get
            {
                if (_allActions == null)
                {
                    _allActions = new List<ActionBag>();
                }
                return _allActions;
            }
        }

        public static ActionBag CreateActionBag(ContextAction ctxAction)
        {
            ActionBag thisACtion = new ActionBag()
            {
                Action = ctxAction
            };
            if (ActionAdded != null)
            {
                ActionAdded(thisACtion);
            }
            return thisACtion;

        }
        public bool Failed { get; set; }
        List<string> _parameters;
        public ContextAction Action { get; set; }
        public List<string> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new List<string>();
                }
                return _parameters;
            }
        }
        public object Entity;
        public EntityStates Before { get; set; }
        public EntityStates After { get; set; }


    }

    public class DataServiceMetadata
    {

        public static IEdmModel ServiceMetadata;
        public static List<IEdmEntityType> EntityTypes
        {
            get
            {
                return ServiceMetadata.SchemaElements.OfType<IEdmEntityType>().ToList();
            }
        }

        public static IEdmEntityType GetEntityType(object entityInstance)
        {
            return EntityTypes.Where(eType => eType.Name == entityInstance.GetType().Name).FirstOrDefault();
        }
        public static IEdmEntityType GetEntityType(string entityTypeName)
        {
            return EntityTypes.Where(eType => eType.Name == entityTypeName).FirstOrDefault();
        }
        public static List<IEdmEntitySet> EntitySets
        {
            get
            {
                IEnumerable<IEdmEntitySet> entitySets = ServiceMetadata.EntityContainer.EntitySets();
                return entitySets.ToList();
            }
        }
        public static IEdmEntityType GetEntityType(IEdmNavigationProperty association)
        {
            if (association.Type.IsCollection())
            {
                return association.Type.AsCollection().ElementType().AsEntity().EntityDefinition();
            }

            return association.Type.AsEntity().EntityDefinition();
        }
        public static void LoadServiceMetadata(string serviceUri)
        {

            Uri metadataUri = new Uri(serviceUri + "/$metadata");

            WebRequest metadataRequest = WebRequest.Create(metadataUri) as WebRequest;
            metadataRequest.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = metadataRequest.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
#if !ClientSKUFramework

                ServiceMetadata = MetadataUtil.IsValidMetadata(responseStream, null);
#endif
            }
        }
    }
    public class SilverlightRemote
    {
        public static SilverlightRemote Remoter;
        public static bool IsClientStarted = false;
        public static DataServiceContext SLDataService;
        public static bool HasRemote;
        private static Uri MessageUri = new Uri("Messages?$filter=WhoSentMe eq 'SL' and ( Method eq '" + "" + "'  or Method eq 'testfailed' )", UriKind.Relative);
        private static TestSL.Message responseMessage;
        private static string smessageUriTemplate = "Messages?$filter=WhoSentMe eq 'SL' and  ( Method eq '{0}'   or Method eq 'testfailed' )";
        internal static bool IsInitialized = false;
        private List<Message> _messagesTillNow = new List<Message>();
        class testIteration
        {
            public testIteration(int instanceID, string TestMethod, string[] Args)
            {
                InstanceID = instanceID;
                testMethod = TestMethod;
                args = Args;
            }
            public int InstanceID { get; set; }
            public string testMethod { get; set; }
            public string[] args { get; set; }
        }

        static public bool IsDebugEnabled = false;
        static public bool CrashDebug = false;

        private static Uri GetXDomainHost(Uri InDomainServiceUri)
        {
            string strXDomainServiceUri = InDomainServiceUri.OriginalString;
            string strMachineName = "http://" + Environment.MachineName.ToLower();
            if (strXDomainServiceUri.Contains("Aruba"))
            {
                strXDomainServiceUri = strXDomainServiceUri.Replace(strMachineName, "http://www.aruba1.com");
            }
            else if (strXDomainServiceUri.Contains("Northwind"))
            {
                strXDomainServiceUri = strXDomainServiceUri.Replace(strMachineName, "http://www.northwind1.com");
            }
            return new Uri(strXDomainServiceUri, UriKind.RelativeOrAbsolute);
        }

        public static void InitializeSilverlight(Uri webServiceUri)
        {
            Remoter = new SilverlightRemote();

            // Construct messaging service URI. 
            string trimmedURI = webServiceUri.OriginalString.TrimEnd('/');
            string validMQuri = trimmedURI.Substring(0, trimmedURI.LastIndexOf('/') + 1) + "AstoriaTestSilverlight.svc";

            if (IsDebugEnabled)
                validMQuri = "http://bpdtechfestjudging/SL/AstoriaTestSilverlight.svc/";

            // Create new data service context to interact with messaging service.
            SLDataService = new DataServiceContext(new Uri(validMQuri, UriKind.Absolute));
            SLDataService.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            HasRemote = true;
        }

        public static bool IsDuplicateInsert
        {
            get;
            set;
        }
        private List<object> instances = new List<object> { };
        private List<testIteration> TestIterations = new List<testIteration>();
        bool sendFailed = false;
        private void SendMessage(Message testMessage)
        {
            SLDataService.AddObject("Messages", testMessage);
            try
            {
                DataServiceResponse response = SLDataService.SaveChanges();
                #region Verify Result
                foreach (ChangeOperationResponse changset in response)
                {
                    // consume the changes
                    if (changset.Descriptor is Microsoft.OData.Client.EntityDescriptor)
                    {
                        if (changset.Error != null)
                        {
                            throw changset.Error;
                        }
                    }
                }
                #endregion
                AstoriaTestLog.TraceLine(AstoriaTraceLevel.Info, "Messaging Service URI :: " + SLDataService.BaseUri.AbsoluteUri.ToString());
                AstoriaTestLog.TraceLine(AstoriaTraceLevel.Info, "\r\nMethod :: " + testMessage.Method);

            }
            catch (Exception saveException)
            {
                AstoriaTestLog.TraceLine(AstoriaTraceLevel.Info, "Failed to Post Message due to :: " + saveException.ToString());
                sendFailed = true;
            }

        }

        private Message PrepareRequest(int instance, string method, params string[] args)
        {
            Message testMessage = new Message();

            testMessage.InstanceID = instance;
            testMessage.Method = method;
            testMessage.WhoSentMe = "Test";

            #region set default values to empty string
            testMessage.Parameter1 = string.Empty;
            testMessage.Parameter2 = string.Empty;
            testMessage.Parameter3 = string.Empty;
            testMessage.Parameter4 = string.Empty;
            testMessage.Parameter5 = string.Empty;
            testMessage.Parameter6 = string.Empty;

            #endregion

            #region Set Message Arguments
            int i = 0;
            while (i < args.Length)
            {
                switch (i)
                {
                    case 0:
                        testMessage.Parameter1 = args[0];
                        break;
                    case 1:
                        testMessage.Parameter2 = args[1];
                        break;
                    case 2:
                        testMessage.Parameter3 = args[2];
                        break;
                    case 3:
                        testMessage.Parameter4 = args[3];
                        break;
                    case 4:
                        testMessage.Parameter5 = args[4];
                        break;
                    case 5:
                        testMessage.Parameter6 = args[5];
                        break;
                }
                i++;
            }
            if (IsDuplicateInsert)
            {
                testMessage.Parameter5 = "TryDuplicate";
            }
            #endregion
            return testMessage;
        }
        public static long RecentCount { get; set; }

        private object PrepareResponse(string method)
        {
            object returnVal = null;

            #region Rethrow Exception if Exception information is present in the response
            if (responseMessage.Exception != null)
            {
                AstoriaTestLog.WriteLineIgnore(responseMessage.Exception);
                if (responseMessage.Exception.Length > 0)
                {
                    if (responseMessage.Exception.Contains("MethodAccessException"))
                    {
                        throw (new MethodAccessException(responseMessage.Exception));
                    }
                    if (responseMessage.Exception.Contains("SecurityException"))
                    {
                        throw (new System.Security.SecurityException(responseMessage.Exception));
                    }

                    if (responseMessage.Exception.Contains("NotSupportedException"))
                    {
                        throw (new NotSupportedException(responseMessage.Exception));
                    }
                    if (responseMessage.Exception.Contains("DataServiceRequestException"))
                    {
                        throw (new DataServiceRequestException(responseMessage.Exception));
                    }
                    if (responseMessage.Exception.Contains("DataServiceQueryException"))
                    {
                        throw (new DataServiceQueryException(responseMessage.Exception));
                    }
                    if (responseMessage.Exception.Contains("DataServiceClientException"))
                    {
                        throw (new DataServiceClientException(responseMessage.Exception));
                    }
                    if (responseMessage.Exception.Contains("InvalidCastException"))
                    {
                        throw (new InvalidCastException(responseMessage.Exception));
                    }
                    if (responseMessage.Exception.Contains("TargetException"))
                    {
                        throw (new TargetException(responseMessage.Exception));
                    }
                    if (responseMessage.Exception.Contains("SerializationException"))
                    {
                        throw (new SerializationException(responseMessage.Exception));
                    }
                    if (responseMessage.Exception.Contains("ArgumentOutOfRangeException"))
                    {
                        throw (new ArgumentOutOfRangeException(responseMessage.Exception));
                    }
                    if (responseMessage.Exception.Contains("ArgumentNullException"))
                    {
                        throw (new ArgumentNullException(responseMessage.Exception));
                    }
                    else if (responseMessage.Exception.Contains("InvalidOperationException"))
                    {
                        throw (new InvalidOperationException(responseMessage.Exception));
                    }
                    else if (responseMessage.Exception.Contains("ArgumentException"))
                    {
                        throw (new ArgumentException(responseMessage.Exception));
                    }
                    else if (responseMessage.Exception.Contains("TargetInvocationException"))
                    {
                        throw (new TargetInvocationException(responseMessage.Exception, null));
                    }
                    else if (responseMessage.Exception.Contains("UriFormatException"))
                    {
                        throw (new UriFormatException(responseMessage.Exception));
                    }
                    else if (responseMessage.Exception.Contains("SerializationException"))
                    {
                        throw (new SerializationException(responseMessage.Exception));
                    }
                    else if (responseMessage.Exception.Contains("SecurityException"))
                    {
                        throw (new System.Security.SecurityException(responseMessage.Exception));
                    }
                    else
                        throw new Exception(responseMessage.Exception);
                }
            }
            #endregion

            if (responseMessage.ReturnValue == "Passed!")
                responseMessage.ReturnValue = "0";

            if (!String.IsNullOrEmpty(responseMessage.ReturnValue) && responseMessage.ReturnValue.Contains("URI:"))
            {
                method = "RunLinqQuery";
                responseMessage.Parameter1 = responseMessage.ReturnValue;
                responseMessage.ReturnValue = "-1";
            }

            #region Depending on Method Called, form the return Value
            switch (method)
            {
                case "get_MergeOption":
                    returnVal = Int32.Parse(responseMessage.ReturnValue ?? "0");
                    break;

                case "get_BaseUri":
                    returnVal = (string)(responseMessage.ReturnValue ?? "");
                    break;
                case "GetMetadataUri":
                    returnVal = (string)(responseMessage.ReturnValue ?? "");
                    break;
                case "Microsoft.OData.Client.DataServiceContext":
                    returnVal = Int32.Parse(responseMessage.ReturnValue ?? "0");
                    break;
                case "Execute":
                case "RemoteLinqQuery":
                    returnVal = Int32.Parse(responseMessage.ReturnValue ?? "0");
                    RecentCount = responseMessage.ActualCount;
                    break;
                case "TypeScheme":
                case "MoveNext":
                    returnVal = (string)(responseMessage.ReturnValue ?? "false");
                    break;
                case "Current":
                    returnVal = (string)(responseMessage.ReturnValue ?? "");
                    break;
                case "Resources":
                    returnVal = (string)(responseMessage.ReturnValue ?? "");
                    break;
                case "GetEnumeratorContent":
                    returnVal = (string)(responseMessage.ReturnValue ?? "0");
                    break;
                case "Binding.GetOECEnumerator":
                case "Links":
                    returnVal = (string)(responseMessage.ReturnValue ?? "0");
                    break;

                case "DeleteObject":
                    returnVal = (string)(responseMessage.ReturnValue ?? "0");
                    break;
                case "UsePostTunneling":
                case "DetachLink":
                    returnVal = Convert.ToBoolean(responseMessage.ReturnValue ?? "false");
                    break;

                case "Detach":
                case "ExecuteBatch":
                case "VerifyResults":
                case "VerifyLinqTest":
                    returnVal = Convert.ToBoolean(responseMessage.ReturnValue ?? "false");
                    break;
                case "UserAgent":
                case "LoadProperty":
                case "TryGetUri":
                case "RehydrateEntity":
                case "IQueryable.FirstOrDefault":
                case "IQueryable.First":
                    returnVal = (string)(responseMessage.ReturnValue ?? "");
                    break;

                case "GetEntityState":
                case "GetLinkState":
                    returnVal = Enum.Parse(typeof(EntityStates), responseMessage.ReturnValue);
                    break;
                case "RunLinqQuery":
                    returnVal = Int32.Parse(responseMessage.ReturnValue ?? "0");
                    AstoriaTestLog.WriteLine("Query URI :{0}", responseMessage.Parameter1);
                    break;
                case "FFClientTest":
                    returnVal = Int32.Parse(responseMessage.ReturnValue ?? "0");
                    break;
                case "SelfEditTest":
                    returnVal = Int32.Parse(responseMessage.ReturnValue ?? "0");
                    break;
                case "AddRelatedObject":
                    returnVal = (string)(responseMessage.ReturnValue ?? "");
                    break;
                case "GetLinkDescriptor":
                    returnVal = (string)(responseMessage.ReturnValue ?? "");
                    break;
                case "GetEntityDescriptor":
                    returnVal = (string)(responseMessage.ReturnValue ?? "");
                    break;
                default:
                    returnVal = Int32.Parse(responseMessage.ReturnValue ?? "0");
                    break;

            }
            #endregion

            return returnVal;
        }
        object returnValue = null;

        private TestStatus ProcessTestMessage(int instance, string method, params string[] args)
        {
            Message message = PrepareRequest(instance, method, args);
            message.CountMode = CountEntities;
            message.ActualCount = ActualCount;
            message.CountAllPages = CountAllPages;
            SendMessage(message);

            if (sendFailed)
            {
                throw (new TestFailedException("Failed to post Message"));
            }
            //Record Message for Playback
            _messagesTillNow.Add(message);
            Uri messageURI = new Uri(String.Format(smessageUriTemplate, method), UriKind.Relative);

            AstoriaTestLog.Trace(AstoriaTraceLevel.Info, "\r\nWaiting for response...");
            TestStatus testStatus = TestStatus.Fail;// BeginExecuteForNextMessage(messageURI);

            while (returnValue == null && testStatus != TestStatus.NoResponse)
            {
                testStatus = BeginExecuteForNextMessage(messageURI);
                if (testStatus == TestStatus.Success)
                    break;
            }

            if (responseMessage != null)
            {
                returnValue = PrepareResponse(method);
            }
            return testStatus;
        }

        private TestStatus ProcessTestMessage(Message testMessage)
        {
            Uri messageURI = new Uri(String.Format(smessageUriTemplate, testMessage.Method), UriKind.Relative);
            SendMessage(testMessage);
            AstoriaTestLog.Trace(AstoriaTraceLevel.Info, "\r\nWaiting for response...");

            TestStatus testStatus = TestStatus.Fail;// BeginExecuteForNextMessage(messageURI);
            if (AstoriaTestProperties.RunXDomain)
            {
                failedAttemptLimit = 120;
            }
            while (returnValue == null && testStatus != TestStatus.NoResponse)
            {
                testStatus = BeginExecuteForNextMessage(messageURI);
                if (testStatus == TestStatus.Success)
                    break;
            }
            if (responseMessage != null)
            {
                returnValue = PrepareResponse(testMessage.Method);
            }
            return testStatus;
        }

        private void ClearMessages()
        {
            DataServiceQuery<string> clearMessges = SLDataService.CreateQuery<string>("ClearMessages");
            clearMessges.Execute();
        }

        public static void RefreshClient(Uri validBaseUri)
        {
            RefreshClient(validBaseUri, false);
        }

        private static void RefreshClient(Uri validBaseUri, bool runXDomain)
        {
            InitializeSilverlight(validBaseUri);

            if (AstoriaTestProperties.IsRemoteVersioning)
            {
                return;
            }

            string folder = SLDataService.BaseUri.ToString();
            if (folder.EndsWith("/"))
            {
                folder = folder.Substring(0, folder.LastIndexOf('/'));
            }
            folder = folder.Substring(0, folder.LastIndexOf('/'));

            if (!IsDebugEnabled && !CrashDebug && AstoriaTestProperties.Client == ClientEnum.SILVERLIGHT)
                EdmWorkspace.StartRemoteClient(folder);
        }

        public static bool CountEntities { get; set; }
        public static bool CountAllPages { get; set; }
        public static long ActualCount { get; set; }
        public object InvokeSend(int instance, string method, params string[] args)
        {
            returnValue = null;
            if (method == "TestEnd")
            {
                ClearMessages();
                _messagesTillNow.Clear();
            }
            if (method == typeof(DataServiceContext).FullName && AstoriaTestProperties.RunXDomain)
            {
                args[0] = GetXDomainHost(new Uri(args[0])).OriginalString;
            }

            //start polling for SL work messages
            TestStatus testStatus = ProcessTestMessage(instance, method, args);
            switch (testStatus)
            {
                case TestStatus.Success:
                    return returnValue;
                //break;
                case TestStatus.NoResponse:
                    #region Clear out old messages
                    ClearMessages();
                    #endregion

                    #region Detach all Entities attached to the SLDataContext
                    foreach (EntityDescriptor entityDesc in SLDataService.Entities)
                    {
                        SLDataService.Detach(entityDesc.Entity);
                    }
                    #endregion

                    #region Replay messages
                    Message prevMessage = null;
                    foreach (Message testMessage in _messagesTillNow)
                    {
                        if (testMessage.Method != "TestEnd")
                        {
                            if (prevMessage != null)
                            {
                                testMessage.InstanceID = prevMessage.InstanceID;
                            }

                            int i = 0;
                            while (i <= 5)
                            {
                                i++;
                                testStatus = ProcessTestMessage(testMessage);
                                if (testStatus == TestStatus.NoResponse)
                                {
                                    //if (args[0].Contains(".svc"))
                                    //{
                                    //    string msgURI = args[0].Substring(0, args[0].LastIndexOf('/') + 1) + "AstoriaTestSilverlight.svc";
                                    //    RefreshClient(new Uri(msgURI, UriKind.RelativeOrAbsolute));
                                    //}

                                    if (i == 5)
                                        throw (new TestFailedException("Tried Rerunning tests ,failed"));
                                }
                                ClearMessages();
                                foreach (EntityDescriptor entityDesc in SLDataService.Entities)
                                {
                                    SLDataService.Detach(entityDesc.Entity);
                                }
                            }
                            prevMessage = testMessage;
                        }

                    }
                    _messagesTillNow.Clear();
                    #endregion

                    break;
            }


            return returnValue;
        }
        static bool wasRemote = false;
        public static void DisableRemote()
        {
            wasRemote = SilverlightRemote.HasRemote;
            SilverlightRemote.HasRemote = false;
        }
        public static void EnableRemote()
        {
            SilverlightRemote.HasRemote = wasRemote;
        }
        int failedAttempts = 0;
        int failedAttemptLimit = 30;
        private enum TestStatus
        {
            Success,
            Fail,
            NoResponse,
            StillWaiting
        }
        private TestStatus BeginExecuteForNextMessage(Uri messageURI)
        {
            TestStatus testStatus = TestStatus.Fail;
            failedAttempts++;

            if (failedAttempts == failedAttemptLimit)
            {
                failedAttempts = 0;
                return TestStatus.NoResponse;
                //throw (new TestFailedException("No Response from Silverlight application, please check Browser"));
            }
            AstoriaTestLog.Trace(AstoriaTraceLevel.Info, ".");
            //Make the test wait for 1/2 second
            System.Threading.Thread.Sleep(500);
            try
            {
                responseMessage = SocketExceptionHandler.Execute(() => SLDataService.Execute<TestSL.Message>(messageURI)).FirstOrDefault<TestSL.Message>();
                if (responseMessage != null)
                {
                    failedAttempts = 0;
                    AstoriaTestLog.TraceLine(AstoriaTraceLevel.Info, "\r\nReceived Response...");
                    SLDataService.DeleteObject(responseMessage); // delete from server
                    DataServiceResponse response = SocketExceptionHandler.Execute(() => SLDataService.SaveChanges());
                    foreach (ChangeOperationResponse changset in response)
                    {
                        // consume the change we saved
                    }
                    testStatus = TestStatus.Success;
                }
                else
                {
                    testStatus = TestStatus.StillWaiting;
                }
            }
            catch (Exception e)
            {
                if (null != responseMessage)
                {
                    responseMessage.Exception = e.ToString();
                    testStatus = TestStatus.Fail;
                }
            }
            return testStatus;
        }

        //serialize the object to a string
        public static string WriteObject(object entity)
        {
            MemoryStream ms = new MemoryStream();
            DataContractSerializer serializer = new DataContractSerializer(entity.GetType());
            serializer.WriteObject(ms, entity);

            return Convert.ToBase64String(ms.ToArray());
        }

        //deserialize the object from a string
        public static object ReadObject(string sEntityName, string sEntity)
        {
            List<Type> known = new List<Type>();
            foreach (Type t in typeof(northwindClient.Customers).Assembly.GetTypes())
            {
                if (t.GetCustomAttributes(typeof(System.Runtime.Serialization.DataContractAttribute), false).Count() > 0 && t.FullName.Contains("northwindClient."))
                    known.Add(t);
            }
            foreach (Type t in typeof(ArubaClient.Run).Assembly.GetTypes())
            {
                if (t.GetCustomAttributes(typeof(System.Runtime.Serialization.DataContractAttribute), false).Count() > 0 && t.FullName.Contains("ArubaClient."))
                    known.Add(t);
            }
            /* foreach (Type t in typeof(NorthwindV2.Orders).Assembly.GetTypes())
             {
                 if (t.FullName.Contains("NorthwindV2.Orders"))
                     known.Add(t);
             }
             foreach (Type t in typeof(NorthwindV2.Customers).Assembly.GetTypes())
             {
                 if (t.FullName.Contains("NorthwindV2.Customers"))
                     known.Add(t);
             }           */

            object clientType = null;
            try
            {
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(sEntity));
                Type entityType = Type.GetType(sEntityName);

                System.Runtime.Serialization.DataContractSerializer serializer = (known.Count == 0) ?
                    new DataContractSerializer(entityType) : new DataContractSerializer(entityType, known);
                clientType = serializer.ReadObject(ms);

            }
            catch (FormatException fException)
            {
                AstoriaTestLog.WriteLineIgnore("Format Exception , Input String is " + sEntity);
            }
            return clientType;
        }
    }


    /// <summary>
    /// Wrapper for DataServiceContext
    /// </summary>

    public enum QueryType
    {
        TopLevelEntitySet,
        Where1Level,
        SelectConstant,
        SelectEmpty,
        SelectAll,
        SelectNavProperty,
        OrderbyAsc,
        OrderByDesc,
        TakeAndSkip,
        TakeOnly,
        SkipOnly,
        First,
        Single,
        ThenByAsc,
        ThenByDesc,
        ExpandNavProp
    }
    public class KVP
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }

    public partial class WebDataCtxWrapper
    {
        public static WebDataCtxWrapper MostRecentContext
        {
            get;
            private set;
        }

        private WebDataCtxWrapper()
        {
            MostRecentContext = this;
        }

        public int instance;
        public ActionBag ContextActions
        {
            get;
            set;
        }

        public Microsoft.OData.Client.DataServiceContext _DataServiceContext;

        public bool CountEntities
        {
            get;
            set;
        }

        public bool CountAllPages
        {
            get;
            set;
        }
        public long RecentCount
        {
            get
            {
                return SilverlightRemote.RecentCount;
            }
        }
        public void AddRelatedObject(object source, string sourceProperty, object target)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                string sSource = String.Empty;
                string sTarget = String.Empty;

                sSource = SilverlightRemote.WriteObject(source);
                sTarget = SilverlightRemote.WriteObject(target);

                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "AddRelatedObject",
                     new string[] { sSource, sourceProperty, sTarget, source == null ? "" : source.GetType().FullName, target == null ? "" : target.GetType().FullName });
            }
            else
            {
                this._DataServiceContext.AddRelatedObject(source, sourceProperty, target);
            }
        }
        public bool RemoteLinqQuery(QueryType queryType, string entitySetName, Type entityType, ExpNode constantExpr)
        {
            string _keyValuePairSerialized = String.Empty;
            AstoriaTestLog.WriteLine("Entity Set :{0} ", entitySetName);
            _keyValuePairSerialized = SerializeKeyExpression(constantExpr, _keyValuePairSerialized);
            return RunRemoteLinq(queryType, entitySetName, entityType, _keyValuePairSerialized, "");
        }
        public bool RemoteLinqQuery(QueryType queryType, string entitySetName, Type entityType, string navigationPropertyName, ExpNode expression)
        {
            string _keyValuePairSerialized = String.Empty;
            AstoriaTestLog.WriteLine("Entity Set :{0} ", entitySetName);
            _keyValuePairSerialized = SerializeKeyExpression(expression, _keyValuePairSerialized);
            return RunRemoteLinq(queryType, entitySetName, entityType, _keyValuePairSerialized, navigationPropertyName);
        }
        public Uri GetReadStreamUri(object entity)
        {
            Uri streamUri = null;
            if (AstoriaTestProperties.IsRemoteClient)
            {
            }
            else
            {
                streamUri = UnderlyingContext.GetReadStreamUri(entity);
            }
            return streamUri;
        }

        public DataServiceStreamResponse GetReadStream(object entity)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                throw new NotSupportedException("Not supported in Silverlight");
            }
            return UnderlyingContext.GetReadStream(entity);
        }

        public void SetSaveStream(object entity, Stream stream, bool closeStream, string contentType, string slug)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                throw new NotSupportedException("Not supported in Silverlight");
            }
            UnderlyingContext.SetSaveStream(entity, stream, closeStream, contentType, slug);
        }

        public void SetSaveStream(object entity, Stream stream, bool closeStream, DataServiceRequestArgs args)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                throw new NotSupportedException("Not supported in Silverlight");
            }
            UnderlyingContext.SetSaveStream(entity, stream, closeStream, args);
        }
        public bool RemoteLinqQuery(QueryType queryType, string entitySetName, Type entityType, PropertyExpression[] sortFields)
        {
            string _keyValuePairSerialized = String.Empty;
            List<string> _sortFields = new List<string>();
            sortFields.ToList().ForEach(sf => _sortFields.Add(sf.Name));
            _keyValuePairSerialized = SilverlightRemote.WriteObject(_sortFields);
            AstoriaTestLog.WriteLine("Entity Set :{0} ", entitySetName);
            SilverlightRemote.ActualCount = this.ActualCount;
            SilverlightRemote.CountAllPages = this.CountAllPages;
            SilverlightRemote.CountEntities = this.CountEntities;
            SilverlightRemote.Remoter.InvokeSend(this.instance, "RunLinqQuery", new string[] { queryType.ToString(), entitySetName, entityType.FullName, "", _keyValuePairSerialized });
            return (bool)SilverlightRemote.Remoter.InvokeSend(this.instance, "VerifyLinqTest", new string[] { "RunLinqQuery" + queryType.ToString() });
        }
        public long ActualCount { get; set; }
        public DataServiceRequest CreateRequest(Type entityType, Uri requestUri)
        {
            return typeof(DataServiceRequest<>).MakeGenericType(entityType).GetConstructor(new Type[] { typeof(Uri) }).Invoke(new object[] { requestUri }) as DataServiceRequest;
        }
        public bool RemoteLinqQuery(QueryType queryType, string entitySetName, Type entityType, PropertyExpression[] sortFields, PropertyExpression[] thenByFields)
        {
            string _keyValuePairSerialized = String.Empty;
            List<string> _sortFields = new List<string>();
            List<string> _thenByfields = new List<string>();
            sortFields.ToList().ForEach(sf => _sortFields.Add(sf.Name));
            thenByFields.ToList().ForEach(sf => _thenByfields.Add(sf.Name));

            _keyValuePairSerialized = SilverlightRemote.WriteObject(_sortFields);
            AstoriaTestLog.WriteLine("Entity Set :{0} ", entitySetName);
            SilverlightRemote.Remoter.InvokeSend(this.instance, "RunLinqQuery", new string[] { queryType.ToString(), entitySetName, entityType.FullName, "", _keyValuePairSerialized, SilverlightRemote.WriteObject(_thenByfields) });
            return (bool)SilverlightRemote.Remoter.InvokeSend(this.instance, "VerifyLinqTest", new string[] { "RunLinqQuery" + queryType.ToString() });
        }

        public bool RemoteLinqQuery(QueryType queryType, string entitySetName, Type entityType, PropertyExpression[] sortFields, int topValue, int skipValue)
        {
            string _keyValuePairSerialized = String.Empty;
            List<string> _sortFields = new List<string>();
            sortFields.ToList().ForEach(sf => _sortFields.Add(sf.Name));
            _keyValuePairSerialized = SilverlightRemote.WriteObject(_sortFields);
            AstoriaTestLog.WriteLine("Entity Set :{0} ", entitySetName);
            SilverlightRemote.Remoter.InvokeSend(this.instance, "RunLinqQuery", new string[] { queryType.ToString(), entitySetName, entityType.FullName, "", _keyValuePairSerialized, topValue.ToString() + "," + skipValue.ToString() });
            return (bool)SilverlightRemote.Remoter.InvokeSend(this.instance, "VerifyLinqTest", new string[] { "RunLinqQuery" + queryType.ToString() });
        }
        public bool RemoteLinqQuery(QueryType queryType, string entitySetName, Type entityType, PropertyExpression[] sortFields, int topValue)
        {
            string _keyValuePairSerialized = String.Empty;
            List<string> _sortFields = new List<string>();
            sortFields.ToList().ForEach(sf => _sortFields.Add(sf.Name));
            _keyValuePairSerialized = SilverlightRemote.WriteObject(_sortFields);
            AstoriaTestLog.WriteLine("Entity Set :{0} ", entitySetName);
            SilverlightRemote.Remoter.InvokeSend(this.instance, "RunLinqQuery", new string[] { queryType.ToString(), entitySetName, entityType.FullName, "", _keyValuePairSerialized, topValue.ToString() });
            return (bool)SilverlightRemote.Remoter.InvokeSend(this.instance, "VerifyLinqTest", new string[] { "RunLinqQuery" + queryType.ToString() });
        }

        List<string> ResourceTypesTraversed { get; set; }

        public bool EntityExistsInStore(object entityInstance)
        {
            bool entityFound = false;
            Uri entityUri = null;
            if (this.TryGetUri(entityInstance, out entityUri))
            {
                HttpWebRequest webRequest = WebRequest.Create(entityUri) as HttpWebRequest;
                webRequest.Credentials = CredentialCache.DefaultCredentials;
                webRequest.Method = "GET";
                try
                {
                    HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse;
                    entityFound = webResponse.StatusCode == HttpStatusCode.OK;
                }
                catch (WebException webException)
                {
                    using (Stream responseStream = webException.Response.GetResponseStream())
                    {
                        StreamReader strReader = new StreamReader(responseStream);
                        string strErrorResponse = strReader.ReadToEnd();
                        AstoriaTestLog.WriteLineIgnore("Failed to find entity in Store -> {0}", strErrorResponse);
                    }
                }

            }
            return entityFound;
        }
        public void EnsureInsert(object entityInstance, ResourceType resourceType)
        {
            ResourceTypesTraversed = new List<string>();
            EnsureInsertCore(entityInstance, resourceType);
        }
        public void EnsureInsert(object entityInstance, ResourceType resourceType, ResourceType skipThisType)
        {
            ResourceTypesTraversed = new List<string>();
            EnsureInsertCore(entityInstance, resourceType, skipThisType);
            depthLevel = 0;
        }
        int depthLevel = 0;
        int maxDepth = 2;
        private void EnsureInsertCore(object entityInstance, ResourceType resourceType)
        {
            EnsureInsertCore(entityInstance, resourceType, null);
        }
        private void EnsureInsertCore(object entityInstance, ResourceType resourceType, ResourceType skipThisType)
        {
            depthLevel++;
            if (skipThisType != null && resourceType == skipThisType)
            {
                return;
            }
            AstoriaTestLog.WriteLine("{0}Inserting Entity -> {1} ", tabSpace, resourceType.Name);
            if (ResourceTypesTraversed.Any(str => resourceType.Name == str) || depthLevel >= maxDepth)
            {
                AstoriaTestLog.WriteLine("Cycle Detected");
                return;
            }
            tabSpace += "\t";
            ResourceTypesTraversed.Add(resourceType.Name);
            if (resourceType.Constraints != null)
            {
                this.UnderlyingContext.SaveChangesDefaultOptions = SaveChangesOptions.BatchWithSingleChangeset;
                foreach (ResourceAssociation insertConstraint in resourceType.Constraints)
                {
                    if (insertConstraint.IsSelfAssociation)
                    {
                        return;
                    }
                    ResourceAssociationEnd otherEnd = insertConstraint.Ends.FirstOrDefault(raend => raend.ResourceType != resourceType);
                    string associationName = insertConstraint.Source == otherEnd ? insertConstraint.Source.Name : insertConstraint.Target.Name;
                    object otherEndInstance = otherEnd.ResourceType.CreateInstance(false);

                    Type entityPropertyValue = entityInstance.GetType().GetProperty(otherEnd.Name).PropertyType;
                    string entitySet = "";
                    Type PropertyType = null;
                    if (entityPropertyValue.IsGenericType)
                    {
                        PropertyType = entityPropertyValue.GetGenericArguments()[0];
                    }
                    else
                    {
                        PropertyType = otherEndInstance.GetType();
                    }
                    if (PropertyType.GetCustomAttributes(typeof(Microsoft.OData.Client.EntitySetAttribute), true).Length > 0)
                    {
                        EntitySetAttribute eSet = PropertyType.GetCustomAttributes(typeof(Microsoft.OData.Client.EntitySetAttribute), true)[0] as EntitySetAttribute;
                        entitySet = eSet.EntitySet;
                    }
                    else
                    {
                        entitySet = otherEndInstance.GetType().Name;
                    }
                    this.AddObject(entitySet, otherEndInstance);
                    if (entityPropertyValue.IsGenericType)
                    {
                        this.AddLink(entityInstance, associationName, otherEndInstance);
                    }
                    else
                    {
                        this.SetLink(entityInstance, associationName, otherEndInstance);
                    }
                    EnsureInsertCore(otherEndInstance, otherEnd.ResourceType, skipThisType);
                }
            }
            return;
        }
        public void EnsureDelete(object entityInstance, ResourceType resourceType)
        {
            ResourceTypesTraversed = new List<string>();
            EnsureDeleteCore(entityInstance, resourceType);
        }
        string tabSpace = "";
        private void EnsureDeleteCore(object entityInstance, ResourceType resourceType)
        {
            AstoriaTestLog.WriteLine("{0}Deleting Entity -> {1} ", tabSpace, resourceType.Name);
            if (ResourceTypesTraversed.Any(str => resourceType.Name == str))
            {
                AstoriaTestLog.WriteLine("Cycle Detected");
                return;
            }
            tabSpace += "\t";
            ResourceTypesTraversed.Add(resourceType.Name);
            if (resourceType.Constraints != null)
            {
                this.UnderlyingContext.SaveChangesDefaultOptions = SaveChangesOptions.BatchWithSingleChangeset;
                foreach (ResourceAssociation deleteConstraint in resourceType.Constraints)
                {
                    if (deleteConstraint.IsSelfAssociation)
                    {
                        return;
                    }
                    ResourceAssociationEnd otherEnd = deleteConstraint.Ends.FirstOrDefault(raend => raend.ResourceType != resourceType);
                    string associationName = deleteConstraint.Source == otherEnd ? deleteConstraint.Source.Name : deleteConstraint.Target.Name;
                    this.LoadProperty(entityInstance, associationName);
                    object entityPropertyValue = entityInstance.GetType().GetProperty(associationName).GetValue(entityInstance, null);
                    if (entityPropertyValue is IList)
                    {
                        IList listOfValues = entityPropertyValue as IList;
                        foreach (object navPropEntity in listOfValues)
                        {
                            this.DeleteObject(navPropEntity);
                            EnsureDeleteCore(navPropEntity, otherEnd.ResourceType);
                        }
                    }
                    else
                    {
                        this.DeleteObject(entityPropertyValue);
                        EnsureDeleteCore(entityPropertyValue, otherEnd.ResourceType);
                    }
                }

            }
            this.DeleteObject(entityInstance);
            return;
        }

        private static string SerializeKeyExpression(ExpNode constantExpr, string _keyValuePairSerialized)
        {
            if (constantExpr != null)
            {
                AstoriaTestLog.WriteLine("Parameters :");

                List<KVP> _KeyValuePairs = new List<KVP>();
                if (constantExpr is ConstantExpression)
                {
                    _KeyValuePairs.Add(
                        new KVP()
                        {
                            Key = constantExpr.Type.ClrType.FullName,
                            Value = ((ConstantExpression)constantExpr).Value.ClrValue
                        }
                        );
                }
                else if (constantExpr is KeyExpression)
                    _KeyValuePairs = ConvertKeyExpression(constantExpr);
                _keyValuePairSerialized = SilverlightRemote.WriteObject(_KeyValuePairs);
            }
            return _keyValuePairSerialized;
        }

        public static List<KVP> ConvertKeyExpression(ExpNode constantExpr)
        {
            List<KVP> _KeyValuePairs = new List<KVP>();
            KeyExpression keyExpression = constantExpr as KeyExpression;
            for (int index = 0; index < keyExpression.Properties.Length; index++)
            {
                string PropertyName = keyExpression.Properties[index].Name;
                object PropertyValue = keyExpression.Values[index].ClrValue;
                _KeyValuePairs.Add(
                    new KVP()
                    {
                        Key = PropertyName,
                        Value = PropertyValue
                    }
                    );
                AstoriaTestLog.WriteLine("Key : {0} , Value : {1}", PropertyName, PropertyValue);
            }
            return _KeyValuePairs;
        }

        private bool RunRemoteLinq(QueryType queryType, string entitySetName, Type entityType, string _keyValuePairSerialized, string navigationPropertyName)
        {
            SilverlightRemote.CountEntities = this.CountEntities;
            SilverlightRemote.CountAllPages = this.CountAllPages;

            SilverlightRemote.ActualCount = this.ActualCount;
            SilverlightRemote.Remoter.InvokeSend(this.instance, "RunLinqQuery", new string[] { queryType.ToString(), entitySetName, entityType.FullName, _keyValuePairSerialized, navigationPropertyName });
            return (bool)SilverlightRemote.Remoter.InvokeSend(this.instance, "VerifyLinqTest", new string[] { "RunLinqQuery" + queryType.ToString() });
        }


        public bool IgnoreHttp404
        {
            get
            {
                return this._DataServiceContext.IgnoreResourceNotFoundException;
            }
            set
            {
                this._DataServiceContext.IgnoreResourceNotFoundException = value;
            }
        }

        public bool ApplyingChanges
        {
            get
            {
                return this._DataServiceContext.ApplyingChanges;
            }
        }

        public DataServiceContext UnderlyingContext
        {
            get { return _DataServiceContext; }
            set { _DataServiceContext = value; }

        }

        public EntityStates GetRemoteState(Object entity)
        {
            EntityStates remoteState = EntityStates.Unchanged;
            string sEntity = SilverlightRemote.WriteObject(entity);
            object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "GetEntityState",
                   new string[] { sEntity, entity == null ? "" : entity.GetType().FullName });
            remoteState = (EntityStates)retValue;
            return remoteState;
        }

        public string UserAgent
        {
            get
            {
                if (AstoriaTestProperties.IsRemoteClient)
                {
                    return SilverlightRemote.Remoter.InvokeSend(this.instance, "UserAgent", "").ToString();
                }
                else
                {
                    return "Microsoft ADO.NET Data Services";
                }
            }
        }
        public WebDataQueryGenericWrapper CreateQuery(ResourceContainer container)
        {
            PropertyInfo pi = _DataServiceContext.GetType().GetProperty(container.Name);
            if (pi == null)
                throw new Microsoft.Test.ModuleCore.TestFailedException("Unable to find property:" + container.Name);
            object query = pi.GetValue(_DataServiceContext, new object[] { });
            WebDataQueryGenericWrapper wrapper = new WebDataQueryGenericWrapper(query);
            return wrapper;
        }
        public WebDataQueryGenericWrapper CreateQueryForContainer(ResourceContainer container)
        {
            PropertyInfo pi = _DataServiceContext.GetType().GetProperty(container.Name);

            if (pi == null)
            {
                return null;
            }

            Object query = pi.GetValue(_DataServiceContext, new Object[] { });
            return new WebDataQueryGenericWrapper(query);
        }
        public IAsyncResult BeginExecuteOfT(Type entityType, Uri requestUri, AsyncCallback callback, object userState)
        {
            MethodInfo method = typeof(DataServiceContext).GetMethod("BeginExecute", new Type[] { typeof(Uri), typeof(AsyncCallback), typeof(object) }).MakeGenericMethod(entityType);
            return (IAsyncResult)method.Invoke(this._DataServiceContext, new object[] { requestUri, callback, userState });
        }
        public QueryOperationResponseWrapper EndExecuteOfT(Type entityType, IAsyncResult asyncResult)
        {
            MethodInfo method = typeof(DataServiceContext).GetMethod("EndExecute").MakeGenericMethod(entityType);
            QueryOperationResponse qoResponse = method.Invoke(this._DataServiceContext, new object[] { asyncResult }) as QueryOperationResponse;
            ConstructorInfo qorTypeConstructor = typeof(QueryOperationResponseWrapper<>).MakeGenericType(entityType).GetConstructor(new Type[] { typeof(QueryOperationResponse<>).MakeGenericType(entityType) });
            return qorTypeConstructor.Invoke(new object[] { qoResponse }) as QueryOperationResponseWrapper;
        }
        public IQueryable CreateQueryOfT(string entitySetName, Type entityType)
        {
            MethodInfo method = typeof(DataServiceContext).GetMethod("CreateQuery").MakeGenericMethod(entityType);
            return (IQueryable)method.Invoke(this._DataServiceContext, new object[] { entitySetName });
        }

        public IEnumerable ExecuteOfTRemote(Type entityType, Uri requestURI)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                int enuminstance = (int)SilverlightRemote.Remoter.InvokeSend(this.instance, "Execute", new string[] { requestURI == null ? "TryNull" : requestURI.ToString(), entityType.ToString() });
                Type enumerableType = typeof(EnumerableWrapper<>).MakeGenericType(entityType);
                IEnumerable enumWrapper = (IEnumerable)Activator.CreateInstance(enumerableType, new object[] { enuminstance });
                PropertyInfo typeHintProperty = enumerableType.GetProperty("TypeHint");
                typeHintProperty.SetValue(enumWrapper, entityType.FullName, null);
                return enumWrapper;
            }
            else
            {
                throw new InvalidOperationException("This operation is valid only for remote tests");
            }
        }
        public IEnumerable ExecuteOfT(Type entityType, Uri requestURI)
        {
            if (SilverlightRemote.HasRemote)
            {
                return ExecuteOfTRemote(entityType, requestURI);
            }
            else
            {
                MethodInfo[] methods = typeof(DataServiceContext).GetMethods(BindingFlags.Public | BindingFlags.Instance);
                MethodInfo method = methods.Where(m => m.Name == "Execute" && m.GetParameters().Count() == 1 && m.GetParameters()[0].ParameterType.ToString().Contains("Uri")).Single();
                method = method.MakeGenericMethod(entityType);
                ConstructorInfo qorTypeConstructor = typeof(QueryOperationResponseWrapper<>).MakeGenericType(entityType).GetConstructor(new Type[] { typeof(QueryOperationResponse<>).MakeGenericType(entityType) });
                object qoResponse = method.Invoke(this._DataServiceContext, new object[] { requestURI });
                return qorTypeConstructor.Invoke(new object[] { qoResponse }) as QueryOperationResponseWrapper;
            }
        }
        public bool TryGetEntityOfT(Type entityType, Uri requestURI)
        {
            object entityInstance = Activator.CreateInstance(entityType);
            MethodInfo method = typeof(DataServiceContext).GetMethod("TryGetEntity").MakeGenericMethod(entityType);
            return (bool)method.Invoke(this._DataServiceContext, new object[] { requestURI, entityInstance });
        }

        private StringBuilder sbCode = new StringBuilder();
        internal string CodeTrace
        {
            get
            {
                return sbCode.ToString();
            }
        }
        private void Log(string code)
        {
            sbCode.AppendLine(code + ";");
        }

        public List<Func<bool>> ValidationFunctions { get; set; }
        /// <summary>namespace for edm primitive types.</summary>
        internal const string EdmNamespace = "Edm";

        /// <summary>edm binary primitive type name</summary>
        internal const string EdmBinaryTypeName = "Edm.Binary";

        /// <summary>edm boolean primitive type name</summary>
        internal const string EdmBooleanTypeName = "Edm.Boolean";

        /// <summary>edm byte primitive type name</summary>
        internal const string EdmByteTypeName = "Edm.Byte";

        /// <summary>edm decimal primitive type name</summary>
        internal const string EdmDecimalTypeName = "Edm.Decimal";

        /// <summary>edm double primitive type name</summary>
        internal const string EdmDoubleTypeName = "Edm.Double";

        /// <summary>edm guid primitive type name</summary>
        internal const string EdmGuidTypeName = "Edm.Guid";

        /// <summary>edm single primitive type name</summary>
        internal const string EdmSingleTypeName = "Edm.Single";

        /// <summary>edm sbyte primitive type name</summary>
        internal const string EdmSByteTypeName = "Edm.SByte";

        /// <summary>edm int16 primitive type name</summary>
        internal const string EdmInt16TypeName = "Edm.Int16";

        /// <summary>edm int32 primitive type name</summary>
        internal const string EdmInt32TypeName = "Edm.Int32";

        /// <summary>edm int64 primitive type name</summary>
        internal const string EdmInt64TypeName = "Edm.Int64";

        /// <summary>edm string primitive type name</summary>
        internal const string EdmStringTypeName = "Edm.String";



        private static string GetEDMTYpe(string ClrTypeName)
        {
            string edmTypeName = String.Empty;

            switch (ClrTypeName.ToLower())
            {
                case "boolean": edmTypeName = EdmBooleanTypeName; break;
                case "byte": edmTypeName = EdmByteTypeName; break;
                case "binary": edmTypeName = EdmBinaryTypeName; break;
                case "bytearray": edmTypeName = EdmBinaryTypeName; break;
                case "decimal": edmTypeName = EdmDecimalTypeName; break;
                case "double": edmTypeName = EdmDoubleTypeName; break;
                case "guid": edmTypeName = EdmGuidTypeName; break;
                case "int16": edmTypeName = EdmInt16TypeName; break;
                case "int32": edmTypeName = EdmInt32TypeName; break;
                case "int64": edmTypeName = EdmInt64TypeName; break;
                case "single": edmTypeName = EdmSingleTypeName; break;
                case "sbyte": edmTypeName = EdmSByteTypeName; break;
                case "string": edmTypeName = "System.String"; break;
            }

            return edmTypeName;
        }

        public void VerifyType(XElement e, PropertyInfo[] entityProperties)
        {

            XName typeAttributeName = XName.Get("type", "http://docs.oasis-open.org/odata/ns/metadata");

            foreach (XNode propertyNode in e.DescendantNodes())
            {
                if (propertyNode is XElement)
                {
                    string xmlPropertyName = ((XElement)propertyNode).Name.LocalName;
                    XAttribute typeAttribute = ((XElement)propertyNode).Attribute(typeAttributeName);

                    if (((XElement)propertyNode).Descendants().Count() > 1)
                    {
                        //VerifyType((XElement)propertyNode, entityProperties);
                        break;
                    }
                    else
                    {
                        PropertyInfo matchingEntityProperty = entityProperties.Where<PropertyInfo>(prop => prop.Name == xmlPropertyName).First<PropertyInfo>();

                        string clrTypeName = matchingEntityProperty.PropertyType.IsGenericType ?
                            matchingEntityProperty.PropertyType.GetGenericArguments()[0].Name :
                            matchingEntityProperty.PropertyType.Name;

                        if (clrTypeName.ToLower().Equals("byte[]"))
                            clrTypeName = "ByteArray";

                        string expectedType = GetEDMTYpe(clrTypeName);
                        string actualType = typeAttribute == null ? "System.String" : typeAttribute.Value;

                        AstoriaTestLog.AreEqual(expectedType, actualType, String.Format("Wrong Type Attribute Generated , Property Name : {0} ,  Expected Type : {1} , Actual Type : {2}", matchingEntityProperty.Name, expectedType, actualType));
                        AstoriaTestLog.TraceLine(AstoriaTraceLevel.Info, String.Format("Property Name : {0} ,  Expected Type : {1} , Actual Type : {2}", matchingEntityProperty.Name, expectedType, actualType));
                    }

                }
            }
        }

        public WebDataCtxWrapper(Uri uri)
            : this()
        {
            Log(String.Format("WebDataCtxWrapper Context = new WebDataCtxWrapper(new Uri(\"{0}\"));", uri.OriginalString));
            if (AstoriaTestProperties.IsRemoteClient)
            {
                if (!SilverlightRemote.IsInitialized)
                {
                    SilverlightRemote.InitializeSilverlight(uri);
                }

                #region Launch Remote Browser if not already started
                if (!SilverlightRemote.IsClientStarted)
                {
                    if (AstoriaTestProperties.Client == ClientEnum.SILVERLIGHT)
                    {
                        if (!SilverlightRemote.IsDebugEnabled)
                        {
                            EdmWorkspace.StartRemoteClient(SilverlightRemote.SLDataService.BaseUri.AbsoluteUri.ToString());
                        }
                        SilverlightRemote.IsClientStarted = true;
                    }
                    else if (AstoriaTestProperties.IsRemoteVersioning)
                    {
                        SilverlightRemote.IsClientStarted = false;
                    }
                }
                #endregion Launch Remote Browser if not already started


                SilverlightRemote.Remoter.InvokeSend(-1, "TestEnd", "");
                instance = (int)SilverlightRemote.Remoter.InvokeSend(-1, typeof(DataServiceContext).FullName, uri.OriginalString);

                //TODO: Investigate removing this 
                this._DataServiceContext = new Microsoft.OData.Client.DataServiceContext(uri);
            }
            else
            {
                this._DataServiceContext = new Microsoft.OData.Client.DataServiceContext(uri);
                this._DataServiceContext.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(OnSendingRequest2);
            }
            ContextActions = new ActionBag();
            ActionBag.ActionAdded += new ActionBag.ActionAddedDelegate(ActionBag_ActionAdded);
            RecordContextCreate(uri.OriginalString);

        }


        void ActionBag_ActionAdded(ActionBag addedAction)
        {
            ContextActions.AllActions.Add(addedAction);
        }

        private void RecordContextCreate(string serviceURI)
        {
            ActionBag action = ActionBag.CreateActionBag(ContextAction.CreateContext);
            action.Parameters.Add(String.Format("ServiceURI  : {0}", serviceURI));
            action.Before = EntityStates.Detached;
            action.After = EntityStates.Detached;

        }

        public WebDataCtxWrapper(Microsoft.OData.Client.DataServiceContext context)
            : this()
        {
            if (context != null)
            {
                RecordContextCreate(context.BaseUri.OriginalString);
            }

            this._DataServiceContext = context;
        }

        public static long CreateQueryOfTCount = 0;
        public WebDataQueryWrapper<T> CreateQuery<T>(String relativeUri)
        {
            AstoriaTestLog.WriteLineIgnore("{0} ) CreateQuery called with type : {1}", CreateQueryOfTCount++, typeof(T).FullName);
            int enuminstance = -1;
            if (AstoriaTestProperties.IsRemoteClient)
            {
                enuminstance = (int)SilverlightRemote.Remoter.InvokeSend(this.instance, "CreateQuery<T>", new string[] { relativeUri == null ? "TryNull" : relativeUri.ToString(), typeof(T).ToString() });
            }
            return new WebDataQueryWrapper<T>(this._DataServiceContext.CreateQuery<T>(relativeUri))
            {
                InstanceID = enuminstance
            };
        }

        public System.Uri GetMetadataUri()
        {
            if (SilverlightRemote.HasRemote)
            {
                return new System.Uri((string)SilverlightRemote.Remoter.InvokeSend(this.instance, "GetMetadataUri", "0"));

            }
            else
            {
                return this._DataServiceContext.GetMetadataUri();
            }
        }

        #region non request-sending methods

        #region Entity and Link Tracking

        /// <summary>Gets the entity descriptor corresponding to a particular entity</summary>
        /// <param name="entity">Entity for which to find the entity descriptor</param>
        /// <returns>EntityDescriptor for the <paramref name="entity"/> or null if not found</returns>
        public EntityDescriptorWrapper GetEntityDescriptor(object entity)
        {
            if (SilverlightRemote.HasRemote)
            {
                string sEntity = String.Empty;
                if (entity == null)
                {
                    sEntity = "TryNull";
                }
                else
                {
                    sEntity = SilverlightRemote.WriteObject(entity);
                }

                string retValue = (string)SilverlightRemote.Remoter.InvokeSend(this.instance, "GetEntityDescriptor",
                     new string[] { sEntity, (entity == null) ? "" : entity.GetType().FullName });

                EntityDescriptorWrapper edw = new EntityDescriptorWrapper();
                object edescriptor = SilverlightRemote.ReadObject(edw.GetType().FullName, retValue);
                EntityDescriptorWrapper ed = (EntityDescriptorWrapper)edescriptor;
                return ed;
            }
            else
            {
                return new EntityDescriptorWrapper(
                    this._DataServiceContext.GetEntityDescriptor(entity));
            }
        }

        /// <summary>
        /// Gets the link descriptor corresponding to a particular link b/w source and target objects
        /// </summary>
        /// <param name="source">Source entity</param>
        /// <param name="sourceProperty">Property of <paramref name="source"/></param>
        /// <param name="target">Target entity</param>
        /// <returns>LinkDescriptor for the relationship b/w source and target entities or null if not found</returns>
        public LinkDescriptorWrapper GetLinkDescriptor(object source, string sourceProperty, object target)
        {
            if (SilverlightRemote.HasRemote)
            {
                string sSource = String.Empty;
                string sTarget = String.Empty;

                sSource = SilverlightRemote.WriteObject(source);
                sTarget = SilverlightRemote.WriteObject(target);

                string retValue = (string)SilverlightRemote.Remoter.InvokeSend(this.instance, "GetLinkDescriptor",
                    new string[] { sSource, sourceProperty, sTarget, (source == null) ? "" : source.GetType().FullName, target == null ? "" : target.GetType().FullName });

                LinkDescriptorWrapper ld = new LinkDescriptorWrapper();
                object ldescriptor = SilverlightRemote.ReadObject(ld.GetType().FullName, retValue);
                LinkDescriptorWrapper ldw = (LinkDescriptorWrapper)ldescriptor;
                return ldw;
            }
            else
            {
                var lDescriptor = this._DataServiceContext.GetLinkDescriptor(source, sourceProperty, target);
                if (lDescriptor == null)
                {
                    return null;
                }
                return new LinkDescriptorWrapper(lDescriptor);
            }
        }

        #endregion


        internal EntityStates GetEntityState(WebDataCtxWrapper dataContext, object entity)
        {
            EntityStates thisState = EntityStates.Detached;
            Func<EntityDescriptorWrapper, bool> EntityFinder = ed =>
            {
                return ed.Entity == entity;
            };
            if (dataContext.Resources.Any(EntityFinder))
            {
                thisState = dataContext.Resources.First(EntityFinder).State;
            }
            return thisState;
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public void AddObject(string entitySetName, object entity)
        {
            if (SilverlightRemote.HasRemote)
            {
                string sEntity = String.Empty;
                if (entity == null)
                {
                    sEntity = "TryNull";
                }
                else
                {
                    sEntity = SilverlightRemote.WriteObject(entity);
                }
                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "AddObject",
                    new string[] { entitySetName, sEntity, entity == null ? "" : entity.GetType().FullName });
            }
            else
            {
                this._DataServiceContext.AddObject(entitySetName, entity);
            }
        }

        public void AddLink(Object parent, String parentProperty, Object child)
        {
            if (parent != null && child != null)
            {
                this.Log(string.Format("Context.AddLink({0}Instance,\"{1}\",{2}Instance)", parent.GetType().Name
                    , parentProperty, child.GetType().Name));
            }

            if (SilverlightRemote.HasRemote)
            {
                string sParent;
                string sChild;

                if (parent != null)
                    sParent = SilverlightRemote.WriteObject(parent);
                else
                {
                    sParent = "TryNull";
                }

                if (child != null)
                    sChild = SilverlightRemote.WriteObject(child);
                else
                {
                    sChild = "TryNull";
                }
                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "AddLink", new string[] { parentProperty, sParent, parent != null ? parent.GetType().FullName : "", sChild, child != null ? child.GetType().FullName : "" });
            }
            else
            {
                this._DataServiceContext.AddLink(parent, parentProperty, child);
            }
        }

        public void AttachTo(String entitySetName, Object entity)
        {

            if (entity != null)
            {

                this.Log(string.Format("Context.AttachTo(\"{0}\",{1}Instance)", entitySetName,
                    entity.GetType().Name));
            }

            if (SilverlightRemote.HasRemote)
            {
                string sEntity = String.Empty;
                if (entity == null)
                {
                    sEntity = "TryNull";
                }
                else
                {
                    sEntity = SilverlightRemote.WriteObject(entity);
                }
                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "AttachTo",
                    new string[] { entitySetName, sEntity, entity == null ? "" : entity.GetType().FullName });
            }
            else
            {
                this._DataServiceContext.AttachTo(entitySetName, entity);
            }
        }

        public void AttachLink(Object parent, String parentProperty, Object child)
        {
            if (parent != null && child != null)
            {
                this.Log(string.Format("Context.AddLink({0}Instance,\"{1}\",{2}Instance)", parent.GetType().Name
                    , parentProperty, child.GetType().Name));
            }

            if (SilverlightRemote.HasRemote)
            {
                string sParent;
                string sChild;

                if (parent != null)
                    sParent = SilverlightRemote.WriteObject(parent);
                else
                {
                    sParent = "TryNull";
                }

                if (child != null)
                    sChild = SilverlightRemote.WriteObject(child);
                else
                {
                    sChild = "TryNull";
                }
                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "AttachLink", new string[] { parentProperty, sParent, parent != null ? parent.GetType().FullName : "", sChild, child != null ? child.GetType().FullName : "" });
            }
            else
            {
                this._DataServiceContext.AttachLink(parent, parentProperty, child);
            }
        }

        public void DeleteObject(Object entity)
        {
            if (entity != null)
            {
                this.Log(string.Format("Context.DeleteObject({0}Instance)", entity.GetType().Name));
            }

            if (SilverlightRemote.HasRemote)
            {
                string sEntity = String.Empty;
                if (entity == null)
                {
                    sEntity = "TryNull";
                }
                else
                {
                    sEntity = SilverlightRemote.WriteObject(entity);
                }
                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "DeleteObject",
                    new string[] { "", sEntity, entity == null ? "" : entity.GetType().FullName });
            }
            else
            {
                this._DataServiceContext.DeleteObject(entity);
            }
        }

        public void DeleteLink(Object parent, String parentProperty, Object child)
        {
            if (parent != null && child != null)
            {
                this.Log(string.Format("Context.DeleteLink({0}Instance,\"{1}\",{2}Instance)", parent.GetType().Name
                   , parentProperty, child.GetType().Name));
            }

            if (SilverlightRemote.HasRemote)
            {
                string sParent;
                string sChild;

                if (parent != null)
                    sParent = SilverlightRemote.WriteObject(parent);
                else
                {
                    sParent = "TryNull";
                }

                if (child != null)
                    sChild = SilverlightRemote.WriteObject(child);
                else
                {
                    sChild = "TryNull";
                }
                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "DeleteLink", new string[] { parentProperty, sParent, (parent != null) ? parent.GetType().FullName : "", sChild, (child != null) ? child.GetType().FullName : "" });
            }
            else
            {
                this._DataServiceContext.DeleteLink(parent, parentProperty, child);
            }
        }

        public bool DetachLink(Object parent, String parentProperty, Object child)
        {

            if (parent != null && child != null)
            {
                this.Log(string.Format("Context.DetachLink({0}Instance,\"{1}\",{2}Instance)", parent.GetType().Name
                   , parentProperty, child.GetType().Name));

            }

            if (SilverlightRemote.HasRemote)
            {
                string sParent;
                string sChild;

                if (parent != null)
                    sParent = SilverlightRemote.WriteObject(parent);
                else
                {
                    sParent = "TryNull";
                }

                if (child != null)
                    sChild = SilverlightRemote.WriteObject(child);
                else
                {
                    sChild = "TryNull";
                }
                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "DetachLink", new string[] { parentProperty, sParent, (parent != null) ? parent.GetType().FullName : "", sChild, (child != null) ? child.GetType().FullName : "" });
                return (bool)retValue;
            }
            else
            {
                return this._DataServiceContext.DetachLink(parent, parentProperty, child);
            }
        }

        public bool Detach(Object entity)
        {

            if (entity != null)
            {
                this.Log(string.Format("Context.DeleteObject({0}Instance)", entity.GetType().Name));

            }

            if (SilverlightRemote.HasRemote)
            {
                string sEntity = String.Empty;
                if (entity == null)
                {
                    sEntity = "TryNull";
                }
                else
                {
                    sEntity = SilverlightRemote.WriteObject(entity);
                }
                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "Detach", new string[] { "", sEntity, (entity == null) ? "" : entity.GetType().FullName });
                AstoriaTestLog.WriteLine("!!!Detach!!! " + retValue);
                return (bool)retValue;
            }
            else
            {
                return this._DataServiceContext.Detach(entity);
            }
        }

        public void SetLink(Object parent, String parentProperty, Object child)
        {
            if (parent != null && child != null)
            {
                this.Log(string.Format("Context.SetLink({0}Instance,\"{1}\",{2}Instance)", parent.GetType().Name
                    , parentProperty, child == null ? "null" : child.GetType().Name));

            }

            if (SilverlightRemote.HasRemote)
            {
                string sParent = (parent == null) ? "TryNull" : SilverlightRemote.WriteObject(parent);
                string sChild = (child == null) ? "TryNull" : SilverlightRemote.WriteObject(child);

                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "SetLink", new string[] { parentProperty, sParent, parent != null ? parent.GetType().FullName : "", sChild, child != null ? child.GetType().FullName : "" });
            }
            else
            {
                this._DataServiceContext.SetLink(parent, parentProperty, child);
            }
        }

        public void UpdateObject(Object entity)
        {
            if (entity != null)
            {
                this.Log(string.Format("Context.UpdateObject({0}Instance)", entity.GetType().Name));
            }

            if (SilverlightRemote.HasRemote)
            {
                string sEntity = String.Empty;
                if (entity == null)
                {
                    sEntity = "TryNull";
                }
                else
                {
                    sEntity = SilverlightRemote.WriteObject(entity);
                }
                object retValue = SilverlightRemote.Remoter.InvokeSend(this.instance, "UpdateObject",
                    new string[] { "", sEntity, (entity == null) ? "" : entity.GetType().FullName });

            }
            else
            {
                this._DataServiceContext.UpdateObject(entity);
            }
        }
        #endregion

        #region request-sending methods
        #region LoadProperty
        public System.IAsyncResult BeginLoadProperty(Object entity, String propertyName, AsyncCallback callback, Object state)
        {
            return this._DataServiceContext.BeginLoadProperty(entity, propertyName, callback, state);
        }

        public QueryOperationResponseWrapper EndLoadProperty(IAsyncResult asyncResult)
        {
            QueryOperationResponse qoLoadPropResponse = null;
            SocketExceptionHandler.Execute(() => qoLoadPropResponse = this._DataServiceContext.EndLoadProperty(asyncResult));
            return new QueryOperationResponseWrapper(qoLoadPropResponse);
        }


        public object LoadProperty(Object entity, String propertyName)
        {
            if (SilverlightRemote.HasRemote)
            {
                string sEntity = SilverlightRemote.WriteObject(entity);
                string loadedEntity = (string)SilverlightRemote.Remoter.InvokeSend(this.instance, "LoadProperty", new string[] { sEntity, propertyName, entity.GetType().FullName });
                if (loadedEntity.Length > 0)
                    entity = SilverlightRemote.ReadObject(entity.GetType().FullName, loadedEntity);
                return entity;
            }
            else
            {
                QueryOperationResponse qoLoadPropResponse = null;
                SocketExceptionHandler.Execute(() => qoLoadPropResponse = this._DataServiceContext.LoadProperty(entity, propertyName));
                return new QueryOperationResponseWrapper(qoLoadPropResponse);
            }
        }
        #endregion

        #region SaveChanges
        public System.IAsyncResult BeginSaveChanges(AsyncCallback callback, Object state)
        {
            return this._DataServiceContext.BeginSaveChanges(callback, state);
        }

        public System.IAsyncResult BeginSaveChanges(SaveChangesOptions options, AsyncCallback callback, Object state)
        {
            return this._DataServiceContext.BeginSaveChanges(options, callback, state);
        }

        public DataServiceResponseWrapper EndSaveChanges(IAsyncResult asyncResult)
        {
            return new DataServiceResponseWrapper(SocketExceptionHandler.Execute(() => this._DataServiceContext.EndSaveChanges(asyncResult)));
        }


        public DataServiceResponseWrapper SaveChanges(SaveChangesOptions options)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                int instanceID = (int)SilverlightRemote.Remoter.InvokeSend(this.instance, "SaveChangesWithOptions", new string[] { options.ToString() });
                return new DataServiceResponseWrapper(instanceID);
            }
            else
            {
                DataServiceResponse dsResponse = null;
                ActionBag saveAction = ActionBag.CreateActionBag(ContextAction.SaveChanges);
                DataServiceResponseWrapper dsrW = null;
                dsResponse = SocketExceptionHandler.Execute(() => this._DataServiceContext.SaveChanges(options));
                dsrW = new DataServiceResponseWrapper(dsResponse);
                return dsrW;
            }
        }

        public bool DoRemoteBatchQuery(DataServiceRequestWrapper[] queries)
        {
            List<DataServiceRequestProxy> proxyRequest = new List<DataServiceRequestProxy>();
            queries.ToList().ForEach(
                query => proxyRequest.Add(
                    new DataServiceRequestProxy()
                    {
                        ElementType = query.ElementType.FullName,
                        RequestUri = query.RequestUri.OriginalString,
                        ActualCount = query.ActualCount
                    }
                    )
                );
            string remoteQueries = SilverlightRemote.WriteObject(proxyRequest);
            bool testPassed = (bool)SilverlightRemote.Remoter.InvokeSend(this.instance, "ExecuteBatch", new string[] { remoteQueries });

            return testPassed;
        }


        public object RehydrateEntity(object DataEntity)
        {
            object sDataEntity = null;
            if (AstoriaTestProperties.IsRemoteClient)
            {
                string clientEntity = SilverlightRemote.WriteObject(DataEntity);
                string sEntity = (string)SilverlightRemote.Remoter.InvokeSend(this.instance, "RehydrateEntity", new string[] { DataEntity.GetType().FullName ,
                    clientEntity
                });
                sDataEntity = SilverlightRemote.ReadObject(DataEntity.GetType().FullName, sEntity);
            }
            return sDataEntity;
        }



        public DataServiceResponseWrapper SaveChanges()
        {

            this.Log("Context.SaveChanges()");
            if (AstoriaTestProperties.IsRemoteClient)
            {
                int instanceID = (int)SilverlightRemote.Remoter.InvokeSend(this.instance, "SaveChanges", new string[] { });
                return new DataServiceResponseWrapper(instanceID);
            }
            else
                return new DataServiceResponseWrapper(this._DataServiceContext.SaveChanges());

        }


        #endregion

        #region ExecuteBatch
        public System.IAsyncResult BeginExecuteBatch(AsyncCallback callback, Object state, DataServiceRequest[] queries)
        {
            return this._DataServiceContext.BeginExecuteBatch(callback, state, queries);
        }

        public DataServiceResponseWrapper EndExecuteBatch(IAsyncResult asyncResult)
        {
            return new DataServiceResponseWrapper(SocketExceptionHandler.Execute(() => this._DataServiceContext.EndExecuteBatch(asyncResult)));
        }

        public DataServiceResponseWrapper ExecuteBatch(DataServiceRequest[] queries)
        {
            return new DataServiceResponseWrapper(SocketExceptionHandler.Execute(() => this._DataServiceContext.ExecuteBatch(queries)));
        }
        #endregion

        #region Execute
        public System.IAsyncResult BeginExecute<T>(System.Uri uri, AsyncCallback callback, Object state)
        {
            return this._DataServiceContext.BeginExecute<T>(uri, callback, state);
        }

        public IEnumerable<T> EndExecute<T>(IAsyncResult asyncResult)
        {
            return SocketExceptionHandler.Execute(() => this._DataServiceContext.EndExecute<T>(asyncResult));
        }

        public IEnumerable<T> Execute<T>(System.Uri uri)
        {

            if (SilverlightRemote.HasRemote)
            {
                int enuminstance = (int)SilverlightRemote.Remoter.InvokeSend(this.instance, "Execute", new string[] { uri == null ? "TryNull" : uri.ToString(), typeof(T).ToString() });
                EnumerableWrapper<T> enumWrapper = new EnumerableWrapper<T>(enuminstance);
                enumWrapper.TypeHint = typeof(T).FullName;
                return enumWrapper;
            }
            else
            {
                return SocketExceptionHandler.Execute(() => this._DataServiceContext.Execute<T>(uri));
            }
        }

        public QueryOperationResponse<T> Execute<T>(DataServiceQueryContinuation<T> continuation)
        {
            return this.UnderlyingContext.Execute(continuation);
        }

        #endregion
        #endregion

        //not in Silverlight
        public System.Net.ICredentials Credentials
        {

            get
            {
                return this._DataServiceContext.Credentials;
            }
            set
            {
                if (!SilverlightRemote.HasRemote)
                    this._DataServiceContext.Credentials = value;
            }

        }

        //not in Silverlight
        public int Timeout
        {
            get
            {
                return this._DataServiceContext.Timeout;

            }
            set
            {
                if (!SilverlightRemote.HasRemote)
                    this._DataServiceContext.Timeout = value;
            }
        }

        public bool TryGetUri(Object entity, out Uri resourceUri)
        {
            if (SilverlightRemote.HasRemote)
            {
                string eEntity = SilverlightRemote.WriteObject(entity);
                string sEntity = (string)SilverlightRemote.Remoter.InvokeSend(this.instance, "TryGetUri",
                    new string[] { (entity == null) ? "" : entity.GetType().FullName, eEntity });
                resourceUri = new Uri(sEntity, UriKind.RelativeOrAbsolute);
                return true;
            }
            else
            {
                return this._DataServiceContext.TryGetUri(entity, out resourceUri);
            }

        }

        public bool TryGetEntity<T>(Uri identity, out T entity) where T : class
        {
            return this._DataServiceContext.TryGetEntity<T>(identity, out entity);
        }

        public void CancelRequest(IAsyncResult asyncResult)
        {
            this._DataServiceContext.CancelRequest(asyncResult);
        }

        public System.Uri BaseUri
        {
            get
            {
                if (SilverlightRemote.HasRemote)
                {
                    return new System.Uri((string)SilverlightRemote.Remoter.InvokeSend(this.instance, "get_BaseUri", "0"));
                }
                else
                {
                    return this._DataServiceContext.BaseUri;
                }
            }
        }

        public object Product
        {
            get { return _DataServiceContext; }
        }

        public Microsoft.OData.Client.MergeOption MergeOption
        {

            get
            {
                if (SilverlightRemote.HasRemote)
                {
                    return (MergeOption)SilverlightRemote.Remoter.InvokeSend(this.instance, "get_MergeOption", "0");
                }
                else
                {
                    return this._DataServiceContext.MergeOption;
                }
            }
            set
            {
                if (SilverlightRemote.HasRemote)
                {
                    SilverlightRemote.Remoter.InvokeSend(this.instance, "set_MergeOption", ((int)value).ToString());
                }
                else
                {
                    this._DataServiceContext.MergeOption = value;
                }
            }
        }

        public System.Func<System.Type, string> ResolveName
        {
            get
            {
                return this._DataServiceContext.ResolveName;
            }
            set
            {
                if (!SilverlightRemote.HasRemote)
                    this._DataServiceContext.ResolveName = value;
            }
        }

        public System.Func<string, System.Type> ResolveType
        {
            get
            {
                return this._DataServiceContext.ResolveType;
            }
            set
            {
                if (!SilverlightRemote.HasRemote)
                    this._DataServiceContext.ResolveType = value;
            }
        }

        public System.Collections.Generic.IList<EntityDescriptorWrapper> Resources
        {
            get
            {
                if (SilverlightRemote.HasRemote)
                {
                    string oList = (string)SilverlightRemote.Remoter.InvokeSend(this.instance, "Resources", "");
                    List<EntityDescriptorWrapper> edList = new List<EntityDescriptorWrapper>();

                    object newList = SilverlightRemote.ReadObject(edList.GetType().FullName, oList);

                    edList = newList as List<EntityDescriptorWrapper>;

                    return edList;
                }
                else
                {
                    return this._DataServiceContext.Entities.Select(t => new EntityDescriptorWrapper(t)).ToList();
                }
            }
        }

        public System.Collections.Generic.IList<LinkDescriptorWrapper> Links
        {
            get
            {
                if (SilverlightRemote.HasRemote)
                {
                    string oList = (string)SilverlightRemote.Remoter.InvokeSend(this.instance, "Links", "");
                    List<LinkDescriptorWrapper> edList = new List<LinkDescriptorWrapper>();
                    object newList = SilverlightRemote.ReadObject(edList.GetType().FullName, oList);
                    edList = (List<LinkDescriptorWrapper>)newList;
                    return edList;
                }
                else
                {
                    return this._DataServiceContext.Links.Select(t => new LinkDescriptorWrapper(t)).ToList();
                }
            }
        }

        public bool UsePostTunneling
        {
            get
            {
                if (SilverlightRemote.HasRemote)
                {
                    return ((bool)SilverlightRemote.Remoter.InvokeSend(this.instance, "UsePostTunneling", "0"));
                }
                else
                {
                    return this._DataServiceContext.UsePostTunneling;
                }
            }
            set
            {
                if (SilverlightRemote.HasRemote)
                {
                    SilverlightRemote.Remoter.InvokeSend(this.instance, "UsePostTunneling", value.ToString());
                }
                else
                {
                    this._DataServiceContext.UsePostTunneling = value;
                }

            }
        }

        //public UndeclaredPropertyBehavior UndeclaredPropertyBehaviorType
        //{
        //    get
        //    {
        //        if (SilverlightRemote.HasRemote)
        //        {
        //            return ((UndeclaredPropertyBehavior)SilverlightRemote.Remoter.InvokeSend(this.instance, "UndeclaredPropertyBehavior", "Support"));
        //        }
        //        else
        //        {
        //            return this._DataServiceContext.UndeclaredPropertyBehavior;
        //        }
        //    }
        //    set
        //    {
        //        if (!SilverlightRemote.HasRemote)
        //            this._DataServiceContext.UndeclaredPropertyBehavior = value;
        //    }
        //}

        //Events

        public event System.EventHandler<Microsoft.OData.Client.SendingRequest2EventArgs> SendingRequest2;

        public void OnSendingRequest2(Object sender, SendingRequest2EventArgs e)
        {
            if ((this.SendingRequest2 != null))
            {
                this.SendingRequest2(sender, e);
            }
        }
    }



    ///// <summary>
    ///// Wrapper for ResourceObject
    ///// </summary>
    //public struct ResourceObjectWrapper
    //{
    //    private Microsoft.OData.Client.EntityDescriptor _ResourceObject;
    //    private Microsoft.OData.Client.EntityStates _EntityStates;

    //    public object Resource
    //    {
    //        get
    //        {
    //            return this._ResourceObject.Entity;
    //        }
    //    }

    //    public Microsoft.OData.Client.EntityStates State
    //    {
    //        get
    //        {
    //            return this._ResourceObject.State;
    //        }
    //    }
    //}

    //public class ResourceList : System.Collections.Generic.List<EntityDescriptorWrapper>
    //{

    //    public ResourceList()
    //        : base()
    //    {
    //    }

    //    public int instanceID = 0;

    //    public new long Count()
    //    {
    //        long count = 0;
    //        if (SilverlightRemote.HasRemote)
    //        {
    //            count = (long)SilverlightRemote.Remoter.InvokeSend(instanceID, "ResourceCount", null);
    //        }
    //        else
    //        {
    //            count = base.Count;
    //        }
    //        return count;
    //    }
    //}

    //[DataContract(Name = "LinkDescriptorWrapper")]
    //public class LinkDescriptorWrapper
    //{
    //    [DataMember]
    //    public EntityStates State
    //    {
    //        get;
    //        set;
    //    }
    //    [DataMember]
    //    public string SourceProperty
    //    {
    //        get;
    //        set;
    //    }

    //}
    public class DescriptorWrapper
    {
        [DataMember]
        public Microsoft.OData.Client.EntityStates State { get; set; }
    }

    public class LinkDescriptorWrapper : DescriptorWrapper
    {
        [DataMember]
        public string SourceProperty { get; set; }
        [DataMember]
        public object Source { get; set; }
        [DataMember]
        public object Target { get; set; }

        public LinkDescriptorWrapper() { }

        public LinkDescriptorWrapper(LinkDescriptor ed)
        {
            this.State = ed.State;
            this.SourceProperty = ed.SourceProperty;
            this.Source = ed.Source;
            this.Target = ed.Target;
        }
    }

    /// <summary>
    /// Wrapper for EntityDescriptor
    /// </summary>
    /// 
    public class EntityDescriptorWrapper : DescriptorWrapper
    {
        private object entity;

        public EntityDescriptorWrapper() { }

        public EntityDescriptorWrapper(EntityDescriptor ed)
        {
            this.Entity = ed.Entity;
            this.ETag = ed.ETag;
            this.State = ed.State;
            this.Parent = ed.ParentForInsert;
            this.ParentProperty = ed.ParentPropertyForInsert;
            this.ServerTypeName = ed.ServerTypeName;
            this.EditLink = ed.EditLink;
            this.SelfLink = ed.SelfLink;
        }

        public object Parent { get; set; }
        public object ParentProperty { get; set; }

        public string ETag { get; set; }
        public object Entity { get { return this.entity; } set { this.entity = value; } }
        public String ServerTypeName { get; set; }

        public Uri SelfLink { get; set; }
        public Uri EditLink { get; set; }
    }

    ///// <summary>
    ///// Wrapper for LinkDescriptor
    ///// </summary>
    //public struct LinkDescriptorWrapper
    //{
    //    private Microsoft.OData.Client.LinkDescriptor _LinkDescriptor;

    //    public LinkDescriptorWrapper(LinkDescriptor ld)
    //    {
    //        this._LinkDescriptor = ld;
    //    }

    //    public object Target
    //    {
    //        get
    //        {
    //            return this._LinkDescriptor.Target;
    //        }
    //    }

    //    public Microsoft.OData.Client.EntityStates State
    //    {
    //        get
    //        {
    //            return this._LinkDescriptor.State;
    //        }
    //    }

    //    public string SourceProperty
    //    {
    //        get
    //        {
    //            return this._LinkDescriptor.SourceProperty;
    //        }
    //    }

    //    public object Source
    //    {
    //        get
    //        {
    //            return this._LinkDescriptor.Source;
    //        }
    //    }

    //    public System.Exception Error
    //    {
    //        get
    //        {
    //            return this._LinkDescriptor.Error;
    //        }
    //    }
    //}

    //public abstract class EnumerableWrapper : IEnumerable
    //{
    //    public static EnumerableWrapper Create(Type entityType, WebDataCtxWrapper context)
    //    {
    //        ConstructorInfo constructor = typeof(EnumerableWrapper<>).MakeGenericType(entityType).GetConstructor(new Type[] { typeof(int) });
    //        return constructor.Invoke(new object[] { context.instance }) as EnumerableWrapper;
    //    }

    //    public virtual string TypeHint { get; set; }
    //    #region IEnumerable Members

    //    public virtual IEnumerator GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}
    /// <summary>
    /// Wrapper for IEnumerable<T>
    /// </summary>
    public class EnumerableWrapper<T> : IEnumerable<T>
    {
        public int _instance;
        public string TypeHint { get; set; }

        public EnumerableWrapper(int instance)
        {
            _instance = instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            EnumeratorWrapper<T> enumerator = new EnumeratorWrapper<T>(this._instance, TypeHint);
            enumerator.TypeHint = this.TypeHint;
            return enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            EnumeratorWrapper<T> enumerator = new EnumeratorWrapper<T>(this._instance, TypeHint);
            enumerator.TypeHint = this.TypeHint;
            return enumerator;
        }
    }

    public class EnumeratorWrapper<T> : IEnumerator<T>
    {

        public int _instance;
        public List<string> _underlyingObjects;
        public string TypeHint { get; set; }


        object _currentObject = null;
        int index = -1;
        public EnumeratorWrapper(int instance)
        {
            try
            {
                this._instance = instance;
                _underlyingObjects = new List<string>();
                object sResult = SilverlightRemote.Remoter.InvokeSend(_instance, "GetEnumeratorContent", TypeHint); ;
                if (sResult.ToString().Contains("String too big , cannot serialize"))
                {
                    throw new InvalidOperationException("String too big , cannot serialize");
                }
                _underlyingObjects = (List<string>)SilverlightRemote.ReadObject(_underlyingObjects.GetType().ToString(), (string)sResult);
            }
            catch (SerializationException se)
            {
                throw se;
            }
            catch
            {
                throw;
            }


        }

        public EnumeratorWrapper(int instance, string typeHint)
        {
            try
            {
                TypeHint = typeHint;
                this._instance = instance;
                _underlyingObjects = new List<string>();
                object sResult = SilverlightRemote.Remoter.InvokeSend(_instance, "GetEnumeratorContent", TypeHint); ;
                if (sResult.ToString().Contains("String too big , cannot serialize"))
                {
                    throw new InvalidOperationException("String too big , cannot serialize");
                }
                _underlyingObjects = (List<string>)SilverlightRemote.ReadObject(_underlyingObjects.GetType().ToString(), (string)sResult);
            }
            catch (SerializationException se)
            {
                throw se;
            }
            catch
            {
                throw;
            }


        }

        public T Current
        {
            get
            {
                return (T)_currentObject;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return (T)_currentObject;
            }
        }

        public void Dispose()
        {
            // this._enumerator.Dispose();
        }

        public bool MoveNext()
        {
            index++;
            bool canMoveForward = false;

            if (_underlyingObjects != null)
                canMoveForward = index < _underlyingObjects.Count;

            if (canMoveForward)
            {
                object sResult = _underlyingObjects[index];
                if (sResult.ToString() == "null")
                {
                    _currentObject = null;
                }
                else
                {
                    AstoriaTestLog.TraceLine(AstoriaTraceLevel.Info, "TypeHint is  :: " + TypeHint);
                    if (TypeHint != null && TypeHint.Length > 0)
                    {
                        _currentObject = SilverlightRemote.ReadObject(this.TypeHint, (string)sResult);
                    }
                    else
                    {
                        _currentObject = SilverlightRemote.ReadObject(typeof(T).ToString(), (string)sResult);
                    }
                }
            }

            return canMoveForward;
        }
        public void Reset()
        {
            //this._enumerator.Reset();
        }
    }




}

namespace EDMMetadataExtensions
{
#if !ClientSKUFramework
    using Microsoft.OData.Edm;

    public static class EDMExtensions
    {
        public static List<string> ReferenceNavigationProperties(this IEdmEntityType entityType)
        {
            return GetNavPropsCore(entityType, false);
        }

        public static bool CanDelete(this IEdmEntityType entityType)
        {
            bool canDeleteEntity = false;
            //You can delete an entity type instance if it doesnt have any relations that need to be kept alive
            foreach (var navProperty in entityType.NavigationProperties())
            {
                IEdmEntityType navEntityType = System.Data.Test.Astoria.DataServiceMetadata.GetEntityType(navProperty);
                canDeleteEntity = navEntityType.NavigationProperties().Any(navProp =>
                    System.Data.Test.Astoria.DataServiceMetadata.GetEntityType(navProp) == entityType
                    );
            }
            return canDeleteEntity;
        }


        private static List<string> GetNavPropsCore(IEdmEntityType entityType, bool collections)
        {
            List<string> navProps = new List<string>();
            foreach (var navProperty in entityType.NavigationProperties())
            {
                if (collections == navProperty.Type.IsCollection())
                {
                    navProps.Add(navProperty.Name);
                }
            }
            return navProps;
        }

        public static List<string> CollectionNavigationProperties(this IEdmEntityType entityType)
        {
            return GetNavPropsCore(entityType, true);
        }

        public static bool HasConcurrencyToken(this IEdmEntityType entityType)
        {
            bool entityTypeHasConTokens = false;
            foreach (var property in entityType.StructuralProperties())
            {
                entityTypeHasConTokens = property.ConcurrencyMode == EdmConcurrencyMode.Fixed;
                if (entityTypeHasConTokens)
                    break;
            }
            return entityTypeHasConTokens;
        }

        public static List<string> EtagProperties(this IEdmEntityType entityType)
        {
            List<string> propertiesAsEtags = new List<string>();
            bool entityTypeHasConTokens = false;
            foreach (var property in entityType.StructuralProperties())
            {
                entityTypeHasConTokens = property.ConcurrencyMode == EdmConcurrencyMode.Fixed;
                if (entityTypeHasConTokens)
                {
                    propertiesAsEtags.Add(property.Name);
                }
            }
            return propertiesAsEtags;
        }
    }
#endif

}

namespace LinqExtensions
{
    using System.Data.Test.Astoria;
    using System.Linq.Expressions;

    public static class DataServiceQueryExtensions
    {
        public static IAsyncResult BeginExecuteOfT(this DataServiceQuery dsEntityQuery, AsyncCallback callback, object state)
        {
            MethodInfo beginExecuteMethod = dsEntityQuery.GetType().GetMethod("BeginExecute");
            return beginExecuteMethod.Invoke(dsEntityQuery, new object[] { callback, state }) as IAsyncResult;
        }
        public static QueryOperationResponse EndExecuteOfT(this DataServiceQuery dsEntityQuery, IAsyncResult asyncResult)
        {
            MethodInfo endExecuteMethod = dsEntityQuery.GetType().GetMethod("EndExecute");
            return endExecuteMethod.Invoke(dsEntityQuery, new object[] { asyncResult }) as QueryOperationResponse;
        }
        public static DataServiceQuery SelectNone(this DataServiceQuery dsEntityQuery)
        {
            ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(dsEntityQuery.ElementType, "l");
            Expression fail = Expression.Equal(
                        Expression.Constant(2),
                        Expression.Constant(3)
                        );
            var finalExpression = (LambdaExpression)BuildLamdaExpression(fail, parameterExpression, dsEntityQuery.ElementType, typeof(bool));

            Type[] typeArgs = new Type[] { dsEntityQuery.ElementType };
            var result = Expression.Call(typeof(Queryable), "Where", typeArgs, dsEntityQuery.Expression, finalExpression);
            return (DataServiceQuery)dsEntityQuery.Provider.CreateQuery(result);

        }

        public static DataServiceQuery SelectAll(this DataServiceQuery dsEntityQuery)
        {
            ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(dsEntityQuery.ElementType, "l");
            Expression fail = Expression.Equal(
                        Expression.Constant(2),
                        Expression.Constant(2)
                        );
            var finalExpression = (LambdaExpression)BuildLamdaExpression(fail, parameterExpression, dsEntityQuery.ElementType, typeof(bool));

            Type[] typeArgs = new Type[] { dsEntityQuery.ElementType };
            var result = Expression.Call(typeof(Queryable), "Where", typeArgs, dsEntityQuery.Expression, finalExpression);
            return (DataServiceQuery)dsEntityQuery.Provider.CreateQuery(result);

        }

        public static DataServiceQuery OrderBy
            (this DataServiceQuery entitySetQuery, List<string> oderByFields)
        {
            ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(entitySetQuery.ElementType, "l");
            entitySetQuery = OrderByCore(entitySetQuery, oderByFields, parameterExpression, "OrderBy");
            return entitySetQuery;
        }

        public static DataServiceQuery OrderByDescending
           (this DataServiceQuery entitySetQuery, List<string> oderByFields)
        {
            ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(entitySetQuery.ElementType, "l");
            entitySetQuery = OrderByCore(entitySetQuery, oderByFields, parameterExpression, "OrderByDescending");
            return entitySetQuery;
        }

        private static DataServiceQuery OrderByCore(DataServiceQuery entitySetQuery,
            List<string> oderByFields,
            System.Linq.Expressions.ParameterExpression parameterExpression,
            string orderBYMethod)
        {
            foreach (string field in oderByFields)
            {
                Type fieldType = entitySetQuery.ElementType.GetProperty(field).PropertyType;
                Expression constant = Expression.Property(parameterExpression, field);

                var lambda = BuildLamdaExpression(constant, parameterExpression, entitySetQuery.ElementType, fieldType);

                Type[] typeArgs = new Type[] { entitySetQuery.ElementType, lambda.Body.Type };
                var mcExpression = Expression.Call(typeof(Queryable), orderBYMethod, typeArgs, entitySetQuery.Expression, lambda);
                entitySetQuery = (DataServiceQuery)entitySetQuery.Provider.CreateQuery(mcExpression);
            }
            return entitySetQuery;
        }

        public static DataServiceQuery Skip(this DataServiceQuery dsEntityQuery, int topValue, bool dummyFieldToAvoidCollision)
        {
            Type[] typeArgs = new Type[] { dsEntityQuery.ElementType };
            Expression topValueExpression = Expression.Constant(topValue);
            var result = Expression.Call(typeof(Queryable), "Skip", typeArgs, dsEntityQuery.Expression, topValueExpression);
            return (DataServiceQuery)dsEntityQuery.Provider.CreateQuery(result);
        }
        public static DataServiceQuery Take(this DataServiceQuery dsEntityQuery, int topValue, bool dummyFieldToAvoidCollision)
        {
            Type[] typeArgs = new Type[] { dsEntityQuery.ElementType };
            Expression topValueExpression = Expression.Constant(topValue);
            var result = Expression.Call(typeof(Queryable), "Take", typeArgs, dsEntityQuery.Expression, topValueExpression);
            return (DataServiceQuery)dsEntityQuery.Provider.CreateQuery(result);
        }
        public static DataServiceQuery OfType(this DataServiceQuery dsEntityQuery, Type typeToCheck)
        {
            Type[] typeArgs = new Type[] { dsEntityQuery.ElementType };
            Expression queryExpression = Expression.Call(typeof(Queryable), "OfType", typeArgs, dsEntityQuery.Expression);
            return (DataServiceQuery)dsEntityQuery.Provider.CreateQuery(queryExpression);
        }

        public static object First(this DataServiceQuery dsEntityQuery, bool dummyFieldToAvoidCollision)
        {
            Type[] typeArgs = new Type[] { dsEntityQuery.ElementType };
            var result = Expression.Call(typeof(Queryable), "First", typeArgs, dsEntityQuery.Expression);
            return dsEntityQuery.Provider.Execute(result);
        }
        public static object FirstOrDefault(this DataServiceQuery dsEntityQuery, bool dummyFieldToAvoidCollision)
        {
            Type[] typeArgs = new Type[] { dsEntityQuery.ElementType };
            var result = Expression.Call(typeof(Queryable), "FirstOrDefault", typeArgs, dsEntityQuery.Expression);
            return dsEntityQuery.Provider.Execute(result);
        }

        public static object Single(this DataServiceQuery dsEntityQuery, bool dummyFieldToAvoidCollision)
        {
            Type[] typeArgs = new Type[] { dsEntityQuery.ElementType };
            var result = Expression.Call(typeof(Queryable), "Single", typeArgs, dsEntityQuery.Expression);
            return (DataServiceQuery)dsEntityQuery.Provider.CreateQuery(result);
        }


        public static DataServiceQuery Expand(this DataServiceQuery dsEntityQuery, string navigationPropertyName)
        {
            //Expression navPropExpression = Expression.Constant(navigationPropertyName);
            MethodInfo expandMethod = dsEntityQuery.GetType().GetMethod("Expand", new Type[] { typeof(string) });
            return (DataServiceQuery)expandMethod.Invoke(dsEntityQuery, new object[] { navigationPropertyName });
            //Type[] typeArgs = new Type[] { dsEntityQuery.ElementType };
            //var result = Expression.Call(typeof(DataServiceQuery<>), "Expand", typeArgs, dsEntityQuery.Expression, navPropExpression);
            //return (DataServiceQuery)dsEntityQuery.Provider.CreateQuery(result);
        }
        public static DataServiceQuery Where(this DataServiceQuery entitySetQuery, List<System.Data.Test.Astoria.KVP> KeyValuePairs)
        {
            return (DataServiceQuery)(entitySetQuery.Provider.CreateQuery(GetKeyExpression(entitySetQuery, KeyValuePairs)));
        }
        public static DataServiceQuery ThenByAscending
            (this DataServiceQuery entitySetQuery, List<string> oderByFields)
        {
            entitySetQuery = ThenbyCore(entitySetQuery, oderByFields, "ThenBy");
            return entitySetQuery;
        }

        public static DataServiceQuery ThenByDescending
            (this DataServiceQuery entitySetQuery, List<string> oderByFields)
        {
            entitySetQuery = ThenbyCore(entitySetQuery, oderByFields, "ThenByDescending");
            return entitySetQuery;
        }

        public static object SelectNavigationProperty(this DataServiceQuery dsEntityQuery, string navigationPropertyName)
        {
            ParameterExpression parameterExpression = Expression.Parameter(dsEntityQuery.ElementType, "l");
            Expression navProp = Expression.Property(parameterExpression, navigationPropertyName);

            var finalExpression = BuildLamdaExpression(navProp, parameterExpression, dsEntityQuery.ElementType, dsEntityQuery.ElementType.GetProperty(navigationPropertyName).PropertyType);
            Type[] typeArgs = new Type[] { dsEntityQuery.ElementType, dsEntityQuery.ElementType.GetProperty(navigationPropertyName).PropertyType };
            var result = Expression.Call(typeof(Queryable), "Select", typeArgs, dsEntityQuery.Expression, finalExpression);
            return (DataServiceQuery)dsEntityQuery.Provider.CreateQuery(result);

        }

        public static DataServiceQuery SelectConstant
            (this DataServiceQuery entitySetQuery, object value)
        {
            ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(entitySetQuery.ElementType, "l");
            System.Linq.Expressions.ConstantExpression constant = Expression.Constant(value, value.GetType());
            var finalExpression = BuildLamdaExpression(constant, parameterExpression, entitySetQuery.ElementType, value.GetType());
            Type[] typeArgs = new Type[] { entitySetQuery.ElementType, finalExpression.Body.Type };
            var result = Expression.Call(typeof(Queryable), "Select", typeArgs, entitySetQuery.Expression, finalExpression);
            return (DataServiceQuery)entitySetQuery.Provider.CreateQuery(result);
        }

        public static DataServiceQuery
            SelectEntitySet(string entitySetName, WebDataCtxWrapper dsContext, Type entityType)
        {
            MethodInfo miGenCQ = dsContext.GetType().GetMethod("CreateQuery").MakeGenericMethod(entityType);
            return (DataServiceQuery)miGenCQ.Invoke(dsContext, new object[] { entitySetName });
        }


        #region Private helper methods
        private static LambdaExpression
            BuildLamdaExpression(Expression condition, System.Linq.Expressions.ParameterExpression parameterExpression, Type entityType, Type returnType)
        {
            var genericFunc = LambdaExpression.GetFuncType(entityType, returnType);
            var lambdaExpression = Expression.Lambda(condition, parameterExpression);
            return lambdaExpression;
        }
        private static DataServiceQuery ThenbyCore(DataServiceQuery entitySetQuery, List<string> oderByFields
            , string functionName)
        {
            ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(entitySetQuery.ElementType, "l");
            foreach (string field in oderByFields)
            {

                Type fieldType = entitySetQuery.ElementType.GetProperty(field).PropertyType;
                Expression fieldExpression = Expression.Property(parameterExpression, field);

                var expr = BuildLamdaExpression(fieldExpression, parameterExpression,
                    entitySetQuery.ElementType, fieldType);

                Type[] typeArgs = new Type[] { entitySetQuery.ElementType, ((LambdaExpression)expr).Body.Type };
                var mcExpression = Expression.Call(typeof(Queryable), functionName, typeArgs, entitySetQuery.Expression, (Expression)expr);
                entitySetQuery = (DataServiceQuery)entitySetQuery.Provider.CreateQuery(mcExpression);
            }
            return entitySetQuery;
        }

        public static Expression GetKeyExpression(DataServiceQuery entitySetQuery, List<System.Data.Test.Astoria.KVP> KeyValuePairs)
        {
            ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(entitySetQuery.ElementType, "l");
            Expression KeyExpression = CreateWhereExpression(parameterExpression, KeyValuePairs);
            var finalExpression = (LambdaExpression)BuildLamdaExpression(KeyExpression, parameterExpression, entitySetQuery.ElementType, typeof(bool));
            Type[] typeArgs = new Type[] { entitySetQuery.ElementType };
            var keyMethodCall = Expression.Call(typeof(Queryable), "Where", typeArgs, entitySetQuery.Expression, finalExpression);
            return keyMethodCall;
        }
        public static Expression CreateWhereExpression(System.Linq.Expressions.ParameterExpression parameterExpression
            , List<KVP> KeyValuePairs)
        {
            Expression KeyExpression = null;
            foreach (System.Data.Test.Astoria.KVP keyProperty in KeyValuePairs)
            {
                Expression left = Expression.Property(parameterExpression, keyProperty.Key);
                Expression compareExpression = Expression.Equal(left, Expression.Constant(keyProperty.Value));
                KeyExpression = (KeyExpression == null)
                                ? compareExpression
                                : Expression.And(KeyExpression, compareExpression);
            }
            return KeyExpression;
        }
        #endregion


    }
}
