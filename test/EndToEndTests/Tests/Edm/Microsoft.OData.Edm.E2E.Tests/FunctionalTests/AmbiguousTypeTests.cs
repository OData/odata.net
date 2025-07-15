//---------------------------------------------------------------------
// <copyright file="AmbiguousTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
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

    [Fact]
    public void Should_IdentifyAmbiguousOperationsInModel()
    {
        // Arrange
        string csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Annotation"" Type=""Edm.Int32"" />
    <EntityType Name=""Entity"" >
        <Key>
            <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""False"" />
    </EntityType>
    <Function Name=""Function"">
        <Parameter Name=""Parameter"" Type=""Edm.String"" />
        <Parameter Name=""Parameter2"" Type=""Ref(DefaultNamespace.Entity)"" />
        <ReturnType Type=""Edm.Int32"" />
    </Function>
    <Function Name=""Function"">
        <Parameter Name=""Parameter"" Type=""Edm.String"" />
        <Parameter Name=""Parameter2"" Type=""Ref(DefaultNamespace.Entity)"" />
        <ReturnType Type=""Edm.Int32"" />
    </Function>
    <Function Name=""Function"">
        <Parameter Name=""Parameter"" Type=""Edm.String"" />
        <Parameter Name=""Parameter2"" Type=""Ref(DefaultNamespace.Entity)"" />
        <ReturnType Type=""Edm.Int32"" />
    </Function>
    <Annotations Target=""DefaultNamespace.Function(Edm.String, Ref(DefaultNamespace.Entity))"">
        <Annotation Term=""AnnotationNamespace.Annotation"">
            <Int>42</Int>
        </Annotation>
    </Annotations>
</Schema>";

        // Act & Assert
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmVocabularyAnnotation annotation = model.VocabularyAnnotations.First();
        IEdmOperation firstOperation = model.FindOperations("DefaultNamespace.Function").First();
        var ambiguousOperation = annotation.Target as IEdmOperation;
        Assert.NotNull(ambiguousOperation);
        Assert.Equal(EdmSchemaElementKind.Function, ambiguousOperation.SchemaElementKind);
        Assert.Null(ambiguousOperation.ReturnType);
        Assert.Equal("DefaultNamespace", ambiguousOperation.Namespace);
        Assert.Equal(firstOperation.Parameters.First(), ambiguousOperation.Parameters.First());
        Assert.Equal(firstOperation.FindParameter("Parameter"), ambiguousOperation.FindParameter("Parameter"));
    }

    [Fact]
    public void Should_IdentifyAmbiguousOperationImportsInEntityContainer()
    {
        // Arrange
        string csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Annotations Target=""DefaultNamespace.Container/FunctionImport"">
        <Annotation Term=""AnnotationNamespace.Annotation"">
          <Int>42</Int>
        </Annotation>
      </Annotations>
      <Action Name=""Operation"">
        <Parameter Name=""Parameter"" Type=""Edm.String"" />
        <ReturnType Type=""Edm.Int32"" />
      </Action>

     <EntityContainer Name=""Container"">
        <ActionImport Name=""FunctionImport"" Action=""DefaultNamespace.Operation"" />
        <ActionImport Name=""FunctionImport"" Action=""DefaultNamespace.Operation"" />
      </EntityContainer>
    </Schema>";

        // Act & Assert
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmVocabularyAnnotation annotation = model.VocabularyAnnotations.First();
        IEdmEntityContainer container = model.EntityContainer;
        IEdmOperationImport firstOperationImport = container.FindOperationImports("FunctionImport").First();

        var ambiguousOperationImport = annotation.Target as IEdmOperationImport;
        Assert.NotNull(ambiguousOperationImport);
        Assert.Equal(EdmContainerElementKind.ActionImport, ambiguousOperationImport.ContainerElementKind);
        Assert.Equal(firstOperationImport.Operation.ReturnType, ambiguousOperationImport.Operation.ReturnType);
        Assert.Equal(firstOperationImport.Operation.Parameters.First(), ambiguousOperationImport.Operation.Parameters.First());
        Assert.Equal(firstOperationImport.Operation.FindParameter("Parameter"), ambiguousOperationImport.Operation.FindParameter("Parameter"));
        Assert.Equal(container, ambiguousOperationImport.Container);
        Assert.Null(ambiguousOperationImport.EntitySet);
    }

    [Fact]
    public void Should_DetectUnresolvedAnnotationTargetsInModel()
    {
        // Arrange
        string csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Term"" Type=""Edm.Int32"" />
    <EntityType Name=""Entity"" >
        <Key>
            <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""False"" />
    </EntityType>
    <Function Name=""Function"">
        <Parameter Name=""Parameter"" Type=""Edm.String"" />
        <ReturnType Type=""Edm.Int32"" />
    </Function>
    <EntityContainer Name=""Container"">
        <FunctionImport Name=""FunctionImport"" Function=""DefaultNamespace.Function"" />
    </EntityContainer>
    <Annotations Target=""DefaultNamespace.Container/BadEntitySet"">
        <Annotation Term=""AnnotationNamespace.Term"">
            <Int>42</Int>
        </Annotation>
    </Annotations>
    <Annotations Target=""DefaultNamespace.Entity/BadProperty"">
        <Annotation Term=""AnnotationNamespace.Term"">
            <Int>42</Int>
        </Annotation>
    </Annotations>
    <Annotations Target=""DefaultNamespace.Function(Edm.String)/BadParameter"">
        <Annotation Term=""AnnotationNamespace.Annotation"">
            <Int>42</Int>
        </Annotation>
    </Annotations>
    <Annotations Target=""DefaultNamespace.BadEntity/BadProperty"">
        <Annotation Term=""AnnotationNamespace.Term"">
            <Int>42</Int>
        </Annotation>
    </Annotations>
    <Annotations Target=""Impossible/Target/With/Too/Long/A/Path"">
        <Annotation Term=""AnnotationNamespace.Term"">
            <Int>42</Int>
        </Annotation>
    </Annotations>
</Schema>";

        // Act & Assert
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal(5, model.VocabularyAnnotations.Count());
        var badEntitySet = model.VocabularyAnnotations.ElementAt(0).Target as IEdmEntitySet;
        var badProperty = model.VocabularyAnnotations.ElementAt(1).Target as IEdmStructuralProperty;
        var badOperationParameter = model.VocabularyAnnotations.ElementAt(2).Target as IEdmOperationParameter;
        var badPropertyWithBadType = model.VocabularyAnnotations.ElementAt(3).Target as IEdmStructuralProperty;
        var badElement = model.VocabularyAnnotations.ElementAt(4).Target as IEdmElement;

        Assert.NotNull(badEntitySet);
        Assert.NotNull(badProperty);
        Assert.NotNull(badOperationParameter);
        Assert.NotNull(badPropertyWithBadType);
        Assert.NotNull(badElement);

        Assert.True(badEntitySet.IsBad());
        Assert.True(badProperty.IsBad());
        Assert.Equal("BadUnresolvedProperty:BadProperty:[UnknownType Nullable=True]", badProperty.ToString());
        Assert.Equal("BadUnresolvedProperty:[UnknownType Nullable=True]", badProperty.Type.ToString());
        Assert.True(badOperationParameter.IsBad());
        Assert.True(badPropertyWithBadType.IsBad());
        Assert.True(badElement.IsBad());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Should_ValidateElementsWithKindOfNone(EdmVersion edmVersion)
    {
        // Arrange
        StubEdmModel model = new StubEdmModel();
        EdmEntityContainer container = new EdmEntityContainer("namespace", "container");
        model.Add(container);

        model.Add(new NoneKinds1("namespace", "badThing", container));
        var type = new EdmEntityType("namespace", "type");
        type.AddProperty(new NoneKinds2("namespace", "otherBadThing", EdmCoreModel.Instance.GetInt32(false), type));
        model.Add(type);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Act & Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Equal(7, actualErrors.Count());

        Assert.Equal(EdmErrorCode.TypeMustNotHaveKindOfNone, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.E2E.Tests.FunctionalTests.AmbiguousTypeTests+NoneKinds1)", actualErrors.ElementAt(0).ErrorLocation.ToString());
        Assert.Equal(EdmErrorCode.EntityContainerElementMustNotHaveKindOfNone, actualErrors.ElementAt(1).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.E2E.Tests.FunctionalTests.AmbiguousTypeTests+NoneKinds1)", actualErrors.ElementAt(1).ErrorLocation.ToString());
        Assert.Equal(EdmErrorCode.SchemaElementMustNotHaveKindOfNone, actualErrors.ElementAt(2).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.E2E.Tests.FunctionalTests.AmbiguousTypeTests+NoneKinds1)", actualErrors.ElementAt(2).ErrorLocation.ToString());
        Assert.Equal(EdmErrorCode.PrimitiveTypeMustNotHaveKindOfNone, actualErrors.ElementAt(3).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.E2E.Tests.FunctionalTests.AmbiguousTypeTests+NoneKinds2)", actualErrors.ElementAt(3).ErrorLocation.ToString());
        Assert.Equal(EdmErrorCode.SchemaElementMustNotHaveKindOfNone, actualErrors.ElementAt(4).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.E2E.Tests.FunctionalTests.AmbiguousTypeTests+NoneKinds2)", actualErrors.ElementAt(4).ErrorLocation.ToString());
        Assert.Equal(EdmErrorCode.TypeMustNotHaveKindOfNone, actualErrors.ElementAt(5).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.E2E.Tests.FunctionalTests.AmbiguousTypeTests+NoneKinds2)", actualErrors.ElementAt(5).ErrorLocation.ToString());
        Assert.Equal(EdmErrorCode.PropertyMustNotHaveKindOfNone, actualErrors.ElementAt(6).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.E2E.Tests.FunctionalTests.AmbiguousTypeTests+NoneKinds2)", actualErrors.ElementAt(6).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.False(serializedCsdls.Any());
    }

    [Fact]
    public void Should_ValidateSerializationBlockingErrorsInModel()
    {
        // Arrange
        EdmModel model = new EdmModel();

        EdmComplexType complexWithBadProperty = new EdmComplexType("Foo", "Bar");
        complexWithBadProperty.AddProperty(new EdmStructuralProperty(complexWithBadProperty, "baz", new EdmComplexTypeReference(new EdmComplexType("", ""), false)));
        model.AddElement(complexWithBadProperty);

        EdmEntityType baseType = new EdmEntityType("Foo", "");
        IEdmStructuralProperty keyProp = new EdmStructuralProperty(baseType, "Id", EdmCoreModel.Instance.GetInt32(false));
        baseType.AddProperty(keyProp);
        baseType.AddKeys(keyProp);

        EdmEntityType derivedType = new EdmEntityType("Foo", "Quip", baseType);
        model.AddElement(baseType);
        model.AddElement(derivedType);
        EdmNavigationProperty navProp = derivedType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "navProp", Target = derivedType, TargetMultiplicity = EdmMultiplicity.One });

        EdmEntityContainer container = new EdmEntityContainer("Foo", "Container");
        model.AddElement(container);
        container.AddElement(new EdmEntitySet(container, "badNameSet", baseType));

        // Act & Assert
        StringBuilder sb = new StringBuilder();
        bool written = model.TryWriteSchema(XmlWriter.Create(sb), out IEnumerable<EdmError> errors);
        Assert.False(written);

        Assert.Equal(EdmErrorCode.ReferencedTypeMustHaveValidName, errors.ElementAt(0).ErrorCode);
        Assert.Equal("([. Nullable=False])", errors.ElementAt(0).ErrorLocation.ToString());
        Assert.Equal(EdmErrorCode.ReferencedTypeMustHaveValidName, errors.ElementAt(1).ErrorCode);
        Assert.Equal("(Foo.Quip)", errors.ElementAt(1).ErrorLocation.ToString());
        Assert.Equal(EdmErrorCode.ReferencedTypeMustHaveValidName, errors.ElementAt(2).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.EdmEntitySet)", errors.ElementAt(2).ErrorLocation.ToString());
    }

    // Inline vocabulary annotation on an operation import nullrefs when computing the annotation's Term
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Should_HandleInlineLabeledElementsInAnnotations(EdmVersion edmVersion)
    {
        // Arrange
        const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Term"" Type=""Edm.Int32"" >
    <Annotation Term=""foo.Term"">
      <LabeledElement Name=""Exp1"" >
        <Int>1</Int>
      </LabeledElement>
    </Annotation>
  </Term>
  <Term Name=""CollectionTerm"" Type=""Collection(Edm.Int32)"">
    <Annotation Term=""foo.CollectionTerm"">
      <Collection>
        <LabeledElementReference Name=""Exp1"" />
        <LabeledElementReference Name=""Exp2"" />
        <LabeledElementReference Name=""Exp3"" />
        <LabeledElementReference Name=""Exp4"" />
        <LabeledElementReference Name=""Exp5"" />
        <LabeledElementReference Name=""Exp6"" />
        <LabeledElementReference Name=""Exp7"" />
        <LabeledElementReference Name=""Exp8"" />
      </Collection>
    </Annotation>
  </Term>
  <Term Name=""EntityTerm"" Type=""foo.Entity"" />
  <EntityType Name=""Entity"" >
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""False"" >
      <Annotation Term=""foo.EntityTerm"">
        <Record>
          <PropertyValue Property=""ID"">
            <LabeledElement Name=""Exp2"" >
              <Int>2</Int>
            </LabeledElement>
          </PropertyValue>
        </Record>
      </Annotation>
    </Property>
    <Annotation Term=""foo.Term"">
      <LabeledElement Name=""Exp3"" >
        <Int>3</Int>
      </LabeledElement>
    </Annotation>
  </EntityType>
  <Action Name=""FunctionImport"">
    <Parameter Name=""Parameter"" Type=""Edm.String"">
      <Annotation Term=""foo.Term"">
        <LabeledElement Name=""Exp4"" >
          <Int>4</Int>
        </LabeledElement>
      </Annotation>
    </Parameter>
    <ReturnType Type=""Edm.Int32"" />
    <Annotation Term=""foo.Term"">
      <LabeledElement Name=""Exp5"" >
        <Int>5</Int>
      </LabeledElement>
    </Annotation>
  </Action>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Set"" EntityType=""foo.Entity"">
      <Annotation Term=""foo.Term"">
        <LabeledElement Name=""Exp6"" >
          <Int>6</Int>
        </LabeledElement>
      </Annotation>
    </EntitySet>
    <ActionImport Name=""FunctionImport"" Action=""foo.FunctionImport"">
      <Annotation Term=""foo.Term"">
        <LabeledElement Name=""Exp7"" >
          <Int>8</Int>
        </LabeledElement>
      </Annotation>
    </ActionImport>
    <Annotation Term=""foo.Term"">
      <If>
        <LabeledElement Name=""Condition"">
          <Path>Id</Path>
        </LabeledElement>
        <LabeledElement Name=""Exp8"">
          <Int>100</Int>
        </LabeledElement>
        <LabeledElement Name=""IfFalse"">
          <Int>200</Int>
        </LabeledElement>
      </If>
    </Annotation>
  </EntityContainer>
</Schema>";

        // Act
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        IEdmModel? roundtrippedModel = null;
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError>? validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    class NoneKinds1 : IEdmType, IEdmEntityContainerElement, IEdmSchemaElement
    {
        public NoneKinds1(string namespaceName, string name, IEdmEntityContainer container)
        {
            this.Namespace = namespaceName;
            this.Name = name;
            this.Container = container;
            TypeKind = EdmTypeKind.None;
            SchemaElementKind = EdmSchemaElementKind.None;
            ContainerElementKind = EdmContainerElementKind.None;
        }

        public string Namespace { get; set; }
        public string Name { get; set; }
        public EdmTypeKind TypeKind { get; set; }
        public EdmSchemaElementKind SchemaElementKind { get; set; }
        public EdmContainerElementKind ContainerElementKind { get; set; }
        public IEdmEntityContainer Container { get; set; }
    }

    class NoneKinds2 : IEdmPrimitiveType, IEdmProperty
    {
        public NoneKinds2(string namespaceName, string name, IEdmTypeReference type, IEdmStructuredType declaringType)
        {
            this.Namespace = namespaceName;
            this.Name = name;
            this.Type = type;
            this.DeclaringType = declaringType;
            PropertyKind = EdmPropertyKind.None;
            SchemaElementKind = EdmSchemaElementKind.None;
            PrimitiveKind = EdmPrimitiveTypeKind.None;
            TypeKind = EdmTypeKind.None;
        }

        public string Namespace { get; set; }
        public string Name { get; set; }
        public IEdmTypeReference Type { get; set; }
        public IEdmStructuredType DeclaringType { get; set; }
        public EdmPropertyKind PropertyKind { get; set; }
        public EdmSchemaElementKind SchemaElementKind { get; set; }
        public EdmPrimitiveTypeKind PrimitiveKind { get; set; }
        public EdmTypeKind TypeKind { get; set; }
    }
}
