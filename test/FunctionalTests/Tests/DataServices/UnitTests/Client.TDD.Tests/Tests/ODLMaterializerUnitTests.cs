//---------------------------------------------------------------------
// <copyright file="ODLMaterializerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using AstoriaUnitTests.TDD.Common;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataLib = Microsoft.OData.Core;

    [TestClass]
    public sealed class ODLMaterializerUnitTests
    {
        private const string ServiceUri = "http://localhost/foo/";
        private readonly Uri serviceUri = new Uri(ServiceUri);

        private DataServiceContext context;

        [TestInitialize]
        public void TestInitialize()
        {
            this.context = new DataServiceContext(this.serviceUri, Microsoft.OData.Client.ODataProtocolVersion.V4);
            this.context.EnableAtom = true;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.context = null;
        }

        [TestMethod]
        public void MaterializeEntityWithCollectionValueResolvedToEntity()
        {
            TestNegativeEntityMaterialization(
@"<entry xml:base='##BASEURI##' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
    <id>##BASEURI##Entity1(1)</id>
    <link rel='edit' title='Entity1' href='Entity1(1)' />
    <category term='TestInvalidPayloadEntity' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
        <m:properties>
            <d:ID>1</d:ID>
            <d:ComplexTypes>
                <m:element>
                    <d:ID>1</d:ID>
                </m:element>
            </d:ComplexTypes>
        </m:properties>
    </content>
</entry>",
                 new TestInvalidPayloadEntity
                 {
                     ID = 1,
                 },
                 (ex) =>
                 {
                     Assert.AreEqual(ODataLib.Strings.ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties("ComplexTypes", "AstoriaUnitTests.Tests.Client.TestInvalidPayloadEntity"), ex.Message);
                 }
             );
        }

        [TestMethod]
        public void MaterializeEntityWithComplexValueResolvedToEntity()
        {
            TestNegativeEntityMaterialization(
@"<entry xml:base='##BASEURI##' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
    <id>##BASEURI##Entity1(1)</id>
    <link rel='edit' title='Entity1' href='Entity1(1)' />
    <category term='TestInvalidPayloadEntity' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
        <m:properties>
            <d:ID>1</d:ID>
            <d:ComplexType>
                <d:ID>1</d:ID>
            </d:ComplexType>
        </m:properties>
    </content>
</entry>",
                 new TestInvalidPayloadEntity
                 {
                     ID = 1,
                 },
                 (ex) =>
                 {
                     Assert.AreEqual(ODataLib.Strings.ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties("ComplexType", "AstoriaUnitTests.Tests.Client.TestInvalidPayloadEntity"), ex.Message);
                 }
             );
        }

        [TestMethod]
        public void MaterializeEntityWithEditLinkAfterNavLink()
        {
            TestPositiveEntityMaterialization(@"<entry xml:base='##BASEURI##' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom' m:etag='W/&quot;1081496382&quot;'>
    <link rel='http://docs.oasis-open.org/odata/ns/related/ReferenceSet' type='application/atom+xml;type=feed' title='ReferenceSet' href='Customer(1)/ReferenceSet'/>
    <category term='Entity1' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
        <m:properties>
            <d:ID>1</d:ID>
            <d:StringValue>abc</d:StringValue>
            <d:DateTimeOffsetValue m:type='Edm.DateTime'>2004-10-13T00:00:00Z</d:DateTimeOffsetValue>
        </m:properties>
    </content>
    <id>##BASEURI##Entity1(1)</id>
    <link rel='edit' title='Entity1' href='Entity1(1)' />
    <link rel='self' title='Entity1' href='Entity1(1)' />
</entry>",
                 new Entity1
                 {
                     ID = 1,
                     StringValue = "abc",
                     DateTimeOffsetValue = new DateTime(2004, 10, 13, 0, 0, 0, DateTimeKind.Utc)
                 });
            Assert.AreEqual(ServiceUri + "Entity1(1)", this.context.Entities.First().Identity.OriginalString);
            Assert.AreEqual(ServiceUri + "Entity1(1)", this.context.Entities.First().EditLink.OriginalString);
            Assert.AreEqual(ServiceUri + "Entity1(1)", this.context.Entities.First().SelfLink.OriginalString);
            Assert.AreEqual("W/\"1081496382\"", this.context.Entities.First().ETag);
        }

        [TestMethod]
        public void MaterializeEntity_NullAsNonNullableEnum_Error()
        {
            TestNegativeEntityMaterialization(@"<entry xml:base='##BASEURI##' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom' m:etag='W/&quot;1081496382&quot;'>
    <link rel='http://docs.oasis-open.org/odata/ns/related/ReferenceSet' type='application/atom+xml;type=feed' title='ReferenceSet' href='Customer(1)/ReferenceSet'/>
    <category term='Entity1' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
        <m:properties>
            <d:ID>1</d:ID>
            <d:MyFlagsColorValue m:null=""true""/>
            <d:StringValue>abc</d:StringValue>
            <d:DateTimeOffsetValue m:type='Edm.DateTime'>2004-10-13T00:00:00Z</d:DateTimeOffsetValue>
        </m:properties>
    </content>
    <id>##BASEURI##Entity1(1)</id>
    <link rel='edit' title='Entity1' href='Entity1(1)' />
    <link rel='self' title='Entity1' href='Entity1(1)' />
</entry>",
                 new Entity1
                 {
                     ID = 1,
                     MyColorValue = null,
                     MyFlagsColorValue = MyFlagsColor.Blue,
                     StringValue = "abc",
                     DateTimeOffsetValue = new DateTime(2004, 10, 13, 0, 0, 0, DateTimeKind.Utc)
                 },
                 (ex) =>
                 {
                     Assert.AreEqual(ODataLib.Strings.ReaderValidationUtils_NullNamedValueForNonNullableType("MyFlagsColorValue", "AstoriaUnitTests.Tests.Client.MyFlagsColor"), ex.Message);
                 });
        }

        [TestMethod]
        public void MaterializeEntity_NullAsNullableEnum()
        {
            TestPositiveEntityMaterialization(@"<entry xml:base='##BASEURI##' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom' m:etag='W/&quot;1081496382&quot;'>
    <link rel='http://docs.oasis-open.org/odata/ns/related/ReferenceSet' type='application/atom+xml;type=feed' title='ReferenceSet' href='Customer(1)/ReferenceSet'/>
    <category term='Entity1' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
        <m:properties>
            <d:ID>1</d:ID>
            <d:MyFlagsColorValue>Blue</d:MyFlagsColorValue>
            <d:StringValue>abc</d:StringValue>
            <d:DateTimeOffsetValue m:type='Edm.DateTime'>2004-10-13T00:00:00Z</d:DateTimeOffsetValue>
        </m:properties>
    </content>
    <id>##BASEURI##Entity1(1)</id>
    <link rel='edit' title='Entity1' href='Entity1(1)' />
    <link rel='self' title='Entity1' href='Entity1(1)' />
</entry>",
                 new Entity1
                 {
                     ID = 1,
                     MyColorValue = null,
                     MyFlagsColorValue = MyFlagsColor.Blue,
                     StringValue = "abc",
                     DateTimeOffsetValue = new DateTime(2004, 10, 13, 0, 0, 0, DateTimeKind.Utc)
                 });
        }

        [TestMethod]
        public void MaterializeEntity_ValueAsNullableEnum()
        {
            TestPositiveEntityMaterialization(@"<entry xml:base='##BASEURI##' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom' m:etag='W/&quot;1081496382&quot;'>
    <link rel='http://docs.oasis-open.org/odata/ns/related/ReferenceSet' type='application/atom+xml;type=feed' title='ReferenceSet' href='Customer(1)/ReferenceSet'/>
    <category term='Entity1' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
        <m:properties>
            <d:ID>1</d:ID>Yellow
            <d:MyColorValue>Yellow</d:MyColorValue>
            <d:MyFlagsColorValue>Blue</d:MyFlagsColorValue>
            <d:StringValue>abc</d:StringValue>
            <d:DateTimeOffsetValue m:type='Edm.DateTime'>2004-10-13T00:00:00Z</d:DateTimeOffsetValue>
        </m:properties>
    </content>
    <id>##BASEURI##Entity1(1)</id>
    <link rel='edit' title='Entity1' href='Entity1(1)' />
    <link rel='self' title='Entity1' href='Entity1(1)' />
</entry>",
                 new Entity1
                 {
                     ID = 1,
                     MyColorValue = MyColor.Yellow,
                     MyFlagsColorValue = MyFlagsColor.Blue,
                     StringValue = "abc",
                     DateTimeOffsetValue = new DateTime(2004, 10, 13, 0, 0, 0, DateTimeKind.Utc)
                 });
        }

        [TestMethod]
        public void MaterializeEntityComplex_ValueAsNullableEnum_ValueAsNonNullableEnum()
        {
            TestPositiveEntityMaterialization(
@"<entry xml:base='##BASEURI##' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
    <id>##BASEURI##Entity1(1)</id>
    <link rel='edit' title='Entity1' href='Entity1(1)' />
    <category term='Entity1' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
        <m:properties>
            <d:ID>1</d:ID>
            <d:ComplexValue1>
                <d:MyColorValue>Green</d:MyColorValue>
                <d:MyFlagsColorValue>Red</d:MyFlagsColorValue>
                <d:StringValue>myStrValue</d:StringValue>
            </d:ComplexValue1>
        </m:properties>
    </content>
</entry>",
                 new Entity1
                 {
                     ID = 1,
                     ComplexValue1 = new ComplexValue1()
                     {
                         MyColorValue = MyColor.Green,
                         MyFlagsColorValue = MyFlagsColor.Red,
                         StringValue = "myStrValue"
                     }
                 });
        }

        [TestMethod]
        public void MaterializeEntityCollection_ValueAsNonNullableEnum()
        {
            TestPositiveEntityMaterialization(
@"<entry xml:base='##BASEURI##' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
    <id>##BASEURI##Entity1(1)</id>
    <link rel='edit' title='Entity1' href='Entity1(1)' />
    <category term='Entity1' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
        <m:properties>
            <d:ID>1</d:ID>
            <d:MyFlagsColorCollection1 d:type=""Collection(MyFlagsColorValue)"">
                <m:element>Blue</m:element>
                <m:element>Red</m:element>
                <m:element>Blue</m:element>
            </d:MyFlagsColorCollection1>
        </m:properties>
    </content>
</entry>",
                 new Entity1
                 {
                     ID = 1,
                     MyFlagsColorCollection1 = new List<MyFlagsColor>() { MyFlagsColor.Blue, MyFlagsColor.Red, MyFlagsColor.Blue },
                 });
        }

        [TestMethod]
        public void MaterializeTopLevelProperty_ValueAsNonNullableEnum()
        {
            TestPositiveTopLevelMaterialization(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"" 
m:context=""http://chhodata2:9090/ODL635211743018882142/OData/$metadata#AstoriaUnitTests.Tests.Client.MyFlagsColor"" 
m:type=""#AstoriaUnitTests.Tests.Client.MyFlagsColor"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  Blue
</m:value>",
                 MyFlagsColor.Blue);
        }

        // TODO: toplevel property value as enum

        private void TestPositiveTopLevelMaterialization<T>(string content, T value, Func<T, T, bool> equalsPredicate = null)
        {
            TestPositiveMaterialization<T>(content, TestConstants.MimeApplicationXml, // TestConstants.MimeApplicationXml: top level "<m:value> ...<>" is not Atom
                materializer =>
                {
                    Assert.IsTrue(object.ReferenceEquals(materializer.Current, null) || materializer.Current is T);
                    Assert.IsTrue(equalsPredicate == null ? object.Equals(value, materializer.Current) : equalsPredicate(value, (T)materializer.Current));
                });
        }

        private void TestPositiveEntityMaterialization<T>(string content, T value, Func<T, T, bool> equalsPredicate = null)
        {
            TestPositiveMaterialization<T>(content, TestConstants.MimeApplicationAtom,
                materializer =>
                {
                    Assert.IsTrue(object.ReferenceEquals(materializer.Current, null) || materializer.Current is T);
                    Assert.IsTrue(equalsPredicate == null ? object.Equals(value, materializer.Current) : equalsPredicate(value, (T)materializer.Current));
                });
        }

        private void TestNegativeEntityMaterialization<T>(string content, T value, Action<Exception> exceptionAction)
        {
            TestNegativeMaterialization<T>(content, TestConstants.MimeApplicationAtom, exceptionAction);
        }

        private void TestPositiveMaterialization<T>(string content, string contentType, Action<MaterializeAtom> testAction)
        {
            TestMaterialization<T>(content, contentType,
                (materializer) =>
                {
                    Assert.IsTrue(materializer.MoveNext());
                    testAction(materializer);
                    Assert.IsTrue(!materializer.MoveNext());
                });
        }

        private void TestNegativeMaterialization<T>(string content, string contentType, Action<Exception> testAction)
        {
            TestMaterialization<T>(content, contentType,
                (materializer) =>
                {
                    bool gotException = false;
                    try
                    {
                        materializer.MoveNext();
                        Assert.Fail("Did not expect to get this far");
                    }
                    catch (Exception e)
                    {
                        gotException = true;
                        if (testAction != null)
                        {
                            testAction(e);
                        }
                    }
                    Assert.IsTrue(gotException);
                });
        }

        private void TestMaterialization<T>(string content, string contentType, Action<MaterializeAtom> testAction, ProjectionPlan plan = null)
        {
            content = content.Replace("##BASEURI##", ServiceUri);

            using (MaterializeAtom materializer = CreateMaterializer<T>(content, contentType, plan))
            {
                testAction(materializer);
            }
        }

        private MaterializeAtom CreateMaterializer<T>(string content, string contentType, ProjectionPlan plan)
        {
            ResponseInfo info = new ResponseInfo(new RequestInfo(this.context), MergeOption.AppendOnly);
            QueryComponents qc = new QueryComponents(this.serviceUri, Util.ODataVersion4, typeof(T), null, null);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(XmlConstants.HttpContentType, contentType);
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            TestResponseMessage responseMessage = new TestResponseMessage(headers, HttpStatusCode.OK, () => new System.IO.MemoryStream(bytes));
            return new MaterializeAtom(info, qc, plan, responseMessage, ODataPayloadKind.Unsupported);
        }

        internal static bool CompareCollection<T>(ICollection<T> left, ICollection<T> right, Func<T, T, bool> comparePredicate)
        {
            if (left == null && right == null)
            {
                return true;
            }

            return (left.Count == right.Count) && left.Zip<T, T, bool>(right, (l, r) => comparePredicate(l, r)).All(b => b);
        }

        /// <summary>
        /// IODataResponseMessage interface implementation
        /// </summary>
        class TestResponseMessage : IODataResponseMessage
        {
            /// <summary>Cached headers.</summary>
            private Dictionary<string, string> headers;

            private HttpStatusCode statusCode;

            /// <summary>A func which returns the response stream.</summary>
            private Func<Stream> getResponseStream;

#if DEBUG
            /// <summary>set to true once the GetStream was called.</summary>
            private bool streamReturned;
#endif

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="httpResponse">The response object to wrap.</param>
            /// <param name="getResponseStream">A func which returns the response stream.</param>
            internal TestResponseMessage(Dictionary<string, string> headers, HttpStatusCode statusCode, Func<Stream> getResponseStream)
            {
                Debug.Assert(headers != null, "headers != null");
                Debug.Assert(getResponseStream != null, "getResponseStream != null");

                this.headers = headers;
                this.statusCode = statusCode;
                this.getResponseStream = getResponseStream;
            }

            /// <summary>
            /// Returns the collection of response headers.
            /// </summary>
            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get
                {
                    return this.headers;
                }
            }

            /// <summary>
            /// The response status code.
            /// </summary>
            public int StatusCode
            {
                get
                {
                    return (int)this.statusCode;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Returns the value of the header with the given name.
            /// </summary>
            /// <param name="headerName">Name of the header.</param>
            /// <returns>Returns the value of the header with the given name.</returns>
            public string GetHeader(string headerName)
            {
                if (this.headers.ContainsKey(headerName))
                {
                    return this.headers[headerName];
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Sets the value of the header with the given name.
            /// </summary>
            /// <param name="headerName">Name of the header.</param>
            /// <param name="headerValue">Value of the header.</param>
            public void SetHeader(string headerName, string headerValue)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the stream to be used to read the response payload.
            /// </summary>
            /// <returns>Stream from which the response payload can be read.</returns>
            public Stream GetStream()
            {
#if DEBUG
                Debug.Assert(!this.streamReturned, "The GetStream can only be called once.");
                this.streamReturned = true;
#endif

                Stream responseStream = this.getResponseStream();
                Debug.Assert(responseStream != null, "The func to get the response stream returned null.");
                return responseStream;
            }
        }
    }

    public enum MyColor : long
    {
        Green = 1,
        Yellow = 4
    }

    [Flags]
    public enum MyFlagsColor
    {
        Red = 2,
        Blue = 8
    }

    public class ComplexValue1
    {
        public MyColor? MyColorValue { get; set; }
        public MyFlagsColor MyFlagsColorValue { get; set; }
        public string StringValue { get; set; }

        internal static bool Compare(ComplexValue1 left, ComplexValue1 right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            else if (left == null || right == null)
            {
                return false;
            }
            else
            {
                return left.StringValue == right.StringValue
                    && left.MyFlagsColorValue == right.MyFlagsColorValue && left.MyColorValue == right.MyColorValue;
            }
        }
    }

    public class ComplexValue2
    {
        public string StringValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public ComplexValue1 ComplexValue1 { get; set; }

        internal static bool Compare(ComplexValue2 left, ComplexValue2 right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            else if (left == null || right == null)
            {
                return false;
            }
            else
            {
                return left.StringValue == right.StringValue && left.DateTimeValue == right.DateTimeValue && ComplexValue1.Compare(left.ComplexValue1, right.ComplexValue1);
            }
        }
    }

    public class Entity1
    {
        public Entity1()
        {
            this.ReferenceSet = new List<Entity2>();
        }

        public int ID { get; set; }

        public MyColor? MyColorValue { get; set; }

        public MyFlagsColor MyFlagsColorValue { get; set; }

        public string StringValue { get; set; }

        public DateTimeOffset DateTimeOffsetValue { get; set; }

        public ComplexValue1 ComplexValue1 { get; set; }

        public List<MyFlagsColor> MyFlagsColorCollection1 { get; set; }

        public Entity1 SelfReference { get; set; }

        public Entity2 SingleReference { get; set; }

        public ICollection<Entity2> ReferenceSet { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as Entity1);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        internal static bool Equals(Entity1 left, Entity1 right, bool shallow = false)
        {
            if (left == null && right == null)
            {
                return true;
            }
            else if (left == null || right == null)
            {
                return false;
            }
            else if (shallow)
            {
                return left.ID == right.ID;
            }
            else
            {
                return
                    left.ID == right.ID &&
                    left.MyColorValue == right.MyColorValue &&
                    left.MyFlagsColorValue == right.MyFlagsColorValue &&
                    left.StringValue == right.StringValue &&
                    ODLMaterializerUnitTests.CompareCollection(left.MyFlagsColorCollection1, right.MyFlagsColorCollection1, (l, r) => (Entity2.Equals(l, r))) &&
                    left.DateTimeOffsetValue == right.DateTimeOffsetValue &&
                    ComplexValue1.Compare(left.ComplexValue1, right.ComplexValue1) &&
                    Entity2.Equals(left.SingleReference, right.SingleReference) &&
                    ODLMaterializerUnitTests.CompareCollection(left.ReferenceSet, right.ReferenceSet, (l, r) => (Entity2.Equals(l, r))) &&
                    Entity1.Equals(left.SelfReference, right.SelfReference, true);
            }
        }
    }

    public class Entity2
    {
        public int ID { get; set; }

        public string StringValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as Entity2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        internal static bool Equals(Entity2 left, Entity2 right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            else if (left == null || right == null)
            {
                return false;
            }
            else
            {
                return left.ID == right.ID && left.StringValue == right.StringValue && left.DateTimeValue == right.DateTimeValue;
            }
        }
    }

    public class FooBase
    {
        public FooBase()
        {
        }

        public int ID { get; set; }

        public string StringValue { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as FooBase);
        }

        public override int GetHashCode()
        {
            return unchecked(this.ID >> 4 + (this.StringValue == null ? 0 : this.StringValue.GetHashCode()));
        }

        protected static bool Equals(FooBase left, FooBase right)
        {
            return object.ReferenceEquals(left, right) ||
                (left != null && right != null && left.GetType() == right.GetType() && left.ID == right.ID && left.StringValue == right.StringValue);
        }
    }

    public class BarBase
    {
        public BarBase()
        {
        }

        public int ID { get; set; }

        public string StringValue { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as BarBase);
        }

        public override int GetHashCode()
        {
            return unchecked(this.ID >> 4 + (this.StringValue == null ? 0 : this.StringValue.GetHashCode()));
        }

        protected static bool Equals(BarBase left, BarBase right)
        {
            return object.ReferenceEquals(left, right) ||
                (left != null && right != null && left.GetType() == right.GetType() && left.ID == right.ID && left.StringValue == right.StringValue);
        }
    }

    public class TestInvalidPayloadEntity
    {
        public TestInvalidPayloadEntity()
        {
        }

        public int ID { get; set; }

        public InvalidComplexType ComplexType { get; set; }

        public ICollection<InvalidComplexType> ComplexTypes { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as TestInvalidPayloadEntity);
        }

        public bool Equals(TestInvalidPayloadEntity left, TestInvalidPayloadEntity right)
        {
            return object.ReferenceEquals(left, right) ||
                left != null && right != null && left.ID == right.ID && object.Equals(left.ComplexType, right.ComplexType) &&
                ODLMaterializerUnitTests.CompareCollection(left.ComplexTypes, right.ComplexTypes, (l, r) => object.Equals(l, r));
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }

    public class InvalidComplexType
    {
        public int ID { get; set; }

        public override bool Equals(object obj)
        {
            InvalidComplexType complexType = obj as InvalidComplexType;

            return complexType != null && this.ID == complexType.ID;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}
