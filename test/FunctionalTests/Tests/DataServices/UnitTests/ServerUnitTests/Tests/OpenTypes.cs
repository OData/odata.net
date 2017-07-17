//---------------------------------------------------------------------
// <copyright file="OpenTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    [TestClass()]
    public class OpenTypes
    {
        private static readonly MethodInfo GetValueMethod = typeof(DataServiceProviderMethods).GetMethod(
                "GetValue",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new Type[] { typeof(object), typeof(string) },
                null);

        private static readonly MethodInfo GetSequenceValueMethod = typeof(DataServiceProviderMethods).GetMethod(
                "GetSequenceValue",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new Type[] { typeof(object), typeof(string) },
                null);

#if LATE_BOUND_METHOD

        [TestMethod]
        public void LateBoundBinaryOperators()
        {
            // http://server/service.svc/Products?$filter=ProductName eq 'XBox'
            ParameterExpression pe = Expression.Parameter(typeof(object), "o");

            var query = openNorthwindContext.OpenProducts.
                Where(
                    Expression.Lambda<Func<OpenProduct, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.Equal,
                            Expression.Call(GetValueMethod, 
                                new Expression[] { pe, Expression.Constant("ProductName") }),
                                Expression.Constant("XBox", typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("Equal")),
                        new ParameterExpression[] { pe }));

            var baseline = from p in openNorthwindContext.OpenProducts.AsEnumerable().Cast<OpenProduct>()
                            where p.ProductName == "XBox"
                            select p;

            VerifyResults(query, baseline);

            // http://server/service.svc/Products?$filter=ProductName eq 'XBox'

            pe = Expression.Parameter(typeof(object), "o");

            var query2 = openNorthwindContext.OpenProducts.
                Where(
                    Expression.Lambda<Func<OpenProduct, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.NotEqual,
                            Expression.Call(GetValueMethod,
                                new Expression[] { pe, Expression.Constant("ProductName") }),
                                Expression.Constant("XBox", typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("NotEqual")),
                        new ParameterExpression[] { pe }));

            var baseline2 = from p in openNorthwindContext.OpenProducts.AsEnumerable().Cast<OpenProduct>()
                           where p.ProductName != "XBox"
                           select p;

            VerifyResults(query2, baseline2);

            // http://server/service.svc/Orders?$filter=Quantity add 1 eq 100

            pe = Expression.Parameter(typeof(object), "o");

            var query3 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.Equal,
                            Expression.MakeBinary(ExpressionType.Add,
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("Quantity") }),
                                Expression.Constant(1, typeof(object)),
                                false,
                                typeof(LateBoundMethods).GetMethod("Add")),
                            Expression.Constant(101, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("Equal")),
                        new ParameterExpression[] { pe }));

            var baseline3 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                            where (int)o.OpenProperties["Quantity"] + 1 == 101
                            select o;

            VerifyResults(query3, baseline3);

            // http://server/service.svc/Orders?$filter=Quantity sub 1 eq 99

            pe = Expression.Parameter(typeof(object), "o");

            var query4 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.Equal,
                            Expression.MakeBinary(ExpressionType.Subtract,
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("Quantity") }),
                                Expression.Constant(1, typeof(object)),
                                false,
                                typeof(LateBoundMethods).GetMethod("Subtract")),
                            Expression.Constant(99, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("Equal")),
                        new ParameterExpression[] { pe }));

            var baseline4 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                            where (int)o.OpenProperties["Quantity"] - 1 == 99
                            select o;

            VerifyResults(query4, baseline4);

            // http://server/service.svc/Orders?$filter=Quantity mult 2 eq 200

            pe = Expression.Parameter(typeof(object), "o");

            var query5 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.Equal,
                            Expression.MakeBinary(ExpressionType.Multiply,
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("Quantity") }),
                                Expression.Constant(2, typeof(object)),
                                false,
                                typeof(LateBoundMethods).GetMethod("Multiply")),
                            Expression.Constant(200, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("Equal")),
                        new ParameterExpression[] { pe }));

            var baseline5 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                            where (int)o.OpenProperties["Quantity"] * 2 == 200
                            select o;

            VerifyResults(query5, baseline5);

            // http://server/service.svc/Orders?$filter=Quantity div 2 eq 50

            pe = Expression.Parameter(typeof(object), "o");

            var query6 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.Equal,
                            Expression.MakeBinary(ExpressionType.Divide,
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("Quantity") }),
                                Expression.Constant(2, typeof(object)),
                                false,
                                typeof(LateBoundMethods).GetMethod("Divide")),
                            Expression.Constant(50, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("Equal")),
                        new ParameterExpression[] { pe }));

            var baseline6 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                            where (int)o.OpenProperties["Quantity"] / 2 == 50
                            select o;

            VerifyResults(query6, baseline6);

            // http://server/service.svc/Orders?$filter=Quantity mod 3 eq 1

            pe = Expression.Parameter(typeof(object), "o");

            var query7 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.Equal,
                            Expression.MakeBinary(ExpressionType.Modulo,
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("Quantity") }),
                                Expression.Constant(3, typeof(object)),
                                false,
                                typeof(LateBoundMethods).GetMethod("Modulo")),
                            Expression.Constant(1, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("Equal")),
                        new ParameterExpression[] { pe }));

            var baseline7 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                            where (int)o.OpenProperties["Quantity"] % 3 == 1
                            select o;

            VerifyResults(query7, baseline7);

            // http://server/service.svc/Products(1000)?$filter=Quantity lt 100

            pe = Expression.Parameter(typeof(object), "o");

            var query8 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.LessThan,
                            Expression.Call(GetValueMethod,
                                new Expression[] { pe, Expression.Constant("Quantity") }),
                            Expression.Constant(100, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("LessThan")),
                        new ParameterExpression[] { pe }));

            var baseline8 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                            where (int)o.OpenProperties["Quantity"] < 100
                            select o;

            VerifyResults(query8, baseline8);

            // http://server/service.svc/Products(1000)?$filter=Quantity lte 100

            pe = Expression.Parameter(typeof(object), "o");

            var query9 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.LessThanOrEqual,
                            Expression.Call(GetValueMethod,
                                new Expression[] { pe, Expression.Constant("Quantity") }),
                            Expression.Constant(100, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("LessThanOrEqual")),
                        new ParameterExpression[] { pe }));

            var baseline9 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                            where (int)o.OpenProperties["Quantity"] <= 100
                            select o;

            VerifyResults(query9, baseline9);

            // http://server/service.svc/Products(1000)?$filter=Quantity gt 100

            pe = Expression.Parameter(typeof(object), "o");

            var query10 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.GreaterThan,
                            Expression.Call(GetValueMethod,
                                new Expression[] { pe, Expression.Constant("Quantity") }),
                            Expression.Constant(100, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("GreaterThan")),
                        new ParameterExpression[] { pe }));

            var baseline10 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                             where (int)o.OpenProperties["Quantity"] > 100
                             select o;

            VerifyResults(query10, baseline10);

            // http://server/service.svc/Products(1000)?$filter=Quantity gte 100

            pe = Expression.Parameter(typeof(object), "o");

            var query11 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.GreaterThanOrEqual,
                            Expression.Call(GetValueMethod,
                                new Expression[] { pe, Expression.Constant("Quantity") }),
                            Expression.Constant(100, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("GreaterThanOrEqual")),
                        new ParameterExpression[] { pe }));

            var baseline11 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                             where (int)o.OpenProperties["Quantity"] >= 100
                             select o;

            VerifyResults(query11, baseline11);

            // http://server/service.svc/Products(1000)?$filter=Shipped and PaidFor

            pe = Expression.Parameter(typeof(object), "o");

            var query12 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.AndAlso,
                            Expression.Convert(
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("Shipped") }),
                                typeof(bool)),
                            Expression.Convert(
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("PaidFor") }),
                                typeof(bool)),
                            false,
                            null),
                        new ParameterExpression[] { pe }));

            var baseline12 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                             where (bool)o.OpenProperties["PaidFor"] && (bool)o.OpenProperties["Shipped"]
                             select o;

            VerifyResults(query12, baseline12);

            // http://server/service.svc/Products(1000)?$filter=Shipped or PaidFor

            pe = Expression.Parameter(typeof(object), "o");

            var query13 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.OrElse,
                            Expression.Convert(
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("Shipped") }),
                                typeof(bool)),
                            Expression.Convert(
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("PaidFor") }),
                                typeof(bool)),
                            false,
                            null),
                        new ParameterExpression[] { pe }));

            var baseline13 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                             where (bool)o.OpenProperties["PaidFor"] || (bool)o.OpenProperties["Shipped"]
                             select o;

            VerifyResults(query13, baseline13);

        }

        [TestMethod]
        public void LateBoundUnaryOperators()
        {
            // http://server/service.svc/Orders?$filter= -Quantity eq -100

            ParameterExpression pe = Expression.Parameter(typeof(object), "o");

            var query1 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.Equal,
                            Expression.MakeUnary(
                                ExpressionType.Negate,
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("Quantity") }), 
                                typeof(object),
                                typeof(LateBoundMethods).GetMethod("Negate")),
                            Expression.Constant(-100, typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("Equal")),
                        new ParameterExpression[] { pe }));

            var baseline1 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                             where -(int)o.OpenProperties["Quantity"] == -100
                             select o;

            VerifyResults(query1, baseline1);

            // http://server/service.svc/Orders?$filter= not(Quantity eq 100)

            pe = Expression.Parameter(typeof(object), "o");

            var query2 = openNorthwindContext.OpenOrders.
                Where(
                    Expression.Lambda<Func<OpenOrder, bool>>(
                        Expression.MakeUnary(ExpressionType.Not,
                            Expression.MakeBinary(
                                ExpressionType.Equal,
                                Expression.Call(GetValueMethod,
                                    new Expression[] { pe, Expression.Constant("Quantity") }),
                                Expression.Constant(100, typeof(object)),
                                false,
                                typeof(LateBoundMethods).GetMethod("Equal")),
                            typeof(bool)),
                        new ParameterExpression[] { pe }));

            var baseline2 = from o in openNorthwindContext.OpenOrders.AsEnumerable().Cast<OpenOrder>()
                            where !((int)o.OpenProperties["Quantity"] == 100)
                            select o;

            VerifyResults(query2, baseline2);

            // http://server/service.svc/Products?$filter= IsOf(Product, OpenProduct)

            pe = Expression.Parameter(typeof(OpenProduct), "o");

            var query4 = openNorthwindContext.OpenProducts.
                Where(
                    Expression.Lambda<Func<OpenProduct, bool>>(
                        Expression.TypeIs(pe, typeof(OpenProduct)),
                        new ParameterExpression[] { pe }));

            var baseline4 = from o in openNorthwindContext.OpenProducts.AsEnumerable()
                            where o is OpenProduct
                            select o;

            VerifyResults(query4, baseline4);
        }

        [TestMethod]
        public void LateBoundMethodsFromQueryOps()
        {
            // http://server/service.svc/Products?$filter=toupper(ProductName) eq 'XBOX'

            ParameterExpression pe = Expression.Parameter(typeof(OpenProduct), "o");

            var query = openNorthwindContext.OpenProducts.
                Where(
                    Expression.Lambda<Func<OpenProduct, bool>>(
                        Expression.MakeBinary(
                            ExpressionType.Equal,
                            Expression.Call(
                                Expression.Convert(
                                    Expression.Call(GetValueMethod,
                                        new Expression[] { pe, Expression.Constant("ProductName") }),
                                    typeof(string)),
                                typeof(string).GetMethod("ToUpper", Type.EmptyTypes)),
                            Expression.Constant("XBOX", typeof(object)),
                            false,
                            typeof(LateBoundMethods).GetMethod("Equal")),
                        new ParameterExpression[] { pe }));

            var baseline = from p in openNorthwindContext.OpenProducts.AsEnumerable().Cast<OpenProduct>()
                            where p.ProductName.ToUpper() == "XBOX"
                            select p;

            VerifyResults(query, baseline);

        }

        private void VerifyResults(IQueryable query, IEnumerable baseline)
        {
            IEnumerator left = query.GetEnumerator();
            IEnumerator right = baseline.GetEnumerator();

            try
            {
                IDataServiceProvider provider = UnitTestsUtil.GetProvider(typeof(OpenNorthwindContext));
                while (left.MoveNext() && right.MoveNext())
                {
                    if (left.Current == null && right.Current == null)
                    {
                        break;
                    }

                    if ((left.Current == null || right.Current == null) ||
                        !left.Current.Equals(right.Current))
                    {
                        throw new Exception("Test Failed");
                    }
                }
                if (left.MoveNext() || right.MoveNext())
                {
                    throw new Exception("Test Failed");
                }
            }
            finally
            {
                typeof(LateBoundMethods).GetProperty("Provider", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, null, null);
            }
        }

        // TODO: enable this once we have support for expanding open properties
        //[TestMethod]
        public void OpenTypeExpandTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(TypedCustomDataContext<OpenElement>);
                using (StaticCallbackManager<PopulatingValuesEventArgs<OpenElement>>.RegisterStatic((sender, args) =>
                {
                    var o = new OpenElement();
                    o.ID = "100";
                    o.Properties.Add("sampleValue1", new OpenElement() { ID = "101" });
                    o.Properties.Add("sampleValue2", new OpenElement[] { new OpenElement() { ID = "102" } });
                    o.Properties.Add("address", new Address() { StreetAddress = "L1", City = "City", State = "S1", PostalCode="98052" });
                    o.Properties.Add("primitive", "abc");
                    o.Properties.Add("thenull", null);
                    args.Values.Add(o);
                }))
                {
                    TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                    {
                        SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];
                        request.Accept = format.MimeTypes[0];

                        XmlDocument document;
                        string[] validation;

                        request.RequestUriString = "/Values('100')";
                        request.SendRequest();
                        document = format.LoadXmlDocumentFromStream(request.GetResponseStream());
                        if (format == SerializationFormatData.Atom)
                        {
                            validation = new string[]
                            {
                                "/atom:entry/atom:link[@title='OpenTypes_OpenElement']",
                                "/atom:entry/atom:content/adsm:properties/ads:ID[text()='100']",
                                "/atom:entry/atom:link[@title='sampleValue1' and @type='application/atom+xml;type=entry']",
                                "/atom:entry/atom:link[@title='sampleValue2' and @type='application/atom+xml;type=feed']",
                                //"/atom:entry/atom:content/ads:address/ads:Line1[text()='L1']",
                                //"/atom:entry/atom:content/ads:address/ads:Line2[@adsm:null='true']",
                                "/atom:entry/atom:content/adsm:properties/ads:primitive[text()='abc']"
                            };
                        }
                        else
                        {
                            Debug.Assert(format == SerializationFormatData.Json);
                            validation = new string[]
                            {
                                "/Object/__metadata/type[text()='AstoriaUnitTests.Tests.OpenTypes_OpenElement']",
                                "/Object/ID[text()='100']",
                                "/Object/sampleValue1/__deferred",
                                "/Object/sampleValue2/__deferred",
                                //"/Object/address/Line1[text()='L1']",
                                //"/Object/address/Line2[@IsNull='true']",
                                "/Object/primitive[text()='abc']"
                            };
                        }
                        TestUtil.TraceXml(document);
                        foreach (string v in validation)
                        {
                            TestUtil.AssertSelectSingleElement(document, v);
                        }

                        request.RequestUriString = "/Values('100')?$expand=sampleValue1";
                        request.SendRequest();
                        document = format.LoadXmlDocumentFromStream(request.GetResponseStream());
                        TestUtil.TraceXml(document);
                        if (format == SerializationFormatData.Atom)
                        {
                            TestUtil.AssertSelectSingleElement(
                                document,
                                "/atom:entry/atom:link[@title='sampleValue1']/*/atom:entry/atom:content/adsm:properties/ads:ID[text()='101']");
                        }
                        else
                        {
                            TestUtil.AssertSelectSingleElement(
                                document, 
                                "/Object/sampleValue1/__metadata/type[text()='AstoriaUnitTests.Tests.OpenTypes_OpenElement']");
                        }

                        request.RequestUriString = "/Values('100')?$expand=sampleValue2";
                        request.SendRequest();
                        document = format.LoadXmlDocumentFromStream(request.GetResponseStream());
                        TestUtil.TraceXml(document);
                        if (format == SerializationFormatData.Atom)
                        {
                            TestUtil.AssertSelectSingleElement(
                                document,
                                "/atom:entry/atom:link[@title='sampleValue2']/*/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID[text()='102']");
                        }
                        else
                        {
                            TestUtil.AssertSelectSingleElement(
                                document,
                                "/Object/sampleValue2/Array/Object/__metadata/type[text()='AstoriaUnitTests.Tests.OpenTypes_OpenElement']");
                        }
                    });
                }
            }
        }

