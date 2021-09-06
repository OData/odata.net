//---------------------------------------------------------------------
// <copyright file="CodeCoverageBoostingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using EdmLibTests.StubEdm;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CodeCoverageBoostingTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void AmbiguousOperationTest()
        {
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

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmVocabularyAnnotation annotation = model.VocabularyAnnotations.First();
            IEdmOperation firstOperation = model.FindOperations("DefaultNamespace.Function").First();
            IEdmOperation ambiguousOperation = annotation.Target as IEdmOperation;
            Assert.IsNotNull(ambiguousOperation, "Function not null");
            Assert.AreEqual(EdmSchemaElementKind.Function, ambiguousOperation.SchemaElementKind, "Correct schema element kind");
            Assert.IsNull(ambiguousOperation.ReturnType, "Null return type");
            Assert.AreEqual("DefaultNamespace", ambiguousOperation.Namespace, "Correct namespace");
            Assert.AreEqual(firstOperation.Parameters.First(), ambiguousOperation.Parameters.First(), "Function gets parameters from first function");
            Assert.AreEqual(firstOperation.FindParameter("Parameter"), ambiguousOperation.FindParameter("Parameter"), "Find parameter defers to first function");
        }

        [TestMethod]
        public void AmbiguousOperationImportTest()
        {
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

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmVocabularyAnnotation annotation = model.VocabularyAnnotations.First();
            IEdmEntityContainer container = model.EntityContainer;
            IEdmOperationImport firstOperationImport = container.FindOperationImports("FunctionImport").First();
            IEdmOperationImport ambiguousOperationImport = annotation.Target as IEdmOperationImport;
            Assert.IsNotNull(ambiguousOperationImport, "Function not null");
            Assert.AreEqual(EdmContainerElementKind.ActionImport, ambiguousOperationImport.ContainerElementKind, "Correct schema element kind");
            Assert.AreEqual(firstOperationImport.Operation.ReturnType, ambiguousOperationImport.Operation.ReturnType, "Function gets return type from first function");
            Assert.AreEqual(firstOperationImport.Operation.Parameters.First(), ambiguousOperationImport.Operation.Parameters.First(), "Function gets parameters from first function");
            Assert.AreEqual(firstOperationImport.Operation.FindParameter("Parameter"), ambiguousOperationImport.Operation.FindParameter("Parameter"), "Find parameter defers to first function");
            Assert.AreEqual(container, ambiguousOperationImport.Container, "Correct container");
            Assert.AreEqual(null, ambiguousOperationImport.EntitySet, "Correct container");
        }

        [TestMethod]
        public void UnresolvedAnnotationTargetsTest()
        {
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

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            Assert.AreEqual(5, model.VocabularyAnnotations.Count(), "Correct number of annotations");
            IEdmEntitySet badEntitySet = model.VocabularyAnnotations.ElementAt(0).Target as IEdmEntitySet;
            IEdmStructuralProperty badProperty = model.VocabularyAnnotations.ElementAt(1).Target as IEdmStructuralProperty;
            IEdmOperationParameter badOperationParameter = model.VocabularyAnnotations.ElementAt(2).Target as IEdmOperationParameter;
            IEdmStructuralProperty badPropertyWithBadType = model.VocabularyAnnotations.ElementAt(3).Target as IEdmStructuralProperty;
            IEdmElement badElement = model.VocabularyAnnotations.ElementAt(4).Target as IEdmElement;

            Assert.IsNotNull(badEntitySet, "Target not null");
            Assert.IsNotNull(badProperty, "Target not null");
            Assert.IsNotNull(badOperationParameter, "Target not null");
            Assert.IsNotNull(badPropertyWithBadType, "Target not null");
            Assert.IsNotNull(badElement, "Target not null");

            Assert.IsTrue(badEntitySet.IsBad(), "Target is bad");
            Assert.IsTrue(badProperty.IsBad(), "Target is bad");
            Assert.AreEqual("BadUnresolvedProperty:BadProperty:[UnknownType Nullable=True]", badProperty.ToString(), "Correct bad property to string");
            Assert.AreEqual("BadUnresolvedProperty:[UnknownType Nullable=True]", badProperty.Type.ToString(), "Correct bad property to string");
            Assert.IsTrue(badOperationParameter.IsBad(), "Target is bad");
            Assert.IsTrue(badPropertyWithBadType.IsBad(), "Target is bad");
            Assert.IsTrue(badElement.IsBad(), "Target is bad");
        }

        [TestMethod]
        public void ValidateKindsOfNone()
        {
            StubEdmModel model = new StubEdmModel();
            EdmEntityContainer container = new EdmEntityContainer("namespace", "container");
            model.Add(container);

            model.Add(new NoneKinds1("namespace", "badThing", container));
            var type = new EdmEntityType("namespace", "type");
            type.AddProperty(new NoneKinds2("namespace", "otherBadThing", EdmCoreModel.Instance.GetInt32(false), type));
            model.Add(type);

            var expectedErrors = new EdmLibTestErrors()
            {
                { "(EdmLibTests.FunctionalTests.CodeCoverageBoostingTests+NoneKinds1)", EdmErrorCode.TypeMustNotHaveKindOfNone },
                { "(EdmLibTests.FunctionalTests.CodeCoverageBoostingTests+NoneKinds1)", EdmErrorCode.EntityContainerElementMustNotHaveKindOfNone },
                { "(EdmLibTests.FunctionalTests.CodeCoverageBoostingTests+NoneKinds1)", EdmErrorCode.SchemaElementMustNotHaveKindOfNone },
                { "(EdmLibTests.FunctionalTests.CodeCoverageBoostingTests+NoneKinds2)", EdmErrorCode.PrimitiveTypeMustNotHaveKindOfNone },
                { "(EdmLibTests.FunctionalTests.CodeCoverageBoostingTests+NoneKinds2)", EdmErrorCode.SchemaElementMustNotHaveKindOfNone },
                { "(EdmLibTests.FunctionalTests.CodeCoverageBoostingTests+NoneKinds2)", EdmErrorCode.TypeMustNotHaveKindOfNone },
                { "(EdmLibTests.FunctionalTests.CodeCoverageBoostingTests+NoneKinds2)", EdmErrorCode.PropertyMustNotHaveKindOfNone },
            };
            this.VerifySemanticValidation(model, expectedErrors);

        }

        [TestMethod]
        public void ValidateSerializationBlockingErrors()
        {
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
            EdmNavigationProperty navProp = derivedType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo(){ Name="navProp", Target=derivedType, TargetMultiplicity=EdmMultiplicity.One });

            EdmEntityContainer container = new EdmEntityContainer("Foo", "Container");
            model.AddElement(container);
            container.AddElement(new EdmEntitySet(container, "badNameSet", baseType));

            StringBuilder sb = new StringBuilder();
            IEnumerable<EdmError> errors;
            bool written = model.TryWriteSchema(XmlWriter.Create(sb), out errors);
            var expectedErrors = new EdmLibTestErrors()
            {
                { "([. Nullable=False])", EdmErrorCode.ReferencedTypeMustHaveValidName },
                { "(Foo.Quip)", EdmErrorCode.ReferencedTypeMustHaveValidName },
                { "(Microsoft.OData.Edm.EdmEntitySet)", EdmErrorCode.ReferencedTypeMustHaveValidName },
            };

            this.CompareErrors(errors, expectedErrors);
        }

        // Inline vocabulary annotation on an operation import nullrefs when computing the annotation's Term
        [TestMethod]
        public void InlineLabeledElements()
        {
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
          <IsType Type=""Int32"">
            <LabeledElement Name=""IsTypeOperand"">
              <Path>Id</Path>
            </LabeledElement>
          </IsType>
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

            IEnumerable<EdmError> errors;
            IEdmModel model;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
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
}
