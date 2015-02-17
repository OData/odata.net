//---------------------------------------------------------------------
// <copyright file="RemoteClient.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using Microsoft.OData.Client;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using TestSL;
using northwindClient;
using ArubaClient;
using System.Data.Test.Astoria;
using System.Data.Test.Astoria.Tests;
using WrapperTypes;

namespace RemoteClient
{


    public enum TestLanguageEnum
    {
        VB,
        CSHARP
    }


    public class RemoteClient
    {
        public static string ServiceRootURI
        {
            get;
            set;
        }

        public static TestLanguageEnum TestLanguage
        {
            get;
            set;
        }

        public static DataServiceContext GetServiceContext()
        {
            Uri serviceRoot = new Uri(ServiceRootURI, UriKind.RelativeOrAbsolute);
            return new DataServiceContext(serviceRoot);
        }


        public delegate void ExecutionStart(string MethodName);
        public delegate void ExecutionEnd(string MethodName, Message request, Message response);
        public delegate void ExecutionError(string MethodName, Message request, Exception Error);

        public static event ExecutionStart StartExecution;
        public static event ExecutionEnd EndExecution;
        public static event ExecutionError ErrorInExecution;



        public static List<string> pendingMessages = new List<string>();
        public static List<object> Instances = new List<Object>();
        public static int ResourceStarvationLimit = 10;
        public static int ResourceStarvationCounter = 0;

        public static Uri MessagesUri = new Uri("Messages?$filter=WhoSentMe eq 'Test'", UriKind.Relative);


        public static ExecutionModes RemoteClientExecutionMode
        {
            get;
            set;
        }




        #region Message Executor


        #region Common Execution Methods

#if !SILVERLIGHT
        static string DataSent = "";
        static void RemoteClient_SendingRequest(object sender, SendingRequestEventArgs e)
        {
            DataSent += "\r\n" + e.Request.RequestUri.AbsoluteUri;
        }

        public static void callSaveChanges(SaveChangesOptions scOptions, DataServiceContext instance)
        {
            DataServiceResponse dsResponse = ((DataServiceContext)instance).SaveChanges(scOptions);
        }

#endif
        #endregion
        public static bool EventHandlersInitialized = false;
        public static void InitializeEventHandlers()
        {
            EventHandlersInitialized = true;

            switch (RemoteClient.TestLanguage)
            {
                case TestLanguageEnum.CSHARP:
                    CSDispatch.Dispatcher.NormalizeEntities += new Delegates.NormalizeEntitiesDelegate(GetNormalizedEntities);
                    CSDispatch.Dispatcher.NormalizeEntity += new Delegates.NormalizeEntityDelegate(GetNormalizedEntity);
                    CSDispatch.Dispatcher.ResolveSameEntity += new Delegates.ResolveEntityDelegate(GetSameObject);
                    CSDispatch.Dispatcher.VerifyLinqTest += new Delegates.VerifyLinqTestDelegate(RunTest);
                    CSDispatch.Dispatcher.ErrorInExecution += new Delegates.ExecutionError(Dispatcher_ErrorInExecution);
                    CSDispatch.Dispatcher.EndExecution += new Delegates.ExecutionEnd(Dispatcher_EndExecution);
                    CSDispatch.Dispatcher.ResolveSameEntityNoKey += new Delegates.ResolveEntityNoKeyDelegate(Dispatcher_ResolveSameEntityNoKey);

                    break;
                case TestLanguageEnum.VB:
                    VBDispatch.Dispatcher.ErrorInExecution += new VBDispatch.Dispatcher.ErrorInExecutionEventHandler(Dispatcher_ErrorInExecution);
                    VBDispatch.Dispatcher.NormalizeEntities += new VBDispatch.Dispatcher.NormalizeEntitiesEventHandler(GetNormalizedEntities);
                    VBDispatch.Dispatcher.NormalizeEntity += new VBDispatch.Dispatcher.NormalizeEntityEventHandler(GetNormalizedEntity);
                    VBDispatch.Dispatcher.ResolveSameEntity += new VBDispatch.Dispatcher.ResolveSameEntityEventHandler(GetSameObjectVB);
                    VBDispatch.Dispatcher.VerifyLinqTest += new VBDispatch.Dispatcher.VerifyLinqTestEventHandler(RunTestVB);
                    VBDispatch.Dispatcher.ErrorInExecution += new VBDispatch.Dispatcher.ErrorInExecutionEventHandler(Dispatcher_ErrorInExecution);
                    VBDispatch.Dispatcher.EndExecution += new VBDispatch.Dispatcher.EndExecutionEventHandler(Dispatcher_EndExecution);
                    break;

            }

        }

