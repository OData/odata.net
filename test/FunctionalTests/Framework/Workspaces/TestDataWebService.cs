//---------------------------------------------------------------------
// <copyright file="TestDataWebService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Service;
using System.Data.Test.Astoria.CallOrder;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.ServiceModel.Web;
using System.Threading;
using System.Xml.Linq;
using System.Data.Test.Astoria.Providers;
#if !ClientSKUFramework
using System.Data.Linq;
using System.Xml;
#endif

namespace System.Data.Test.Astoria
{
    #region trace listener that turns assertions into exceptions
    internal class ExceptionThrowingTraceListener : System.Diagnostics.DefaultTraceListener
    {
        public ExceptionThrowingTraceListener()
            : base()
        {
            this.AssertUiEnabled = false;
            this.LogFileName = null;
            this.TraceOutputOptions = System.Diagnostics.TraceOptions.None;
        }

        public override void Fail(string message)
        {
            throw new Exception(message);
        }

        public override void Fail(string message, string detailMessage)
        {
            throw new Exception(message, new Exception(detailMessage));
        }
    }
    #endregion

    [CLSCompliant(false)]
    public abstract partial class TestDataWebService<T> : DataService<T>
    {
        public TestDataWebService()
            : base()
        {
#if !ASTORIA_PRE_V2
            this.ProcessingPipeline.ProcessedChangeset += new EventHandler<EventArgs>(APICallLog_ProcessedChangeset);
            this.ProcessingPipeline.ProcessedRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(APICallLog_ProcessedRequest);
            this.ProcessingPipeline.ProcessingChangeset += new EventHandler<EventArgs>(APICallLog_ProcessingChangeset);
            this.ProcessingPipeline.ProcessingRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(APICallLog_ProcessingRequest);
#endif

            OnConstruction();
        }

        protected virtual void OnConstruction()
        {
            // Do nothing here, will be overidden as needed during workspace creation
        }

        #region Processing pipeline hooks
#if !ASTORIA_PRE_V2
        private static void APICallLog_ProcessingRequest(object sender, DataServiceProcessingPipelineEventArgs e)
        {
            APICallLog.Current.ProcessingPipeline.ProcessingRequest(sender, e);
            APICallLog.Current.Pop();
        }

        private static void APICallLog_ProcessingChangeset(object sender, EventArgs e)
        {
            APICallLog.Current.ProcessingPipeline.ProcessingChangeset(sender, e);
            APICallLog.Current.Pop();
        }

        private static void APICallLog_ProcessedRequest(object sender, DataServiceProcessingPipelineEventArgs e)
        {
            APICallLog.Current.ProcessingPipeline.ProcessedRequest(sender, e);
            APICallLog.Current.Pop();
        }

        private static void APICallLog_ProcessedChangeset(object sender, EventArgs e)
        {
            APICallLog.Current.ProcessingPipeline.ProcessedChangeset(sender, e);
            APICallLog.Current.Pop();
        }
#endif
        #endregion

        private static ConfigurationSettings configurationSettings = new ConfigurationSettings() { UseVerboseErrors = true };

        public static ConfigurationSettings ConfigurationSettings
        {
            get { return configurationSettings; }
        }

        private static bool ClearAllIfRequired()
        {
            bool ShouldClear = false;
            if (WebOperationContext.Current != null)
            {
                if (WebOperationContext.Current.IncomingRequest.Headers.AllKeys.Any(head => head == "ClearAllSettings"))
                {
                    ShouldClear = true;
                    configurationSettings.ClearAll();
                }

                foreach (var header in WebOperationContext.Current.IncomingRequest.Headers.AllKeys)
                {
                    const string prefix = "OverrideServiceConfig-";
                    if (header.StartsWith(prefix))
                    {
                        string headerValue = WebOperationContext.Current.IncomingRequest.Headers[header];
                        string propertyName = header.Substring(prefix.Length);
                        if (propertyName == "MaxProtocolVersion")
                        {
                            int intValue;
                            if(int.TryParse(headerValue, out intValue))
                            {
                                configurationSettings.MaxProtocolVersion = intValue;
                            }
                            else
                            {
                                configurationSettings.MaxProtocolVersion = null;
                            }
                        }
                    }
                }
            }

            return ShouldClear;
        }

