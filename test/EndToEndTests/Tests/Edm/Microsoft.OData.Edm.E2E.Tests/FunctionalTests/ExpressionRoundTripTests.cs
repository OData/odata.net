//---------------------------------------------------------------------
// <copyright file="ExpressionRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ExpressionRoundTripTests : EdmLibTestCaseBase
{
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTripInvalidTypeUsingCastCollectionCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendInfo"" Type=""NS.Friend"" />
    <ComplexType Name=""Friend"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""NickNames"" Type=""Collection(String)"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Annotations Target=""NS.FriendInfo"">
        <Annotation Term=""NS.FriendInfo"">
            <Cast Type=""NS.Address"">
                <Collection>
                    <String>foo</String>
                    <String>bar</String>
                </Collection>
            </Cast>
        </Annotation>
    </Annotations>
</Schema>";

        var expectedCsdls = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(expectedCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.True(!errors.Any());

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        
        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTripCastNullableToNonNullableCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendInfo"" Type=""Collection(NS.Friend)"" />
    <ComplexType Name=""Friend"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""NickNames"" Type=""Collection(String)"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Annotations Target=""NS.FriendInfo"">
        <Annotation Term=""NS.FriendInfo"">
            <Cast Type=""NS.Friend"">
                <Collection>
                    <String>foo</String>
                    <String>bar</String>
                </Collection>
            </Cast>
        </Annotation>
    </Annotations>
</Schema>";

        var expectedCsdls = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(expectedCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        
        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTripCastNullableToNonNullableOnInlineAnnotationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendInfo"" Type=""Collection(NS.Friend)"">
        <Annotation Term=""NS.FriendInfo"">
            <Cast Type=""NS.Friend"">
                <Collection>
                    <String>foo</String>
                    <String>bar</String>
                </Collection>
            </Cast>
        </Annotation>
    </Term>
    <ComplexType Name=""Friend"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""NickNames"" Type=""Collection(String)"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
</Schema>";

        var expectedCsdls = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(expectedCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        
        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTripCastResultFalseEvaluationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Friend"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Term Name=""FriendTerm"" Type=""NS.Friend""/>
    <Annotations Target=""NS.Friend"">
        <Annotation Term=""NS.FriendTerm"">
            <Record>
                <PropertyValue Property=""Name"" String=""foo"" />
                <PropertyValue Property=""Address"">
                    <Cast Type=""NS.Address"">
                        <Collection>
                            <String>foo</String>
                            <String>bar</String>
                        </Collection>
                    </Cast>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        var expectedCsdls = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(expectedCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        
        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTripCastResultTrueEvaluationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Friend"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Term Name=""FriendTerm"" Type=""NS.Friend"" />
    <Annotations Target=""NS.Friend"">
        <Annotation Term=""NS.FriendTerm"">
            <Record>
                <PropertyValue Property=""Name"" String=""foo"" />
                <PropertyValue Property=""Address"">
                    <Cast Type=""NS.Address"">
                        <Record>
                            <PropertyValue Property=""StreetNumber"" Int=""3"" />
                            <PropertyValue Property=""StreetName"" String=""에O詰　갂คำŚёæ"" />
                        </Record>
                    </Cast>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        var expectedCsdls = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(expectedCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        
        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTripInvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendName"" Type=""Edm.String"" />
    <Annotations Target=""NS.FriendName"">
        <Annotation Term=""NS.FriendName"">
            <IsType Type=""Edm.String"">
                <String>foo</String>
            </IsType>
        </Annotation>
    </Annotations>
</Schema>";

        var expectedCsdls = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(expectedCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        
        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTripInvalidPropertyTypeUsingIsTypeOnInlineAnnotationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""CarTerm"" Type=""NS.Car"" />
    <ComplexType Name=""Car"">
        <Property Name=""Expensive"" Type=""NS.Bike"" />
        <Annotation Term=""NS.CarTerm"">
            <Record>
                <PropertyValue Property=""Expensive"">
                    <IsType Type=""Edm.String"">
                        <String>foo</String>
                    </IsType>
                </PropertyValue>
            </Record>
        </Annotation>
    </ComplexType>
    <ComplexType Name=""Bike"">
        <Property Name=""Color"" Type=""String"" />
    </ComplexType>
</Schema>";

        var expectedCsdls = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(expectedCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        
        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTripIsTypeResultFalseEvaluationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""BooleanFlag"" Type=""Edm.Boolean"" />
    <Annotations Target=""NS.BooleanFlag"">
        <Annotation Term=""NS.BooleanFlag"">
            <IsType Type=""Edm.String"">
                <Int>32</Int>
            </IsType>
        </Annotation>
    </Annotations>
</Schema>";

        var expectedCsdls = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(expectedCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        
        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTripIsTypeResultTrueEvaluationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""BooleanFlag"">
        <Property Name=""Flag"" Type=""Edm.Boolean"" />
    </ComplexType>
    <Term Name=""BooleanFlagTerm"" Type=""NS.BooleanFlag"" />
    <Annotations Target=""NS.BooleanFlag"">
        <Annotation Term=""NS.BooleanFlagTerm"">
            <Record>
                <PropertyValue Property=""Flag"">
                    <IsType Type=""Edm.String"">
                        <String>foo</String>
                    </IsType>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        var expectedCsdls = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(expectedCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }

    private void Compare(List<XElement> expectXElements, List<XElement> actualXElements)
    {
        Assert.Equal(expectXElements.Count, actualXElements.Count);

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        this.CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace")?.Value));

            Assert.NotNull(actualXElement);

            Console.WriteLine("Expected: " + expectXElement.ToString());
            Console.WriteLine("Actual: " + actualXElement.ToString());

            this.CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    private static XElement ExtractElementByName(IEnumerable<XElement> inputSchemas, string elementNameToExtract)
    {
        if (inputSchemas == null || !inputSchemas.Any())
        {
            throw new InvalidOperationException("Needs at least one schema to extract element!");
        }

        XNamespace csdlXNamespace = inputSchemas.First().Name.Namespace;
        var containers = new XElement(csdlXNamespace + "Schema",
                                  new XAttribute("Namespace", "ExtractedElements"));

        foreach (var s in inputSchemas)
        {
            foreach (var c in s.Elements(csdlXNamespace + elementNameToExtract).ToArray())
            {
                c.Remove();
                containers.Add(c);
            }
        }

        return containers;
    }
}
