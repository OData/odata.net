//---------------------------------------------------------------------
// <copyright file="RequestQueryParserTest.cs" company="Microsoft">
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
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Prov = Microsoft.OData.Service.Providers;

    #endregion Namespaces

    /// <summary>
    /// This is a test class for RequestQueryParser and is intended
    /// to contain all RequestQueryParser Unit Tests.
    /// </summary>
    [TestClass]
    public class RequestQueryParserTest
    {
        /// <summary>Gets the Microsoft.OData.Service.RequestQueryParser internal type.</summary>
        public static Type GetRequestQueryParserType()
        {
            Assembly assembly = typeof(IDataServiceHost).Assembly;
            Type type = assembly.GetType("Microsoft.OData.Service.Parsing.RequestQueryParser", true);
            return type;
        }

        /// <summary>Invokes Microsoft.OData.Service.RequestQueryParser.Where(IWebDataServiceProvider, IQueryable, string).</summary>
        public static IQueryable InvokeWhere(object service, object requestDescription, IQueryable source, string predicate)
        {
            Type type = GetRequestQueryParserType();
            MethodInfo method = type.GetMethod("Where", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            try
            {
                Expression body = (Expression)method.Invoke(null, new object[] { service, requestDescription, Expression.Constant(source), predicate });
                Expression<Func<IQueryable>> lambda = Expression.Lambda<Func<IQueryable>>(Expression.Convert(body, typeof(IQueryable)));
                Func<IQueryable> func = lambda.Compile();
                return func();
            }
            catch (TargetInvocationException invocationException)
            {
                invocationException.InnerException.Data["OriginalStackTrace"] = invocationException.InnerException.StackTrace;
                throw invocationException.InnerException;
            }
        }

        public static XmlDocument CreateDocumentFromIQueryable(IQueryable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
            XmlElement sourceElement = document.CreateElement("Source");
            document.AppendChild(sourceElement);
            PopulateNodeForExpression(source.Expression, sourceElement);
            return document;
        }

        public static void AppendNodeForExpression(Expression expression, string name, XmlNode node)
        {
            XmlElement expressionElement = node.OwnerDocument.CreateElement(name);
            PopulateNodeForExpression(expression, node.AppendChild(expressionElement));
        }

        public static void AppendNodeForValue(object value, string name, XmlNode node)
        {
            Debug.Assert(name != null, "name != null");
            Debug.Assert(node != null, "node != null");

            XmlAttribute attribute = node.OwnerDocument.CreateAttribute(name);
            attribute.Value = (value == null) ? "null" : value.ToString();
            node.Attributes.Append(attribute);
        }

        public static void PopulateNodeForExpression(Expression expression, XmlNode node)
        {
            if (expression == null)
            {
                return;
            }

            AppendNodeForValue(expression.NodeType, "NodeType", node);
            AppendNodeForValue(expression.Type, "Type", node);
            if (expression is System.Linq.Expressions.BinaryExpression)
            {
                var e = (System.Linq.Expressions.BinaryExpression)expression;
                AppendNodeForValue(e.IsLifted, "IsLifted", node);
                AppendNodeForValue(e.IsLiftedToNull, "IsLiftedToNull", node);
                AppendNodeForValue(e.Method, "Method", node);
                AppendNodeForExpression(e.Conversion, "Conversion", node);
                AppendNodeForExpression(e.Left, "Left", node);
                AppendNodeForExpression(e.Right, "Right", node);
            }
            else if (expression is ConditionalExpression)
            {
                var ce = (System.Linq.Expressions.ConditionalExpression)expression;
                AppendNodeForExpression(ce.IfFalse, "IfFalse", node);
                AppendNodeForExpression(ce.IfTrue, "IfTrue", node);
                AppendNodeForExpression(ce.Test, "Test", node);
            }
            else if (expression is System.Linq.Expressions.ConstantExpression)
            {
                var e = (System.Linq.Expressions.ConstantExpression)expression;
                AppendNodeForValue(e.Value, "Value", node);
            }
            else if (expression is InvocationExpression)
            {
                var e = (System.Linq.Expressions.InvocationExpression)expression;
                AppendNodeForExpression(e.Expression, "Expression", node);
                foreach (Expression argument in e.Arguments)
                {
                    AppendNodeForExpression(argument, "Argument", node);
                }
            }
            else if (expression is LambdaExpression)
            {
                var e = (System.Linq.Expressions.LambdaExpression)expression;
                AppendNodeForExpression(e.Body, "Body", node);
                foreach (Expression argument in e.Parameters)
                {
                    AppendNodeForExpression(argument, "Parameters", node);
                }
            }
            else if (expression is ListInitExpression)
            {
                throw new NotImplementedException();
            }
            else if (expression is System.Linq.Expressions.MemberExpression)
            {
                var e = (System.Linq.Expressions.MemberExpression)expression;
                AppendNodeForValue(e.Member, "Member", node);
                AppendNodeForExpression(e.Expression, "Expression", node);
            }
            else if (expression is System.Linq.Expressions.MemberInitExpression)
            {
                throw new NotImplementedException();
            }
            else if (expression is System.Linq.Expressions.MethodCallExpression)
            {
                var e = (System.Linq.Expressions.MethodCallExpression)expression;
                AppendNodeForValue(e.Method, "Method", node);
                AppendNodeForExpression(e.Object, "Object", node);
                foreach (Expression argument in e.Arguments)
                {
                    AppendNodeForExpression(argument, "Argument", node);
                }
            }
            else if (expression is NewArrayExpression)
            {
                throw new NotImplementedException();
            }
            else if (expression is System.Linq.Expressions.NewExpression)
            {
                throw new NotImplementedException();
            }
            else if (expression is System.Linq.Expressions.ParameterExpression)
            {
                var e = (System.Linq.Expressions.ParameterExpression)expression;
                AppendNodeForValue(e.Name, "Name", node);
            }
            else if (expression is System.Linq.Expressions.TypeBinaryExpression)
            {
                throw new NotImplementedException();
            }
            else if (expression is System.Linq.Expressions.UnaryExpression)
            {
                var e = (System.Linq.Expressions.UnaryExpression)expression;
                AppendNodeForValue(e.IsLifted, "IsLifted", node);
                AppendNodeForValue(e.IsLiftedToNull, "IsLiftedToNull", node);
                AppendNodeForValue(e.Method, "Method", node);
                AppendNodeForExpression(e.Operand, "Operand", node);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void RequestQueryParserIdentifiersAmbiguous()
        {
            string[] predicates = new string[]
            {
                "ID eq 0",
                "length(length) eq 1",
                "length('length') gt 1",
                "cast(cast, 'Edm.Byte') lt 100",
            };
            var contextType = typeof(TypedCustomDataContext<AllTypes>);
            TypedCustomDataContext<AmbiguousNameType>.ClearHandlers();
            TypedCustomDataContext<AmbiguousNameType>.ClearValues();
            try
            {
                TypedCustomDataContext<AmbiguousNameType>.ValuesRequested += (sender, e) =>
                    {
                        var s = (TypedCustomDataContext<AmbiguousNameType>)sender;
                        s.SetValues(new object[] { new AmbiguousNameType() { ID = 1, cast = 1, length = "l" } });
                    };
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(TypedCustomDataContext<AmbiguousNameType>);
                    foreach (string predicate in predicates)
                    {
                        request.RequestUriString = "/Values?$filter=" + predicate.Replace(" ", "%20");
                        request.SendRequest();
                        //Trace.WriteLine("Running query for " + predicate);
                        //TestUtil.TraceXml(request.GetResponseStreamAsXmlDocument());
                    }
                }
            }
            finally
            {
                TypedCustomDataContext<AmbiguousNameType>.ClearHandlers();
                TypedCustomDataContext<AmbiguousNameType>.ClearValues();
            }
        }

        public class AmbiguousNameType
        {
            public int ID { get; set; }
            public string length { get; set; }
            public int cast { get; set; }
        }

        [TestMethod]
        public void RequestQueryParserNullableTest()
        {
            // Reproes:
            //  - Round doe not work for properties (only constants)
            //  - Protocol: Filter cannot be used with nullable properties
            //  - Can't call year() in filter expression
            //  - get cast exception on server when trying to use a Byte property in a filter option
            string[] predicates = new string[]
            {
                "BoolType eq false",
                "not (BoolType eq null)",
                "NullableBoolType eq null",
                "ByteType lt 100",
                "NullableByteType lt 100",
                "cast(NullableSingleType, 'Edm.Int32') eq null",
                "cast(NullableSingleType, 'Edm.Single') eq null",
                "year(NullableDateTimeOffsetType) eq 2008",
                "round(NullableDoubleType) eq 2008",
                "round(NullableSingleType) eq 2008",
                "NullableInt32Type eq 20",
                "NullableByteType eq 20",
            };

            var contextType = typeof(TypedCustomDataContext<AllTypes>);
            TypedCustomDataContext<AllTypes>.ClearHandlers();
            TypedCustomDataContext<AllTypes>.ClearValues();
            try
            {
                TypedCustomDataContext<AllTypes>.ValuesRequested += (sender, args) =>
                    {
                        var t = (TypedCustomDataContext<AllTypes>)sender;
                        object[] newValues = new object[]
                        {
                            new AllTypes()
                            {
                                ID = 1,
                                NullableDateTimeOffsetType = new DateTime(2008, 1, 20),
                                NullableDoubleType = 2008d,
                                NullableSingleType = 2008,
                                NullableInt32Type = 20,
                                NullableByteType = 20,
                            }
                        };
                        t.SetValues(newValues);
                    };
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = contextType;
                    foreach (string predicate in predicates)
                    {
                        Trace.WriteLine("Requesting " + predicate);
                        request.RequestUriString = "/Values?$filter=" + predicate.Replace(" ", "%20");
                        request.SendRequest();
                    }
                }
            }
            finally
            {
                TypedCustomDataContext<AllTypes>.ClearHandlers();
                TypedCustomDataContext<AllTypes>.ClearValues();
            }
        }

        private static object GetProviderWrapper(object service)
        {
            PropertyInfo providerWrapperProperty = typeof(IDataServiceHost).Assembly
                                                                    .GetType("Microsoft.OData.Service.IDataService")
                                                                    .GetProperty("Provider");
            return providerWrapperProperty.GetValue(service, null);
        }

        private static Prov.IDataServiceMetadataProvider GetProvider(object service)
        {
            object providerWrapper = GetProviderWrapper(service);

            FieldInfo providerField = typeof(IDataServiceHost).Assembly
                .GetType("Microsoft.OData.Service.Providers.DataServiceProviderWrapper")
                .GetField("metadataProvider", BindingFlags.NonPublic | BindingFlags.Instance);

            return (Prov.IDataServiceMetadataProvider)providerField.GetValue(providerWrapper);
        }

        private static object GetResourceSetWrapper(object service, string name)
        {
            object provider = GetProviderWrapper(service);
            MethodInfo tryResolveResourceSet = typeof(IDataServiceHost).Assembly
                .GetType("Microsoft.OData.Service.Providers.DataServiceProviderWrapper")
                .GetMethod("TryResolveResourceSet", BindingFlags.Public | BindingFlags.Instance);
            return tryResolveResourceSet.Invoke(provider, new object[] { name });
        }

        private static Prov.ResourceSet GetResourceSet(object service, string name)
        {
            Prov.IDataServiceMetadataProvider provider = GetProvider(service);
            Prov.ResourceSet resourceSet;
            if (provider.TryResolveResourceSet(name, out resourceSet))
            {
                return resourceSet;
            }

            return null;
        }

        private static Prov.ResourceType GetResourceTypeForContainer(object service, string name)
        {
            return GetResourceSet(service, name).ResourceType;
        }

        private static Prov.ResourceType GetResourceType(object service, string typename)
        {
            Prov.IDataServiceMetadataProvider provider = GetProvider(service);
            Prov.ResourceType type;
            if (provider.TryResolveResourceType(typename, out type))
            {
                return type;
            }

            return null;
        }

        // github: https://github.com/OData/odata.net/issues/878
        // [TestMethod]
        [Ignore] // test case issue, request uri and service root in this case is null.
        public void RequestQueryParserTestFilterTest()
        {
            var service = new OpenWebDataService<CustomDataContext>();
            service.AttachHost(new TestServiceHost2());
            CustomDataContext context = ServiceModelData.InitializeAndGetContext(service);
            Prov.ResourceType customerType = GetResourceTypeForContainer(service, "Customers");
            object customerSet = GetResourceSetWrapper(service, "Customers");
            IQueryable result = InvokeWhere(service, GetRequestDescription(customerType, customerSet, "Customers"), context.Customers, "ID eq 5");
            XmlDocument document = CreateDocumentFromIQueryable(result);
            Trace.WriteLine(document.InnerXml);
            UnitTestsUtil.VerifyXPaths(document,
                "/Source//*[@NodeType='Lambda']",
                "/Source//*[@NodeType='Lambda']/Body[@Type='System.Boolean']",
                "/Source//*[@NodeType='Lambda']/Body[@Type='System.Boolean']/*[@NodeType='Constant' and @Value='5']",
                "/Source//*[@NodeType='Lambda']/Body[@Type='System.Boolean']/*[@NodeType='MemberAccess' and contains(@Member, 'ID')]");
        }

        // github: https://github.com/OData/odata.net/issues/878
        // [TestMethod]
        [Ignore] // test case issue, request uri and service root in this case is null.
        public void RequestQueryParserTestBasicTests()
        {
            using (OpenWebDataServiceHelper.AcceptReplaceFunctionInQuery.Restore())
            {
                OpenWebDataServiceHelper.AcceptReplaceFunctionInQuery.Value = true;
                TestUtil.ClearConfiguration();

                // Repro Protocol: Filter - replace not implemented?
                // Repro Protocol: Can't cast simple primitive value in filter expression
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ServiceModelData", ServiceModelData.Values));

                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    ServiceModelData modelData = (ServiceModelData)values["ServiceModelData"];
                    object service = Activator.CreateInstance(typeof(OpenWebDataService<>).MakeGenericType(modelData.ServiceModelType));
                    service.GetType().GetMethod("AttachHost", BindingFlags.Public | BindingFlags.Instance).Invoke(service, new object[] { new TestServiceHost2() });
                    object context = ServiceModelData.InitializeAndGetContext(service);
                    object provider = ServiceModelData.CreateProvider(context, service);

                    if (!modelData.ContainerNames.Contains("Customers"))
                    {
                        return;
                    }

                    string customerNameProperty = modelData.IsUnitTestProvider ? "Name" : "CompanyName";
                    string integerProperty = modelData.IsUnitTestProvider ? "ID" : "1";

                    string[] predicates = new string[]
                {
                    "1 eq 1",
                    "1 add 1 eq 2",
                    "2 eq 1 add 1",
                    "3.5 eq 3 add 0.5",
                    "3.5 eq 3 add 0.5",
                    "1 add 2 mul 2 eq 5",
                    "(1 add 2) mul 2 eq 6",
                    "2 sub 1 eq 1",
                    "2 sub 1 add 1 ne 1",
                    "1 add 1 lt 5",
                    "1 add 1 le 2",
                    "1 add 1 ge 2",
                    "1 add 1 gt 1",
                    "1 lt " + Int64.MaxValue + "L",
                    "1 gt " + Int64.MinValue  + "L",
                    ///TODO: these 2 should succeed, (double, double, single) shouldn't get -1 from 
                    ///private static int CompareConversions(Type source, Type targetA, Type targetB) , this method needs some fixing.
                    ///"1 lt " + decimal.MaxValue + "00",
                    ///"1 gt " + decimal.MinValue + "00",
                    "not (1 add 1 eq 1)",
                    "1 eq 1 or 2 eq 1",
                    "1 eq 1 and 2 eq 1 add 1",
                    "1 eq 1 and not not (2 eq 1 add 1)",
                    "1 eq 0 sub -1",
                    "1 eq 0 sub - 1",
                    "0 sub 3 eq -(3)",
                    "'a' eq 'a'",
                    "'abc' eq 'abc'",
                    "'''' eq ''''",
                    "-" + integerProperty + " eq 1 sub 2",
                    customerNameProperty + " eq 'John' or 1 eq 1",
                    "10 div 2 eq 5",
                    "3 mod 2 eq 1",
                    "1 gt " + XmlConvert.ToString(10.1E-9),
                    "1 lt " + XmlConvert.ToString(10.1E+3),
                    "binary'12AABBff' eq Binary'12aabbFF'",
                    "binary'000102030405060708090A0B0C0D0E0F' eq BINARY'000102030405060708090a0b0c0d0e0f'",
                    "binary'12AABBff' ne binary'22aabbFF'",

                    // String functions.
                    "endswith('abc', 'bc')",
                    "startswith('abc', 'ab')",
                    "not startswith('abc', 'aab')",
                    "contains('abc','b')",
                    "indexof('abc', 'b') ge 1",
                    "replace('foo','o','g') eq 'fgg'",
                    "substring('123', 1) eq '23'",
                    "substring('123', 1, 1) eq '2'",
                    "tolower('ABC') eq 'abc'",
                    "toupper('AbC') eq 'ABC'",
                    "length(toupper('aBc')) eq length(tolower('aBc'))",
                    "concat('a', 'b') eq 'ab'",
                    "'a' lt 'b'",
                    "'a' le 'a'",
                    "'a' ge 'a'",
                    "'b' ge 'a'",
                    // "'a' add 'b' eq 'ab'",
                    // Repro Protocol: can't call date function in filter expressions on LinqToSql
                    "startswith(" + customerNameProperty + ", 'C')",
                    "endswith(concat(" + customerNameProperty + ", 'b'), 'b')",
                    "contains(" + customerNameProperty + ",'C')",
                    "trim(" + customerNameProperty + ") eq substring(" + customerNameProperty + ", 0)",

                    // DateTime functions.
                    "year(2007-08-08) eq 2007",
                    "month(2007-08-10) eq 08",
                    "day(2007-08-10) eq 10",
                    "hour(2007-08-10T14:11:12Z) eq 14",
                    "minute(2007-08-10T14:11:12Z) eq 11",
                    "second(2007-08-10T14:11:12Z) eq 12",

                    // Math functions.
                    "round(1.1) eq 1",
                    "round(42) eq 42",
                    "floor(1.1M) eq 1",
                    "ceiling(1.1f) eq 2",
                };

                    if (modelData.IsUnitTestProvider)
                    {
                        // Refer to some specific types.
                        string[] customDataContextPredicates = new string[]
                        {
                         //   "(BestFriend ne null and BestFriend/Name eq 'John') or 1 eq 1",
                            "GuidValue ne 5dc82c04-570a-41f5-b48f-c8a4e436f716",
                            "GuidValue ne 5dc82c04-570a-41f5-b48f-c8a4e436f716",
                            "GuidValue ne 5dc82c04-570a-41f5-b48f-c8a4e436f716 and GuidValue ne 5dc82c04-570a-41f5-b48f-c8a4e436f716",
                            // Type functions.
                            "ID eq cast(1, 'Edm.Int32')",
                            String.Format("not isof(1, '{0}')", UnitTestsUtil.ConvertTypeNames("AstoriaUnitTests.Stubs.Customer", context)),
                            String.Format("isof('{0}')", UnitTestsUtil.ConvertTypeNames("AstoriaUnitTests.Stubs.Customer", context)),
                            String.Format("isof('{0}') and cast('{0}')/Birthday gt 1960-01-01", UnitTestsUtil.ConvertTypeNames("AstoriaUnitTests.Stubs.CustomerWithBirthday", context))
                        };

                        if (!(context is CustomRowBasedOpenTypesContext))
                        {
                            predicates = predicates.Concat(new string[] {
                          "ID eq cast(1, 'Edm.Int64')",
                          "ID eq 1L"}).ToArray();
                        }

                        predicates = predicates.Concat(customDataContextPredicates).ToArray();
                    }
                    else if (context is NorthwindModel.NorthwindContext)
                    {
                        string[] northwindPredicates = new string[]
                    {
                        // Repro 602210 - Exception when filter expression contains null
                        "((((CustomerID) eq ('ALFKI')) and ((CustomerID) eq ('ANATR')))) or ((Region) ne (null))"
                    };
                        predicates = predicates.Concat(northwindPredicates).ToArray();
                    }
                    else if (context is AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext)
                    {
                        string[] sqlNorthwindPredicates = new string[]
                    {
                        // Repro Protocol: Can't compare properties to null in LinqToSql filter expression
                        "Categories|CategoryName eq null or 1 add 1 eq 2",
                        "Categories|null eq CategoryName or 1 add 1 eq 2",

                        // Repro Protocol: error trying to use Link property in filter expression (LinqToSql)
                        //"Categories|contains('Beer',CategoryName)",
                        //"Products|contains('Beer',Categories/CategoryName)",

                        // Repro Protocol: can't call date function in filter expressions on LinqToSql
                        //"Employees|month(HireDate) eq 5",
                    };
                        predicates = sqlNorthwindPredicates;
                        // predicates = predicates.Concat(sqlNorthwindPredicates).ToArray();
                    }

                    VerifyResultsExistForWhere(service, context, "Customers", predicates);
                });
            }
        }

        /// <summary></summary>
        /// <remarks>
        /// To get the Unicode code point for a character in hexadecimal notation in Microsoft Word,
        /// type the character and press Alt+X.
        /// </remarks>
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        [Ignore] // Remove Atom
        // [TestMethod]
        public void RequestQueryParserReproTests()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(TypedCustomDataContext<AllTypes>);
                string[] filters = new string[]
                {
                    // Protocol: Can't compare decimal value to literal greater than maxint
                    "DecimalType eq 100000000000000000000000M",

                    // Protocol: Can't use large Decimal in filter expression
                    "79228162514264337593543950335M gt 0",

                    // Protocol: Filter does not match large float values
                    "DoubleType eq 3.4E%2B38",

                   //(double)(3.4E+38f) will be 3.3999999521443642E+38

                    // Protocol: filter can't compare two byte properties
                    "ByteType eq ByteType",
                    "NullableByteType eq NullableByteType",
                    "ByteType eq (1 add 1 sub 2)",

                    // Protocol: contains in filter expression can not find single extended char
                    "null ne StringType and contains(StringType,'\x00A9 \x0040')",
                    "null ne StringType and contains(StringType,'\x00A9')"
                };

                TypedCustomDataContext<AllTypes>.ClearValues();
                TypedCustomDataContext<AllTypes>.ValuesRequested += (sender, y) =>
                {
                    AllTypes[] values = new AllTypes[]
                    {
                        new AllTypes() { ID = 1, DecimalType = 100000000000000000000000m },
                        new AllTypes() { ID = 2, DoubleType = 3.4E+38 },
                        new AllTypes() { ID = 3, StringType = "\x00A9 \x0040" },
                    };
                    TypedCustomDataContext<AllTypes> c = (TypedCustomDataContext<AllTypes>)sender;
                    c.SetValues(values);
                };

                foreach (string filter in filters)
                {
                    request.RequestUriString = "/Values?$format=atom&$filter=" + filter;
                    Trace.WriteLine("Sending request for " + request.RequestUriString);
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, false);
                    XmlDocument document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.AssertSelectNodes(document, "/atom:feed/atom:entry");
                }
            }
        }

        [TestMethod]
        public void RequestQueryParserTestNegativeTests()
        {
            // Repro Protocol: filter throws NYI when using null with arithmetic operators
            var service = new OpenWebDataService<CustomDataContext>();
            service.AttachHost(new TestServiceHost2());
            CustomDataContext context = ServiceModelData.InitializeAndGetContext(service);
            string[] predicates = new string[]
            {
                "1 eq 1 and not not 2 eq 1 add 1",
                "1 eq FakePropertyName",
                "1 eq '1'",
                "1 add",
                "'",
                "?",
                "1 add 1",
                "#",
                "endswith(123, 'abc')",
                "endswith('abc')",
                "endswith('abc', 123)",
                "0xaa eq 0xa",
                "0xaa eq 0xag",
                "null add 4 eq 1",
                "'a' add 'b' eq 'ab'",
                "endswith(Name add 'b', 'b')",
                "BestFriend add null",
            };

            // This way of getting the type is less ideal than getting it based on the
            // container name (see GetResourceTypeForContainer), because it does not work
            // for custom IDataServiceProvider implementations. In this case there seems to be
            // no other option.
            Prov.ResourceType customerType = GetResourceType(service, context.Customers.ElementType.FullName);
            object customerSet = GetResourceSetWrapper(service, "Customers");
            VerifyExceptionForWhere(service, customerSet, customerType, context.Customers, predicates);
        }

        public static void VerifyExceptionForWhere(object service, object resourceSet, Prov.ResourceType resourceType, IQueryable source, params string[] predicates)
        {
            foreach (string predicate in predicates)
            {
                Trace.WriteLine("Verifying exception is thrown for predicate [" + predicate + "]");

                Exception exception = TestUtil.RunCatching(delegate() { InvokeWhere(service, GetRequestDescription(resourceType, resourceSet, predicate), source, predicate); });
                TestUtil.AssertExceptionExpected(exception, true);
                // TestUtil.AssertContainsFalse(exception.ToString(), "binary operator");
                TestUtil.AssertContainsFalse(exception.ToString(), "mplemented");
            }
        }

        public static void VerifyResultsExistForWhere(object service, object context, string defaultSource, params string[] predicates)
        {
            IQueryable source = null;
            IServiceProvider serviceProvider = context as IServiceProvider;
            if (serviceProvider != null)
            {
                Prov.IDataServiceMetadataProvider dataProvider = (Prov.IDataServiceMetadataProvider)serviceProvider.GetService(typeof(Prov.IDataServiceMetadataProvider));
                if (dataProvider != null)
                {
                    Prov.ResourceSet resourceSet;
                    if (dataProvider.TryResolveResourceSet(defaultSource, out resourceSet))
                    {
                        Prov.IDataServiceQueryProvider queryProvider = (Prov.IDataServiceQueryProvider)serviceProvider.GetService(typeof(Prov.IDataServiceQueryProvider));
                        source = queryProvider.GetQueryRootForResourceSet(resourceSet);
                    }
                }
            }

            if (source == null)
            {
                source = (IQueryable)context.GetType().GetProperty(defaultSource).GetValue(context, null);
            }

            TestUtil.RunCombinations(predicates, (predicate) =>
            {
                IQueryable actualSource = source;
                string actualPredicate = predicate;
                int separatorIndex = predicate.IndexOf('|');
                string sourceName = defaultSource;
                if (separatorIndex != -1)
                {
                    sourceName = predicate.Substring(0, separatorIndex);
                    actualSource = (IQueryable)context.GetType().GetProperty(sourceName).GetValue(context, null);
                    actualPredicate = predicate.Substring(separatorIndex + 1);
                }

                Trace.WriteLine("Verifying results exist for predicate [" + actualPredicate + "]");

                Prov.ResourceType resourceType = GetResourceTypeForContainer(service, sourceName);
                object resourceSet = GetResourceSetWrapper(service, sourceName);
                Assert.IsNotNull(resourceType, "Should be able to find the container and its corresponding type");

                IQueryable result = InvokeWhere(service, GetRequestDescription(resourceType, resourceSet, defaultSource), actualSource, actualPredicate);
                bool found = false;
                foreach (object o in result)
                {
                    found = true;
                    break;
                }

                Assert.IsTrue(found, "Results not found for predicate [" + actualPredicate + "]");
            });
        }

        private static object GetRequestDescription(Prov.ResourceType resourceType, object resourceSet, string relativeUri)
        {
            // Create the segment info
            Type segmentInfoType = typeof(Prov.ResourceType).Assembly.GetType("Microsoft.OData.Service.SegmentInfo");
            var segmentInfo = Activator.CreateInstance(segmentInfoType, true /*nonPublic*/);
            segmentInfoType.GetProperty("TargetResourceType", BindingFlags.NonPublic | BindingFlags.Instance).GetSetMethod(true /*nonPublic*/).Invoke(segmentInfo, new object[] { resourceType });
            segmentInfoType.GetProperty("TargetResourceSet", BindingFlags.NonPublic | BindingFlags.Instance).GetSetMethod(true /*nonPublic*/).Invoke(segmentInfo, new object[] { resourceSet });

            // create segmentInfo array
            var segmentInfoArray = Array.CreateInstance(segmentInfoType, 1);
            segmentInfoArray.SetValue(segmentInfo, 0);

            // Create RequestDescription
            Type requestDescriptionType = typeof(Prov.ResourceType).Assembly.GetType("Microsoft.OData.Service.RequestDescription");
            return Activator.CreateInstance(
                requestDescriptionType,
                BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                null,
                new object[] { segmentInfoArray, new Uri(new Uri("http://host", UriKind.Absolute), relativeUri) },
                System.Globalization.CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void RequestQueryParserOperatorsReferences()
        {
            // AllTypesWithReferences
            var referenceTypes = new Type[] 
            { 
                typeof(object),                                 // an open property
                null                                            // a null literal
            };
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("Operator", OperatorData.Values),
                new Dimension("LeftType", referenceTypes),
                new Dimension("RightType", TypeData.Values));
            OpenTypeContextWithReflection<AllTypesWithReferences>.ClearHandlers();
            try
            {
                OpenTypeContextWithReflection<AllTypesWithReferences>.ValuesRequested += (x, y) =>
                {
                    ((OpenTypeContextWithReflection<AllTypesWithReferences>)x).SetValues(new object[] { new AllTypesWithReferences() });
                };

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(OpenTypeContextWithReflection<AllTypesWithReferences>);
                    TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                    {
                        Type left = (Type)values["LeftType"];
                        TypeData right = (TypeData)values["RightType"];

                        if (!right.IsTypeSupported)
                        {
                            return;
                        }

                        string leftName = AllTypesWithReferences.PropertyNameForReferenceType(left);
                        string rightName = AllTypesWithReferences.PropertyNameForType(right.ClrType);
                        OperatorData o = (OperatorData)values["Operator"];
                        string requestText = "/Values?$filter=" + leftName + "%20" + o.Token + "%20" + rightName;
                        request.RequestUriString = requestText;
                        Trace.WriteLine("Sending request " + requestText);
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        if (left == null)
                        {
                            // Anything can be compared to null (but only for equality, no ordering supported).
                            TestUtil.AssertExceptionExpected(exception, !o.IsEqualityComparison);
                        }
                        else if (left == typeof(object))
                        {
                            // Anything can be compared to an open property for anything at parse time.
                            // At execution time, ordering will only be supported for types which can be
                            // ordered as well.
                            //
                            // NOTE: because the sample values are null, reference checks will succeed at runtime.
                            //       (we normally would just fail this at parse time, but because these are reference
                            //       types and provides the open type comparer, it passes; that's probably OK, in case
                            //       a back-end does provider order for these types)
                            TestUtil.AssertExceptionExpected(exception,
                                !o.IsEqualityComparison && !right.IsOrderComparableTo(right)
                                && right.ClrType != typeof(byte[]) && right.ClrType != typeof(System.Data.Linq.Binary)
                                && right.ClrType != typeof(System.Xml.Linq.XElement)
                                && !(null != Nullable.GetUnderlyingType(right.ClrType)));
                        }
                    });
                }
            }
            finally
            {
                OpenTypeContextWithReflection<AllTypesWithReferences>.ClearHandlers();
            }
        }

        [TestMethod]
        public void RequestQueryParserOperatorsPrimitives()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("Operator", OperatorData.Values),
                new Dimension("LeftType", TypeData.Values),
                new Dimension("RightType", TypeData.Values));
            TypedCustomDataContext<AllTypes>.ClearValues();
            TypedCustomDataContext<AllTypes>.ClearHandlers();
            try
            {
                TypedCustomDataContext<AllTypes>.ValuesRequested += (x, y) =>
                {
                    ((TypedCustomDataContext<AllTypes>)x).SetValues(new object[] { new AllTypes() });
                };
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(TypedCustomDataContext<AllTypes>);
                    TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                    {
                        TypeData left = (TypeData)values["LeftType"];
                        TypeData right = (TypeData)values["RightType"];

                        if (!left.IsTypeSupported || !right.IsTypeSupported)
                        {
                            return;
                        }

                        // Big matrix. Let's cut it down by assuming that left and right ordering does not matter.
                        if (Array.IndexOf(TypeData.Values, left) > Array.IndexOf(TypeData.Values, right))
                        {
                            return;
                        }

                        string leftName = AllTypes.PropertyNameForType(left.ClrType);
                        string rightName = AllTypes.PropertyNameForType(right.ClrType);
                        OperatorData o = (OperatorData)values["Operator"];
                        string requestText = "/Values?$filter=" + leftName + "%20" + o.Token + "%20" + rightName;
                        request.RequestUriString = requestText;
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception,
                            o.IsEqualityComparison && !left.IsEqualityComparableTo(right),
                            o.IsOrderingComparison && !left.IsOrderComparableTo(right));
                    });
                }
            }
            finally
            {
                TypedCustomDataContext<AllTypes>.ClearValues();
                TypedCustomDataContext<AllTypes>.ClearHandlers();
            }
        }

        internal class OperatorData
        {
            private static OperatorData[] values;

            private OperatorData()
            {
            }

            public string Name { get; private set; }
            public string Token { get; private set; }

            public static OperatorData[] Values
            {
                get
                {
                    if (values == null)
                    {
                        values = new OperatorData[]
                        {
                            new OperatorData() { Name = "Equals", Token = "eq" },
                            new OperatorData() { Name = "NotEquals", Token = "ne" },
                            new OperatorData() { Name = "LessThan", Token = "lt" },
                            new OperatorData() { Name = "GreatherThan", Token = "gt" },
                            new OperatorData() { Name = "LessThanOrEqual", Token = "le" },
                            new OperatorData() { Name = "GreatherThanOrEqual", Token = "ge" },
                        };
                    }
                    return values;
                }
            }

            public bool IsEqualityComparison
            {
                get
                {
                    return this.Token == "eq" || this.Token == "ne";
                }
            }

            public bool IsOrderingComparison
            {
                get
                {
                    return this.Token == "lt" || this.Token == "gt" || this.Token == "le" || this.Token == "ge";
                }
            }

            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}