        /// <summary>Initializes the service configuration.</summary>
        public static void InitializeService(IDataServiceConfiguration configuration)
        {
            APICallLog.Current.DataService.InitializeService("TestDataWebService");
            //Because the configuration settings can be deliberately messed up on the server-side before calling ClearAll() , 
            //we need this to be the first step in the InitializeService
            ClearAllIfRequired();
            try
            {
                foreach (string setting in configurationSettings.PropertySetList)
                {
                    if (setting == "UseVerboseErrors")
                        configuration.UseVerboseErrors = configurationSettings.UseVerboseErrors;

                    else if (setting == "MaxBatchCount")
                        configuration.MaxBatchCount = configurationSettings.MaxBatchCount;

                    else if (setting == "MaxChangesetCount")
                        configuration.MaxChangesetCount = configurationSettings.MaxChangesetCount;

                    else if (setting == "MaxExpandCount")
                        configuration.MaxExpandCount = configurationSettings.MaxExpandCount;

                    else if (setting == "MaxExpandDepth")
                        configuration.MaxExpandDepth = configurationSettings.MaxExpandDepth;

                    else if (setting == "MaxObjectCountOnInsert")
                        configuration.MaxObjectCountOnInsert = configurationSettings.MaxObjectCountOnInsert;

                    else if (setting == "MaxResultsPerCollection")
                        configuration.MaxResultsPerCollection = configurationSettings.MaxResultsPerCollection;
                }

                foreach (string entity in configurationSettings.EntitySetAccessRules.Keys)
                {
                    configuration.SetEntitySetAccessRule(entity, configurationSettings.EntitySetAccessRules[entity]);
                }

                foreach (string entity in configurationSettings.ServiceOperationAccessRules.Keys)
                {
                    configuration.SetServiceOperationAccessRule(entity, configurationSettings.ServiceOperationAccessRules[entity]);
                }

                MethodInfo mi = configuration.GetType().GetMethod("SetEntitySetPageSize", BindingFlags.Instance | BindingFlags.Public);
                if (mi != null)
                {
                    foreach (string entity in configurationSettings.EntitySetPageSizes.Keys)
                    {
                        mi.Invoke(configuration, new object[] { entity, configurationSettings.EntitySetPageSizes[entity] });
                    }
                }

                foreach (string type in configurationSettings.KnownTypesToRegister)
                {
                    Type t = Type.GetType(type);

                    if (t == null)
                        t = typeof(TestDataWebService<>).Assembly.GetType(type);

                    if (t == null)
                        throw new DataServiceException(500, "Could not resolve type with name '" + type + "'");

                    configuration.RegisterKnownType(t);
                }

                MethodInfo enableTypeAccess = configuration.GetType().GetMethod("EnableTypeAccess", BindingFlags.Instance | BindingFlags.Public);
                if (enableTypeAccess != null)
                {
                    // Note: the provider wrapper for IDSMP is strict about calls to TryResolveResourceType.
                    // However, the DataServiceConfiguration.EnableTypeAccess method does not cache resolved types,
                    // so we need to explicitly turn off the extra validation before calling it.
                    bool oldValue = MetadataProviderWrapper.ValidateTypeResolution;
                    try
                    {
                        MetadataProviderWrapper.ValidateTypeResolution = false;
                        foreach (string typeName in configurationSettings.EnableAccessToTypes)
                        {
                            enableTypeAccess.Invoke(configuration, new object[] { typeName });
                        }
                    }
                    finally
                    {
                        // make sure to turn the validation back on, if it was on
                        MetadataProviderWrapper.ValidateTypeResolution = oldValue;
                    }
                }

                if (configuration.GetType().Name == "DataServiceConfiguration")
                {
                    PropertyInfo dsb = configuration.GetType().GetProperty("DataServiceBehavior");
                    if (dsb != null)
                    {
                        object dataServiceBevhaior = dsb.GetValue(configuration, null);
                        if (dataServiceBevhaior != null)
                        {
                            Type dataServiceBehaviorType = dataServiceBevhaior.GetType();
                            
                            if (configurationSettings.PropertySetList.Contains("AllowCountRequests"))
                            {
                                PropertyInfo propertyInfo = dataServiceBehaviorType.GetProperty("AcceptCountRequests");
                                propertyInfo.SetValue(dataServiceBevhaior, configurationSettings.AllowCountRequests, null);
                            }

                            if (configurationSettings.PropertySetList.Contains("AllowProjectionRequests"))
                            {
                                PropertyInfo propertyInfo = dataServiceBehaviorType.GetProperty("AcceptProjectionRequests");
                                propertyInfo.SetValue(dataServiceBevhaior, configurationSettings.AllowProjectionRequests, null);
                            }

                            if (configurationSettings.PropertySetList.Contains("InvokeInterceptorsOnLinkDelete"))
                            {
                                PropertyInfo propertyInfo = dataServiceBehaviorType.GetProperty("InvokeInterceptorsOnLinkDelete");
                                propertyInfo.SetValue(dataServiceBevhaior, configurationSettings.InvokeInterceptorsOnLinkDelete, null);
                            }

                            if (configurationSettings.PropertySetList.Contains("MaxProtocolVersion") && configurationSettings.MaxProtocolVersion.HasValue)
                            {
                                PropertyInfo propertyInfo = dataServiceBehaviorType.GetProperty("MaxProtocolVersion");
                                propertyInfo.SetValue(dataServiceBevhaior, configurationSettings.MaxProtocolVersion, null);
                            }
                        }
                    }
                    //
                    //object 
                }
            }
            catch (Exception excpt)
            {
                if (!ClearAllIfRequired())
                {
                    throw new DataServiceException(excpt.ToString(), excpt);
                }
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        #region service operations
        [WebGet]
        [SingleResult]
        public IQueryable<int> GetNoParam()
        {
            int[] x = new int[] { 5 };
            return x.AsQueryable();
        }

        [WebInvoke(Method = "POST")]
        [SingleResult]
        public IQueryable<int> PostNoParam()
        {
            return GetNoParam();
        }

        public virtual IQueryable<EntityType> GetEntitySet<EntityType>(string entitySetName)
        {
            // fall-back property-based reflection
            PropertyInfo property = this.CurrentDataSource.GetType().GetProperty(entitySetName);
            if (property == null)
                throw new DataServiceException(500, "Cannot get entity set '" + entitySetName + "' because the corresponding IQueryable property cannot be found");
            object thisEntitySetValue = property.GetValue(this.CurrentDataSource, null);
            return ((IQueryable)thisEntitySetValue).OfType<EntityType>();//.Take(10);
        }

        public EntityType GetEntitySetSingleton<EntityType>(string entitySetName)
        {
            #region Commented Code to create Expressions to return value
            //entity=>typeof(Customers)
            //ParameterExpression param = Expression.Parameter(this.CurrentDataSource.GetType(), "entity");
            //Expression entitySet = Expression.Property(param, entitySetName);
            //PropertyInfo property = this.CurrentDataSource.GetType().GetProperty(entitySetName);
            //object thisEntitySetValue = property.GetValue(this.CurrentDataSource, null);
            //Type[] typeArgs = new Type[] { typeof(EntityType) };
            ////this.CurrentDataSource.Customers.OfType<Customers>()
            //MethodCallExpression entitiesOfThisType = Expression.Call(typeof(Queryable), "OfType", typeArgs, entitySet);
            ////Func<Customers,Customers> func = (cust1,cust2)=>{ return cust1 is cust2; }
            //LambdaExpression callPreviousExpression = Expression.Lambda(entitiesOfThisType, param);
            //typeArgs = new Type[] { callPreviousExpression.Body.Type, typeof(EntityType) };
            ////this.CurrentDataSource.Customers.OfType<Customers>().First()
            //var mcExpression = Expression.Call(typeof(Queryable), "First", typeArgs, ((IQueryable)thisEntitySetValue).Expression, callPreviousExpression);

            //return (EntityType)((IQueryable)thisEntitySetValue).Provider.Execute(mcExpression); 
            #endregion
            PropertyInfo property = this.CurrentDataSource.GetType().GetProperty(entitySetName);
            object thisEntitySetValue = property.GetValue(this.CurrentDataSource, null);
            return ((IQueryable)thisEntitySetValue).OfType<EntityType>().First();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<int> GetSingleParam(int intParam)
        {
            int[] x = new int[] { intParam };
            return x.AsQueryable();
        }

        [WebInvoke(Method = "POST")]
        [SingleResult]
        public IQueryable<int> PostSingleParam(int intParam)
        {
            return GetSingleParam(intParam);
        }

        [WebGet]
        [SingleResult]
        public IQueryable<int> GetTwoParams(int intParam, int intParam2)
        {
            int[] x = new int[] { intParam };
            return x.AsQueryable();
        }

        [WebInvoke(Method = "POST")]
        [SingleResult]
        public IQueryable<int> PostTwoParams(int intParam, int intParam2)
        {
            return GetTwoParams(intParam, intParam2);
        }

        [WebGet]
        [SingleResult]
        public IQueryable<Double> GetAllParamTypes(byte Byte, bool Bool, Decimal dec, Double doub, Int16 int16, Int32 int32,
             Int64 int64, Single single, String str, DateTime dt)
        {
            Double[] x = new Double[] { 2 };
            return x.AsQueryable();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<Guid> GetGuid(Guid guid)
        {
            Guid[] x = new Guid[] { guid };
            return x.AsQueryable();
        }


        [WebGet]
        [SingleResult]
        public IQueryable<byte> GetByte(byte Byte)
        {
            byte[] x = new byte[] { Byte };
            return x.AsQueryable();
        }


        [WebGet]
        [SingleResult]
        public IQueryable<bool> GetBool(bool Bool)
        {
            bool[] x = new bool[] { Bool };
            return x.AsQueryable();
        }


        [WebGet]
        [SingleResult]
        public IQueryable<Decimal> GetDecimal(Decimal dec)
        {
            Decimal[] x = new Decimal[] { dec };
            return x.AsQueryable();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<Double> GetDouble(Double doub)
        {
            Double[] x = new Double[] { doub };
            return x.AsQueryable();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<Int16> GetInt16(Int16 int16)
        {
            Int16[] x = new Int16[] { int16 };
            return x.AsQueryable();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<Int32> GetInt32(Int32 int32)
        {
            Int32[] x = new Int32[] { int32 };
            return x.AsQueryable();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<Int64> GetInt64(Int64 int64)
        {
            Int64[] x = new Int64[] { int64 };
            return x.AsQueryable();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<Single> GetSingle(Single single)
        {
            Single[] x = new Single[] { single };
            return x.AsQueryable();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<String> GetString(String str)
        {
            String[] x = new String[] { str };
            return x.AsQueryable();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<SByte> GetSByte(SByte Sbyte)
        {
            SByte[] x = new SByte[] { Sbyte };
            return x.AsQueryable();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<DateTime> GetDateTime(DateTime datetime)
        {
            DateTime returnDateTime = datetime;

            DateTime[] x = new DateTime[] { returnDateTime };
            return x.AsQueryable();
        }
        [WebGet]
        public void VoidServiceOperation()
        { }

        [WebInvoke(Method = "POST")]
        public void VoidServiceOperationPost()
        { }


        [WebGet]
        public IQueryable<string> MultipleStrings()
        {
            String[] x = new String[] { "Hello", "foo" };
            return x.AsQueryable();
        }

        [WebInvoke(Method = "POST")]
        public IQueryable<string> PostMultipleStrings()
        {
            String[] x = new String[] { "Hello", "foo" };
            return x.AsQueryable();
        }

        private static IQueryable<D> ConvertObjectArrayToTArray<D>(object[] arr)
        {
            D[] tArray = new D[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                tArray[i] = (D)arr[i];
            }
            return tArray.AsQueryable();
        }

        #region supporting code for getting interesting values for each primitive type

        private static List<D> GetPrimitiveTypeValues<D>()
        {
            // values taken from TypeData.cs and StringData.cs
            object[] values = null;
            Type type = typeof(D);
            if (type == typeof(bool))
                values = new object[] { true, false };

            else if (type == typeof(byte))
                values = new object[] { 0, 1, 255 };

            else if (type == typeof(SByte))
                values = new object[] { 0, 1, -1, sbyte.MaxValue, sbyte.MinValue };

            else if (type == typeof(char))
                values = new object[] { '\x0', '\x1', 'A', 'a', '\xFF', '\x100' };

            else if (type == typeof(decimal))
                values = new object[] { Decimal.MaxValue, Decimal.MinValue, Decimal.One, Decimal.Zero, Decimal.MinValue, Decimal.MaxValue };

            else if (type == typeof(double))
                values = new object[] { 0, 1, -0.1, Double.Epsilon, Double.MaxValue, Double.MinValue, Double.NegativeInfinity, Double.PositiveInfinity, Double.NaN, 7E-06, 9e+09, 9E+16 };

            else if (type == typeof(float))
                values = new object[] { 0, 1, -0.1, Single.Epsilon, Single.MaxValue, Single.MinValue, Single.NegativeInfinity, Single.PositiveInfinity, Single.NaN, 7E-06f, 9E+09f };

            else if (type == typeof(Int16))
                values = new object[] { 0, 1, -1, Int16.MaxValue, Int16.MinValue };

            else if (type == typeof(Int32))
                values = new object[] { 0, 1, -1, Int32.MaxValue, Int32.MinValue };

            else if (type == typeof(Int64))
                values = new object[] { 0, 1, -1, Int64.MaxValue, Int64.MinValue };

            else if (type == typeof(UInt16))
                values = new object[] { 0, 1, UInt16.MaxValue, UInt16.MinValue };

            else if (type == typeof(UInt32))
                values = new object[] { 0, 1, UInt32.MaxValue, UInt32.MinValue };

            else if (type == typeof(UInt64))
                values = new object[] { 0, 1, UInt64.MaxValue, UInt64.MinValue };

            else if (type == typeof(string))
                values = new object[] { null, "", "  \t \r\n", ".,();", "\r\n", "\r\n\r\n\r\n\r\n", "\r", "\n", "\n\r", "a\x0302e\x0327\x0627\x0654\x0655",
                    "a surrogate pair: \xd800\xdc00", "left to right \x05d0\x05d1 \x05ea\x05e9 english", "\x1\x2\x3\x4\x5\x20"};

            else if (type == typeof(byte[]))
                values = new object[] { null, new byte[0], new byte[] { 0 }, new byte[] { 0, 1, byte.MinValue, byte.MaxValue }, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 } };

            else if (type == typeof(sbyte[]))
                values = new object[] { null, new sbyte[0], new sbyte[] { 0 }, new sbyte[] { 0, 1, sbyte.MinValue, sbyte.MaxValue } };

            else if (type == typeof(IntPtr))
                values = new object[] { IntPtr.Zero };

            else if (type == typeof(UIntPtr))
                values = new object[] { UIntPtr.Zero };

            else if (type == typeof(DateTime))
                values = new object[] { DateTime.MaxValue, DateTime.MinValue, DateTime.Now, DateTime.Today, DateTime.UtcNow }; // Kinds?

            else if (type == typeof(DateTimeOffset))
                values = new object[] { DateTimeOffset.MaxValue, DateTimeOffset.MinValue, DateTimeOffset.Now, DateTimeOffset.UtcNow };

            else if (type == typeof(TimeSpan))
                values = new object[] { TimeSpan.MaxValue, TimeSpan.MinValue, TimeSpan.FromDays(1.5) };

            else if (type == typeof(Uri))
                values = new object[] { new Uri("foo", UriKind.Relative), new Uri("foo://foo", UriKind.Absolute), new Uri("foo://foo", UriKind.RelativeOrAbsolute) };

            else if (type == typeof(Guid))
                values = new object[] { Guid.Empty, Guid.NewGuid() };
#if !ClientSKUFramework
            else if (type == typeof(Binary))
                values = new object[] { null, new Binary(new byte[0]), new Binary(new byte[] { 1, 2, byte.MaxValue }) };
#endif
            else if (type == typeof(XElement))
                values = new object[] { null, XElement.Parse("<xelement>content<nested><!--comment--></nested> </xelement>") };

            else return null;

            return values.Cast<D>().ToList();
        }
        #endregion

        private static IQueryable<D> PrimitiveTypeWebGet<D>()
        {
            List<D> o = GetPrimitiveTypeValues<D>();

            D d = o.Last(); //TODO: random?
            return new D[] { d }.AsQueryable();
        }

        #region ServiceOperations for testing return of all primitive types

        [WebGet]
        [SingleResult]
        public IQueryable<byte[]> BinaryServiceOpTest()
        {
            return PrimitiveTypeWebGet<byte[]>();

        }
        [WebGet()]
        [SingleResult]
        public IQueryable<Boolean> BooleanServiceOperation()
        {
            return PrimitiveTypeWebGet<Boolean>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<Byte> ByteServiceOperation()
        {
            return PrimitiveTypeWebGet<Byte>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<DateTime> DateTimeServiceOperation()
        {
            return PrimitiveTypeWebGet<DateTime>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<Decimal> DecimalServiceOperation()
        {
            return PrimitiveTypeWebGet<Decimal>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<Double> DoubleServiceOperation()
        {
            return PrimitiveTypeWebGet<Double>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<Guid> GuidServiceOperation()
        {
            return PrimitiveTypeWebGet<Guid>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<Int16> Int16ServiceOperation()
        {
            return PrimitiveTypeWebGet<Int16>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<Int32> Int32ServiceOperation()
        {
            return PrimitiveTypeWebGet<Int32>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<Int64> Int64ServiceOperation()
        {
            return PrimitiveTypeWebGet<Int64>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<SByte> SByteServiceOperation()
        {
            return PrimitiveTypeWebGet<SByte>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<Single> SingleServiceOperation()
        {
            return PrimitiveTypeWebGet<Single>();
        }
        [WebGet]
        [SingleResult]
        public IQueryable<String> StringServiceOperation()
        {
            return PrimitiveTypeWebGet<String>();
        }
        #endregion

        [WebGet]
        [SingleResult]
        public IQueryable<String> GetResourceString(string identifierString)
        {
            // we assume that this is a resource string for Microsoft.OData.Service.dll
            Type type = typeof(DataServiceHost);
            ResourceManager resourceManager = new ResourceManager(type.Namespace, type.Assembly);

            string result = resourceManager.GetString(identifierString, Thread.CurrentThread.CurrentUICulture);
            if (result == null)
                return null;

            return new string[] { result }.AsQueryable();
        }

        #region Configuration Settings Service Ops


        [WebGet]
        public void AllowCountRequests(bool bValue)
        {
            configurationSettings.PropertySetList.Add("AllowCountRequests");
            configurationSettings.AllowCountRequests = bValue;
            ClearMetadataCache();
        }

        [WebGet]
        public void AllowProjectionRequests(bool bValue)
        {
            configurationSettings.PropertySetList.Add("AllowProjectionRequests");
            configurationSettings.AllowProjectionRequests = bValue;
            ClearMetadataCache();
        }
        [WebGet]
        public void InvokeInterceptorsOnLinkDelete(bool bValue)
        {
            configurationSettings.PropertySetList.Add("InvokeInterceptorsOnLinkDelete");
            configurationSettings.InvokeInterceptorsOnLinkDelete = bValue;
            ClearMetadataCache();
        }

        [WebGet]
        public void MaxProtocolVersion(int? value)
        {
            configurationSettings.PropertySetList.Add("MaxProtocolVersion");
            configurationSettings.MaxProtocolVersion = value;
            ClearMetadataCache();
        }

        [WebGet]
        public void UseVerboseErrors(bool value)
        {
            configurationSettings.PropertySetList.Add("UseVerboseErrors");
            configurationSettings.UseVerboseErrors = value;
            ClearMetadataCache();
        }

        public void MaxBatchCount(int count)
        {
            configurationSettings.PropertySetList.Add("MaxBatchCount");
            configurationSettings.MaxBatchCount = count;
            ClearMetadataCache();
        }

        [WebGet]
        public void MaxChangesetCount(int count)
        {
            configurationSettings.PropertySetList.Add("MaxChangesetCount");
            configurationSettings.MaxChangesetCount = count;
            ClearMetadataCache();
        }

        [WebGet]
        public void MaxExpandCount(int count)
        {
            configurationSettings.PropertySetList.Add("MaxExpandCount");
            configurationSettings.MaxExpandCount = count;
            ClearMetadataCache();
        }

        [WebGet]
        public void MaxExpandDepth(int count)
        {
            configurationSettings.PropertySetList.Add("MaxExpandDepth");
            configurationSettings.MaxExpandDepth = count;
            ClearMetadataCache();
        }

        [WebGet]
        public void MaxObjectCountOnInsert(int count)
        {
            configurationSettings.PropertySetList.Add("MaxObjectCountOnInsert");
            configurationSettings.MaxObjectCountOnInsert = count;
            ClearMetadataCache();
        }

        [WebGet]
        public void MaxResultsPerCollection(int count)
        {
            configurationSettings.PropertySetList.Add("MaxResultsPerCollection");
            configurationSettings.MaxResultsPerCollection = count;
            ClearMetadataCache();
        }

        [WebGet]
        public void SetEntitySetAccessRule(string entityName, int entitySetRights)
        {
            configurationSettings.SetEntitySetAccessRule(entityName, (EntitySetRights)entitySetRights);
            ClearMetadataCache();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<int> GetEntitySetAccessRule(string entityName)
        {
            EntitySetRights entitySetRights = configurationSettings.GetEntitySetAccessRule(entityName);
            return new int[] { (int)entitySetRights }.AsQueryable();
        }

        [WebGet]
        public void SetServiceOperationAccessRule(string serviceOperationName, int serviceOperationSetRights)
        {
            configurationSettings.SetServiceOperationAccessRule(serviceOperationName, (ServiceOperationRights)serviceOperationSetRights);
            ClearMetadataCache();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<int> GetServiceOperationAccessRule(string entityName)
        {
            ServiceOperationRights serviceOperationRights = configurationSettings.GetServiceOperationAccessRule(entityName);
            return new int[] { (int)serviceOperationRights }.AsQueryable();
        }

        [WebGet]
        public void SetEntitySetPageSize(string entityName, int pageSize)
        {
            configurationSettings.SetEntitySetPageSize(entityName, pageSize);
            ClearMetadataCache();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<int> GetEntitySetPageSize(string entityName)
        {
            return new int[] { configurationSettings.GetEntitySetPageSize(entityName) }.AsQueryable();
        }

        [WebGet]
        public void AddRegisterKnownType(string typeName)
        {
            configurationSettings.AddRegisterKnownType(typeName);
            ClearMetadataCache();
        }

        [WebGet]
        public void ClearEntitySetAccessRules()
        {
            configurationSettings.ClearEntitySetAccessRules();
            ClearMetadataCache();
        }

        [WebGet]
        public void ClearServiceOperationAccessRules()
        {
            configurationSettings.ClearServiceOperationAccessRules();
            ClearMetadataCache();
        }

        [WebGet]
        public void ClearEntitySetPageSizes()
        {
            configurationSettings.ClearEntitySetPageSizes();
            ClearMetadataCache();
        }

        [WebGet]
        public void ClearRegisteredKnownTypes()
        {
            configurationSettings.ClearRegisteredKnownTypes();
            ClearMetadataCache();
        }

        [WebGet]
        public void EnableTypeAccess(string typeName)
        {
            configurationSettings.EnableAccessToTypes.Add(typeName);
            ClearMetadataCache();
        }

        [WebGet]
        public void SetEnableTypeConversion(bool typeConversionEnabled)
        {
            configurationSettings.EnableTypeConversion = typeConversionEnabled;
            ClearMetadataCache();
        }
        [WebGet]
        public void ClearAll()
        {
            configurationSettings.ClearAll();
            ClearMetadataCache();
        }

        [WebGet]
        public void ClearEnabledTypes()
        {
            configurationSettings.EnableAccessToTypes.Clear();
            ClearMetadataCache();
        }

        [WebGet]
        public void ClearMetadataCache()
        {
            System.Data.Test.Astoria.FullTrust.TrustedMethods.ClearMetadataCache();
        }

        [WebGet]
        [SingleResult]
        public bool IsDebugProductBuild()
        {
            return typeof(DataService<>).Assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(att => att.IsJITTrackingEnabled);
        }
        
        #endregion
        #endregion

        #region overrides for tracking call order
        protected override T CreateDataSource()
        {
            APICallLog.Current.DataService.CreateDataSource(this);
            try
            {
                return base.CreateDataSource();
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        protected override void HandleException(HandleExceptionArgs args)
        {
            APICallLog.Current.DataService.HandleException(this, args);
            try
            {
                base.HandleException(args);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            APICallLog.Current.DataService.OnStartProcessingRequest(this, args);
            try
            {
                if (args.RequestUri.OriginalString.Contains("ClearAll"))
                {
                    ClearAll();
                }
                base.OnStartProcessingRequest(args);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }
        #endregion
    }

    #region ConfigurationSettings
    public class ConfigurationSettings
    {
        private Dictionary<string, int> entitySetPageSize = new Dictionary<string, int>();
        private Dictionary<string, EntitySetRights> entitySetRights = new Dictionary<string, EntitySetRights>();
        private Dictionary<string, ServiceOperationRights> serviceOperationRights = new Dictionary<string, ServiceOperationRights>();

        private HashSet<string> propertySetList = new HashSet<string>();
        private List<string> knownTypesToRegister = new List<string>();
        private List<string> enableAccessToTypes = new List<string>();

        public ConfigurationSettings()
        {
            entitySetRights.Add("*", EntitySetRights.All);
            serviceOperationRights.Add("*", ServiceOperationRights.All);

            this.UseVerboseErrors = true;
            this.AllowCountRequests = true;
            this.AllowProjectionRequests = true;
            this.InvokeInterceptorsOnLinkDelete = true;
            propertySetList.Add("UseVerboseErrors");
        }

        public void ClearAll()
        {
            propertySetList.Clear();
            enableAccessToTypes.Clear();
            this.ClearEntitySetAccessRules();
            this.ClearEntitySetPageSizes();
            this.ClearServiceOperationAccessRules();
            this.ClearRegisteredKnownTypes();
        }

        public HashSet<string> PropertySetList
        {
            get { return propertySetList; }
        }

        public int MaxBatchCount
        {
            get;
            set;
        }

        public int MaxChangesetCount
        {
            get;
            set;
        }

        public bool AllowCountRequests 
        { 
            get; 
            set; 
        }

        public bool AllowProjectionRequests 
        { 
            get; 
            set; 
        }

        public bool InvokeInterceptorsOnLinkDelete 
        { 
            get;
            set; 
        }

        public int MaxExpandCount
        {
            get;
            set;
        }

        public int MaxExpandDepth
        {
            get;
            set;
        }

        public int MaxObjectCountOnInsert
        {
            get;
            set;
        }

        public int MaxResultsPerCollection
        {
            get;
            set;
        }

        public bool UseVerboseErrors
        {
            get;
            set;
        }

        public bool EnableTypeConversion
        {
            get;
            set;
        }

        public int? MaxProtocolVersion   // this should be DataServiceProtocolVersion, but it's not defined in V1
        {
            get;
            set;
        }

        public List<string> EnableAccessToTypes
        {
            get
            {
                return this.enableAccessToTypes;
            }
        }

        public List<string> KnownTypesToRegister
        {
            get { return knownTypesToRegister; }
        }

        public void AddRegisterKnownType(string typeName)
        {
            knownTypesToRegister.Add(typeName);
        }

        public void ClearRegisteredKnownTypes()
        {
            knownTypesToRegister.Clear();
        }

        public Dictionary<string, int> EntitySetPageSizes
        {
            get { return entitySetPageSize; }
        }

        public void SetEntitySetPageSize(string entityName, int size)
        {
            if (entitySetPageSize.ContainsKey(entityName))
                entitySetPageSize[entityName] = size;
            else
                entitySetPageSize.Add(entityName, size);
        }

        public int GetEntitySetPageSize(string entityName)
        {
            if (entitySetPageSize.ContainsKey(entityName))
                return entitySetPageSize[entityName];

            return int.MaxValue;
        }

        public void ClearEntitySetPageSizes()
        {
            entitySetPageSize.Clear();
        }

        public Dictionary<string, EntitySetRights> EntitySetAccessRules
        {
            get { return entitySetRights; }
        }

        public void SetEntitySetAccessRule(string entityName, EntitySetRights entitySetRight)
        {
            if (entitySetRights.ContainsKey(entityName))
                entitySetRights[entityName] = entitySetRight;
            else
                entitySetRights.Add(entityName, entitySetRight);
        }

        public EntitySetRights GetEntitySetAccessRule(string entityName)
        {
            if (entitySetRights.ContainsKey(entityName))
                return entitySetRights[entityName];

            return EntitySetRights.All;
        }

        public void ClearEntitySetAccessRules()
        {
            entitySetRights.Clear();
            entitySetRights.Add("*", EntitySetRights.All);
        }

        public Dictionary<string, ServiceOperationRights> ServiceOperationAccessRules
        {
            get { return serviceOperationRights; }
        }

        public void SetServiceOperationAccessRule(string entityName, ServiceOperationRights serviceOperationRight)
        {
            if (serviceOperationRights.ContainsKey(entityName))
                serviceOperationRights[entityName] = serviceOperationRight;
            else
                serviceOperationRights.Add(entityName, serviceOperationRight);
        }

        public ServiceOperationRights GetServiceOperationAccessRule(string entityName)
        {
            if (serviceOperationRights.ContainsKey(entityName))
                return serviceOperationRights[entityName];

            return ServiceOperationRights.All;
        }

        public void ClearServiceOperationAccessRules()
        {
            serviceOperationRights.Clear();
            serviceOperationRights.Add("*", ServiceOperationRights.All);
        }

       
    }
    #endregion
}
