//---------------------------------------------------------------------
// <copyright file="CollectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.DataWebClientCSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #region Types used in tests

    public class ComplexType
    {
        public string Description { get; set; }
        public int Number { get; set; }
    }

    public class DerivedComplexType : ComplexType
    {
        public ICollection<DerivedComplexType> CollectionOfComplexTypes { get; set; }
        public string Summary { get; set; }
    }

    public class UnrelatedComplexType
    {
        public string Description { get; set; }
        public string AnotherDescription { get; set; }
    }

    public class BaseEntity
    {
        public int ID { get; set; }
    }

    public class ComplexTypeWithCollection<T, U>
        where T : ICollection<U>
    {
        protected T collection;

        public ComplexTypeWithCollection()
        {
            if (InitialCollectionValues != null)
            {
                InitializeCollectionValues(InitialCollectionValues);
            }
        }

        public ComplexTypeWithCollection(IEnumerable collection)
        {
            InitializeCollectionValues(collection);
        }

        public virtual void InitializeCollectionValues(IEnumerable collection)
        {
            if (collection != null)
            {
                Type collectionType = typeof(T);

                if (collectionType.IsInterface)
                {
                    if (collectionType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        collectionType = typeof(Collection<U>);
                    }
                    else if (collectionType.GetGenericTypeDefinition() == typeof(IList<>))
                    {
                        collectionType = typeof(List<U>);
                    }
                    else
                    {
                        Assert.Fail("Unexpected collection type");
                    }
                }

                Collection = (T)Activator.CreateInstance(collectionType);

                foreach (var item in collection)
                {
                    Collection.Add((U)item);
                }

                // Clear log for LoggingCollection
                if (collectionType.GetGenericTypeDefinition() == typeof(LoggingCollection<>))
                {
                    ((List<string>)collectionType.GetProperty("Log").GetValue(Collection, null)).Clear();
                }
            }
            else
            {
                Collection = default(T);
            }
        }

        public static IEnumerable InitialCollectionValues { private get; set; }

        public virtual T Collection
        {
            get { return collection; }
            set { collection = value; }
        }

        public string Description { get; set; }
    }

    public class Entity<T, U> : ComplexTypeWithCollection<T, U>
        where T : ICollection<U>
    {
        public Entity() :
            base()
        { }

        public Entity(IEnumerable collection) :
            base(collection)
        { }

        public int ID { get; set; }
    }


    public class MaterializationTestComplextTypeWithCollection<T, U> : ComplexTypeWithCollection<T, U>
        where T : ICollection<U>
    {
        public MaterializationTestComplextTypeWithCollection()
            : base()
        { }

        public override T Collection
        {
            get { return collection; }
            set
            {
                Assert.AreEqual(0, value.Count, "Collection being set is not emtpy. Materialization should first set the collection and then add elements.");

                // Remove log entry added by the above assert check
                if (typeof(T).GetGenericTypeDefinition() == typeof(LoggingCollection<>))
                {
                    List<string> log = (List<string>)typeof(T).GetProperty("Log").GetValue(value, null);
                    log.RemoveAt(log.Count - 1);
                }

                collection = value;
            }
        }

        public MyObject Object { get; set; }
        public MyObject InitializedObject { get; set; }
        public DataServiceCollection<U> DataServiceCollection { get; set; }
    }

    public class MaterializationTestsEntity<T, U> : MaterializationTestComplextTypeWithCollection<T, U>
        where T : ICollection<U>
    {
        public int ID { get; set; }

        public MaterializationTestsEntity()
            : base()
        {
            InitializedObject = new MyObject();
        }
    }

    public class MyObject
    {
        public string Prop { get; set; }
    }

    public class MaterializationTestsEntityWithComplexTypeWithCollection<T, U>
        where T : ICollection<U>
    {
        public int ID { get; set; }
        public MaterializationTestComplextTypeWithCollection<T, U> Complex { get; set; }

        public static IEnumerable InitialCollectionValues
        {
            set
            {
                MaterializationTestComplextTypeWithCollection<T, U>.InitialCollectionValues = value;
            }
        }
    }

    public class LoggingCollection<T> : ICollection<T>
    {
        private Collection<T> storage = new Collection<T>();
        private List<string> log = new List<string>();

        public void Add(T item)
        {
            log.Add("Add");
            storage.Add(item);
        }

        public void Clear()
        {
            log.Add("Clear");
            storage.Clear();
        }

        public bool Contains(T item)
        {
            log.Add("Contains");
            return storage.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            log.Add("CopyTo");
            storage.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                log.Add("Count");
                return storage.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                log.Add("IsReadOnly");
                return false;
            }
        }

        public bool Remove(T item)
        {
            log.Add("Remove");
            return storage.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            log.Add("GetEnumerator");
            return storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            log.Add("IEnumerable.GetEnumerator");
            return storage.GetEnumerator();
        }

        public List<string> Log { get { return log; } }
    }

    public class CustomCollection<T, U> : List<U>
    {
        public T Property { get; set; }
    }

    #endregion

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    /// <summary>
    /// Tests class for collection on the client
    /// </summary>
    [TestClass]
    public class Collection
    {
        private static readonly XName CollectionElementXName = UnitTestsUtil.MetadataNamespace + "element";
        private static readonly XName CollectionPropertyElementName = UnitTestsUtil.DataNamespace + "CollectionProperty";
        private static readonly XName TypeAttributeName = UnitTestsUtil.MetadataNamespace + "type";

        #region CollectionWireTypeNameParsing
        [TestMethod]
        public void CollectionWireTypeNameParsing()
        {
            // Verify parsing of wire type name that now has to recognize collection.
            Action<string, string>[] testActions = new Action<string, string>[] {
                (wireTypeName, expectedCollectionTypeName) => {
                    object isCollection = InvokeWebUtilMethod("IsWireTypeCollection", new object[] { wireTypeName });
                    Assert.AreEqual(expectedCollectionTypeName != null, (bool)isCollection);
                },
                (wireTypeName, expectedCollectionTypeName) => {
                    object typeName = InvokeWebUtilMethod("GetCollectionItemWireTypeName", new object[] { wireTypeName });
                    Assert.AreEqual(expectedCollectionTypeName, (string)typeName);
                },
            };

            var testCases = new[] {
                new {
                    WireTypeName = string.Empty,
                    ExpectedCollectionTypeName = (string)null,
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "abc",
                    ExpectedCollectionTypeName = (string)null,
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection",
                    ExpectedCollectionTypeName = (string)null,
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection(",
                    ExpectedCollectionTypeName = (string)null,
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection()",
                    ExpectedCollectionTypeName = (string)null,
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = " Collection(Edm.Int32)",
                    ExpectedCollectionTypeName = (string)null,
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection(Edm.Int32) ",
                    ExpectedCollectionTypeName = (string)null,
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection (Edm.Int32)",
                    ExpectedCollectionTypeName = (string)null,
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection(Edm.Int32)",
                    ExpectedCollectionTypeName = "Edm.Int32",
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection(Edm.Int32 )",
                    ExpectedCollectionTypeName = "Edm.Int32 ",
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection( Edm.Int32)",
                    ExpectedCollectionTypeName = " Edm.Int32",
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection( Edm.Int32 )",
                    ExpectedCollectionTypeName = " Edm.Int32 ",
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection( )",
                    ExpectedCollectionTypeName = " ",
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection(Collection())",
                    ExpectedCollectionTypeName = "Collection()",
                    ExceptionExpected = false
                },
                new {
                    WireTypeName = "Collection(Collection(Edm.Int32))",
                    ExpectedCollectionTypeName = "nothing expected - exception should be thrown",
                    ExceptionExpected = true
                }
            };

            TestUtil.RunCombinations(testCases, testActions, (testCase, testAction) =>
            {
                try
                {
                    testAction(testCase.WireTypeName, testCase.ExpectedCollectionTypeName);
                }
                catch (InvalidOperationException ex)
                {
                    if (testCase.ExceptionExpected)
                    {
                        Assert.AreEqual("Collection properties of a collection type are not supported.", ex.Message);
                    }
                    else
                    {
                        throw;
                    }
                }
            });
        }
        #endregion CollectionWireTypeNameParsing

        #region Materialization

        #region Helper class for materialization test cases - useful for cases when the exception may differ depending on options

        // the client may throw slightly different exception in projection cases.
        public class MaterializationTestCase
        {
            private Exception alternateException;
            private bool? isProjection;
            private bool? typeAttributePresent;
            private bool? loadProperty;

            public Type CollectionItemType { get; set; }
            public string CollectionPropertyPayload { get; set; }
            public Exception ExpectedException { private get; set; }
            public Exception ComplexExpectedException { private get; set; }
            public IEnumerable ValuesToInitializeCollection { get; set; }
            public string PropertyName { get; set; }

            public MaterializationTestCase(Exception alternateException = null, bool? isProjection = null, bool? typeAttributePresent = null, bool? loadProperty = null)
            {
                this.isProjection = isProjection;
                this.typeAttributePresent = typeAttributePresent;
                this.alternateException = alternateException;
                this.loadProperty = loadProperty;
            }

            // the client may throw slightly different exception in projection cases 
            public Exception GetException(bool isProjection, bool typeAttributePresent, bool loadProperty, bool isComplexTypeTesting)
            {
                // alternateException can be returned only if any of the setting is not null. Settings set to null mean return the alternate exception regardless of the value
                if ((this.isProjection.HasValue || this.typeAttributePresent.HasValue || this.loadProperty.HasValue) &&
                    (!this.isProjection.HasValue || this.isProjection == isProjection) &&
                    (!this.typeAttributePresent.HasValue || this.typeAttributePresent == typeAttributePresent) &&
                    (!this.loadProperty.HasValue || this.loadProperty == loadProperty))
                {
                    return this.alternateException;
                }
                else
                {
                    if (!isComplexTypeTesting)
                    {
                        return this.ExpectedException;
                    }
                    else
                    {
                        return this.ComplexExpectedException ?? this.ExpectedException;
                    }
                }
            }
        }
        #endregion

        #region test cases

        private static MaterializationTestCase[] testCasesWithoutExceptions = new MaterializationTestCase[] {
        // collection of primitive types 
            //positive cases
            new MaterializationTestCase() { // materializing a collection of integer values - should pass
                CollectionItemType = typeof(int),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>3</m:element>
                        <m:element>2</m:element>
                        <m:element>1</m:element>
                        <m:element>1</m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new int[] { 100, 101, 200 },
                PropertyName = "Collection",
            },
            new MaterializationTestCase() { // materializing a collection of nullable integer values - should pass
                CollectionItemType = typeof(int?),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>3</m:element>
                        <m:element>2</m:element>
                        <m:element>1</m:element>
                        <m:element>1</m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new int?[] { 100, 101, 200 },
                PropertyName = "Collection",
            },
            new MaterializationTestCase() { // Materializing a collection of string values - should pass
                CollectionItemType = typeof(string),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.String)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>abc</m:element>
                        <m:element></m:element>
                        <m:element>123abc</m:element>
                        <m:element />
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new string[] { },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // Materializing an empty collection - should pass
                CollectionItemType = typeof(string),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.String)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" />",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new string[] { "x", string.Empty, "z" },
                PropertyName = "Collection",
            },
            new MaterializationTestCase() { // materializing a collection of integer values with a specified type on an element - should pass (the type should be ignored for primitive types)
                CollectionItemType = typeof(int?),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>3</m:element>
                        <m:element m:Type=""Emd.String"">2</m:element>
                        <m:element>1</m:element>
                        <m:element>1</m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new int?[] { 100, 101, 200 },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of integer values where there are some extraneous elements in non-Astoria namespace - should pass
                CollectionItemType = typeof(int),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>3</m:element>
                        <m:element>2</m:element>
                        <m:element>1</m:element>
                        <m:element>1</m:element>
                        <m:element xmlns="""">12</m:element>
                        <foo xmlns="""">21</foo>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new int[] { 100, 101, 200 },
                 PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of nullable integer values to a collection of non-nullable integers
                CollectionItemType = typeof(int),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(NullableInteger)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>3</m:element>
                        <m:element>2</m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new int[] { 3, 2 },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of complex types - should pass
                CollectionItemType = typeof(ComplexType),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(AstoriaUnitTests.DataWebClientCSharp.ComplexType)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>
                            <Description>TestDescription</Description>
                            <Number>123</Number>
                        </m:element>
                        <m:element>
                            <Number>987</Number>
                        </m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new ComplexType[] { new ComplexType() { Description = "abcdef", Number = 100} },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // verify that outer Collection() will be passed to the type resolver - should pass if type resolver returns typeof(DerivedComplexType) for "Collection()"
                CollectionItemType = typeof(DerivedComplexType),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(Collection())"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>
                            <Description>TestDescription</Description>
                            <Number>123</Number>
                            <Summary>Summary</Summary>
                        </m:element>
                        <m:element>
                            <Number>987</Number>
                            <Summary>Another Summary</Summary>
                        </m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new DerivedComplexType[] { new DerivedComplexType() { Description = "asdf", Number = 365 } },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // A collection of ComplexTypes that have collection - should pass
                CollectionItemType = typeof(DerivedComplexType),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(DerivedComplexType)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>
                            <Description>1st Item</Description>
                            <Number>1</Number>
                            <Summary>Collection 1</Summary>
                            <CollectionOfComplexTypes m:type=""Collection(DerivedComplexType)"">
                                <m:element>
                                    <Description>1.1</Description>
                                    <Summary>1st Item in the 1st nested collection</Summary>
                                    <Number>1</Number>
                                </m:element>
                                <m:element>
                                    <Summary>2nd Item in the 1st nested collection</Summary>
                                    <Description>1.2</Description>
                                </m:element>
                            </CollectionOfComplexTypes>
                        </m:element>
                        <m:element>            
                            <Description>2nd Item</Description>
                            <CollectionOfComplexTypes m:type=""Collection(DerivedComplexType)"">
                                <m:element>
                                    <Summary>1st Item in the 2nd nested collection</Summary>
                                    <Description>2.1</Description>
                                </m:element>
                                <m:element>
                                    <Summary>2nd Item in the 2nd nested collection</Summary>
                                    <Description>2.2</Description>
                                </m:element>
                                <m:element>
                                    <Summary>3rd Item in the 2nd nested collection</Summary>
                                    <Description>2.3</Description>
                                </m:element>
                            </CollectionOfComplexTypes>
                        </m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new DerivedComplexType[] { new DerivedComplexType() { Description = "asdf", Number = 365 } },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of complex types where there are some extraneous elements in non-Astoria namespace - should pass
                CollectionItemType = typeof(ComplexType),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(AstoriaUnitTests.DataWebClientCSharp.ComplexType)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>
                            <Description>TestDescription</Description>
                            <Number>123</Number>
                        </m:element>
                        <m:element>
                            <Number>987</Number>
                        </m:element>
                        <m:element xmlns="""">
                            <Description>Not a collection item</Description>
                            <Number>99999</Number>
                        </m:element>
                        <foo xmlns="""">
                            <Description>Not a collection item</Description>
                            <Number>99999</Number>
                        </foo>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new ComplexType[] { new ComplexType() { Description = "abcdef", Number = 100} },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of complex types where some items are not related to the client type (soft binding) - should pass
                CollectionItemType = typeof(ComplexType),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(AstoriaUnitTests.DataWebClientCSharp.ComplexType)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>
                            <Description>TestDescription</Description>
                            <Number>123</Number>
                        </m:element>
                        <m:element m:type=""AstoriaUnitTests.DataWebClientCSharp.UnrelatedComplexType"">
                            <Description>Derived object without summary</Description>
                        </m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new DerivedComplexType[] { new DerivedComplexType() { Description = "as", Number = 35 } },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection where the collection payload contains mixed content (m:type present)
                CollectionItemType = typeof(int),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">12<m:element>3</m:element></m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new int[] { 3 },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of complex types where one of the item contains mixed content - should fail
                CollectionItemType = typeof(ComplexType),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(AstoriaUnitTests.DataWebClientCSharp.ComplexType)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>
                            <Description>TestDescription</Description>
                            <Number>123</Number>
                        </m:element>
                        <m:element>xyz
                            <Number>987</Number>
                        </m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new ComplexType[] { new ComplexType() { Description = "TestDescription", Number = 123}, new ComplexType{ Number = 123} },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of complex types where no properties are set on a complex type
                CollectionItemType = typeof(ComplexType),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(AstoriaUnitTests.DataWebClientCSharp.ComplexType)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>
                            <Description>TestDescription</Description>
                            <Number>123</Number>
                        </m:element>
                        <m:element>
                            <Number>987</Number>
                        </m:element>
                        <m:element />
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new ComplexType[] { new ComplexType() { Description = "TestDescription", Number = 123}, new ComplexType{ Number = 987}, new ComplexType() },
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of complex types where no properties are set on a complex type but there is extra content in the complex type element -- should ignore extra content
                CollectionItemType = typeof(ComplexType),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(AstoriaUnitTests.DataWebClientCSharp.ComplexType)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>
                            <Description>TestDescription</Description>
                            <Number>123</Number>
                        </m:element>
                        <m:element>
                            <Number>987</Number>
                        </m:element>
                        <m:element>4</m:element>
                    </m:value>",
                ExpectedException = (Exception)null,
                ValuesToInitializeCollection = (IEnumerable)new ComplexType[] { new ComplexType() { Description = "TestDescription", Number = 123}, new ComplexType{ Number = 987}, new ComplexType() },
                PropertyName = "Collection"
            },
        };

        private static MaterializationTestCase[] testCasesWithExceptions = new MaterializationTestCase[] {
            // negative test cases
            new MaterializationTestCase() { // Materializing a collection that contains null values - should fail
                CollectionItemType = typeof(string),
                CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.String)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>abc</m:element>
                        <m:element m:null=""true"" />
                        <m:element>abc</m:element>
                    </m:value>",
                ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ReaderValidationUtils_NullValueForNonNullableType", "Edm.String")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // Materializing a collection that contains null values and is backed by a collection of nullables  - should fail
                CollectionItemType = typeof(int?),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>1</m:element>
                        <m:element m:null=""true"" />
                        <m:element>2</m:element>
                    </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ReaderValidationUtils_NullValueForNonNullableType", "Edm.Int32")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // Materializing a collection whose value is null - should fail
                CollectionItemType = typeof(string),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.String)"" m:null=""true"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" />",
                ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ReaderValidationUtils_NullValueForNonNullableType", "Collection(Edm.String)")),
                ComplexExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ReaderValidationUtils_NullNamedValueForNonNullableType", "Collection", "Collection(Edm.String)")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
           },
            new MaterializationTestCase() { // Materializing a collection that contains values incompatible with the declared collection item type - should fail
                CollectionItemType = typeof(int),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>1</m:element>
                        <m:element>abc</m:element>
                        <m:element>2</m:element>
                    </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ReaderValidationUtils_CannotConvertPrimitiveValue", "abc", "Edm.Int32")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
            },
            new MaterializationTestCase( // Assigning a collection to a property that does not implement ICollection<T> and is not initialized- should fail. For .LoadProperty we have a different exception since we are not able to distiguish between a collection and single entity in AtomMaterializer
                alternateException : new InvalidOperationException(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Collection(Edm.Int32)", "Complex", "Collection")),
                loadProperty: true)  {
                CollectionItemType = typeof(DateTimeOffset),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:element>1</m:element>
                </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Collection(Edm.Int32)", "Complex", "Collection")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Object"
            },
            new MaterializationTestCase( // Assigning a collection to a property that does not implement ICollection<T> and is initialized- should fail. For .LoadProperty we have a different exception since we are not able to distiguish between a collection and single entity in AtomMaterializer
                alternateException : new InvalidOperationException(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Collection(Edm.Int32)", "Complex", "Collection")),
                loadProperty: true)  {
                CollectionItemType = typeof(DateTimeOffset),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:element>1</m:element>
                </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Collection(Edm.Int32)", "Complex", "Collection")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "InitializedObject"
            },
            new MaterializationTestCase(  // Assigning an empty collection to a property that does not implement ICollection<T> - should fail. For .LoadProperty we have a different exception since we are not able to distiguish between a collection and single entity in AtomMaterializer
                alternateException : new InvalidOperationException(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Collection(Edm.Int32)", "Complex", "Collection")),
                loadProperty: true)  {
                CollectionItemType = typeof(int),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" />",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Collection(Edm.Int32)", "Complex", "Collection")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Object"
            },
            // Assigning collection to a primitive type - should fail
            new MaterializationTestCase(
                (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Collection(Edm.String)", "Primitive", "Collection")), true, true) {
                CollectionItemType = typeof(string),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.String)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:element>1</m:element>
                </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Collection(Edm.String)", "Primitive", "Collection")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Description"
            },
            new MaterializationTestCase() {  // Using DataServiceCollection to store collection items - should fail
                CollectionItemType = typeof(string),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.String)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:element>1</m:element>
                </m:value>",
               ExpectedException = (Exception)new InvalidOperationException("A DataServiceCollection can only contain entity types. Primitive and complex types cannot be contained by this kind of collection."),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "DataServiceCollection"
            },
            new MaterializationTestCase() {  // A collection of primitive values that contains a complex type - should fail
                CollectionItemType = typeof(int),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:element>1</m:element>
                    <m:element>1</m:element>
                    <m:element>
                        <d:Description>TestDescription</d:Description>
                        <d:Number>123</d:Number>
                    </m:element>
                </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("XmlReaderExtension_InvalidNodeInStringValue", "Element")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
            },
            new MaterializationTestCase() {  // A collection of primitive values that contains only a complex type - should fail
                CollectionItemType = typeof(int),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:element>
                        <d:Description>TestDescription</d:Description>
                        <d:Number>123</d:Number>
                    </m:element>
                </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("XmlReaderExtension_InvalidNodeInStringValue", "Element")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of integer values when an element contains mixed content - should fail
                CollectionItemType = typeof(int),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>3</m:element>
                        <m:element>2<x /></m:element>
                    </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("XmlReaderExtension_InvalidNodeInStringValue", "Element")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
            },
            new MaterializationTestCase() { // materializing a collection of integer values where there are some extraneous elements in Astoria namespace - should fail
                CollectionItemType = typeof(int),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(Edm.Int32)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>3</m:element>
                        <m:element>2</m:element>
                        <m:element>1</m:element>
                        <m:element>1</m:element>
                        <d:foo>21</d:foo>
                    </m:value>",
                ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement", "foo", "http://docs.oasis-open.org/odata/ns/metadata")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
           },
            new MaterializationTestCase() { // materializing a collection of complex types where there are some extraneous elements in Astoria namespace - should fail
                CollectionItemType = typeof(ComplexType),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(AstoriaUnitTests.DataWebClientCSharp.ComplexType)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>
                            <d:Description>TestDescription</d:Description>
                            <d:Number>123</d:Number>
                        </m:element>
                        <m:element>
                            <d:Number>987</d:Number>
                        </m:element>
                        <d:foo>
                            <d:Description>Not a collection item</d:Description>
                            <d:Number>99999</d:Number>
                        </d:foo>
                    </m:value>",
                ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement", "foo", "http://docs.oasis-open.org/odata/ns/metadata")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
           },
            new MaterializationTestCase() { // Collection property without m:type attribute and with complex type in the payload - should fail
                CollectionItemType = typeof(ComplexType),
                 CollectionPropertyPayload = @"<m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <d:Description>TestDescription</d:Description>
                    <d:Number>123</d:Number>
                </m:value>",
                ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement", "Description", "http://docs.oasis-open.org/odata/ns/metadata")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
           },
            new MaterializationTestCase(new InvalidOperationException(ODataLibResourceUtil.GetString("XmlReaderExtension_InvalidNodeInStringValue", "Element")), true, null) { // Collection payload for a property that returns a primitive value - should fail
                CollectionItemType = typeof(ComplexType),
                 CollectionPropertyPayload = @"<m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:element>abc</m:element>
                    <m:element>123</m:element>
                </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(ODataLibResourceUtil.GetString("XmlReaderExtension_InvalidNodeInStringValue", "Element")),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Description"
            },
            new MaterializationTestCase( // Complex type on the server but entity on the client with m:type - should fail. For .LoadProperty we have a different exception since we are not able to distiguish between a collection and single entity in AtomMaterializer
                alternateException: new InvalidOperationException(DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidEntityType", typeof(BaseEntity))),
                loadProperty: true) {
                CollectionItemType = typeof(BaseEntity),
                 CollectionPropertyPayload = @"<m:value m:type=""Collection(AstoriaUnitTests.DataWebClientCSharp.BaseEntity)"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:element>
                        <d:ID>3</d:ID>
                    </m:element>
                </m:value>",
               ExpectedException = (Exception)new InvalidOperationException(DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidEntityType", typeof(BaseEntity))),
                ValuesToInitializeCollection = (IEnumerable)null,
                PropertyName = "Collection"
            },
        };

        private MaterializationTestCase[] testCases = testCasesWithoutExceptions.Union(testCasesWithExceptions).ToArray();
        #endregion

        #region Common methods for materialization test cases

        private static DataServiceContext GetContextForCollectionMaterializationTestCases(string baseUri)
        {
            DataServiceContext ctx = new DataServiceContext(new Uri(baseUri), ODataProtocolVersion.V4);
            //ctx.EnableAtom = true;

            ctx.ResolveType = (typeName) =>
            {
                switch (typeName)
                {
                    case "Collection()":
                        return typeof(DerivedComplexType);
                    case "NullableInteger":
                        return typeof(int?);
                    default:
                        Assert.IsFalse(typeName.StartsWith("Collection(") && typeName.EndsWith(")"), "Collection must not be tried to be resolved");
                        return null;
                }
            };

            return ctx;
        }

        private static Type GetEntityTypeForCollectionMaterializationTestCases(Type entityGenericType, Type collectionGenericType, Type collectionItemType)
        {
            Type entityType = null;

            // CustomCollection has two generic parameters - hence the special case
            if (collectionGenericType != typeof(CustomCollection<,>))
            {
                entityType = entityGenericType.MakeGenericType(collectionGenericType.MakeGenericType(collectionItemType), collectionItemType);
            }
            else
            {
                entityType = entityGenericType.MakeGenericType(collectionGenericType.MakeGenericType(typeof(int), collectionItemType), collectionItemType);
            }

            return entityType;
        }

        private static void GetCollectionParentForCollectionMaterializationTestCases(object entity, out Type collectionParentType, out object collectionParentInstance)
        {
            collectionParentType = null;
            collectionParentInstance = null;

            if (entity.GetType().GetGenericTypeDefinition() == typeof(MaterializationTestsEntityWithComplexTypeWithCollection<,>))
            {
                PropertyInfo pi = entity.GetType().GetProperty("Complex");
                collectionParentType = pi.PropertyType;
                collectionParentInstance = pi.GetValue(entity, null);

            }
            else
            {
                collectionParentType = entity.GetType();
                collectionParentInstance = entity;
            }
        }

        #endregion
        [Ignore] // Remove Atom
        // [TestMethod]
        public void MaterializationOfCollection()
        {
            Type[] collectionGenericTypes = {
                typeof(ICollection<>),
                typeof(IList<>),
                typeof(List<>),
                typeof(ObservableCollection<>),
                typeof(LoggingCollection<>),
                typeof(CustomCollection<,>)
            };

            bool[] initializeEntityCollection = { false, true };
            bool[] includeTypeAttribute = { false, true };
            bool[] includeCollectionProjections = { false, true };

            Type[] entityGenericTypes = {
                typeof(MaterializationTestsEntity<,>),
                typeof(MaterializationTestsEntityWithComplexTypeWithCollection<,>)
            };

            // it's not really feasible to test it against .LoadProperty and the variety of generic types. Move this back to the common materialization test cases once the bug is fixed.
            List<MaterializationTestCase> updatedTestCases = new List<MaterializationTestCase>(testCases);
            updatedTestCases.Add(
                new MaterializationTestCase()
                { // Complex type on the server but entity on the client with no m:type.
                    CollectionItemType = typeof(BaseEntity),
                    CollectionPropertyPayload = @"<m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:element>
                        <d:ID>3</d:ID>
                    </m:element>
                </m:value>",
                    ExpectedException = (Exception)new InvalidOperationException(DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidEntityType", typeof(BaseEntity))),
                    ValuesToInitializeCollection = (IEnumerable)null,
                    PropertyName = "Collection"
                });

            using (TestWebRequest host = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.OverridingPlayback.Restore())
            {
                host.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                host.StartService();

                TestUtil.RunCombinations(updatedTestCases, collectionGenericTypes, initializeEntityCollection, includeTypeAttribute, entityGenericTypes, includeCollectionProjections,
                    (testCase, collectionGenericType, initializeCollection, includeTypeAttr, entityGenericType, projectCollectionProperty) =>
                    {
                        bool isComplexTypeTesting = entityGenericType == typeof(MaterializationTestsEntityWithComplexTypeWithCollection<,>);

                        // for negative testcases we don't want to touch the payload so we skip them if m:type attribute is supposed to be removed
                        if (testCase.GetException(projectCollectionProperty, includeTypeAttr, false, true) != null && !includeTypeAttr)
                        {
                            return;
                        }

                        // projections of properties on complex types is not supported 
                        if (projectCollectionProperty && isComplexTypeTesting)
                        {
                            return;
                        }

                        XElement collectionPayload = XElement.Parse(testCase.CollectionPropertyPayload);

                        if (!includeTypeAttr)
                        {
                            collectionPayload.Attribute(UnitTestsUtil.MetadataNamespace + "type").Remove();
                        }

                        // for nested collection we need to add the corresponding property to the payload
                        PlaybackService.OverridingPlayback.Value = CreatePayload(
                            (isComplexTypeTesting ?
                                string.Format(
                                    "{0}{1}{2}",
                                    @"<d:Complex m:type=""Complex"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">",
                                    collectionPayload.ToString().Replace("m:value", "d:" + testCase.PropertyName),
                                    "</d:Complex>") :
                                     collectionPayload.ToString().Replace("m:value", "d:" + testCase.PropertyName)),
                            host.BaseUri);

                        DataServiceContext ctx = GetContextForCollectionMaterializationTestCases(host.BaseUri);
                        ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;

                        Type entityType = GetEntityTypeForCollectionMaterializationTestCases(entityGenericType, collectionGenericType, testCase.CollectionItemType);
                        entityType.GetProperty("InitialCollectionValues", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).SetValue(null, initializeCollection ? testCase.ValuesToInitializeCollection : null, null);

                        MethodInfo createQueryMethod = typeof(DataServiceContext).GetMethod("CreateQuery", new Type[] { typeof(string) }).MakeGenericMethod(entityType);
                        var query = createQueryMethod.Invoke(ctx, new object[] { "Entities" });
                        if (projectCollectionProperty)
                        {
                            ParameterExpression p = Expression.Parameter(entityType, "p");

                            var selector = Expression.Lambda(
                                Expression.MemberInit(
                                    Expression.New(entityType),
                                    Expression.Bind(
                                        entityType.GetProperty(testCase.PropertyName),
                                        Expression.MakeMemberAccess(p, entityType.GetProperty(testCase.PropertyName)))),
                                p);

                            MethodInfo select = typeof(System.Linq.Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "Select").First();
                            select = select.MakeGenericMethod(entityType, selector.ReturnType);
                            query = select.Invoke(null, new object[] { query, selector });
                        }

                        IEnumerator enumerator = ((IQueryable)query).GetEnumerator();

                        try
                        {
                            bool result = enumerator.MoveNext();
                            Assert.IsNull(testCase.GetException(projectCollectionProperty, includeTypeAttr, false, true), "Exception expected but not thrown");
                            Assert.IsTrue(result, "Entity set must not be empty.");

                            object currentEntity = enumerator.Current;

                            VerifyCollectionPropertyMaterialization(collectionPayload, currentEntity, initializeCollection);

                            result = enumerator.MoveNext();
                            Assert.IsFalse(result, "Only one entity expected.");
                        }
                        catch (AssertFailedException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Exception expectedException = testCase.GetException(projectCollectionProperty, includeTypeAttr, false, true);
                            if (expectedException != null)
                            {
                                Exception actualException = ex is TargetInvocationException ? ex.InnerException : ex;

                                if (expectedException.Message == DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidEntityType", typeof(BaseEntity)))
                                {
                                    if (entityType.Name == typeof(MaterializationTestComplextTypeWithCollection<,>).Name || entityType.Name == typeof(MaterializationTestsEntityWithComplexTypeWithCollection<,>).Name)
                                    {
                                        TestUtil.AssertContains(actualException.Message, "The property 'Collection' is of entity type and it cannot be a property of the type '");
                                        TestUtil.AssertContains(actualException.Message, "', which is not of entity type.  Only entity types can contain navigation properties.");
                                        return;
                                    }
                                    else if (entityType.Name == typeof(MaterializationTestsEntity<,>).Name)
                                    {
                                        TestUtil.AssertContains(actualException.Message, ODataLibResourceUtil.GetString(
                                            "ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties",
                                            "Collection",
                                            GetModelFullName(entityType)));
                                        return;
                                    }
                                }

                                Assert.AreEqual(expectedException.Message, actualException.Message);
                                Assert.IsInstanceOfType(actualException, expectedException.GetType());
                            }
                        }
                        finally
                        {
                            entityType.GetProperty("InitialCollectionValues", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).SetValue(null, null, null);
                        }
                    });
            }
        }

        public class EntityWithCollectionHavingNoSetter : BaseEntity
        {
            private List<string> collection = new List<string>();

            public List<string> NoSetterCollection
            {
                get { return collection; }
            }
        }

        public class EntityWithICollectionBagType_Interface : BaseEntity
        {
            public ICollection<string> ICollectionBagProperty
            {
                get;
                set;
            }
        }

        public class EntityWithICollectionBagType_Concrete : BaseEntity
        {
            public List<string> ListBagProperty
            {
                get;
                set;
            }
        }

        public class ListWithNoPublicConstructor<T> : List<T>
        {
            private ListWithNoPublicConstructor() { }
        }

        public class EntityWithCollectionWithNoPublicCtor : BaseEntity
        {
            public ListWithNoPublicConstructor<string> Collection { get; set; }
        }
        #endregion

        #region LoadProperty
        [Ignore] // Remove Atom
        // [TestMethod]
        public void LoadPropertyNonErrorTestsAsync()
        {
            LoadPropertyTests(testCasesWithoutExceptions, true);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void LoadPropertyNonErrorTestsSync()
        {
            LoadPropertyTests(testCasesWithoutExceptions, false);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void LoadPropertyErrorTestsAsync()
        {
            LoadPropertyTests(testCasesWithExceptions, true);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void LoadPropertyErrorTestsSync()
        {
            LoadPropertyTests(testCasesWithExceptions, false);
        }

        public void LoadPropertyTests(MaterializationTestCase[] payloadTestCases, bool invokeAsync)
        {
            Type[] collectionGenericTypes = {
                typeof(ICollection<>),
                typeof(IList<>),
                typeof(List<>),
                typeof(ObservableCollection<>),
                typeof(LoggingCollection<>),
                typeof(CustomCollection<,>)
            };

            bool[] initializeEntityCollection = { false, true };
            bool[] includeTypeAttribute = { true, false };

            Type[] entityGenericTypes = {
                typeof(MaterializationTestsEntity<,>),
                typeof(MaterializationTestsEntityWithComplexTypeWithCollection<,>)
            };

            using (TestWebRequest host = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.OverridingPlayback.Restore())
            {
                host.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                host.StartService();

                TestUtil.RunCombinations(payloadTestCases, collectionGenericTypes, initializeEntityCollection, includeTypeAttribute, entityGenericTypes,
                    (testCase, collectionGenericType, initializeCollection, includeTypeAttr, entityGenericType) =>
                    {
                        // .LoadProperty should behave the same as the regular materialization if the collection property is not the top level property being loaded but its descendant.
                        bool loadProperty = entityGenericType != typeof(MaterializationTestsEntityWithComplexTypeWithCollection<,>);
                        bool isComplexTypeTesting = entityGenericType == typeof(MaterializationTestsEntityWithComplexTypeWithCollection<,>);

                        // for negative testcases we don't want to touch the payload so we skip them if m:type attribute is supposed to be removed
                        if (testCase.GetException(false, includeTypeAttr, loadProperty, isComplexTypeTesting) != null && !includeTypeAttr)
                        {
                            return;
                        }

                        XElement collectionPayload = XElement.Parse(testCase.CollectionPropertyPayload);

                        if (!includeTypeAttr)
                        {
                            collectionPayload.Attributes(UnitTestsUtil.MetadataNamespace + "type").Remove();
                        }

                        // for nested collection we need to add the corresponding property to the payload
                        PlaybackService.OverridingPlayback.Value =
@"HTTP/1.1 200 OK
Proxy-Connection: Keep-Alive
Content-Type: application/xml;charset=utf-8
Server: Microsoft-IIS/7.5
Cache-Control: no-cache
OData-Version: 4.0;
X-AspNet-Version: 4.0.21006
X-Powered-By: ASP.NET

<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>" +
                        (isComplexTypeTesting ?
                        string.Format(
                            "{0}{1}{2}",
                            @"<m:value m:type=""Complex"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">",
                            collectionPayload.ToString().Replace("m:value", "d:" + testCase.PropertyName),
                            "</m:value>") :
                        collectionPayload.ToString());

                        DataServiceContext ctx = GetContextForCollectionMaterializationTestCases(host.BaseUri);
                        ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
                        Type entityType = GetEntityTypeForCollectionMaterializationTestCases(entityGenericType, collectionGenericType, testCase.CollectionItemType);
                        entityType.GetProperty("InitialCollectionValues", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).SetValue(null, initializeCollection ? testCase.ValuesToInitializeCollection : null, null);

                        object currentEntity = Activator.CreateInstance(entityType);
                        ctx.AttachTo("Entities", currentEntity);

                        try
                        {
                            // Load property can only load top level properties
                            string propertyName = entityGenericType == typeof(MaterializationTestsEntityWithComplexTypeWithCollection<,>) ? "Complex" : testCase.PropertyName;

                            if (invokeAsync)
                            {
                                IAsyncResult ar = ctx.BeginLoadProperty(currentEntity, propertyName, null, null);
                                ctx.EndLoadProperty(ar);
                            }
                            else
                            {
                                ctx.LoadProperty(currentEntity, propertyName);
                            }

                            VerifyCollectionPropertyMaterialization(collectionPayload, currentEntity, initializeCollection);

                            Assert.IsNull(testCase.GetException(false, includeTypeAttr, loadProperty, isComplexTypeTesting), "Exception expected but not thrown");

                        }
                        catch (AssertFailedException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Exception expectedException = testCase.GetException(false, includeTypeAttr, loadProperty, isComplexTypeTesting);
                            if (expectedException != null)
                            {
                                Exception actualException = ex is TargetInvocationException || ex is DataServiceQueryException ? ex.InnerException : ex;

                                if (expectedException.Message == DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidEntityType", typeof(BaseEntity)) &&
                                    (entityType.Name == typeof(MaterializationTestComplextTypeWithCollection<,>).Name || entityType.Name == typeof(MaterializationTestsEntityWithComplexTypeWithCollection<,>).Name))
                                {
                                    TestUtil.AssertContains(actualException.Message, "The property 'Collection' is of entity type and it cannot be a property of the type '");
                                    TestUtil.AssertContains(actualException.Message, "', which is not of entity type.  Only entity types can contain navigation properties.");
                                }
                                else
                                {
                                    Assert.AreEqual(expectedException.Message, actualException.Message);
                                }

                                Assert.IsInstanceOfType(actualException, expectedException.GetType());
                            }
                            else
                            {
                                throw;
                            }
                        }
                        finally
                        {
                            entityType.GetProperty("InitialCollectionValues", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).SetValue(null, null, null);
                        }
                    });
            }
        }

        #endregion

        #region Serialization

        [Ignore] // Remove Atom
        // [TestMethod]
        public void SerializationOfCollection()
        {
            Type[] collectionGenericTypes = new Type[]
            {
                typeof(ICollection<>),
                typeof(IList<>),
                typeof(List<>),
                typeof(ObservableCollection<>),
            };

            var testCases = new[] {
             // collection of primitive types 
                // positive cases
                new { // serializing a collection of integer values - should pass
                    CollectionItemType = typeof(int),
                    ExpectedCollectionWireType = "#Collection(Int32)",
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new int[] { 100, 101, 200 }
                },
                new { // serializing a collection of nullable integer values - should pass
                    CollectionItemType = typeof(int?),
                    ExpectedCollectionWireType = "#Collection(Int32)",
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new int?[] { 10, 11, 20 }
                },
                new { // serializing a collection of DateTimeOffset values - should pass
                    CollectionItemType = typeof(DateTimeOffset),
                    ExpectedCollectionWireType = "#Collection(DateTimeOffset)",
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new DateTimeOffset[] {
                        new DateTime(2010, 3, 16, 10, 15, 20, DateTimeKind.Utc),
                        new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2132, 11, 14, 23, 47, 07, DateTimeKind.Utc) }
                },
                new { // Serializaing a collection of string values - should pass
                    CollectionItemType = typeof(string),
                    ExpectedCollectionWireType = "#Collection(String)",
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new string[] { "ab    c", string.Empty, "123abc ", string.Empty, " ", "\txyz\t", "\rabc", "abc\n" }
                },
                new { // serializing an empty collection - should pass
                    CollectionItemType = typeof(string),
                    ExpectedCollectionWireType = "#Collection(String)",
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new string[0]
                },
                new { // serializing a collection that contains null values - should pass
                    CollectionItemType = typeof(string),
                    ExpectedCollectionWireType = "#Collection(String)",
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new string[] { "abc", null, "123" }
                },
                new { // serializing a collection that contains null values and is backed by a collection of nullables  - should fail
                    CollectionItemType = typeof(int?),
                    ExpectedCollectionWireType = "#Collection(Int32)",
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new int?[] { -1, null, 1 },
                },

                // negative test cases
                new { // serializing a collection whose value is null - should fail
                    CollectionItemType = typeof(string),
                    ExpectedCollectionWireType = "#Collection(String)",
                    ExpectedException = (Exception)new InvalidOperationException("The value of the property 'Collection' is null. Properties that are a collection type of primitive or complex types cannot be null."),
                    ValuesToInitializeCollection = (IEnumerable)null
                },
                new { // serializing a collection whose items are collection - should fail
                    CollectionItemType = typeof(List<int>),
                    ExpectedCollectionWireType = default(string),
                    ExpectedException = (Exception)new InvalidOperationException("Collection properties of a collection type are not supported."),
                    ValuesToInitializeCollection = (IEnumerable)new List<int>[] { new List<int>(new int[] {1, 2}), new List<int>(new int[] {3, 4}) }
                },

              //collection of complex types 
              //   positive cases
                new { // serializing a collection of complex types - should pass
                    CollectionItemType = typeof(ComplexType),
                    ExpectedCollectionWireType = default(string),
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new ComplexType[] {
                        new ComplexType() { Description = "TestDescription", Number = 123 },
                        new ComplexType() { Number = 098 } }
                },
                new { // verify that collection item type is passed to the type resolver when serializing and it is passed only once - should pass
                    CollectionItemType = typeof(UnrelatedComplexType),
                    ExpectedCollectionWireType = "#Collection(epyTxelpmoCdetalernU)",
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new UnrelatedComplexType[] {
                        new UnrelatedComplexType() { Description = "Unrelated ComplexType", AnotherDescription = "Another Description" },
                        new UnrelatedComplexType() { Description = "XYZ" } }
                },
                new { // verify serialization of collection of complex types that have collection - should pass
                    CollectionItemType = typeof(DerivedComplexType),
                    ExpectedCollectionWireType = default(string),
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new DerivedComplexType[] {
                        new DerivedComplexType() { Description = "1st Item", Number = 1, Summary = "Collection 1", CollectionOfComplexTypes = new DerivedComplexType[] {
                            new DerivedComplexType() { Description = "1.1", Number = 1, Summary = "1st item in the 1st nested collection", CollectionOfComplexTypes = new List<DerivedComplexType>() },
                            new DerivedComplexType() { Description = "1.2", Number = 2, Summary = "2nd item in the 1st nested collection", CollectionOfComplexTypes = new List<DerivedComplexType>() }}},
                        new DerivedComplexType() { Description = "2nd Item", Number = 2, Summary = "Collection 2", CollectionOfComplexTypes = new DerivedComplexType[] {
                            new DerivedComplexType() { Description = "1.1", Number = 1, Summary = "1st item in the 2nd nested collection", CollectionOfComplexTypes = new List<DerivedComplexType>() },
                            new DerivedComplexType() { Description = "1.2", Number = 2, Summary = "2nd item in the 2nd nested collection", CollectionOfComplexTypes = new List<DerivedComplexType>() }}}}
                },

                //negative test cases
                new { // verify that serialization of collection of complex types fails if the collection contains null items - should pass
                    CollectionItemType = typeof(ComplexType),
                    ExpectedCollectionWireType = default(string),
                    ExpectedException = (Exception)null,
                    ValuesToInitializeCollection = (IEnumerable)new ComplexType[] {
                        new ComplexType() { Number = 1, Description = "ComplexType" },
                        null,
                        new ComplexType() { Number = 3, Description = "ComplexType" }}
                },
                new { // verify that serialization of collection of complex types that has cycles fails - should fail
                    CollectionItemType = typeof(DerivedComplexType),
                    ExpectedCollectionWireType = default(string),
                    ExpectedException = (Exception)new InvalidOperationException("A circular loop was detected while serializing the property 'CollectionOfComplexTypes'. You must make sure that loops are not present in properties that return a collection or complex type."),
                    ValuesToInitializeCollection = (IEnumerable)CreateCollectionWithCycle()
                }
            };

            using (TestWebRequest host = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.OverridingPlayback.Restore())
            using (PlaybackService.InspectRequestPayload.Restore())
            {
                host.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                host.StartService();

                PlaybackService.OverridingPlayback.Value =
                "HTTP/1.1 201 Created\r\n" +
                "Content-Length: 0\r\n" +
                "Location: " + host.BaseUri + "/\r\n" +
                "OData-EntityId: " + host.BaseUri + "/\r\n" +
                "\r\n";

                TestUtil.RunCombinations(testCases, collectionGenericTypes, new string[] { "POST", "PUT", "PATCH" }, (testCase, collectionGenericType, httpMethod) =>
                {
                    // negative test cases does not apply to PUT and PATCH - since there is no way of creating the entity that will be updated
                    if (testCase.ExpectedException != null && (httpMethod != "POST"))
                    {
                        return;
                    }

                    Type entityType = typeof(Entity<,>).MakeGenericType(collectionGenericType.MakeGenericType(testCase.CollectionItemType), testCase.CollectionItemType);
                    object entity = Activator.CreateInstance(entityType, testCase.ValuesToInitializeCollection);

                    int resolvedCount = 0;

                    PlaybackService.InspectRequestPayload.Value = (payload) =>
                    {
                        XDocument payloadXml = XDocument.Load(payload);

                        Assert.IsNull(testCase.ExpectedException, "Exception expected but not thrown. Payload: " + payloadXml.ToString());

                        XElement collectionPayload = payloadXml.Descendants(UnitTestsUtil.DataNamespace + "Collection").First();

                        Assert.AreEqual(testCase.ExpectedCollectionWireType, (string)collectionPayload.Attribute(UnitTestsUtil.MetadataNamespace + "type"));

                        // verify we resolve the type only once rather than for each collection item
                        if (testCase.CollectionItemType == typeof(UnrelatedComplexType))
                        {
                            Assert.AreEqual(3, resolvedCount, "The type should be resolved three times");
                            resolvedCount = 0; // reset the count
                        }

                        VerifyCollectionAndPayloadMatch(collectionPayload, (IEnumerable)entity.GetType().GetProperty("Collection").GetValue(entity, null));
                    };

                    DataServiceContext ctx = new DataServiceContext(new Uri(host.BaseUri), ODataProtocolVersion.V4);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.AddObject("Entities", entity);

                    ctx.ResolveName = (type) =>
                    {
                        if (type == typeof(UnrelatedComplexType))
                        {
                            resolvedCount++;
                            return "epyTxelpmoCdetalernU";
                        }

                        return null;
                    };

                    SaveChangesOptions? saveChangesOptions = null;
                    switch (httpMethod)
                    {
                        case "PUT":
                        case "PATCH":
                            ctx.SaveChanges();
                            ctx.UpdateObject(entity);
                            saveChangesOptions = httpMethod == "PUT" ? SaveChangesOptions.ReplaceOnUpdate : SaveChangesOptions.None;
                            break;
                        case "POST":
                            saveChangesOptions = SaveChangesOptions.None;
                            break;
                        default:
                            Assert.Fail("Only POST, PUT, PATCH, PATCH verbs are supported");
                            break;
                    }

                    try
                    {
                        ctx.SaveChanges(saveChangesOptions.Value);
                    }
                    catch (AssertFailedException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        if (testCase.ExpectedException != null)
                        {
                            Exception actualException = ex is DataServiceRequestException ? ex.InnerException : ex;

                            Assert.IsNotNull(actualException);
                            Assert.IsTrue(actualException.Message.Contains(testCase.ExpectedException.Message), "Message: '{0}' does not contain expected '{1}'", testCase.ExpectedException.Message, actualException.Message);
                            Assert.IsInstanceOfType(actualException, testCase.ExpectedException.GetType());
                        }
                        else
                        {
                            throw;
                        }
                    }
                });
            }
        }

        private DerivedComplexType[] CreateCollectionWithCycle()
        {
            DerivedComplexType[] array = new DerivedComplexType[] {
                new DerivedComplexType() {
                    Number = 1,
                    Description = "Collection with loops",
                }
            };

            array[0].CollectionOfComplexTypes = array;

            return array;
        }

        public class EntityWithUntypedCollection<T> : BaseEntity
        {
            public EntityWithUntypedCollection()
            {
                Collection = new UntypedCollection<T>(new T[] { });
            }

            public UntypedCollection<T> Collection { get; set; }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void InvalidItemsInCollection()
        {
            // Negative tests for collection that contain items of a type that is not compatible with the collection type.
            var testCases = new[] {
                new { // complex type in a collection of primitive types
                    TypeOfEntity =  typeof(EntityWithUntypedCollection<string>),
                    Collection  = (object)new UntypedCollection<string>(new object[] { "primitive", new ComplexType() { Number = 1, Description = "ComplexTypes" } }),
                    ExpectedException = (Exception)new InvalidOperationException("A collection property of primitive types cannot contain an item of a complex type.")
                },
                new { // primitive type in a collection of complex types
                    TypeOfEntity =  typeof(EntityWithUntypedCollection<ComplexType>),
                    Collection  = (object)new UntypedCollection<ComplexType>(new object[] { new ComplexType() { Number = 1, Description = "ComplexTypes" }, "primitive" }),
                    ExpectedException = (Exception)new InvalidOperationException("A collection property of complex types cannot contain an item of a primitive type.")
                },
                new { // unrelated complex type in the collection
                    TypeOfEntity =  typeof(EntityWithUntypedCollection<ComplexType>),
                    Collection  = (object)new UntypedCollection<ComplexType>(new object[] {
                        new ComplexType() { Number = 1, Description = "ComplexTypes" },
                        new UnrelatedComplexType() { Description = "Unrelated" } }),
                    ExpectedException = (Exception)new InvalidOperationException("An item in the collection property 'Collection' is not of the correct type. All items in the collection property must be of the collection item type.")
                },
                new { // incompatible primitive type in the collection of primitive types
                    TypeOfEntity =  typeof(EntityWithUntypedCollection<int>),
                    Collection  = (object)new UntypedCollection<int>(new object[] { 1, "two", 3 }),
                    ExpectedException = (Exception)new InvalidOperationException("An item in the collection property 'Collection' is not of the correct type. All items in the collection property must be of the collection item type.")
                },
                new { // incompatible primitive type in the collection of primitive types
                    TypeOfEntity =  typeof(EntityWithUntypedCollection<byte>),
                    Collection  = (object)new UntypedCollection<byte>(new object[] { 1, Int32.MaxValue, 3 }),
                    ExpectedException = (Exception)new InvalidOperationException("An item in the collection property 'Collection' is not of the correct type. All items in the collection property must be of the collection item type.")
                },
            };

            using (TestWebRequest host = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.OverridingPlayback.Restore())
            {
                host.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                host.StartService();

                PlaybackService.OverridingPlayback.Value =
                "HTTP/1.1 201 Created\r\n" +
                "Content-Length: 0\r\n" +
                "Location: " + host.BaseUri + "/\r\n" +
                "OData-EntityId: " + host.BaseUri + "/\r\n" +
                "\r\n";

                TestUtil.RunCombinations(testCases, new string[] { "POST", "PUT", "PATCH" }, (testCase, httpMethod) =>
                {
                    object entity = Activator.CreateInstance(testCase.TypeOfEntity);

                    DataServiceContext ctx = new DataServiceContext(new Uri(host.BaseUri), ODataProtocolVersion.V4);
                    ctx.AddObject("Entities", entity);

                    if (httpMethod == "PUT" || httpMethod == "PATCH")
                    {
                        ctx.SaveChanges();
                        ctx.UpdateObject(entity);
                    }

                    entity.GetType().GetProperty("Collection").SetValue(entity, testCase.Collection, null);

                    try
                    {
                        ctx.SaveChanges();
                        Assert.Fail("Exception expected but not thrown.");
                    }
                    catch (Exception ex)
                    {
                        Exception actualException = ex is DataServiceRequestException ? ex.InnerException : ex;

                        Assert.IsNotNull(actualException);
                        Assert.IsTrue(actualException.Message.Contains(testCase.ExpectedException.Message), "Message: '{0}' does not contain expected '{1}'", testCase.ExpectedException.Message, actualException.Message);
                        Assert.IsInstanceOfType(actualException, testCase.ExpectedException.GetType());
                    }
                });
            }
        }

        public class EntityWithDictionary : BaseEntity
        {
            public Dictionary<string, string> Collection { get; set; }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void SerializationOfEntityWithDictionary()
        {
            // Validates that IDictionary (can be used as a storage for open property key/value pairs) is not treated as a collection.
            using (TestWebRequest host = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.OverridingPlayback.Restore())
            using (PlaybackService.InspectRequestPayload.Restore())
            {
                PlaybackService.OverridingPlayback.Value =
                    "HTTP/1.1 201 Created\r\n" +
                    "Content-Length: 0\r\n" +
                    "Location: http://www.microsoft.com/\r\n" +
                    "OData-EntityId: http://www.microsoft.com/\r\n" +
                    "\r\n";

                host.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                host.StartService();

                Dictionary<string, string> openProperties = new Dictionary<string, string>();
                openProperties.Add("property1", "value1");
                openProperties.Add("property2", "value2");
                EntityWithDictionary entity = new EntityWithDictionary()
                {
                    ID = 1,
                    Collection = openProperties
                };

                PlaybackService.InspectRequestPayload.Value = (payload) =>
                {
                    XElement collectionPayload = XDocument.Load(payload).Descendants(UnitTestsUtil.DataNamespace + "Collection").FirstOrDefault();
                    Assert.IsNull(collectionPayload, "Collection should not be created for dictionary");
                };

                DataServiceContext ctx = new DataServiceContext(new Uri(host.BaseUri));
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                ctx.AddObject("Entities", entity);
                ctx.SaveChanges();
            }
        }

        #region Types for testing versioning

        public class EntityWithReferenceSet : BaseEntity
        {
            public List<BaseEntity> ReferenceSet { get; set; }
        }

        public class EntityWithCollection : BaseEntity
        {
            public ICollection<int> CollectionOfIntegers { get; set; }
        }

        public class EntityWithNavigationPropertyToEntityWithCollection : BaseEntity
        {
            public EntityWithCollection NavigationProperty { get; set; }
        }

        public class EntityWithReferenceSetToEntitiesWithCollection : BaseEntity
        {
            public List<EntityWithCollection> ReferenceSet { get; set; }
        }

        public class EntityWithComplexTypeWithCollection : BaseEntity
        {
            public DerivedComplexType ComplexTypeWithCollection { get; set; }
        }

        public class EntityWithComplexTypeHierarchy : BaseEntity
        {
            public ComplexTypeWithComplexType NestedComplexType { get; set; }
        }

        public class ComplexTypeWithComplexType
        {
            public DerivedComplexType ComplexTypeWithCollection { get; set; }
        }

        public class NestedComplexType
        {
            public DerivedComplexType ComplexTypeWithCollection { get; set; }
        }

        public class EntityWithSelfReferringComplexType : BaseEntity
        {
            public SelfReferringComplexType ComplexType { get; set; }
        }

        public class SelfReferringComplexType
        {
            public SelfReferringComplexType ChildComplexType { get; set; }
        }

        public class EntityWithSelfReferringComplexTypeAndCollection : EntityWithSelfReferringComplexType
        {
            public List<int> ListOfInt { get; set; }
        }

        #endregion

        #endregion

        #region client LINQ support for collection
        // Note - reflection provider doesn't support inheritance on complex types, so just a simple case here

        public class ComplexTypeWithConstructor
        {
            public ICollection<string> Names { get; set; }

            public ComplexTypeWithConstructor(ICollection<string> names)
            {
                this.Names = names;
            }
        }

        public class ListCollection : List<string>
        {
        }

        public class CollectionEntityType
        {
            public int ID { get; set; }
            public ICollection<string> Names { get; set; }
            public List<string> NamesAsList { get; set; }
            public ListCollection ListCollection { get; set; }
            public List<ComplexType> ComplexCollection { get; set; }
            public CollectionEntityType SelfReference { get; set; }
        }

        public class CollectionNonEntityType
        {
            public ICollection<string> Names { get; set; }
            public List<string> NamesAsList { get; set; }
            public ListCollection ListCollection { get; set; }
            public List<ComplexType> ComplexCollection { get; set; }
            public CollectionEntityType SelfReference { get; set; }
        }

        public class CollectionContext
        {
            private static List<CollectionEntityType> entities;

            static CollectionContext()
            {
                CollectionEntityType entity = new CollectionEntityType() { ID = 0, NamesAsList = new List<string>(), ListCollection = new ListCollection(), ComplexCollection = new List<ComplexType>() };
                entity.NamesAsList.Add("");
                entity.NamesAsList.Add("value");
                entity.Names = entity.NamesAsList;
                entity.ListCollection.AddRange(entity.Names);
                entity.ComplexCollection.Add(new ComplexType() { Description = "Description 1", Number = 1 });
                entity.ComplexCollection.Add(new ComplexType() { Description = "Description 2", Number = 2 });
                entity.SelfReference = entity;
                entities = new List<CollectionEntityType>();
                entities.Add(entity);
            }

            public IQueryable<CollectionEntityType> Entities { get { return entities.AsQueryable(); } }
        }

        void ValidateNames(string[] baseNames, string[] names)
        {
            Assert.IsNotNull(names, "names != null");
            Assert.AreEqual(baseNames.Length, names.Length);
            for (int i = 0; i < names.Length; i++)
            {
                Assert.AreEqual(baseNames[i], names[i]);
            }
        }

        void ValidateComplexCollection(ComplexType[] baseComplexCollection, ComplexType[] complexCollection)
        {
            Assert.IsNotNull(complexCollection, "complexCollection != null");
            Assert.AreEqual(baseComplexCollection.Length, complexCollection.Length);
            for (int i = 0; i < complexCollection.Length; i++)
            {
                Assert.AreEqual(baseComplexCollection[i].Description, complexCollection[i].Description);
                Assert.AreEqual(baseComplexCollection[i].Number, complexCollection[i].Number);
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void Collection_SupportedClientLINQQueries()
        {
            // Test supported client LINQ queries.
            string[] baseNames = new string[] { "", "value" };
            ComplexType[] baseComplexCollection = new ComplexType[] { new ComplexType { Description = "Description 1", Number = 1 }, new ComplexType { Description = "Description 2", Number = 2 } };

            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CollectionContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                ctx.MergeOption = MergeOption.NoTracking;

                string[] names;
                ComplexType[] complexCollection;

                // Test navigation
                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select e.Names;
                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.ToArray();
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)/Names", q.ToString());
                }
                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select e.ComplexCollection;

                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(complexCollection, "only one result is expected");
                        complexCollection = e.ToArray();
                    }

                    ValidateComplexCollection(baseComplexCollection, complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)/ComplexCollection", q.ToString());
                }
                {
                    // test projection with entity type
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new CollectionEntityType()
                            {
                                Names = e.Names
                            };

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=Names", q.ToString());
                }
                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new CollectionEntityType()
                            {
                                ComplexCollection = e.ComplexCollection
                            };

                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(complexCollection, "only one result is expected");
                        complexCollection = e.ComplexCollection.ToArray();
                    }

                    ValidateComplexCollection(baseComplexCollection, complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=ComplexCollection", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new CollectionEntityType()
                            {
                                ListCollection = e.ListCollection
                            };

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.ListCollection.ToArray();
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=ListCollection", q.ToString());
                }

                //test projection into anonymous type
                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                Names = e.Names
                            };

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=Names", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                ComplexCollection = e.ComplexCollection
                            };

                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(complexCollection, "only one result is expected");
                        complexCollection = e.ComplexCollection.ToArray();
                    }

                    ValidateComplexCollection(baseComplexCollection, complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=ComplexCollection", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                ListCollection = e.ListCollection
                            };

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.ListCollection.ToArray();
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=ListCollection", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                Names = e.Names.Where(n => n != "")
                            };

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                    }

                    ValidateNames(new string[] { "value" }, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=Names", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                complexcollection = e.ComplexCollection,
                                firstComplexCollection = e.ComplexCollection[0]
                            };

                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(complexCollection, "only one result is expected");
                        complexCollection = e.complexcollection.ToArray();
                        Assert.AreEqual("Description 1", e.firstComplexCollection.Description);
                    }

                    ValidateComplexCollection(baseComplexCollection, complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=ComplexCollection,ComplexCollection", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new ComplexTypeWithConstructor(e.Names);

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=Names", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                Names = e.Names,
                                NamesJoined = String.Join(",", e.Names),
                                ComplexCollection = e.ComplexCollection
                            };

                    names = null;
                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                        Assert.AreEqual(",value", e.NamesJoined);
                        complexCollection = e.ComplexCollection.ToArray();
                    }

                    ValidateNames(baseNames, names);
                    ValidateComplexCollection(baseComplexCollection, complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=Names,Names,ComplexCollection", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                Names = new List<string>(new string[] { e.NamesAsList[0], e.NamesAsList[1] })
                            };


                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=NamesAsList,NamesAsList", q.ToString());
                }

                // test type casting
                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new CollectionEntityType()
                            {
                                Names = new List<string>(e.Names)
                            };

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                        Assert.IsInstanceOfType(e.Names, typeof(ICollection<string>));
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=Names", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                Names = e.NamesAsList as ICollection<string>
                            };

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                        Assert.IsInstanceOfType(e.Names, typeof(ICollection<string>));
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=NamesAsList", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                Names = new Collection<string>(new List<string>(e.Names.Select(s => s)))
                            };

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                        Assert.IsInstanceOfType(e.Names, typeof(Collection<string>));
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=Names", q.ToString());
                }

                {
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                names = e.NamesAsList,
                                count = e.NamesAsList.Count()
                            };

                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.names.ToArray();
                        Assert.IsInstanceOfType(e.names, typeof(List<string>));
                        Assert.AreEqual(e.names.Count, e.count);
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=NamesAsList,NamesAsList", q.ToString());
                }

                {
                    // Test navigation to collection
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select e.SelfReference.SelfReference.Names;
                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.ToArray();
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)/SelfReference/SelfReference/Names", q.ToString());
                }

                {
                    // Test navigation to collection
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select e.SelfReference.SelfReference.ComplexCollection;
                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(complexCollection, "only one result is expected");
                        complexCollection = e.ToArray();
                    }

                    ValidateComplexCollection(baseComplexCollection, complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)/SelfReference/SelfReference/ComplexCollection", q.ToString());
                }

                {
                    // Test expansions and projections of collection - entity type
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in context.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select new CollectionEntityType
                            {
                                ID = e.ID,
                                SelfReference = new CollectionEntityType
                                {
                                    ID = e.SelfReference.ID,
                                    Names = e.SelfReference.Names
                                }
                            };
                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.SelfReference.Names.ToArray();
                        Assert.IsNotNull(context.GetEntityDescriptor(e), "The entity should be tracked.");
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)?$expand=SelfReference($select=ID),SelfReference($select=Names)&$select=ID", q.ToString());
                }

                {
                    // Test expansions and projections of collection - anonymous/nonentity type
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in context.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select new
                            {
                                Entity = new
                                {
                                    Names = e.SelfReference.Names
                                }
                            };
                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Entity.Names.ToArray();
                        Assert.IsNull(context.GetEntityDescriptor(e), "The entity should not be tracked.");
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)?$expand=SelfReference($select=Names)", q.ToString());
                }

                {
                    // Test projecting collection to entity types
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in context.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select new CollectionEntityType
                            {
                                Names = e.Names
                            };
                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                        Assert.IsNotNull(context.GetEntityDescriptor(e), "The entity should be tracked.");
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)?$select=Names", q.ToString());
                }

                {
                    // Test projecting collection to entity types
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in context.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select new CollectionEntityType
                            {
                                ComplexCollection = e.ComplexCollection
                            };
                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(complexCollection, "only one result is expected");
                        complexCollection = e.ComplexCollection.ToArray();
                        Assert.IsNotNull(context.GetEntityDescriptor(e), "The entity should be tracked.");
                    }

                    ValidateComplexCollection(baseComplexCollection, complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)?$select=ComplexCollection", q.ToString());
                }

                {
                    // Test projecting collection to non-entity types
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in context.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select new CollectionNonEntityType
                            {
                                Names = e.Names
                            };
                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                        Assert.IsNull(context.GetEntityDescriptor(e), "The entity should not be tracked.");
                    }

                    ValidateNames(baseNames, names);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)?$select=Names", q.ToString());
                }
                {
                    // Test projecting collection to non-entity types
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in context.CreateQuery<CollectionEntityType>("Entities")
                            where e.ID == 0
                            select new CollectionNonEntityType
                            {
                                ComplexCollection = e.ComplexCollection
                            };
                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(complexCollection, "only one result is expected");
                        complexCollection = e.ComplexCollection.ToArray();
                        Assert.IsNull(context.GetEntityDescriptor(e), "The entity should not be tracked.");
                    }

                    ValidateComplexCollection(baseComplexCollection, complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities(0)?$select=ComplexCollection", q.ToString());
                }

                // Test projecting collection to non-entity types
                {
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                ComplexCollection = e.ComplexCollection.Where(cb => cb.Description == "Description 1")
                            };
                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(complexCollection, "only one result is expected");
                        complexCollection = e.ComplexCollection.ToArray();
                        Assert.IsNull(context.GetEntityDescriptor(e), "The entity should not be tracked.");
                    }

                    ValidateComplexCollection(baseComplexCollection.Where(cb => cb.Description == "Description 1").ToArray(), complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=ComplexCollection", q.ToString());

                }
                {
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                ComplexCollection = from cb in e.ComplexCollection
                                                    select new ComplexType() { Description = cb.Description + "abc" }
                            };
                    complexCollection = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(complexCollection, "only one result is expected");
                        complexCollection = e.ComplexCollection.ToArray();
                        Assert.IsNull(context.GetEntityDescriptor(e), "The entity should not be tracked.");
                    }

                    ValidateComplexCollection((from cb in baseComplexCollection select new ComplexType() { Description = cb.Description + "abc" }).ToArray(),
                                                complexCollection);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=ComplexCollection", q.ToString());
                }
                {
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                Names = new List<string>(new string[] { e.ComplexCollection[0].Description, e.ComplexCollection[1].Description })
                            };
                    names = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(names, "only one result is expected");
                        names = e.Names.ToArray();
                        Assert.IsNull(context.GetEntityDescriptor(e), "The entity should not be tracked.");
                    }

                    ValidateNames(new string[] { baseComplexCollection[0].Description, baseComplexCollection[1].Description }, names);
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=ComplexCollection,ComplexCollection", q.ToString());
                }
                {
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();
                    var q = from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                            select new
                            {
                                firstComplexCollectionDescription = e.ComplexCollection[0].Description
                            };
                    string firstComplexCollectionDescription = null;
                    foreach (var e in q)
                    {
                        Assert.IsNull(firstComplexCollectionDescription, "only one result is expected");
                        firstComplexCollectionDescription = e.firstComplexCollectionDescription;
                        Assert.AreEqual(baseComplexCollection[0].Description, firstComplexCollectionDescription);
                        Assert.IsNull(context.GetEntityDescriptor(e), "The entity should not be tracked.");
                    }

                    Assert.IsNotNull(firstComplexCollectionDescription, "one result is expected");
                    Assert.AreEqual(request.BaseUri + "/Entities?$select=ComplexCollection", q.ToString());
                }
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void Collection_NotSupportedClientLINQQueries()
        {
            // Test not supported client LINQ queries.
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CollectionContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);

                var queries = new object[]
                    {
                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        where  "value" == e.NamesAsList[0]
                        select e,

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        let firstName = e.NamesAsList[0]
                        where  "value" == firstName
                        select e,

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        where  e.ID == 0 ? e.ComplexCollection[0].Description == "" : false
                        select e,

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        orderby e.Names
                        select e,

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        orderby e.ComplexCollection[0].Description
                        select e,

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        where e.ID == 0
                        select e.NamesAsList.Count(),

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        where e.ID == 0
                        select e.NamesAsList.Count,

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        select new CollectionEntityType()
                        {
                            Names = new DataServiceCollection<string>(e.Names, TrackingMode.None)
                        },

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        select new CollectionEntityType()
                        {
                            Names = e.Names.Where(str => str !="").ToList()
                        },

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        select new CollectionEntityType()
                        {
                            Names = new List<string>(new string[] {"a", "b"})
                        },

                        from e in ctx.CreateQuery<CollectionEntityType>("Entities")
                        where e.ID == 0
                        from names in e.Names
                        select names,

                        ctx.CreateQuery<CollectionEntityType>("Entities").Where(e1 => e1.ID == 0).SelectMany(e2 => e2.Names),
                    };


                TestUtil.RunCombinations(queries, (query) =>
                {
                    LinqTests.VerifyNotSupportedQuery((IQueryable)query);
                });
            }
        }
        #endregion client LINQ support for collection

        #region Collection Client: Client does not write complex types that have only collection properties.
        public class ComplexTypeWithCollectionOfArrays
        {
            public List<byte[]> ByteArray { get; set; }
            public List<char[]> CharArray { get; set; }
        }

        public class TestEntity
        {
            public int ID { get; set; }
            public ComplexTypeWithCollectionOfArrays ComplexType { get; set; }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void ClientShouldWriteComplexTypesWithOnlyCollectionProperties()
        {
            // Collection Client: Client does not write complex types that have only collection properties.
            TestEntity[] testEntities = new TestEntity[] {
                new TestEntity() {
                    ID = 2010,
                    ComplexType = new ComplexTypeWithCollectionOfArrays() {
                                ByteArray = new List<byte[]>(new [] {
                                    new byte[] { 0x20, 0x30, 0x40 },
                                    new byte[] { 0xa0, 0xb0, 0xc0 }}),
                                CharArray = new List<char[]>(new [] {
                                    new char[] { 'd', 'd', '1', '@' },
                                    new char[] { 'q', 'w', 'e', 'r', 't', 'y' }})
                    }},
                new TestEntity() {
                    ID = 2011,
                    ComplexType = new ComplexTypeWithCollectionOfArrays() {
                        ByteArray = new List<byte[]>(),
                        CharArray = new List<char[]>()
                    }
                }
            };

            using (TestWebRequest host = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.OverridingPlayback.Restore())
            using (PlaybackService.InspectRequestPayload.Restore())
            {
                host.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                host.StartService();

                PlaybackService.OverridingPlayback.Value =
                "HTTP/1.1 201 Created\r\n" +
                "Content-Length: 0\r\n" +
                "Location: " + host.BaseUri + "/\r\n" +
                "OData-EntityId: " + host.BaseUri + "/\r\n" +
                "\r\n";

                TestUtil.RunCombinations(new string[] { "POST", "PUT", "PATCH" }, testEntities, (httpMethod, entity) =>
                {
                    PlaybackService.InspectRequestPayload.Value = (payload) =>
                    {
                        XDocument payloadXml = XDocument.Load(payload);

                        VerifyItemAndItemPayloadMatch(payloadXml.Descendants(UnitTestsUtil.MetadataNamespace + "properties").Single(), entity);
                    };

                    DataServiceContext ctx = new DataServiceContext(new Uri(host.BaseUri), ODataProtocolVersion.V4);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();

                    SaveChangesOptions? saveChangesOptions = null;
                    switch (httpMethod)
                    {
                        case "PUT":
                        case "PATCH":
                            ctx.AttachTo("Entities", entity);
                            ctx.UpdateObject(entity);
                            saveChangesOptions = httpMethod == "PUT" ? SaveChangesOptions.ReplaceOnUpdate : SaveChangesOptions.None;
                            break;
                        case "POST":
                            ctx.AddObject("Entities", entity);
                            saveChangesOptions = SaveChangesOptions.None;
                            break;
                        default:
                            Assert.Fail("Only POST, PUT, PATCH verbs are supported");
                            break;
                    }

                    try
                    {
                        ctx.SaveChanges(saveChangesOptions.Value);
                    }
                    catch (AssertFailedException)
                    {
                        throw;
                    }
                });
            }
        }
        #endregion

        #region Collection_CollectionTypeRecognition
        public class CollectionTypeRecognitionEntity<T>
        {
            public int ID { get; set; }
            public T CollectionProperty { get; set; }
        }

        public class CollectionImplementation<T> : ICollection<T>
        {
            private List<T> storage;
            public CollectionImplementation() { storage = new List<T>(); }
            public void Add(T item) { this.storage.Add(item); }
            public void Clear() { this.storage.Clear(); }
            public bool Contains(T item) { throw new NotImplementedException(); }
            public void CopyTo(T[] array, int arrayIndex) { throw new NotImplementedException(); }
            public int Count { get { throw new NotImplementedException(); } }
            public bool IsReadOnly { get { throw new NotImplementedException(); } }
            public bool Remove(T item) { throw new NotImplementedException(); }
            public IEnumerator<T> GetEnumerator() { return this.storage.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return this.storage.GetEnumerator(); }
        }

        public class CollectionWithSettableProperty<T> : CollectionImplementation<T>
        {
            public string SettableProperty { get; set; }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void Collection_CollectionTypeRecognition()
        {
            // Verifies that certain types are recognized as collection or are not recognized as collection.
            var testCases = new[] {
                new {
                    Entity = (object)new CollectionTypeRecognitionEntity<int> { ID = 1, CollectionProperty = 42 },
                    IsCollection = false
                },
                new {
                    Entity = (object)new CollectionTypeRecognitionEntity<byte[]> { ID = 1, CollectionProperty = new byte[] { 1, 2, 3} },
                    IsCollection = false
                },
                new {
                    Entity = (object)new CollectionTypeRecognitionEntity<char[]> { ID = 1, CollectionProperty = new char[] { 'a', 'b', 'c'} },
                    IsCollection = false
                },
                new {
                    Entity = (object)new CollectionTypeRecognitionEntity<XElement> { ID = 1, CollectionProperty = new XElement("root") },
                    IsCollection = false
                },
                new {
                    Entity = (object)new CollectionTypeRecognitionEntity<List<int>> { ID = 1, CollectionProperty = new List<int> { 1, 2, 3} },
                    IsCollection = true
                },
                new {
                    Entity = (object)new CollectionTypeRecognitionEntity<CollectionWithSettableProperty<int>> { ID = 1, CollectionProperty = new CollectionWithSettableProperty<int> { 1, 2, 3} },
                    IsCollection = true
                },
            };

            PlaybackServiceDefinition service = new PlaybackServiceDefinition();
            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                TestUtil.RunCombinations(
                    testCases,
                    (testCase) =>
                    {
                        service.ProcessRequestOverride = (r) =>
                        {
                            XElement collectionProperty = XDocument.Load(r.GetRequestStream()).Root.Element(UnitTestsUtil.AtomNamespace + "content")
                                .Element(UnitTestsUtil.MetadataNamespace + "properties").Element(CollectionPropertyElementName);
                            Assert.IsNotNull(collectionProperty, "The collectionProperty is missing from the payload.");
                            if (testCase.IsCollection)
                            {
                                Assert.IsNotNull(collectionProperty.Elements(CollectionElementXName).FirstOrDefault(), "The type should be recognized as a collection and thus the payload should have he m:element child.");
                            }
                            else
                            {
                                Assert.IsNull(collectionProperty.Elements(CollectionElementXName).FirstOrDefault(), "The type should not be recognized as a collection and thus the payload should not have he m:element child.");
                            }

                            r.SetResponseStatusCode(204);
                            r.ResponseHeaders["Location"] = "http://test.org/service/entity(1)";
                            r.ResponseHeaders["OData-EntityId"] = "http://test.org/service/entity(1)";
                            return r;
                        };

                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        ctx.AddObject("entity", testCase.Entity);
                        ctx.SaveChanges();
                    });
            }
        }
        #endregion Collection_CollectionTypeRecognition

        #region Helper methods

        private static object InvokeWebUtilMethod(string methodName, object[] parameters)
        {
            try
            {
                Type webUtilType = typeof(DataServiceContext).Assembly.GetType("Microsoft.OData.Client.WebUtil");
                MethodInfo mi = webUtilType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(m => m.Name == methodName && m.GetParameters().Count() == parameters.Length).Single();
                return mi.Invoke(null, parameters);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static string CreatePayload(string payloadFragment, string baseUri, string navigationPropertyLinks = null)
        {
            return
            @"HTTP/1.1 200 OK
Server: ASP.NET Development Server/10.0.0.0
Date: Wed, 27 Jan 2010 18:06:26 GMT
X-AspNet-Version: 4.0.30107
OData-Version: 4.0;
Set-Cookie: ASP.NET_SessionId=d0ieqfv0tr5pfq4rqafszurj; path=/; HttpOnly
Cache-Control: no-cache
Content-Type: application/atom+xml;charset=utf-8
Connection: Close

<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <title type=""text"">Entities</title>
  <id>" + baseUri + @"</id>
  <updated>2010-01-27T18:06:26Z</updated>
  <link rel=""self"" title=""Entities"" href=""Entities"" />
  <entry>
    <id>" + baseUri + @"/Entities(5)</id>
    <title type=""text""></title>
    <updated>2010-01-27T18:06:26Z</updated>
    <author>
      <name />
    </author>
    <link rel=""edit"" title=""Entity"" href=""Entities(5)"" />
" + (navigationPropertyLinks ?? string.Empty) + @"
    <category term=""#WebApplication1.Entity"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">5</d:ID>"
                    + payloadFragment +
                   @"</m:properties>
    </content>
  </entry>
</feed>";
        }

        private static void VerifyCollectionPropertyMaterialization(XElement collectionPayload, object entity, bool collectionPreinitialized)
        {
            Type collectionParentType = null;
            object collectionParentInstance = null;
            GetCollectionParentForCollectionMaterializationTestCases(entity, out collectionParentType, out collectionParentInstance);
            object materializedCollectionInstance = collectionParentType.GetProperty("Collection").GetValue(collectionParentInstance, null);
            Type collectionGenericType = materializedCollectionInstance.GetType().GetGenericTypeDefinition();

            if (collectionGenericType == typeof(LoggingCollection<>))
            {
                List<string> log = (List<string>)typeof(LoggingCollection<>).MakeGenericType(new Type[] { materializedCollectionInstance.GetType().GetGenericArguments()[0] }).GetProperty("Log").GetValue(materializedCollectionInstance, null);
                VerifyOrderCall(log, collectionPayload, !collectionPreinitialized);
            }

            VerifyCollectionAndPayloadMatch(collectionPayload, (IEnumerable)materializedCollectionInstance);

            // The client will instantiate ObservableCollection only if the property was null. Therefore this check does not apply if initializeCollection 
            // is true since the ObservableCollection instance will be populated in the constructor and the client will re-use it instead of creating a new one. 
            if (collectionGenericType.IsInterface && !collectionPreinitialized)
            {
                Assert.AreEqual(typeof(ObservableCollection<>), materializedCollectionInstance.GetType().GetGenericTypeDefinition());
            }
        }

        private static void VerifyOrderCall(List<string> log, XElement collectionPayload, bool wasCreatedByContext)
        {
            int currLogEntryIdx = 0;

            if (!wasCreatedByContext)
            {
                Assert.AreEqual("Clear", log[currLogEntryIdx]);
                currLogEntryIdx++;
            }

            for (int i = 0; i < collectionPayload.Elements(CollectionElementXName).Count(); i++, currLogEntryIdx++)
            {
                Assert.IsTrue(currLogEntryIdx < log.Count, "Not all operations were logged (where do the extra elements come from?).");
                Assert.AreEqual("Add", log[currLogEntryIdx]);
            }

            Assert.AreEqual(currLogEntryIdx, log.Count, "Unexpected operation(s) found in the log.");
        }

        private static void VerifyCollectionAndPayloadMatch(XElement collectionPayload, IEnumerable collectionInstance)
        {
            // collection payload is null and the collection has not been initialized - it is fine to leave
            if (collectionPayload == null && collectionInstance == null)
            {
                return;
            }

            Assert.IsNotNull(collectionInstance, "Collection properties must not be null");

            foreach (var collectionItem in collectionInstance)
            {
                XElement collectionItemPayloadElement = collectionPayload.Elements(CollectionElementXName).FirstOrDefault();
                Assert.IsNotNull(collectionItemPayloadElement, "Payload does not contain this item.");

                VerifyItemAndItemPayloadMatch(collectionItemPayloadElement, collectionItem);
                collectionItemPayloadElement.Remove();
            }

            Assert.IsTrue(collectionPayload.Elements(CollectionElementXName).Count() == 0, "The payload contains some elements that are not present in the collection. Outstanding elements in the payload: " + collectionPayload.ToString());
        }

        private static void VerifyItemAndItemPayloadMatch(XElement itemPayloadElement, object item)
        {
            if (item == null)
            {
                // verify that if item is null the property either was not in the payload or was marked as null
                Assert.IsTrue(itemPayloadElement == null || (bool)itemPayloadElement.Attribute(UnitTestsUtil.MetadataNamespace + "null"), "item is null but the payload property for this item exists and is not marked as null.");
            }
            else if (IsPrimitiveType(item.GetType()))
            {
                if (itemPayloadElement == null)
                {
                    // this is to check if the value is default(T) if the property was not specified in the payload
                    Assert.AreEqual(Activator.CreateInstance(item.GetType()), item);
                }
                else
                {
                    string itemStringValue =
                            item is DateTimeOffset ? XmlConvert.ToString((DateTimeOffset)item) :
                            item is byte[] ? Convert.ToBase64String((byte[])item) :
                            item is char[] ? new string((char[])item) :
                                item.ToString();

                    XAttribute xmlSpaceAttr = itemPayloadElement.Attribute(XNamespace.Xml + "space");

                    if (!string.IsNullOrEmpty(itemStringValue))
                    {
                        if (XmlConvert.IsWhitespaceChar(itemStringValue[0]) || XmlConvert.IsWhitespaceChar(itemStringValue[itemStringValue.Length - 1]))
                        {
                            Assert.IsNotNull(xmlSpaceAttr, "xml:space attribute missing");
                            Assert.AreEqual((string)xmlSpaceAttr, "preserve", "xml:space=\"preserve\" expected, found: " + xmlSpaceAttr.ToString());
                        }
                    }

                    Assert.AreEqual((string)itemPayloadElement, itemStringValue);
                }
            }
            else
            {
                Assert.IsNotNull(itemPayloadElement, "Serialization error - the property value has not been serialized.");
                foreach (PropertyInfo pi in item.GetType().GetProperties())
                {
                    XElement propertyElement = itemPayloadElement.Elements(XName.Get(pi.Name, UnitTestsUtil.DataNamespace.NamespaceName)).SingleOrDefault();
                    object propertyValue = pi.GetValue(item, null);

                    // is this a collection? Note: navigation properties not handled correctly here as they are not expected to be present in the test data
                    if (!IsPrimitiveType(pi.PropertyType) && IsICollectionOf(pi.PropertyType))
                    {
                        if (propertyValue != null)
                        {
                            VerifyCollectionAndPayloadMatch(propertyElement, (IEnumerable)propertyValue);
                        }
                        else
                        {
                            Assert.IsNull(propertyElement, "property has not been initialized but payload contains data for this property.");
                        }
                    }
                    else
                    {
                        VerifyItemAndItemPayloadMatch(propertyElement, pi.GetValue(item, null));
                    }
                }
            }
        }

        private static bool IsPrimitiveType(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;

            return t == typeof(string) || t == typeof(int) || t == typeof(DateTimeOffset) || t == typeof(char[]) || t == typeof(byte[]);
        }

        private static bool IsICollectionOf(Type t)
        {
            while (t != null)
            {
                if (t.GetInterface("System.Collections.Generic.ICollection`1") != null ||
                    (t.IsInterface && t.GetGenericTypeDefinition() == typeof(ICollection<>)))
                {
                    return true;
                }

                t = t.BaseType;
            }

            return false;
        }

        private static string GetModelFullName(Type t)
        {
            MethodInfo methodInfo = typeof(DataServiceContext).Assembly.GetType("Microsoft.OData.Client.CommonUtil").GetMethod("GetModelTypeName", BindingFlags.NonPublic | BindingFlags.Static);
            string typeName = (string)methodInfo.Invoke(null, new object[] { t });
            return t.Namespace + "." + typeName;
        }

        #endregion
    }
}
