//---------------------------------------------------------------------
// <copyright file="QueriesModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.KoKoMo;
using Microsoft.Test.ModuleCore;
using Microsoft.OData.Client;
using System.Net;
using System.Reflection;
using System.Data.Test.Astoria.FullTrust;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    //  Queries Model
    //
    ////////////////////////////////////////////////////////   

    public class QueriesModel : Model
    {
        //Data
        protected Workspace _workspace = null;
        protected SerializationFormatKind _kind;
        protected ExpNode _query = null;
        protected ResourceContainer _container;
        protected Type _pType = null;
        protected string _queryType = "";

        //Constructor
        public QueriesModel(Workspace workspace, SerializationFormatKind kind, string queryType)
            : base()
        {
            _workspace = workspace;
            _kind = kind;
            _queryType = queryType;
        }

        public ExpNode Result
        {
            get
            {
                return _query;
            }
        }

        public ResourceContainer Container
        {
            get
            {
                return _container;
            }
        }

        public Type ResultType
        {
            get { return _pType; }
        }


        //Actions
        [ModelAction]
        public virtual void RunModel()
        {
            QueryModel model = new QueryModel(_workspace, _kind, null);
            ModelEngine engine = new ModelEngine(model);
            engine.Options.WeightScheme = WeightScheme.Custom;
            engine.Options.Timeout = 100;

            engine.RunUntil(delegate()
            {
                return model.Actions.Accessed > 5;
            });


            _query = model.QueryResult;
            _container = model.ResContainer;
            _pType = model.ResultType;

            if (_query != null || _pType != null)
            {
                switch (_queryType)
                {
                    case "server":
                        VerifyServer(_query);
                        break;
                    case "client":
                        VerifyClient(model);
                        break;
                    case "linq":
                        VerifyClientLinq(model);
                        break;
                }
            }
        }

        [ModelAction]
        public virtual void DirectedTests()
        {
            QueryModel model = new QueryModel(_workspace, SerializationFormatKind.Atom, null);
            ModelEngine engine = new ModelEngine(model);

            engine.Options.WeightScheme = WeightScheme.Custom;
            engine.Options.Timeout = 100;

            engine.RunUntil(delegate()
            {
                return engine.Models.Actions.AllCovered;
            });

            //engine.RunScenario(model.Actions.Find("From", "Where", " Navigation","Select","Expand"));
            //engine.RunUntil(delegate()
            //{
            //    return model.Actions.Accessed > 4;
            //});

             _query = model.QueryResult;
            _container = model.ResContainer;
            _pType = model.ResultType;

            if (_query != null || _pType != null)
            {
                VerifyClient(model);
            }
        }



        public void VerifyServer(ExpNode q)
        {
            AstoriaTestLog.WriteLineIgnore("Verify server query");
            try
            {

                UriQueryBuilder ub = new UriQueryBuilder(_workspace, "");
                string ruri = ub.Build(q);
                AstoriaTestLog.WriteLineIgnore(ruri);


                AstoriaRequest request = _workspace.CreateRequest(q);
                request.Format = _kind;

                if (request.URI.Length > 260) { return; }

                AstoriaResponse response = request.GetResponse();
                response.Verify();
            }
            catch (Exception e)
            {
                AstoriaTestLog.WriteLineIgnore(e.ToString());
            }

        }

        public void VerifyClient(QueryModel qm)
        {
            WebDataCtxWrapper ctx = new WebDataCtxWrapper(new Uri(qm.Workspace.ServiceUri));
            ctx.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                UriQueryBuilder ub = new UriQueryBuilder(qm.Workspace, "");
                string ruri = ub.Build(qm.QueryResult);

                string uriRel = ruri.Substring(ruri.LastIndexOf(".svc/") + 5);

                if ((ctx.BaseUri + "/" + uriRel).Length > 260) { return; }
                AstoriaTestLog.WriteLineIgnore(ctx.BaseUri + "/" + uriRel);

                ResolveClientType(qm.Workspace, ctx, qm.ResultType);

                object enumerable = ExecuteQuery(ctx, qm.ResultType, uriRel, "sync");
                Verify(qm.Workspace, qm.QueryResult, (IEnumerable)enumerable, qm.ResType, null, qm.IsCount);
            }
            catch (Exception e)
            {
                AstoriaTestLog.FailAndContinue(e);
            }

        }

        public static IEnumerable ExecuteQuery(WebDataCtxWrapper ctx, Type pType, string sUri, string execType)
        {
            MethodInfo mi = null;
            MethodInfo miConstructed = null;
            object enumerable = null;
            object[] args = { new Uri(sUri, UriKind.Relative) };

            Type cType = typeof(WebDataCtxWrapper);

            switch (execType)
            {
                case "sync":
                    mi = cType.GetMethod("Execute", new Type[] { typeof(System.Uri) });
                    miConstructed = mi.MakeGenericMethod(pType);
                    enumerable = miConstructed.Invoke(ctx, args);
                    break;
                case "async":
                    mi = cType.GetMethod("BeginExecute", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(System.Uri), typeof(System.AsyncCallback), typeof(object) }, null);
                    miConstructed = mi.MakeGenericMethod(pType);

                    object asyncResult = miConstructed.Invoke(ctx, new object[] { new Uri(sUri, UriKind.Relative), null, null });

                    MethodInfo endexecute = cType.GetMethod("EndExecute", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IAsyncResult) }, null);
                    MethodInfo miConstructed2 = endexecute.MakeGenericMethod(pType);
                    enumerable = miConstructed2.Invoke(ctx, new object[] { asyncResult });
                    break;
            }
            return (IEnumerable)enumerable;
        }

        public static void Verify(Workspace w, ExpNode q, IEnumerable results, ResourceType resType)
        {
            Verify(w, q, results, resType, null, false);
        }

        public static void Verify(Workspace w, ExpNode q, IEnumerable results, ResourceType resType, ComplexType cType, bool bCount)
        {
            long count = 0;
            if (bCount)
            {

                object[] args = new object[] { results };
                Type qorType = typeof(QueryOperationResponseWrapper<>).MakeGenericType(resType.ClientClrType);
                object qor = Activator.CreateInstance(qorType, args);

                PropertyInfo pi = qor.GetType().GetProperty("GetTotalCount");
                object i = pi.GetValue(qor, new object[] { });

                count = (long)i;

                LinqQueryBuilder lb = new LinqQueryBuilder(w);
                lb.CountingMode = true;
                lb.Build(q);

                object baselineElementsCount = CommonPayload.CreateList(lb.QueryResult).Count;
                AstoriaTestLog.IsTrue(count == Convert.ToInt64(baselineElementsCount), "Count is different.Count is " + count.ToString() + ". Baseline count is " + baselineElementsCount.ToString());
            }

            LinqQueryBuilder linqBuilder = new LinqQueryBuilder(w);
            linqBuilder.Build(q);

            IQueryable baselines = linqBuilder.QueryResult;

            IEnumerator b = null;

            IEnumerator r = null;

            try
            {
                b = TrustedMethods.IQueryableGetEnumerator(baselines);
                r = results.GetEnumerator();
            }
            catch (InvalidOperationException invalidOperation)
            {
                if (!AstoriaTestProperties.IsRemoteClient)
                    throw invalidOperation;
            }

            PropertyInfo propertyInfo = null;
            Object expectedResult1 = null;
            Object expectedResult2 = null;

            try
            {
                while (b.MoveNext() && r.MoveNext())
                {
                    if (b.Current == null)
                    {
                        return;
                    }

                    if (r.Current == null)
                    {
                        throw new TestFailedException("Less results than expected");

                    }
                    //skip verification for Binary data type for Linq to Sql
                    if (AstoriaTestProperties.DataLayerProviderKinds[0] == DataLayerProviderKind.LinqToSql && b.Current is System.Byte[])
                        return;

                    if (b.Current is System.Byte[])
                    {
                        byte[] newBase = (byte[])b.Current;
                        byte[] newAct = (byte[])r.Current;

                        if (newBase.Length != newAct.Length)
                            throw new TestFailedException("Failed to compare the results!");
                    }
#if !ClientSKUFramework

                    else if (b.Current is System.Data.Linq.Binary)
                    {
                        System.Data.Linq.Binary newBase = (System.Data.Linq.Binary)b.Current;
                        System.Data.Linq.Binary newAct = new System.Data.Linq.Binary((byte[])r.Current);

                        if (newBase.Length != newAct.Length)
                            throw new TestFailedException("Failed to compare the results!");

                    }
#endif

                    else if (b.Current is System.Xml.Linq.XElement)
                    {
                        if (b.Current.ToString() != r.Current.ToString())
                            throw new TestFailedException("Failed to compare the results!");
                    }
                    else
                    {
                        if (!b.Current.Equals(r.Current))
                        {
                            if (cType != null)
                            {
                                foreach (ResourceProperty property in cType.Properties)
                                {
                                    if (!property.IsNavigation && !property.IsComplexType && !(property.Type is CollectionType))
                                    {
                                        propertyInfo = b.Current.GetType().GetProperty(property.Name);
                                        expectedResult1 = propertyInfo.GetValue(b.Current, null);

                                        PropertyInfo propertyInfo2 = r.Current.GetType().GetProperty(property.Name);
                                        expectedResult2 = propertyInfo2.GetValue(r.Current, null);

                                        if (expectedResult1 != expectedResult2)
                                        {
                                            if (expectedResult1 is System.Xml.Linq.XElement)
                                            {
                                                expectedResult1 = ((System.Xml.Linq.XElement)expectedResult1).ToString(System.Xml.Linq.SaveOptions.None);
                                            }
                                            if (expectedResult2 is System.Xml.Linq.XElement)
                                            {
                                                expectedResult2 = ((System.Xml.Linq.XElement)expectedResult2).ToString(System.Xml.Linq.SaveOptions.None);
                                            }
					    #if !ClientSKUFramework

                                            if (expectedResult1 is byte[])
                                            {
                                                expectedResult1 = new System.Data.Linq.Binary((byte[])expectedResult1);
                                            }

                                            if (expectedResult2 is byte[])
                                            {
                                                expectedResult2 = new System.Data.Linq.Binary((byte[])expectedResult2);
                                            }
					    #endif


                                            AstoriaTestLog.AreEqual(expectedResult1, expectedResult2, String.Format("Resource value for {0} does not match", property.Name), false);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (ResourceProperty property in resType.Properties)
                                {
                                    if (!property.IsNavigation && !property.IsComplexType && !(property.Type is CollectionType))
                                    {
                                        //skip verification for Binary data type for Linq to Sql
                                        if (AstoriaTestProperties.DataLayerProviderKinds[0] == DataLayerProviderKind.LinqToSql && property.Type.Name == "LinqToSqlBinary")
                                            return;

                                        propertyInfo = b.Current.GetType().GetProperty(property.Name);
                                        expectedResult1 = propertyInfo.GetValue(b.Current, null);

                                        PropertyInfo propertyinfo2 = r.Current.GetType().GetProperty(property.Name);
                                        expectedResult2 = propertyinfo2.GetValue(r.Current, null);

                                        if (expectedResult1 != expectedResult2)
                                        {
                                            if (expectedResult1 is System.Xml.Linq.XElement)
                                            {
                                                expectedResult1 = ((System.Xml.Linq.XElement)expectedResult1).ToString(System.Xml.Linq.SaveOptions.None);
                                            }

                                            if (expectedResult2 is System.Xml.Linq.XElement)
                                            {
                                                expectedResult2 = ((System.Xml.Linq.XElement)expectedResult2).ToString(System.Xml.Linq.SaveOptions.None);
                                            }
					    #if !ClientSKUFramework
                                            if (expectedResult1 is byte[])
                                            {
                                                expectedResult1 = new System.Data.Linq.Binary((byte[])expectedResult1);
                                            }

                                            if (expectedResult2 is byte[])
                                            {
                                                expectedResult2 = new System.Data.Linq.Binary((byte[])expectedResult2);
                                            }
					    #endif
                                            AstoriaTestLog.AreEqual(expectedResult1, expectedResult2, String.Format("Resource value for {0} does not match", property.Name), false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AstoriaTestLog.WriteLine(e.ToString());
                throw;
            }
        }

        public static void ResolveClientType(Workspace workspace, WebDataCtxWrapper ctx, Type resType)
        {
            if (workspace.Name == "Aruba")
            {
                switch (AstoriaTestProperties.DataLayerProviderKinds[0])
                {
                    case DataLayerProviderKind.Edm:
                        ctx.ResolveType = delegate(string typeName)
                        {
                            return resType.Assembly.GetType("ArubaClient." + typeName.Substring("Aruba.".Length), true, false);
                        };

                        ctx.ResolveName = delegate(Type type)
                        {
                            return "Aruba." + type.Name;

                        };
                        break;
                    case DataLayerProviderKind.LinqToSql:
                        ctx.ResolveType = delegate(string typeName)
                        {
                            return resType.Assembly.GetType("ArubaClientLTS." + typeName.Substring("Aruba.".Length), true, false);
                        };

                        ctx.ResolveName = delegate(Type type)
                        {
                            return "Aruba." + type.Name;

                        };
                        break;
                }
            }
        }

        public void VerifyClientLinq(QueryModel qm)
        {

            try
            {
                ExecuteLinq(qm.Workspace, qm.QueryResult, qm.ResContainer);
            }
            catch (Exception e)
            {
                AstoriaTestLog.FailAndContinue(e);
            }

        }


        public static void ExecuteLinq(Workspace workspace, ExpNode q, ResourceContainer container)
        {
            try
            {
                ExecuteLinq(workspace, q, container, false, null);
            }
            catch (Exception e)
            {
                AstoriaTestLog.FailAndContinue(e);
            }
        }

        public static void ExecuteLinq(Workspace workspace, ExpNode q, ResourceContainer container, bool bSingle, ExpNode altq)
        {

            System.Uri uri = new Uri(workspace.ServiceUri);

            WebDataCtxWrapper ctx = new WebDataCtxWrapper(uri);
            ctx.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            ctx.MergeOption = MergeOption.NoTracking;

            Type resType = container.BaseType.ClientClrType;
            ResolveClientType(workspace, ctx, resType);

            Type cType = typeof(WebDataCtxWrapper);
            MethodInfo mi = cType.GetMethod("CreateQuery", new Type[] { typeof(string) });

            Type pType = container.BaseType.ClientClrType;
            MethodInfo miConstructed = mi.MakeGenericMethod(pType);

            string uri2 = container.Name;
            AstoriaTestLog.WriteLineIgnore(workspace.ServiceUri + "/" + uri2);

            object[] args = { uri2 };
            object query = miConstructed.Invoke(ctx, args);

            LinqQueryBuilder lb = new LinqQueryBuilder(workspace, (IQueryable)query);
            string uri3 = lb.Build(q);
            AstoriaTestLog.WriteLineIgnore("Linq expression: " + lb.QueryExpression);

            //if (bSingle)
            //{
            //    var single = lb.QueryResultSingle;
            //    VerifyLinqSingle(workspace, altq, single, container);
            //}
            //else
            //{
            var queryable = lb.QueryResult;
            VerifyLinq(workspace, q, (IQueryable)queryable);
            //}

        }

        public static void VerifyLinq(Workspace workspace, ExpNode q, IQueryable results)
        {
            //verify if the results are ok before building the URI
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            foreach (object element in results)
                list.Add(element);


            //UriQueryBuilder ub = new UriQueryBuilder(workspace, "");
            // string ruri = ub.Build(q);

            //System.Uri uri = new Uri(workspace.ServiceUri);
            // string uriRel = ruri.Substring(ruri.IndexOf("/") + 1);
            // AstoriaTestLog.WriteLineIgnore(uri.ToString() + uriRel);

            AstoriaRequest request = workspace.CreateRequest(q);
            request.Format = SerializationFormatKind.Atom;

            try
            {
                AstoriaResponse response = request.GetResponse();
                //response.VerifyHttpStatusCodeOk(response.StatusCode);

                CommonPayload payload = response.CommonPayload;
                if (payload.Value != null)
                    payload.CompareValue(results, false, false);
                else
                    payload.Compare(results);

                //AstoriaTestLog.AreEqual(response.ContentType, SerializationFormatKinds.ContentTypeFromKind(response.OriginalRequest.SerializationKind),
                //"Content-Type does not match Accept header request");
            }
            catch (Exception e)
            {
                AstoriaTestLog.FailAndContinue(e);
            }
        }



    }
}