        static object Dispatcher_ResolveSameEntityNoKey(object objToMatch, DataServiceContext ctx)
        {
            return GetSameObjectMatchOnlyProperties(objToMatch, ctx);
        }

        static void Dispatcher_EndExecution(string MethodName, Message request, Message response)
        {
            EndExecution(MethodName, request, response);
        }

        public static void GetSameObjectVB(object objToMatch, DataServiceContext ctx, ref object entity)
        {
            entity = GetSameObject(objToMatch, ctx);
        }
        public static void RunTestVB(IEnumerator baseline, IEnumerator linqQuery, ref bool result)
        {
            result = RunTest(baseline, linqQuery);
        }
        #region Synchronous Execution Methods

        static void Dispatcher_ErrorInExecution(string MethodName, Message request, Exception Error)
        {
            ErrorInExecution(MethodName, request, Error);
        }


        #endregion Synchronous Execution Methods

        /// <summary>
        /// Executes a method in a remote context
        /// </summary>
        /// <param name="message">The Message containing the request params</param>
        /// <param name="response">The Initialized Message containing the response params</param>
        /// <returns></returns>
        public static void Dispatch(Message message, Message response)
        {

            if (pendingMessages.Contains(message.MessageID.ToString()))
                return;

            pendingMessages.Add(message.MessageID.ToString());

            if (StartExecution != null)
                StartExecution(message.Method);

            if (!EventHandlersInitialized)
            {
                InitializeEventHandlers();
            }

            bool requiresCallback = false;




            switch (TestLanguage)
            {
                #region if C# Test Code
                case TestLanguageEnum.CSHARP:

                    if (CSDispatch.Dispatcher.Instances == null)
                        CSDispatch.Dispatcher.Instances = RemoteClient.Instances;
                    CSDispatch.Dispatcher.ExecutionMode = RemoteClientExecutionMode;
                    requiresCallback = CSDispatch.Dispatcher.Dispatch(message, ref response);
                    break;
                #endregion
                #region if VB Test Code
                case TestLanguageEnum.VB:
                    if (VBDispatch.Dispatcher.Instances == null)
                    {
                        VBDispatch.Dispatcher.Instances = RemoteClient.Instances;
                    }
                    //VBDispatch.Dispatcher.Instances;
                    VBDispatch.Dispatcher.ExecutionMode = RemoteClientExecutionMode;
                    requiresCallback = VBDispatch.Dispatcher.Dispatch(message, ref response);
                    break;
                #endregion
            }

            if (!requiresCallback)
            {
                if (EndExecution != null)
                {
                    EndExecution(message.Method, message, response);
                }
            }
        }

        #endregion

        public static void ReleaseResources(Message message)
        {

        }



        public static bool RunTest(IEnumerator baseline, IEnumerator linqQuery)
        {
            bool objectsEqual = true;
            IEnumerator l = null;
            IEnumerator r = null;
            try
            {
                l = baseline;
                r = linqQuery;
                if (l != null && r != null)
                    objectsEqual = VerifyResults(l, r);
            }
            finally
            {
                if (l is IDisposable) ((IDisposable)l).Dispose();
                if (r is IDisposable) ((IDisposable)r).Dispose();
            }
            return objectsEqual;
        }