#endif

        [TestMethod]
        public void OpenTypeUrlTest()
        {
            string[] queries = new string[]
            {
                "/Values",
                "/Values('100')/sampleValue2",
                "/Values('100')/sampleValue4",  // no () after sample value 4, it's an array, gets rejected
                "/Values('100')/notfound",      // This should be bad query since we expect to return 404 for open-properties not found
            };

            string[] badQueries = new string[]
            {
                "/Values/sampleValue1",         // no () after Values
                "/Values('100')/sampleValue4()",
                "/Values('100')/sampleValue4('101')",
                "/Values('100')/sampleValue4('101')/ID",
                "/Values('100')/sampleValue4(101)",
                // Since we don't detect types during runtime, this queries will fail
                "/Values('100')/sampleValue3/Identity/Name",
                "/Values('100')/sampleValue3/Identity",
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(OpenTypeContextWithReflection<OpenElement>);
                int i = 0;
                using (StaticCallbackManager<PopulatingValuesEventArgs<OpenElement>>.RegisterStatic((sender, args) =>
                {
                    var o = new OpenElement();
                    o.ID = "100";
                    o.Properties.Add("sampleValue1", "abc");
                    o.Properties.Add("sampleValue2", 12345);
                    args.Values.Add(o);
                }))
                {
                    foreach (string query in queries)
                    {
                        i++;
                        Trace.WriteLine(query);
                        request.RequestUriString = query;
                        request.SendRequest();
                        Trace.WriteLine(request.GetResponseStreamAsText());
                    }

                    foreach (string query in badQueries)
                    {
                        i++;
                        Trace.WriteLine(query);
                        request.RequestUriString = query;
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, true);
                    }
                }
            }
        }

        [TestMethod]
        public void OpenTypeOrderByTest()
        {
            string[][] queries = new string[][]
            {
                new string[] { "/Values?$orderby=sampleValue1", "abc", "ABC" },
                new string[] { "/Values?$orderby=sampleValue1 desc", "ABC", "abc" },
                new string[] { "/Values?$orderby=sampleValue1,sampleValue2", "abc", "ABC" },
                new string[] { "/Values('101')/sampleValue4()?$orderby=ID", "101", "102" },
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(OpenTypeContextWithReflection<OpenElement>);

                using (StaticCallbackManager<PopulatingValuesEventArgs<OpenElement>>.RegisterStatic((sender, args) =>
                {
                    var o = new OpenElement();
                    o.ID = "101";
                    o.Properties.Add("sampleValue1", "abc");
                    o.Properties.Add("sampleValue2", 12345);
                    o.Properties.Add("sampleValue3", true);
                    args.Values.Add(o);

                    o = new OpenElement();
                    o.Properties.Add("sampleValue1", "ABC");
                    o.Properties.Add("sampleValue2", 1);
                    o.Properties.Add("sampleValue3", false);
                    args.Values.Add(o);
                }))
                {
                    int i = 0;
                    foreach (string[] queryParts in queries)
                    {
                        string query = queryParts[0];
                        Trace.WriteLine("Running " + query);
                        request.RequestUriString = query;
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, i == queries.Length - 1);
                        if (exception == null)
                        {
                            string response = request.GetResponseStreamAsText();
                            int firstOffset = response.IndexOf(queryParts[1]);
                            int secondOffset = response.IndexOf(queryParts[2]);
                            if (firstOffset >= secondOffset)
                            {
                                Assert.Fail(
                                    "For '" + query + "' the offset for " + queryParts[1] + " (" + firstOffset + ") should be less than " +
                                    "the offset for " + queryParts[2] + " (" + secondOffset + ") in '" + response + "'");
                            }
                        }
                        i++;
                    }
                }
            }
        }

        private void OpenTypeFilterPageCustomizer(DataServiceConfiguration config, Type serviceType)
        {
            config.SetEntitySetPageSize("*", 1);
        }

        [TestMethod]
        public void ComplexResourceType_Open()
        {
            Microsoft.OData.Service.Providers.ResourceType rt = new Microsoft.OData.Service.Providers.ResourceType(
                    typeof(RowComplexType),
                    ResourceTypeKind.ComplexType,
                    null,
                    "AstoriaUnitTests.Stubs",
                    "Address",
                    false);
            bool errorOccurred = false;

            try
            {
                rt.IsOpenType = true;
            }
            catch (InvalidOperationException)
            {
                errorOccurred = true;
            }

            Assert.IsFalse(errorOccurred);
        }

        [TestMethod]
        public void OpenTypeBasicTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("TypeData", TypeData.Values));
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(OpenTypeContextWithReflection<OpenElement>);
                request.RequestUriString = "/Values";

                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    TypeData data = (TypeData)values["TypeData"];
                    foreach (object sampleValue in data.SampleValues)
                    {
                        using (StaticCallbackManager<PopulatingValuesEventArgs<OpenElement>>.RegisterStatic((sender, args) =>
                        {
                            var o = new OpenElement();
                            o.Properties.Add("sampleValue", sampleValue);
                            args.Values.Add(o);
                        }))
                        {
                            Exception exception = TestUtil.RunCatching(request.SendRequest);
                            // If we choose to throw when an open property is, say, IntPtr, use this:
                            // Also check for null, since when the value is null, there is no way to know the datatype of the property
                            TestUtil.AssertExceptionExpected(exception, !data.IsTypeSupported && sampleValue != null);
                        }
                    }
                });
            }
        }

        [TestMethod]
        public void OpenTypeIncorrectPropertyNameTest()
        {
            string[] invalidNames = new string[]
            {
                null, "", " ", "1", "@for", 
                //"a.", 
                "a;", "a`", "a,", 
                //"a-", 
                "a+", "a\'", "a[", "a]", "a ", " a", 
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                foreach (string name in invalidNames)
                {
                    using (StaticCallbackManager<PopulatingValuesEventArgs<OpenElement>>.RegisterStatic((sender, args) =>
                    {
                        var o = new OpenElement();
                        o.Properties[name] = 1;
                        args.Values.Add(o);
                    }))
                    {
                        request.DataServiceType = typeof(OpenTypeContextWithReflection<OpenElement>);
                        request.Accept = "application/atom+xml,application/xml";
                        request.RequestUriString = "/Values";
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, true);
                    }
                }
            }
        }

        [TestMethod]
        public void OpenTypeJsonWithNulls()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                using (StaticCallbackManager<PopulatingValuesEventArgs<OpenElement>>.RegisterStatic((sender, args) =>
                {
                    var o = new OpenElement();
                    o.Properties["Foo"] = null;
                    args.Values.Add(o);
                }))
                {
                    request.DataServiceType = typeof(OpenTypeContextWithReflection<OpenElement>);
                    request.RequestUriString = "/Values";
                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    String response = request.GetResponseStreamAsText();
                    Assert.AreEqual(exception, null);
                    Assert.IsTrue(response.Contains("\"Foo\":null"));
                }
            }
        }

        [TestMethod]
        public void RequestQueryRecursionLimitReached()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                using (StaticCallbackManager<PopulatingValuesEventArgs<OpenElement>>.RegisterStatic((sender, args) =>
                {
                    var o = new OpenElement();
                    o.Properties["Foo"] = null;
                    args.Values.Add(o);
                }))
                {
                    request.DataServiceType = typeof(OpenTypeContextWithReflection<OpenElement>);
                    request.RequestUriString = "/Values?$filter=length(length(length(length(length(length(length(length(length(length(length(length(length(length(length(((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((Foo))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))) gt 5";
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsTrue(exception != null, "Exception must be thrown");
                    Assert.IsTrue(exception.InnerException.Message.Contains("Recursion"));
                }
            }
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        [Ignore] // Remove Atom
        // [TestMethod]
        public void OpenTypeGetOpenPropertyValuesWithNull()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenTypeContextWithReflectionConfig), "NullOnGetOpenPropertyValues"))
                using (StaticCallbackManager<PopulatingValuesEventArgs<OpenElement>>.RegisterStatic((sender, args) =>
                {
                    var o = new OpenElement();
                    o.Properties["Foo"] = 1;
                    args.Values.Add(o);
                }))
                {
                    OpenTypeContextWithReflectionConfig.NullOnGetOpenPropertyValues = true;
                    request.DataServiceType = typeof(OpenTypeContextWithReflection<OpenElement>);
                    request.RequestUriString = "/Values";
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsTrue(exception == null, "GetOpenPropertyValues should handle null as an empty collection.");
                }
            }
        }

        public class OpenChangeInterceptorService<T> : DataService<T>, IServiceProvider where T : class, new()
        {
            private T service;

            public OpenChangeInterceptorService()
            {
                this.service = new T();
            }

            public static void InitializeService(IDataServiceConfiguration configuration)
            {
                configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
            }

            [ChangeInterceptor("Values")]
            public void ChangeValues(OpenElement entity, UpdateOperations operation)
            {
                OpenChangeInterceptorServiceState.InterceptorFired = true;
            }


            #region IServiceProvider Members

            public object GetService(Type serviceType)
            {
                return (this.service as IServiceProvider).GetService(serviceType);
            }

            #endregion
        }

        public class OpenChangeInterceptorServiceState
        {
            public static bool InterceptorFired { get; set; }
        }

        [TestMethod]
        public void OpenTypeMetadataTest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);
                request.RequestUriString = "/$metadata";
                request.SendRequest();
                using (Stream responseStream = request.GetResponseStream())
                {
                    var document = new System.Xml.XPath.XPathDocument(responseStream);

                    // Ensure the OpenType attribute is there.
                    var expression = System.Xml.XPath.XPathExpression.Compile("//csdl:EntityType[@OpenType]", TestUtil.TestNamespaceManager);
                    var nodeIterator = document.CreateNavigator().Select(expression);
                    int count = 0;
                    while (nodeIterator.MoveNext())
                    {
                        Assert.AreEqual("true", nodeIterator.Current.SelectSingleNode("@OpenType").Value);
                        count++;
                    }

                    // The OpenType attribute is present at all levels of the type hierarchy; expect it on all types.
                    Assert.AreEqual(3, count);
                }
            }
        }

        #region Helper types.

        public class MismatchedOpenElement
        {
            public MismatchedOpenElement() { this.ID = "100"; Properties = new Dictionary<string, object>(); }
            public string ID { get; set; }
            public Dictionary<string, object> Properties { get; private set; }
        }

        public class OpenElement
        {
            public OpenElement() { this.ID = "100"; Properties = new Dictionary<string, object>(); }
            public string ID { get; set; }
            public Dictionary<string, object> Properties { get; private set; }
        }

        #endregion Helper types.
    }
}
