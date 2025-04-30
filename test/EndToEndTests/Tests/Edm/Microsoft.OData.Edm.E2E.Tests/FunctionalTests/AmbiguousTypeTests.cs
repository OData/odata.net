//---------------------------------------------------------------------
// <copyright file="AmbiguousTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.E2E.Tests.StubEdm;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class AmbiguousTypeTests : EdmLibTestCaseBase
{
    [Fact]
    public void Should_ReturnBadTerm_When_MultipleTermsWithSameNameExist()
    {
        EdmModel model = new EdmModel();

        IEdmTerm term1 = new EdmTerm("Foo", "Bar", EdmPrimitiveTypeKind.Byte);
        IEdmTerm term2 = new EdmTerm("Foo", "Bar", EdmPrimitiveTypeKind.Decimal);
        IEdmTerm term3 = new EdmTerm("Foo", "Bar", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, false));

        model.AddElement(term1);
        Assert.Equal(term1, model.FindTerm("Foo.Bar"));

        model.AddElement(term2);
        model.AddElement(term3);

        IEdmTerm ambiguous = model.FindTerm("Foo.Bar");
        Assert.True(ambiguous.IsBad(), "Ambiguous binding is bad");

        Assert.Equal(EdmSchemaElementKind.Term, ambiguous.SchemaElementKind);
        Assert.Equal("Foo", ambiguous.Namespace);
        Assert.Equal("Bar", ambiguous.Name);
        Assert.True(ambiguous.Type.IsBad(), "Type is bad.");
    }

    [Fact]
    public void Should_ReturnBadEntitySet_When_MultipleEntitySetsWithSameNameExist()
    {
        EdmEntityContainer container = new EdmEntityContainer("NS1", "Baz");

        IEdmEntitySet set1 = new StubEdmEntitySet("Foo", container);
        IEdmEntitySet set2 = new StubEdmEntitySet("Foo", container);
        IEdmEntitySet set3 = new StubEdmEntitySet("Foo", container);

        container.AddElement(set1);
        Assert.Equal(set3.Name, container.FindEntitySet("Foo").Name);

        container.AddElement(set2);
        container.AddElement(set3);

        IEdmEntitySet ambiguous = container.FindEntitySet("Foo");
        Assert.True(ambiguous.IsBad(), "Ambiguous binding is bad");

        Assert.Equal(EdmContainerElementKind.EntitySet, ambiguous.ContainerElementKind);
        Assert.Equal("NS1.Baz", ambiguous.Container.FullName());
        Assert.Equal("Foo", ambiguous.Name);
        Assert.True(ambiguous.EntityType.IsBad(), "Association is bad.");
    }

    [Fact]
    public void Should_ReturnBadType_When_MultipleTypesWithSameNameExist()
    {
        EdmModel model = new EdmModel();

        IEdmSchemaType type1 = new StubEdmComplexType("Foo", "Bar");
        IEdmSchemaType type2 = new StubEdmComplexType("Foo", "Bar");
        IEdmSchemaType type3 = new StubEdmComplexType("Foo", "Bar");

        model.AddElement(type1);
        Assert.Equal(type1, model.FindType("Foo.Bar"));

        model.AddElement(type2);
        model.AddElement(type3);

        IEdmSchemaType ambiguous = model.FindType("Foo.Bar");
        Assert.True(ambiguous.IsBad(), "Ambiguous binding is bad");

        Assert.Equal(EdmSchemaElementKind.TypeDefinition, ambiguous.SchemaElementKind);
        Assert.Equal("Foo", ambiguous.Namespace);
        Assert.Equal("Bar", ambiguous.Name);
        Assert.Equal(EdmTypeKind.None, ambiguous.TypeKind);
    }

    [Fact]
    public void Should_ReturnBadProperty_When_MultiplePropertiesWithSameNameExist()
    {
        EdmComplexType complex = new EdmComplexType("Bar", "Foo");

        StubEdmStructuralProperty prop1 = new StubEdmStructuralProperty("Foo");
        StubEdmStructuralProperty prop2 = new StubEdmStructuralProperty("Foo");
        StubEdmStructuralProperty prop3 = new StubEdmStructuralProperty("Foo");

        prop1.DeclaringType = complex;
        prop2.DeclaringType = complex;
        prop3.DeclaringType = complex;

        complex.AddProperty(prop1);
        complex.AddProperty(prop2);
        complex.AddProperty(prop3);

        IEdmProperty ambiguous = complex.FindProperty("Foo");
        Assert.True(ambiguous.IsBad(), "Ambiguous binding is bad");

        Assert.Equal(EdmPropertyKind.None, ambiguous.PropertyKind);
        Assert.Equal(complex, ambiguous.DeclaringType);
        Assert.True(ambiguous.Type.IsBad(), "Type is bad.");

        complex = new EdmComplexType("Bar", "Foo");
        prop3 = new StubEdmStructuralProperty("Foo");
        prop3.DeclaringType = complex;
        complex.AddProperty(prop3);
        Assert.Equal(prop3, complex.FindProperty("Foo"));
    }
}