        public static void MyCallback(IAsyncResult asyncResult)
        {

            PropertyInfo[] props = asyncResult.AsyncState.GetType().GetProperties();

            object instance = props[0].GetValue(asyncResult.AsyncState, null);



            Message response = (Message)props[1].GetValue(asyncResult.AsyncState, null);
            Type entityType = (Type)props[2].GetValue(asyncResult.AsyncState, null);
            Message message = (Message)props[3].GetValue(asyncResult.AsyncState, null);

            string instanceTypeName = instance.GetType().FullName;
            object results = null;
            if (instanceTypeName.Contains("DataServiceQuery"))
            {
                if (instanceTypeName.Contains("Orders"))
                {

                    instance = instance as DataServiceQuery<northwindClient.Orders>;
                    try
                    {
                        results = ((DataServiceQuery<northwindClient.Orders>)instance).EndExecute(asyncResult);
                    }
                    catch (Exception except)
                    {
                        if (ErrorInExecution != null)
                            ErrorInExecution(message.Method, message, except);
                    }
                }
                else if (instanceTypeName.Contains("Customers"))
                {
                    instance = instance as DataServiceQuery<northwindClient.Customers>;
                }
                else
                {
                    instance = instance as DataServiceQuery;
                }
            }
            else if (instanceTypeName.Contains("DataServiceContext"))
            {
                instance = instance as DataServiceContext;
            }

            MethodInfo endExecute = instance.GetType().GetMethod("EndExecute");

            bool exceptionOccured = false;
            string exceptionMessage = String.Empty;
            try
            {
                if (results == null)
                {
                    if (instance is DataServiceQuery)
                    {
                        results = endExecute.Invoke(instance, new object[] { asyncResult });

                    }
                    else
                    {
                        MethodInfo endExecuteGeneric = endExecute.MakeGenericMethod(entityType);
                        results = endExecuteGeneric.Invoke(instance, new object[] { asyncResult });
                    }
                }
            }
            catch (Exception exception)
            {
                if (ErrorInExecution != null)
                    ErrorInExecution(message.Method, message, exception);
            }
            finally
            {
                if (!exceptionOccured)
                {
                    IEnumerator r = ((IEnumerable)results).GetEnumerator();
                    Instances.Add(r);
                    response.InstanceID = message.InstanceID;
                    response.ReturnValue = (Instances.Count - 1).ToString();
                }
                else
                {
                    response.Exception = exceptionMessage;
                    response.ReturnValue = "-1";
                }
                if (EndExecution != null)
                {
                    EndExecution(message.Method, message, response);
                }

            }
        }



