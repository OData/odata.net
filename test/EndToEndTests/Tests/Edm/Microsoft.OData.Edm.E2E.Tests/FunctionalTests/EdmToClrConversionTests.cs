//---------------------------------------------------------------------
// <copyright file="EdmToClrConversionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class EdmToClrConversionTests : EdmLibTestCaseBase
{
    [Fact]
    public void Convert_BinaryValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        byte[] b = { 1, 2, 3 };
        Assert.Equal(b, c.AsClrValue(new EdmBinaryConstant(b), typeof(byte[])));
        Assert.Equal(b, c.AsClrValue<byte[]>(new EdmBinaryConstant(b)));
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(byte[])));
    }

    [Fact]
    public void Convert_StringValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal("qwerty", c.AsClrValue(new EdmStringConstant("qwerty"), typeof(string)));
        Assert.Equal("qwerty", c.AsClrValue<string>(new EdmStringConstant("qwerty")));
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(string)));
    }

    [Fact]
    public void Convert_BooleanValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal(true, c.AsClrValue(new EdmBooleanConstant(true), typeof(bool)));
        Assert.True(c.AsClrValue<bool>(new EdmBooleanConstant(true)));

        bool? nullableBool = true;
        Assert.Equal(nullableBool, c.AsClrValue(new EdmBooleanConstant(true), typeof(bool?)));
        Assert.Equal(typeof(bool), c.AsClrValue(new EdmBooleanConstant(true), typeof(bool?)).GetType());
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(bool?)));
    }

    [Fact]
    public void Convert_CharValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal('q', c.AsClrValue(new EdmIntegerConstant('q'), typeof(char)));
        Assert.Equal('q', c.AsClrValue<char>(new EdmIntegerConstant('q')));

        char? nullableChar = 'q';
        Assert.Equal(nullableChar, c.AsClrValue(new EdmIntegerConstant('q'), typeof(char?)));
        Assert.Equal(typeof(char), c.AsClrValue(new EdmIntegerConstant('q'), typeof(char?)).GetType());
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(char?)));
    }

    [Fact]
    public void Convert_SByteValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((sbyte)(-12), c.AsClrValue(new EdmIntegerConstant(-12), typeof(sbyte)));
        Assert.Equal((sbyte)(-12), c.AsClrValue<sbyte>(new EdmIntegerConstant(-12)));

        sbyte? nullableSByte = -12;
        Assert.Equal(nullableSByte, ((sbyte?)c.AsClrValue(new EdmIntegerConstant(-12), typeof(sbyte?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(sbyte?)));
    }

    [Fact]
    public void Convert_ByteValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((byte)12, c.AsClrValue(new EdmIntegerConstant(12), typeof(byte)));
        Assert.Equal((byte)12, c.AsClrValue<byte>(new EdmIntegerConstant(12)));

        Assert.Throws<OverflowException>(() => c.AsClrValue<byte>(new EdmIntegerConstant(257)));

        byte? nullableByte = 12;
        Assert.Equal(nullableByte, ((byte?)c.AsClrValue(new EdmIntegerConstant(12), typeof(byte?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(byte?)));

        Assert.Throws<OverflowException>(() => c.AsClrValue(new EdmIntegerConstant(257), typeof(byte?)));
    }

    [Fact]
    public void Convert_Int16ValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((Int16)(-12), c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int16)));
        Assert.Equal((Int16)(-12), c.AsClrValue<Int16>(new EdmIntegerConstant(-12)));

        Int16? nullableInt16 = -12;
        Assert.Equal(nullableInt16, ((Int16?)c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int16?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(Int16?)));
    }

    [Fact]
    public void Convert_UInt16ValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((UInt16)12, c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt16)));
        Assert.Equal((UInt16)12, c.AsClrValue<UInt16>(new EdmIntegerConstant(12)));

        Assert.Equal((UInt16)12, ((UInt16?)c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt16?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(UInt16?)));
    }

    [Fact]
    public void Convert_Int32ValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((Int32)(-12), c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int32)));
        Assert.Equal((Int32)(-12), c.AsClrValue<Int32>(new EdmIntegerConstant(-12)));

        Assert.Equal((Int32)(-12), ((Int32?)c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int32?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(Int32?)));
    }

    [Fact]
    public void Convert_UInt32ValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((UInt32)12, c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt32)));
        Assert.Equal((UInt32)12, c.AsClrValue<UInt32>(new EdmIntegerConstant(12)));

        Assert.Equal((UInt32)12, ((UInt32?)c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt32?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(UInt32?)));
    }

    [Fact]
    public void Convert_Int64ValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((Int64)(-12), c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int64)));
        Assert.Equal((Int64)(-12), c.AsClrValue<Int64>(new EdmIntegerConstant(-12)));

        Assert.Equal((Int64)(-12), ((Int64?)c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int64?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(Int64?)));
    }

    [Fact]
    public void Convert_UInt64ValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((UInt64)12, c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt64)));
        Assert.Equal((UInt64)12, c.AsClrValue<UInt64>(new EdmIntegerConstant(12)));

        Assert.Equal((UInt64)12, ((UInt64?)c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt64?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(UInt64?)));
    }

    [Fact]
    public void Convert_DoubleValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((double)1.2, c.AsClrValue(new EdmFloatingConstant(1.2), typeof(double)));
        Assert.Equal((double)1.2, c.AsClrValue<double>(new EdmFloatingConstant(1.2)));

        Assert.Equal((double)1.2, ((double?)c.AsClrValue(new EdmFloatingConstant(1.2), typeof(double?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(double?)));
    }

    [Fact]
    public void Convert_SingleValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((Single)1.2, c.AsClrValue(new EdmFloatingConstant(1.2), typeof(Single)));
        Assert.Equal((Single)1.2, c.AsClrValue<Single>(new EdmFloatingConstant(1.2)));

        Assert.Equal((Single)1.2, ((Single?)c.AsClrValue(new EdmFloatingConstant(1.2), typeof(Single?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(Single?)));
    }

    [Fact]
    public void Convert_DecimalValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Assert.Equal((decimal)1.2, c.AsClrValue(new EdmDecimalConstant(1.2m), typeof(decimal)));
        Assert.Equal((decimal)1.2, c.AsClrValue<decimal>(new EdmDecimalConstant(1.2m)));

        Assert.Equal((decimal)1.2, ((decimal?)c.AsClrValue(new EdmDecimalConstant(1.2m), typeof(decimal?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(decimal?)));
    }

    [Fact]
    public void Convert_DateTimeOffsetValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        DateTime dt = new DateTime(2011, 10, 18, 14, 48, 16);
        DateTimeOffset dto = new DateTimeOffset(dt, new TimeSpan(-8, 0, 0));

        Assert.Equal(dto, c.AsClrValue(new EdmDateTimeOffsetConstant(dto), typeof(DateTimeOffset)));
        Assert.Equal(dto, c.AsClrValue<DateTimeOffset>(new EdmDateTimeOffsetConstant(dto)));

        Assert.Equal(dto, ((DateTimeOffset?)c.AsClrValue(new EdmDateTimeOffsetConstant(dto), typeof(DateTimeOffset?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(DateTimeOffset?)));
    }

    [Fact]
    public void Convert_DateValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        Date date = new Date(2014, 8, 8);

        Assert.Equal(date, c.AsClrValue(new EdmDateConstant(date), typeof(Date)));
        Assert.Equal(date, c.AsClrValue<Date>(new EdmDateConstant(date)));
        Assert.Equal(date, ((Date?)c.AsClrValue(new EdmDateConstant(date), typeof(Date?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(Date?)));
    }

    [Fact]
    public void Convert_TimeOfDayValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();
        TimeOfDay time = new TimeOfDay(12, 5, 30, 900);

        Assert.Equal(time, c.AsClrValue(new EdmTimeOfDayConstant(time), typeof(TimeOfDay)));
        Assert.Equal(time, c.AsClrValue<TimeOfDay>(new EdmTimeOfDayConstant(time)));
        Assert.Equal(time, ((TimeOfDay?)c.AsClrValue(new EdmTimeOfDayConstant(time), typeof(TimeOfDay?))).Value);
        Assert.Null(c.AsClrValue(EdmNullExpression.Instance, typeof(TimeOfDay?)));
    }

    [Fact]
    public void Convert_CollectionsToClrType_Successfully()
    {
        string[] elements = { "ab", "cd", "ef", null };
        EdmCollectionValue c = new EdmCollectionValue(
            EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)),
            elements.Select(e => e == null ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStringConstant(e)));

        EdmToClrConverter cv = new EdmToClrConverter();

        var converted = ((IEnumerable<string>)cv.AsClrValue(c, typeof(IEnumerable<string>))).ToArray();
        Assert.Equal(elements.Length, converted.Length);
        Assert.Equal("ab", converted[0]);
        Assert.Equal("cd", converted[1]);
        Assert.Equal("ef", converted[2]);
        Assert.Null(converted[3]);

        converted = cv.AsClrValue<IEnumerable<string>>(c).ToArray();
        Assert.Equal(elements.Length, converted.Length);
        Assert.Equal("ab", converted[0]);
        Assert.Equal("cd", converted[1]);
        Assert.Equal("ef", converted[2]);
        Assert.Null(converted[3]);
    }

    [Fact]
    public void Convert_EnumValuesToClrType_Successfully()
    {
        EdmToClrConverter c = new EdmToClrConverter();

        EdmEnumType e32 = new EdmEnumType("", "");
        var r32 = e32.AddMember("Red", new EdmEnumMemberValue(10));
        e32.AddMember("Blue", new EdmEnumMemberValue(20));

        EdmEnumValue evRed = new EdmEnumValue(new EdmEnumTypeReference(e32, false), r32);
        EdmEnumValue ev20 = new EdmEnumValue(new EdmEnumTypeReference(e32, false), new EdmEnumMemberValue(20));
        EdmEnumValue ev30 = new EdmEnumValue(new EdmEnumTypeReference(e32, false), new EdmEnumMemberValue(30));

        Assert.Equal(RedBlue.Red, c.AsClrValue<RedBlue>(evRed));
        Assert.Equal(RedBlue.Blue, c.AsClrValue<RedBlue>(ev20));
        Assert.Equal((RedBlue)30, c.AsClrValue<RedBlue>(ev30));

        Assert.Null(c.AsClrValue<RedBlue?>(EdmNullExpression.Instance));

        Assert.Equal(RedBlueByte.Red, c.AsClrValue<RedBlueByte>(evRed));
        Assert.Equal(RedBlueByte.Blue, c.AsClrValue<RedBlueByte>(ev20));
        Assert.Equal((RedBlueByte)30, c.AsClrValue<RedBlueByte>(ev30));

        Assert.Equal(RedBlueLong.Red, c.AsClrValue<RedBlueLong>(evRed));
        Assert.Equal(RedBlueLong.Blue, c.AsClrValue<RedBlueLong>(ev20));
        Assert.Equal((RedBlueLong)30, c.AsClrValue<RedBlueLong>(ev30));

        Assert.Equal(RedBlueLong.Red, c.AsClrValue(evRed, typeof(RedBlueLong)));
        Assert.Equal(RedBlueLong.Blue, c.AsClrValue(ev20, typeof(RedBlueLong)));
        Assert.Equal((RedBlueLong)30, c.AsClrValue(ev30, typeof(RedBlueLong)));
    }

    [Fact]
    public void Convert_StructuredValuesToClrType_Successfully()
    {
        IEdmStructuredValue c1_inner = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("P1", new EdmStringConstant("c1ip1")),
            new EdmPropertyValue("P3", new EdmIntegerConstant(110))
        });

        IEdmStructuredValue c2 = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("Q1", new EdmStringConstant("c2q1")),
            new EdmPropertyValue("Q2", c1_inner),
            new EdmPropertyValue("Q3", new EdmIntegerConstant(22))
        });

        IEdmStructuredValue c1 = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("P1", new EdmStringConstant("c1p1")),
            new EdmPropertyValue("P2", c2),
            new EdmPropertyValue("P3", new EdmIntegerConstant(11))
        });

        EdmToClrConverter cv = new EdmToClrConverter();

        C1[] clrC1s = { (C1)cv.AsClrValue(c1, typeof(C1)), cv.AsClrValue<C1>(c1) };

        Assert.All(clrC1s, clrC1 =>
        {
            // C1
            Assert.NotNull(clrC1);
            Assert.Equal("c1p1", clrC1.P1);
            Assert.Equal(11, clrC1.P3);

            // C1.P2
            Assert.NotNull(clrC1.P2);
            Assert.Equal("c2q1", clrC1.P2.Q1);
            Assert.Equal(22, clrC1.P2.Q3);

            // C1.P2.Q2
            Assert.NotNull(clrC1.P2.Q2);
            Assert.Equal("c1ip1", clrC1.P2.Q2.P1);
            Assert.Equal(110, clrC1.P2.Q2.P3);

            // C1.P2.Q2.P2
            Assert.Null(clrC1.P2.Q2.P2);
        });
    }

    [Fact]
    public void Convert_GraphWithLoopsToClrType_Successfully()
    {
        var c1_inner_loopProperty = new EdmPropertyValue("P2");

        IEdmStructuredValue c1_inner = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("P1", new EdmStringConstant("c1ip1")),
            c1_inner_loopProperty,
            new EdmPropertyValue("P3", new EdmIntegerConstant(110)),
            new EdmPropertyValue("NoClrCounterpart", new EdmIntegerConstant(110)) // conversion should ignore this and not choke
        });

        IEdmStructuredValue c2 = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("Q1", new EdmStringConstant("c2q1")),
            new EdmPropertyValue("Q2", c1_inner),
            new EdmPropertyValue("Q3", EdmNullExpression.Instance)
        });

        c1_inner_loopProperty.Value = c2;

        IEdmStructuredValue c1 = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("P1", new EdmStringConstant("c1p1")),
            new EdmPropertyValue("P2", c2),
            new EdmPropertyValue("P3", new EdmIntegerConstant(11))
        });

        EdmToClrConverter cv = new EdmToClrConverter();

        C1[] clrC1s = { (C1)cv.AsClrValue(c1, typeof(C1)), cv.AsClrValue<C1>(c1) };

        Assert.All(clrC1s, clrC1 =>
        {
            // C1
            Assert.NotNull(clrC1);
            Assert.Equal("c1p1", clrC1.P1);
            Assert.Equal(11, clrC1.P3);

            // C1.P2
            Assert.NotNull(clrC1.P2);
            Assert.Equal("c2q1", clrC1.P2.Q1);
            Assert.Null(clrC1.P2.Q3);

            // C1.P2.Q2
            Assert.NotNull(clrC1.P2.Q2);
            Assert.Equal("c1ip1", clrC1.P2.Q2.P1);
            Assert.Equal(110, clrC1.P2.Q2.P3);

            // C1.P2.Q2.P2
            Assert.True(Object.ReferenceEquals(clrC1.P2.Q2.P2, clrC1.P2));
        });
    }

    [Fact]
    public void Convert_CollectionPropertiesToClrType_Successfully()
    {
        string[] products1 = { "p11", "p12" };
        string[] products2 = { null, "p22" };
        string[] products3 = { "p31", "p32" };
        string[] products4 = { "p41", null };
        string[] products5 = { "p51", "p52" };
        string[] products6 = { "p61", "p62" };
        string[] products7 = { "p71", "p72" };
        int[] ints = { 1, 2, 3 };
        string[] productsPoly = { "p1", "d1", "p2", "d2", "D3" };

        EdmEntityType productType = new EdmEntityType("", "Product");
        productType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        IEdmEntityTypeReference productTypeRef = new EdmEntityTypeReference(productType, true);

        EdmEntityType derivedProductType = new EdmEntityType("", "Derived", productType);
        derivedProductType.AddStructuralProperty("Name2", EdmCoreModel.Instance.GetString(false));
        IEdmEntityTypeReference derivedProductTypeRef = new EdmEntityTypeReference(derivedProductType, true);

        EdmEntityType derived2ProductType = new EdmEntityType("", "Derived2", derivedProductType);
        IEdmEntityTypeReference derived2ProductTypeRef = new EdmEntityTypeReference(derived2ProductType, true);

        var edmProducts1 = products1.Select(p => new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })).Cast<IEdmDelayedValue>().ToList();

        IEdmStructuredValue category = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("Name", new EdmStringConstant("SomeCategoryX")),
            new EdmPropertyValue("Products1", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef), edmProducts1)),
            new EdmPropertyValue("Products11", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef), edmProducts1)),
            new EdmPropertyValue("Products2", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                products2.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
            new EdmPropertyValue("Products3", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                products3.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
            new EdmPropertyValue("Products4", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                products4.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
            new EdmPropertyValue("Products5", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                products5.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
            new EdmPropertyValue("Products6", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                products6.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
            new EdmPropertyValue("Products7", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                products7.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
            new EdmPropertyValue("ProductsNull", EdmNullExpression.Instance),
            new EdmPropertyValue("Ints", new EdmCollectionValue(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true)),
                ints.Select(p => new EdmIntegerConstant(p)).Cast<IEdmDelayedValue>())),
            new EdmPropertyValue("IntsEnum", new EdmCollectionValue(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true)),
                ints.Select(p => new EdmIntegerConstant(p*10)).Cast<IEdmDelayedValue>())),
            new EdmPropertyValue("ProductsPoly", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                productsPoly.Select(p =>
                    {
                        if (p.StartsWith("p"))
                            return new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) });
                        else if (p.StartsWith("d"))
                            return new EdmStructuredValue(derivedProductTypeRef, new IEdmPropertyValue[]
                            {
                                new EdmPropertyValue("Name", new EdmStringConstant(p)),
                                new EdmPropertyValue("Name2", new EdmStringConstant(p + "***")),
                            });
                        else
                            return new EdmStructuredValue(derived2ProductTypeRef, new IEdmPropertyValue[]
                            {
                                new EdmPropertyValue("Name", new EdmStringConstant(p)),
                                new EdmPropertyValue("Name2", new EdmStringConstant(p + "***")),
                            });
                    }).Cast<IEdmDelayedValue>())),
        });

        // Test polymorphic collection creation.
        EdmToClrConverter cv = new EdmToClrConverter(
            (IEdmStructuredValue edmValue, Type clrType, EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized) =>
            {
                objectInstance = null;
                objectInstanceInitialized = false;
                if (edmValue.Type != null && edmValue.Type.Definition.IsEquivalentTo(derivedProductType))
                {
                    objectInstance = new Derived();
                }
                else if (edmValue.Type != null && edmValue.Type.Definition.IsEquivalentTo(derived2ProductType))
                {
                    EdmToClrConverter clrConverter = new EdmToClrConverter();
                    objectInstance = new Derived2(clrConverter.AsClrValue<string>(edmValue.FindPropertyValue("Name").Value),
                                                  clrConverter.AsClrValue<string>(edmValue.FindPropertyValue("Name2").Value));
                    objectInstanceInitialized = true;
                }
                return objectInstance != null;
            });


        Category clrCategory = cv.AsClrValue<Category>(category);
        Assert.NotNull(clrCategory);
        Assert.Equal("SomeCategoryX", clrCategory.Name);

        Assert.Equal(2, clrCategory.Products1.Count);
        Assert.Equal("p11", clrCategory.Products1[0].Name);
        Assert.Equal("p12", clrCategory.Products1[1].Name);

        Assert.Equal(clrCategory.Products1.Count, clrCategory.Products11.Count);
        Assert.Equal(clrCategory.Products1[0], clrCategory.Products11[0]);
        Assert.Equal(clrCategory.Products1[1], clrCategory.Products11[1]);

        Assert.Equal(2, clrCategory.Products2.Count);
        Assert.Null(clrCategory.Products2.ElementAt(0));
        Assert.Equal("p22", clrCategory.Products2.ElementAt(1).Name);

        Assert.Equal(3, clrCategory.Products3.Count);
        Assert.Equal("sentinel3", clrCategory.Products3.ElementAt(0).Name);
        Assert.Equal("p31", clrCategory.Products3.ElementAt(1).Name);
        Assert.Equal("p32", clrCategory.Products3.ElementAt(2).Name);

        Assert.Equal(3, clrCategory.Products4.Count);
        Assert.Equal("sentinel4", clrCategory.Products4.ElementAt(0).Name);
        Assert.Equal("p41", clrCategory.Products4.ElementAt(1).Name);
        Assert.Null(clrCategory.Products4.ElementAt(2));

        Assert.Equal(3, clrCategory.Products5.Count);
        Assert.Equal("sentinel5", clrCategory.Products5.ElementAt(0).Name);
        Assert.Equal("p51", clrCategory.Products5.ElementAt(1).Name);
        Assert.Equal("p52", clrCategory.Products5.ElementAt(2).Name);

        Assert.Equal(3, clrCategory.Products6.Count);
        Assert.Equal("sentinel6", clrCategory.Products6.ElementAt(0).Name);
        Assert.Equal("p61", clrCategory.Products6.ElementAt(1).Name);
        Assert.Equal("p62", clrCategory.Products6.ElementAt(2).Name);

        Assert.Equal(3, clrCategory.Products7.Count);
        Assert.Equal("sentinel7", clrCategory.Products7.ElementAt(0).Name);
        Assert.Equal("p71", clrCategory.Products7.ElementAt(1).Name);
        Assert.Equal("p72", clrCategory.Products7.ElementAt(2).Name);

        Assert.Equal(3, clrCategory.Ints.Count);
        Assert.Equal(1, clrCategory.Ints.ElementAt(0));
        Assert.Equal(2, clrCategory.Ints.ElementAt(1));
        Assert.Equal(3, clrCategory.Ints.ElementAt(2));

        Assert.Equal(3, clrCategory.IntsEnum.Count());
        Assert.Equal(10, clrCategory.IntsEnum.ElementAt(0));
        Assert.Equal(20, clrCategory.IntsEnum.ElementAt(1));
        Assert.Equal(30, clrCategory.IntsEnum.ElementAt(2));

        Assert.Equal(5, clrCategory.ProductsPoly.Count);
        Assert.Equal("p1", clrCategory.ProductsPoly.ElementAt(0).Name);
        Assert.Equal("p1", clrCategory.ProductsPoly.ElementAt(0).Name);
        Assert.Equal("d1", clrCategory.ProductsPoly.ElementAt(1).Name);
        Assert.Equal("d1***", ((Derived)clrCategory.ProductsPoly.ElementAt(1)).Name2);
        Assert.Equal("p2", clrCategory.ProductsPoly.ElementAt(2).Name);
        Assert.Equal("d2", clrCategory.ProductsPoly.ElementAt(3).Name);
        Assert.Equal("d2***", ((Derived)clrCategory.ProductsPoly.ElementAt(3)).Name2);
        Assert.Equal("D3", clrCategory.ProductsPoly.ElementAt(4).Name);
        Assert.Equal("D3***", ((Derived2)clrCategory.ProductsPoly.ElementAt(4)).Name);
        Assert.Equal("D3", ((Derived2)clrCategory.ProductsPoly.ElementAt(4)).Name2);
    }

    [Fact]
    public void Convert_ToClassWithNewProperties_Successfully()
    {
        IEdmStructuredValue cc2 = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("P1", new EdmIntegerConstant(20)),
        });

        EdmToClrConverter cv = new EdmToClrConverter();

        Cc2 cc2Clr = cv.AsClrValue<Cc2>(cc2);

        Assert.Equal(((Cc1)cc2Clr).P1, null as string);
        Assert.Equal(20, cc2Clr.P1);
    }

    [Fact]
    public void Convert_CollectionOfCollectionsToClrType_Successfully()
    {
        var innerColType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
        var innerCol1 = new EdmCollectionValue(innerColType,
            new IEdmDelayedValue[] { new EdmIntegerConstant(1), new EdmIntegerConstant(2), new EdmIntegerConstant(3) });
        var innerCol2 = new EdmCollectionValue(innerColType,
            new IEdmDelayedValue[] { new EdmIntegerConstant(4), new EdmIntegerConstant(5), new EdmIntegerConstant(6) });

        var outerCol1 = new EdmCollectionValue(EdmCoreModel.GetCollection(innerColType),
            new IEdmDelayedValue[] { innerCol1, innerCol2 });

        EdmToClrConverter cv = new EdmToClrConverter();

        ICollection<ICollection<int>> colOfCol = cv.AsClrValue<ICollection<ICollection<int>>>(outerCol1);
        Assert.Equal(2, colOfCol.Count());
        Assert.Equal(3, colOfCol.ElementAt(0).Count());
        Assert.Equal(1, colOfCol.ElementAt(0).ElementAt(0));
        Assert.Equal(2, colOfCol.ElementAt(0).ElementAt(1));
        Assert.Equal(3, colOfCol.ElementAt(0).ElementAt(2));
        Assert.Equal(4, colOfCol.ElementAt(1).ElementAt(0));
        Assert.Equal(5, colOfCol.ElementAt(1).ElementAt(1));
        Assert.Equal(6, colOfCol.ElementAt(1).ElementAt(2));

        IList<IList<int>> listOfList = cv.AsClrValue<IList<IList<int>>>(outerCol1);
        Assert.Equal(2, listOfList.Count());
        Assert.Equal(3, listOfList.ElementAt(0).Count());
        Assert.Equal(1, listOfList.ElementAt(0).ElementAt(0));
        Assert.Equal(2, listOfList.ElementAt(0).ElementAt(1));
        Assert.Equal(3, listOfList.ElementAt(0).ElementAt(2));
        Assert.Equal(4, listOfList.ElementAt(1).ElementAt(0));
        Assert.Equal(5, listOfList.ElementAt(1).ElementAt(1));
        Assert.Equal(6, listOfList.ElementAt(1).ElementAt(2));

        IEnumerable<IEnumerable<int>> enumOfEnum = cv.AsClrValue<IEnumerable<IEnumerable<int>>>(outerCol1);
        Assert.Equal(2, enumOfEnum.Count());
        Assert.Equal(3, enumOfEnum.ElementAt(0).Count());
        Assert.Equal(1, enumOfEnum.ElementAt(0).ElementAt(0));
        Assert.Equal(2, enumOfEnum.ElementAt(0).ElementAt(1));
        Assert.Equal(3, enumOfEnum.ElementAt(0).ElementAt(2));
        Assert.Equal(4, enumOfEnum.ElementAt(1).ElementAt(0));
        Assert.Equal(5, enumOfEnum.ElementAt(1).ElementAt(1));
        Assert.Equal(6, enumOfEnum.ElementAt(1).ElementAt(2));
    }

    [Fact]
    public void Convert_CollectionOfCollectionsPropertiesToClrType_Successfully()
    {
        int i = 0;
        Func<IEdmValue> createColOfColValue = () =>
        {
            var innerColType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
            var innerCol1 = new EdmCollectionValue(innerColType,
                new IEdmDelayedValue[] { new EdmIntegerConstant(++i), new EdmIntegerConstant(++i), new EdmIntegerConstant(++i) });
            var innerCol2 = new EdmCollectionValue(innerColType,
                new IEdmDelayedValue[] { new EdmIntegerConstant(++i), new EdmIntegerConstant(++i), new EdmIntegerConstant(++i) });
            var outerCol = new EdmCollectionValue(EdmCoreModel.GetCollection(innerColType),
                new IEdmDelayedValue[] { innerCol1, innerCol2 });
            return outerCol;
        };

        IEdmStructuredValue colListEnumProp = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("ColCol", createColOfColValue()),
            new EdmPropertyValue("ColCol2", createColOfColValue()),
            new EdmPropertyValue("ColEnum", createColOfColValue()),
            new EdmPropertyValue("ColEnum2", createColOfColValue()),
            new EdmPropertyValue("ColList", createColOfColValue()),
            new EdmPropertyValue("ColList2", createColOfColValue()),
            new EdmPropertyValue("ListEnum", createColOfColValue()),
            new EdmPropertyValue("ListEnum2", createColOfColValue()),
            new EdmPropertyValue("EnumList", createColOfColValue()),
            new EdmPropertyValue("EnumEnum", createColOfColValue()),
        });

        EdmToClrConverter cv = new EdmToClrConverter();

        i = 0;
        Action<string, IEnumerable> checkColOfColValue = (name, value) =>
        {
            IEnumerable<IEnumerable<int>> enumOfEnum = value.Cast<IEnumerable<int>>();
            Assert.Equal(2, enumOfEnum.Count());
            Assert.Equal(3, enumOfEnum.ElementAt(0).Count());
            Assert.Equal(++i, enumOfEnum.ElementAt(0).ElementAt(0));
            Assert.Equal(++i, enumOfEnum.ElementAt(0).ElementAt(1));
            Assert.Equal(++i, enumOfEnum.ElementAt(0).ElementAt(2));
            Assert.Equal(++i, enumOfEnum.ElementAt(1).ElementAt(0));
            Assert.Equal(++i, enumOfEnum.ElementAt(1).ElementAt(1));
            Assert.Equal(++i, enumOfEnum.ElementAt(1).ElementAt(2));
        };

        ColListEnumProp colListEnumPropClr = cv.AsClrValue<ColListEnumProp>(colListEnumProp);
        checkColOfColValue("ColCol", colListEnumPropClr.ColCol);
        checkColOfColValue("ColCol2", colListEnumPropClr.ColCol2);
        checkColOfColValue("ColEnum", colListEnumPropClr.ColEnum);
        checkColOfColValue("ColEnum2", colListEnumPropClr.ColEnum2);
        checkColOfColValue("ColList", colListEnumPropClr.ColList);
        checkColOfColValue("ColList2", colListEnumPropClr.ColList2);
        checkColOfColValue("ListEnum", colListEnumPropClr.ListEnum);
        checkColOfColValue("ListEnum2", colListEnumPropClr.ListEnum2);
        checkColOfColValue("EnumList", colListEnumPropClr.EnumList);
        checkColOfColValue("EnumEnum", colListEnumPropClr.EnumEnum);
    }

    [Fact]
    public void Convert_DuplicatePropertyValues_ThrowsInvalidCastException()
    {
        IEdmStructuredValue cc1 = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("P1", new EdmStringConstant("1")),
            new EdmPropertyValue("P1", new EdmStringConstant("2")),
        });

        EdmToClrConverter cv = new EdmToClrConverter();

        var exception = Assert.Throws<InvalidCastException>(() => cv.AsClrValue<Cc2>(cc1));
        Assert.Contains("Unable to cast object of type 'Microsoft.OData.Edm.Vocabularies.EdmStringConstant' to type 'Microsoft.OData.Edm.Vocabularies.IEdmIntegerValue'.", exception.Message);
    }

    [Fact]
    public void Convert_NotSupportedCollectionProperties_ThrowsInvalidCastException()
    {
        var innerColType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
        IEdmStructuredValue colStruct = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("Prop", new EdmCollectionValue(innerColType,
                    new IEdmDelayedValue[] { new EdmIntegerConstant(1), new EdmIntegerConstant(2), new EdmIntegerConstant(3) })),
            new EdmPropertyValue("Prop2", new EdmIntegerConstant(1)),
        });

        EdmToClrConverter cv = new EdmToClrConverter();
        var exception = Assert.Throws<InvalidCastException>(() => cv.AsClrValue<NotSupportedCollectionProps>(colStruct));
        Assert.Contains("System.Int32[]", exception.Message);
        Assert.Contains("System.Collections.Generic.IEnumerable{T}", exception.Message);
        Assert.Contains("System.Collections.Generic.IList{T}", exception.Message);
        Assert.Contains("System.Collections.Generic.ICollection{T}", exception.Message);
        Assert.Equal("Conversion of an edm collection value to the CLR type 'System.Int32[]' is not supported. " +
            "EDM collection values can be converted to System.Collections.Generic.IEnumerable{T}, System.Collections.Generic.IList{T} or System.Collections.Generic.ICollection{T}.", exception.Message);

        cv = new EdmToClrConverter();
        var exception2 = Assert.Throws<InvalidCastException>(() => cv.AsClrValue<NotSupportedCollectionProps2>(colStruct));
        Assert.Contains("System.Collections.IEnumerable", exception2.Message);
        Assert.Contains("System.Collections.Generic.IEnumerable{T}", exception2.Message);
        Assert.Contains("System.Collections.Generic.IList{T}", exception2.Message);
        Assert.Contains("System.Collections.Generic.ICollection{T}", exception2.Message);
        Assert.Equal("Conversion of an edm collection value to the CLR type 'System.Collections.IEnumerable' is not supported. " +
            "EDM collection values can be converted to System.Collections.Generic.IEnumerable{T}, System.Collections.Generic.IList{T} or System.Collections.Generic.ICollection{T}.", exception2.Message);

        cv = new EdmToClrConverter();
        var exception3 = Assert.Throws<InvalidCastException>(() => cv.AsClrValue<NotSupportedCollectionProps3>(colStruct));
        Assert.Contains("IEdmIntegerConstantExpression", exception3.Message);
        Assert.Contains("Microsoft.OData.Edm.E2E.Tests.FunctionalTests.EdmToClrConversionTests+NotSupportedCollectionProps", exception3.Message);
        Assert.Equal("Conversion of an EDM value of the type 'IEdmIntegerConstantExpression' to the CLR type " +
            "'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.EdmToClrConversionTests+NotSupportedCollectionProps' is not supported.", exception3.Message);
    }

    [Fact]
    public void Convert_WithTryCreateObjectInstanceReturningWrongObject_ThrowsInvalidCastException()
    {
        IEdmStructuredValue colStruct = new EdmStructuredValue(null, new IEdmPropertyValue[]
        {
            new EdmPropertyValue("Prop", new EdmIntegerConstant(1))
        });

        var cv = new EdmToClrConverter((IEdmStructuredValue edmValue, Type clrType, EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized) =>
        {
            objectInstance = new object();
            objectInstanceInitialized = false;
            return true;
        });

        var exception = Assert.Throws<InvalidCastException>(() => cv.AsClrValue<NotSupportedCollectionProps2>(colStruct));
        Assert.Contains("System.Object", exception.Message);
        Assert.Contains("TryCreateObjectInstance", exception.Message);
        Assert.Contains("Microsoft.OData.Edm.E2E.Tests.FunctionalTests.EdmToClrConversionTests+NotSupportedCollectionProps2", exception.Message);
        Assert.Equal("The type 'System.Object' of the object returned by the TryCreateObjectInstance delegate " +
            "is not assignable to the expected type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.EdmToClrConversionTests+NotSupportedCollectionProps2'.", exception.Message);
    }

    #region Local variables for tests

    public enum RedBlue : int
    {
        Red = 10,
        Blue = 20
    }

    public enum RedBlueByte : byte
    {
        Red = 10,
        Blue = 20
    }

    public enum RedBlueLong : long
    {
        Red = 10,
        Blue = 20
    }

    public class ColListEnumProp
    {
        public ICollection<ICollection<int>> ColCol { get; set; }
        public ICollection<ICollection<int>> ColCol2 { get { return this.colCol2; } }
        private List<ICollection<int>> colCol2 = new List<ICollection<int>>();

        public ICollection<IEnumerable<int>> ColEnum { get; set; }
        public ICollection<IEnumerable<int>> ColEnum2 { get { return this.colEnum2; } }
        private List<IEnumerable<int>> colEnum2 = new List<IEnumerable<int>>();

        public ICollection<IList<int>> ColList { get; set; }
        public ICollection<IList<int>> ColList2 { get { return this.colList2; } }
        private List<IList<int>> colList2 = new List<IList<int>>();

        public IList<IEnumerable<int>> ListEnum { get; set; }
        public IList<IEnumerable<int>> ListEnum2 { get { return this.listEnum2; } }
        private List<IEnumerable<int>> listEnum2 = new List<IEnumerable<int>>();

        public IEnumerable<IList<int>> EnumList { get; set; }

        public IEnumerable<IEnumerable<int>> EnumEnum { get; set; }
    }

    public abstract class C1Base
    {
        public string P1 { get; set; }
    }

    public class C1 : C1Base
    {
        public C2 P2 { get; set; }
        public int P3 { get; set; }
    }

    public class C2
    {
        public C2()
        {
        }

        public string Q1 { get; set; }
        public C1 Q2 { get; set; }
        public int? Q3 { get; set; }
    }

    public class Product
    {
        public string Name { get; set; }
    }

    public class Derived : Product
    {
        public string Name2 { get; set; }
    }

    public class Cc1
    {
        public string P1 { get; set; }
    }

    public class Cc2 : Cc1
    {
        new public int P1 { get; set; }
    }

    public class NotSupportedCollectionProps
    {
        public int[] Prop { get; set; }
    }

    public class NotSupportedCollectionProps2
    {
        public IEnumerable Prop { get; set; }
    }

    public class NotSupportedCollectionProps3
    {
        public NotSupportedCollectionProps Prop2 { get; set; }
    }

    public class Derived2 : Derived
    {
        public Derived2(string name, string name2)
        {
            base.Name = name;
            base.Name2 = name2;
        }

        new public string Name { get { return base.Name2; } }
        new public string Name2 { get { return base.Name; } }
    }

    public class MyList<T> : List<T>, IList<int>
    {
        #region IList<int>
        int IList<int>.IndexOf(int item)
        {
            throw new NotImplementedException();
        }

        void IList<int>.Insert(int index, int item)
        {
            throw new NotImplementedException();
        }

        void IList<int>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        int IList<int>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection<int>.Add(int item)
        {
            throw new NotImplementedException();
        }

        void ICollection<int>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<int>.Contains(int item)
        {
            throw new NotImplementedException();
        }

        void ICollection<int>.CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<int>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<int>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<int>.Remove(int item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ProductsList : List<Product>, ICollection<int>
    {
        #region ICollection<int>
        void ICollection<int>.Add(int item)
        {
            throw new NotImplementedException();
        }

        void ICollection<int>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<int>.Contains(int item)
        {
            throw new NotImplementedException();
        }

        void ICollection<int>.CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<int>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<int>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<int>.Remove(int item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class MyProductsList : MyList<Product>, IList<int>
    {
        #region IList<int>
        int IList<int>.IndexOf(int item)
        {
            throw new NotImplementedException();
        }

        void IList<int>.Insert(int index, int item)
        {
            throw new NotImplementedException();
        }

        void IList<int>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        int IList<int>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection<int>.Add(int item)
        {
            throw new NotImplementedException();
        }

        void ICollection<int>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<int>.Contains(int item)
        {
            throw new NotImplementedException();
        }

        void ICollection<int>.CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<int>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<int>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<int>.Remove(int item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class Category
    {
        public Category()
        {
            Products3.Add(new Product() { Name = "sentinel3" });
            Products4.Add(new Product() { Name = "sentinel4" });
            Products5.Add(new Product() { Name = "sentinel5" });
            Products6.Add(new Product() { Name = "sentinel6" });
            Products7.Add(new Product() { Name = "sentinel7" });
        }

        public string Name { get; set; }

        public IList<Product> Products1 { get; set; }

        public IList<Product> Products11 { get; set; }

        public ICollection<Product> Products2 { get; set; }

        public IList<Product> Products3
        {
            get { return products3; }
        }
        private List<Product> products3 = new List<Product>();

        public ICollection<Product> Products4
        {
            get { return products4; }
        }
        private Collection<Product> products4 = new Collection<Product>();

        public IList<Product> Products5
        {
            get { return (IList<Product>)products5; }
        }
        private MyList<Product> products5 = new MyList<Product>();

        public IList<Product> Products6
        {
            get { return products6; }
        }
        private ProductsList products6 = new ProductsList();

        public IList<Product> Products7
        {
            get { return (IList<Product>)products7; }
        }
        private MyProductsList products7 = new MyProductsList();

        public IList<int> Ints { get; set; }

        public IEnumerable<int> IntsEnum { get; set; }

        public IList<Product> ProductsPoly { get; set; }
    }

    #endregion
}