        public class LinqTestExecutor
        {
            public string TestName { get; set; }
            public IEnumerator testResults { get; set; }
            public IEnumerator baseLine { get; set; }
        }
        static List<LinqTestExecutor> runningTests = new List<LinqTestExecutor>();
        public static void LinqTestCallback(IAsyncResult asyncResult)
        {
            PropertyInfo[] props = asyncResult.AsyncState.GetType().GetProperties();
            object instance = props[0].GetValue(asyncResult.AsyncState, null);
            Message response = (Message)props[1].GetValue(asyncResult.AsyncState, null);
            Type entityType = (Type)props[2].GetValue(asyncResult.AsyncState, null);
            Message message = (Message)props[3].GetValue(asyncResult.AsyncState, null);
            string TestName = props[4].GetValue(asyncResult.AsyncState, null).ToString();

            string instanceTypeName = instance.GetType().FullName;
            object results = null;
            bool exceptionOccured = false;
            string exceptionMessage = String.Empty;

            LinqTestExecutor currentTest = runningTests.Where(t => t.TestName == TestName).FirstOrDefault<LinqTestExecutor>();
            //If Product Linq Query 
            if (instanceTypeName.Contains("DataServiceQuery"))
            {
                if (entityType.Name.Contains("Orders"))
                {
                    instance = instance as DataServiceQuery<northwindClient.Orders>;
                    try
                    {
                        results = ((DataServiceQuery<northwindClient.Orders>)instance).EndExecute(asyncResult);
                        currentTest.testResults = ((IEnumerable)results).GetEnumerator();
                    }
                    catch (Exception except)
                    {
                        if (ErrorInExecution != null)
                            ErrorInExecution(message.Method, message, except);
                    }
                }
                else if (entityType.Name.Contains("Customers"))
                {
                    instance = instance as DataServiceQuery<northwindClient.Customers>;
                    try
                    {
                        results = ((DataServiceQuery<northwindClient.Customers>)instance).EndExecute(asyncResult);
                        currentTest.testResults = ((IEnumerable)results).GetEnumerator();
                    }
                    catch (Exception except)
                    {
                        if (ErrorInExecution != null)
                            ErrorInExecution(message.Method, message, except);
                    }
                }
                else if (entityType.Name.Contains("Order_Details"))
                {
                    instance = instance as DataServiceQuery<northwindClient.Order_Details>;
                    try
                    {
                        results = ((DataServiceQuery<northwindClient.Order_Details>)instance).EndExecute(asyncResult);
                        currentTest.testResults = ((IEnumerable)results).GetEnumerator();
                    }
                    catch (Exception except)
                    {
                        if (ErrorInExecution != null)
                            ErrorInExecution(message.Method, message, except);

                    }
                }
                else if (entityType.Name.Contains("Int32"))
                {
                    instance = instance as DataServiceQuery<Int32>;
                    try
                    {
                        results = ((DataServiceQuery<Int32>)instance).EndExecute(asyncResult);
                        currentTest.testResults = ((IEnumerable)results).GetEnumerator();
                    }
                    catch (Exception except)
                    {
                        if (ErrorInExecution != null)
                            ErrorInExecution(message.Method, message, except);
                    }
                }
            }
            else //If Baseline Execute call
            {
                instance = instance as DataServiceContext;
                if (entityType.Name.Contains("Orders"))
                {
                    try
                    {
                        results = ((DataServiceContext)instance).EndExecute<northwindClient.Orders>(asyncResult);
                        currentTest.baseLine = ((IEnumerable)results).GetEnumerator();
                    }
                    catch (Exception except)
                    {
                        if (ErrorInExecution != null)
                            ErrorInExecution(message.Method, message, except);
                    }
                }
                else if (entityType.Name.Contains("Order_Details"))
                {
                    try
                    {
                        results = ((DataServiceContext)instance).EndExecute<northwindClient.Order_Details>(asyncResult);
                        currentTest.baseLine = ((IEnumerable)results).GetEnumerator();
                    }
                    catch (Exception except)
                    {
                        if (ErrorInExecution != null)
                            ErrorInExecution(message.Method, message, except);
                    }
                }
                else
                {
                    try
                    {
                        results = ((DataServiceContext)instance).EndExecute<northwindClient.Customers>(asyncResult);
                        currentTest.baseLine = ((IEnumerable)results).GetEnumerator();
                    }
                    catch (Exception except)
                    {
                        if (ErrorInExecution != null)
                            ErrorInExecution(message.Method, message, except);
                    }
                }
            }

            if (!exceptionOccured)
            {
                response.Exception = exceptionMessage;
                response.ReturnValue = "-1";
            }
            if (currentTest.testResults != null && currentTest.baseLine != null)
            {
                if (EndExecution != null)
                {
                    EndExecution(message.Method, message, response);
                }
            }

        }

        public static bool RunTest(LinqTestExecutor linqTestExecutor)
        {
            return RunTest(linqTestExecutor.baseLine, linqTestExecutor.testResults);
        }

        public static bool VerifyResults(IEnumerator l, IEnumerator r)
        {
            bool objectsEqual = false;
            object left = null;
            object right = null;

            while (true)
            {
                bool lMoved = l.MoveNext();
                bool rMoved = r.MoveNext();

                if (!lMoved && !rMoved)
                {
                    objectsEqual = true;
                    break;
                }

                objectsEqual = lMoved == rMoved;

                left = l.Current;
                right = r.Current;

                if (left == null && right == null)
                {
                    continue;
                }

                if (left is IEnumerable && right is IEnumerable)
                {
                    VerifyResults(((IEnumerable)left).GetEnumerator(), ((IEnumerable)right).GetEnumerator());
                    continue;
                }

                objectsEqual = left.GetType().Equals(right.GetType());
                objectsEqual = left.Equals(right);
            }

            if (l.MoveNext() || r.MoveNext())
            {
                objectsEqual = false;
            }

            return objectsEqual;
        }

        public static string[] ArithMaticOperations = new string[]{
            "eq","ne","and","or"
        };

        public static string[] OrderByArithMaticOperations = new string[]{
            "add",
            "sub",
            "div",
            "mul",
            "mod",
            "addchecked",
            "subchecked",
            "mulchecked" 
        };
        public static void GetNormalizedEntity(Message message, object instance, ref object entity)
        {
            GetNormalizedEntity(message, instance, ref entity, true);
        }
        public static void GetNormalizedEntity(Message message, object instance, ref object entity, bool MatchKeyOnly)
        {
            if (message.Parameter2 == "TryNull")
            {
                entity = null;
            }
            else
            {

                entity = Serializer.ReadObject(message.Parameter3, message.Parameter2);
            }
            object cached = entity;
            if (entity != null)
                entity = GetSameObject(entity, ((DataServiceContext)instance), MatchKeyOnly);
            if (entity == null)
            {
                entity = cached;
            }
        }

        public static void GetNormalizedEntities(Message message, object instance, ref object parent, ref object child)
        {
            if (message.Parameter2 == "TryNull")
            {
                parent = null;
            }
            else
            {
                parent = Serializer.ReadObject(message.Parameter3, message.Parameter2);
            }
            if (message.Parameter4 == "TryNull")
            {
                child = null;
            }
            else
            {
                child = Serializer.ReadObject(message.Parameter5, message.Parameter4);
            }

            DataServiceContext AdsInstance = ((DataServiceContext)instance);

            object cached = parent;
            if (parent != null)
                parent = GetSameObject(parent, AdsInstance);
            if (parent == null)
                parent = cached;

            cached = child;
            if (child != null)
                child = GetSameObject(child, AdsInstance);
            if (child == null)
                child = cached;
        }

        public static void UpdateEntityProperties(object source, ref object destination)
        {
            PropertyInfo[] properties = source.GetType().GetProperties();
            #region Get Key Property
            PropertyInfo keyProperty = null;


            object[] attributes = source.GetType().GetCustomAttributes(true);
            string keyPropertyName = "";
            foreach (object attribute in attributes)
            {
                if (attribute.GetType().FullName.Contains("Key"))
                {
                    Microsoft.OData.Client.KeyAttribute key = (Microsoft.OData.Client.KeyAttribute)attribute;
                    keyPropertyName = key.KeyNames.Last();
                    break;
                }
            }
            keyProperty = source.GetType().GetProperty(keyPropertyName);
            #endregion Get Key Property

            var filteredProperties = from p in properties
                                     where !p.PropertyType.FullName.Contains("Collection")
                                     && !p.PropertyType.FullName.Contains("Byte")
                                     && !p.PropertyType.Namespace.Contains("northwindClient")
                                     && !p.PropertyType.Namespace.Contains("ArubaClient")
                                     && !p.Equals(keyProperty)
                                     select p;

            foreach (PropertyInfo prop in filteredProperties)
            {
                prop.SetValue(destination, prop.GetValue(source, null), null);
            }

        }
        public static object GetSameEntity(object objToMatch, DataServiceContext ctx)
        {
            object matchedObject = null;
            PropertyInfo[] properties = objToMatch.GetType().GetProperties();
            bool Ismatch = false;
            var sameTypeObjects = ctx.Entities.Where(t => t.Entity.GetType().FullName == objToMatch.GetType().FullName);

            #region Get Key Property
            PropertyInfo keyProperty = null;


            object[] attributes = objToMatch.GetType().GetCustomAttributes(true);
            string keyPropertyName = "";
            foreach (object attribute in attributes)
            {
                if (attribute.GetType().FullName.Contains("Key"))
                {
                    Microsoft.OData.Client.KeyAttribute key = (Microsoft.OData.Client.KeyAttribute)attribute;
                    keyPropertyName = key.KeyNames.Last();
                    break;
                }
            }
            keyProperty = objToMatch.GetType().GetProperty(keyPropertyName);
            #endregion Get Key Property

            var filteredProperties = from p in properties
                                     where !p.PropertyType.FullName.Contains("Collection")
                                     && !p.PropertyType.FullName.Contains("Byte")
                                     && !p.PropertyType.Namespace.Contains("northwindClient")
                                     && !p.PropertyType.Namespace.Contains("ArubaClient")
                                     && !p.Equals(keyProperty)
                                     select p;
            string left, right;
            foreach (EntityDescriptor entityDescriptor in sameTypeObjects)
            {
                if (Ismatch)
                    break;
                foreach (PropertyInfo prop in filteredProperties)
                {
                    left = prop.GetValue(entityDescriptor.Entity, null) == null ? "" : prop.GetValue(entityDescriptor.Entity, null).ToString();
                    right = prop.GetValue(objToMatch, null) == null ? "$" : prop.GetValue(objToMatch, null).ToString();

                    Ismatch = left == right;

                }
                if (Ismatch)
                {
                    matchedObject = entityDescriptor.Entity;
                }
            }
            return matchedObject;

        }

        public static object GetSameObject(object objToMatch, DataServiceContext ctx)
        {
            return GetSameObject(objToMatch, ctx, true);
        }

        public static object GetSameObjectMatchOnlyProperties(object objToMatch, DataServiceContext ctx)
        {
            object matchedObject = null;
            bool skip = false;
            PropertyInfo[] properties = objToMatch.GetType().GetProperties();
            object[] dskAttributes = objToMatch.GetType().GetCustomAttributes(typeof(Microsoft.OData.Client.KeyAttribute), true);
            PropertyInfo keyProperty = null;
            string keyPropertyName = "";

            foreach (object attr in dskAttributes)
            {
                keyPropertyName = ((Microsoft.OData.Client.KeyAttribute)attr).KeyNames.Last();
            }

            keyProperty = objToMatch.GetType().GetProperty(keyPropertyName);
            var sameTypeObjects = ctx.Entities.Where(t => t.Entity.GetType().FullName == objToMatch.GetType().FullName);
            foreach (EntityDescriptor entityDescriptor in sameTypeObjects)
            {

                if (skip) break;

                var filteredProperties = from p in properties
                                         where !p.PropertyType.FullName.Contains("Collection")
                                         && !p.PropertyType.FullName.Contains("Byte")
                                         && !p.PropertyType.Namespace.Contains("northwindClient")
                                         && !p.PropertyType.Namespace.Contains("ArubaClient")
                                         && !p.Equals(keyProperty)
                                         select p;

                object left = matchedObject;
                object right = objToMatch;
                int mathchingPropCount = 0;
                foreach (PropertyInfo prop in filteredProperties)
                {
                    left = prop.GetValue(entityDescriptor.Entity, null) == null ? "" : prop.GetValue(entityDescriptor.Entity, null).ToString();
                    right = prop.GetValue(objToMatch, null) == null ? "" : prop.GetValue(objToMatch, null).ToString();

                    if (left.ToString() == right.ToString())
                    {
                        mathchingPropCount++;
                        if (mathchingPropCount == filteredProperties.Count())
                        {
                            matchedObject = entityDescriptor.Entity;
                            skip = true;
                            break;
                        }
                    }


                }
            }
            return matchedObject;

        }

        public static object GetSameObject(object objToMatch, DataServiceContext ctx, bool onlyKeyMatch)
        {
            object matchedObject = null;
            //Find Key property
            PropertyInfo[] properties = objToMatch.GetType().GetProperties();
            string keyPropertyName = "";
            PropertyInfo keyProperty = null;

            object[] attributes = objToMatch.GetType().GetCustomAttributes(true);

            foreach (object attribute in attributes)
            {
                if (attribute.GetType().FullName.Contains("Key"))
                {
                    Microsoft.OData.Client.KeyAttribute key = (Microsoft.OData.Client.KeyAttribute)attribute;
                    keyPropertyName = key.KeyNames.Last();
                    break;
                }
            }

            //foreach (PropertyInfo property in properties)
            //{
            //    if (property.Name.ToLower().Equals("id"))
            //    {

            //    }

            //}
            //string keyPropertyValue = keyProperty.GetValue(objToMatch, null).ToString();
            keyProperty = objToMatch.GetType().GetProperty(keyPropertyName);
            bool skip = false;
            if (keyProperty != null)
            {
                if (keyProperty.GetValue(objToMatch, null) == null)
                {
                    return objToMatch;
                }
                string keyPropertyValue = keyProperty.GetValue(objToMatch, null).ToString();

                var sameTypeObjects = ctx.Entities.Where(t => t.Entity.GetType().FullName == objToMatch.GetType().FullName);
                foreach (EntityDescriptor entityDescriptor in sameTypeObjects)
                {

                    if (skip) break;

                    if (keyProperty.GetValue(entityDescriptor.Entity, null).ToString() == keyPropertyValue)
                    {
                        if (onlyKeyMatch)
                        {
                            matchedObject = entityDescriptor.Entity;
                            foreach (PropertyInfo nonKeyProperty in entityDescriptor.Entity.GetType().GetProperties().
                                Where(prop => prop != keyProperty))
                            {
                                nonKeyProperty.SetValue(matchedObject, nonKeyProperty.GetValue(objToMatch, null), null);
                            }
                            skip = true;
                            break;
                        }
                        else
                        {
                            var filteredProperties = from p in properties
                                                     where !p.PropertyType.FullName.Contains("Collection")
                                                     && !p.PropertyType.FullName.Contains("Byte")
                                                     && !p.PropertyType.Namespace.Contains("northwindClient")
                                                     && !p.PropertyType.Namespace.Contains("ArubaClient")
                                                     && !p.Equals(keyProperty)
                                                     select p;

                            object left = matchedObject;
                            object right = objToMatch;

                            foreach (PropertyInfo prop in filteredProperties)
                            {
                                left = prop.GetValue(entityDescriptor.Entity, null) == null ? "" : prop.GetValue(entityDescriptor.Entity, null).ToString();
                                right = prop.GetValue(objToMatch, null) == null ? "$" : prop.GetValue(objToMatch, null).ToString();

                                if (left == right)
                                {
                                    matchedObject = entityDescriptor.Entity;
                                    skip = true;
                                    break;
                                }

                            }

                        }
                    }

                }
            }
            return matchedObject;

        }



        //callback for LoadProperty
        public static void LoadPropCallback(IAsyncResult asyncResult)
        {
            PropertyInfo[] props = asyncResult.AsyncState.GetType().GetProperties();

            DataServiceContext instance = (DataServiceContext)props[0].GetValue(asyncResult.AsyncState, null);
            Message response = (Message)props[1].GetValue(asyncResult.AsyncState, null);
            Message message = (Message)props[2].GetValue(asyncResult.AsyncState, null);

            instance.EndLoadProperty(asyncResult);
            response.InstanceID = message.InstanceID;


        }

        //deserialize the object from a string




    }



}
