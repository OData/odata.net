//---------------------------------------------------------------------
// <copyright file="XElementAnnotationModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Vocabularies;

    public static class XElementAnnotationModelBuilder
    {
        public static IEnumerable<XElement> AnnotationWithoutChildrenCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""ComplexType"">
        <Property Name=""Data"" Type=""String"" />
        <Annotation xmlns=""http://foo"" />
    </ComplexType>
</Schema>");
        }

        public static EdmModel AnnotationWithoutChildrenModel()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("DefaultNamespace", "ComplexType");
            complexType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));
            model.AddElement(complexType);

            XElement annotationElement = new XElement("{http://foo}Annotation");
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(complexType, "http://foo", "Annotation", annotation);

            return model;
        }

        public static IEnumerable<XElement> NestedXElementWithNoValueCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""SimpleType"">
    <Property Name=""Id"" Type=""Edm.Int32"">
      <Annotation xmlns=""http://foo"">
        <Child>
          <GrandChild>
            <GreatGrandChild>
              <GreateGreatGrandChild />
            </GreatGrandChild>
          </GrandChild>
        </Child>
      </Annotation>
    </Property>
  </ComplexType>
</Schema>");
        }

        public static EdmModel NestedXElementWithNoValueModel()
        {
            EdmModel model = new EdmModel();

            EdmComplexType simpleType = new EdmComplexType("DefaultNamespace", "SimpleType");
            EdmStructuralProperty simpleTypeId = simpleType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(simpleType);

            XElement annotationElement = 
                new XElement("{http://foo}Annotation",
                    new XElement("{http://foo}Child",
                        new XElement("{http://foo}GrandChild",
                            new XElement("{http://foo}GreatGrandChild",
                                new XElement("{http://foo}GreateGreatGrandChild")
                            )
                        )
                    )
                );
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(simpleTypeId, "http://foo", "Annotation", annotation);

            return model;
        }

        public static IEnumerable<XElement> NestedXElementWithValueCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""SimpleFunction"">
    <ReturnType Type=""Edm.String""/>
    <Annotation xmlns=""http://foo"">1<Child>2<GrandChild>3<GreatGrandChild>4<GreateGreatGrandChild>5</GreateGreatGrandChild></GreatGrandChild></GrandChild></Child></Annotation>
  </Function>
</Schema>");
        }

        public static EdmModel NestedXElementWithValueModel()
        {
            EdmModel model = new EdmModel();

            EdmOperation simpleOperation = new EdmFunction("DefaultNamespace", "SimpleFunction", EdmCoreModel.Instance.GetString(true));
            model.AddElement(simpleOperation);

            XElement annotationElement =
                new XElement("{http://foo}Annotation",
                    "1",
                    new XElement("{http://foo}Child",
                        "2",
                        new XElement("{http://foo}GrandChild",
                            "3",
                            new XElement("{http://foo}GreatGrandChild",
                                "4",
                                new XElement("{http://foo}GreateGreatGrandChild", "5")
                            )
                        )
                    )
                );
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(simpleOperation, "http://foo", "Annotation", annotation);

            return model;
        }


        public static IEnumerable<XElement> AnnotationWithValueCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""SimpleFunction"">
    <ReturnType Type=""Edm.String""/>
    <Parameter Name=""Id"" Type=""Edm.Int32"">
      <Annotation xmlns=""http://foo"">Value 1.0</Annotation>
    </Parameter>
  </Function>
</Schema>");
        }

        public static EdmModel AnnotationWithValueModel()
        {
            EdmModel model = new EdmModel();

            EdmOperation simpleOperation = new EdmFunction("DefaultNamespace", "SimpleFunction", EdmCoreModel.Instance.GetString(true));
            EdmOperationParameter simpleOperationId = new EdmOperationParameter(simpleOperation, "Id", EdmCoreModel.Instance.GetInt32(true));
            simpleOperation.AddParameter(simpleOperationId);
            model.AddElement(simpleOperation);

            XElement annotationElement =
                new XElement("{http://foo}Annotation",
                    "Value 1.0"
                );
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(simpleOperationId, "http://foo", "Annotation", annotation);

            return model;
        }

        public static IEnumerable<XElement> AnnotationWithSchemaTagValueCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""SimpleType"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Annotation xmlns=""http://foo"">&lt;/Schema&gt;</Annotation>
  </EntityType>
</Schema>");
        }

        public static EdmModel AnnotationWithSchemaTagValueModel()
        {
            EdmModel model = new EdmModel();

            EdmEntityType simpleType = new EdmEntityType("DefaultNamespace", "SimpleType");
            EdmStructuralProperty simpleTypeId = simpleType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            simpleType.AddKeys(simpleTypeId);
            model.AddElement(simpleType);

            XElement annotationElement =
                new XElement("{http://foo}Annotation",
                    "</Schema>"
                );
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(simpleType, "http://foo", "Annotation", annotation);

            return model;
        }

        public static IEnumerable<XElement> AnnotationWithEntitySetTagInEntityContainerCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""Container"">
    <EntitySet xmlns=""http://foo"">1</EntitySet>
  </EntityContainer>
</Schema>");
        }

        public static EdmModel AnnotationWithEntitySetTagInEntityContainerModel()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            model.AddElement(container);

            XElement annotationElement =
                new XElement("{http://foo}EntitySet",
                    "1"
                );
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(container, "http://foo", "EntitySet", annotation);

            return model;
        }

        public static IEnumerable<XElement> DifferentAnnotationNamespaceCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""Container"">
    <EntitySet Name=""SimpleSet"" EntityType=""DefaultNamespace.SimpleType"">
      <Annotation xmlns=""http://foo""><Child xmlns=""http://foo1""><GrandChild xmlns=""http://foo2"">1</GrandChild></Child></Annotation>
    </EntitySet>
  </EntityContainer>
  <EntityType Name=""SimpleType"">
    <Key>
      <PropertyRef Name=""Id""/>
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
</Schema>");
        }

        public static EdmModel DifferentAnnotationNamespaceModel()
        {
            EdmModel model = new EdmModel();

            EdmEntityType simpleType = new EdmEntityType("DefaultNamespace", "SimpleType");
            EdmStructuralProperty simpleTypeId = simpleType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            simpleType.AddKeys(simpleTypeId);
            model.AddElement(simpleType);

            EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "Container");
            EdmEntitySet simpleSet = container.AddEntitySet("SimpleSet", simpleType);

            model.AddElement(container);

            XElement annotationElement =
                new XElement("{http://foo}Annotation",
                    new XElement("{http://foo1}Child",
                        new XElement("{http://foo2}GrandChild",
                            "1"
                        )
                    )
                );
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(simpleSet, "http://foo", "Annotation", annotation);

            return model;
        }

        public static EdmModel SetChildAnnotationAsAnnotationModel()
        {
            EdmModel model = new EdmModel();

            EdmTerm note = new EdmTerm("DefaultNamespace", "Note", EdmPrimitiveTypeKind.Int16);
            model.AddElement(note);

            XElement annotationElement =
                new XElement("{http://foo}Annotation",
                    new XElement("{http://foo1}Child",
                      "1"
                    )
                );
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(note, "http://foo1", "Child", annotation);

            return model;
        }

        public static IEnumerable<XElement> ComplexNamespaceOverlappingCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Action Name=""SimpleFunction"">
    <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
  </Action>
  <EntityContainer Name=""Container"">
    <ActionImport Action=""Default.SimpleFunction"" Name=""SimpleFunction"">
      <Annotation xmlns=""http://foo"">
        <Child>
          <GrandChild xmlns=""http://foo1"">
            <GreatGrandChild xmlns=""http://foo"">1</GreatGrandChild>
          </GrandChild>
        </Child>
        <Child xmlns=""http://foo1"">
          <GrandChild xmlns=""http://foo"">1</GrandChild>
        </Child>
      </Annotation>
    </ActionImport>
  </EntityContainer>
</Schema>");
        }

        public static EdmModel ComplexNamespaceOverlappingModel()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction simpleOperationAction = new EdmAction("Default", "SimpleFunction", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(simpleOperationAction);
            EdmActionImport simpleOperation = new EdmActionImport(container, "SimpleFunction", simpleOperationAction);
            container.AddElement(simpleOperation);
            model.AddElement(container);

            XElement annotationElement =
                new XElement("{http://foo}Annotation",
                    new XElement("{http://foo}Child",
                        new XElement("{http://foo1}GrandChild",
                            new XElement("{http://foo}GreatGrandChild",
                              "1"
                            )
                        )
                    ),
                    new XElement("{http://foo1}Child",
                        new XElement("{http://foo}GrandChild",
                          "1"
                        )
                    )
                );
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(simpleOperation, "http://foo", "Annotation", annotation);

            return model;
        }

        public static IEnumerable<XElement> ActionImportParameterWithAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Action Name=""SimpleFunction"">
    <Parameter Name=""Name"" Nullable=""true"" Type=""Edm.String"">
      <Annotation xmlns=""http://foo"">1</Annotation>
    </Parameter>
    <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
  </Action>
  <EntityContainer Name=""Container"">
    <ActionImport Action=""Default.SimpleFunction"" Name=""SimpleFunction"" />
  </EntityContainer>
</Schema>");
        }

        public static EdmModel OperationImportParameterWithAnnotationModel()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction simpleOperationAction = new EdmAction("Default", "SimpleFunction", EdmCoreModel.Instance.GetInt32(false));
            EdmOperationParameter simpleOperationName = new EdmOperationParameter(simpleOperationAction, "Name", EdmCoreModel.Instance.GetString(true));
            simpleOperationAction.AddParameter(simpleOperationName);
            model.AddElement(simpleOperationAction);
            EdmOperationImport simpleOperation = new EdmActionImport(container, "SimpleFunction", simpleOperationAction);
            container.AddElement(simpleOperation);
            model.AddElement(container);

            var annotationElement = 
                new XElement("{http://foo}Annotation", 1);
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(simpleOperationName, "http://foo", "Annotation", annotation);

            return model;
        }

        public static IEnumerable<XElement> TermWithAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Note"" Type=""Edm.String"">
    <Annotation xmlns=""http://foo"">1</Annotation>
  </Term>
</Schema>");
        }

        public static EdmModel TermWithAnnotationModel()
        {
            EdmModel model = new EdmModel();

            EdmTerm note = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetString(true));
            model.AddElement(note);

            var annotationElement =
                new XElement("{http://foo}Annotation", 1);
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(note, "http://foo", "Annotation", annotation);

            return model;
        }

        public static IEnumerable<XElement> EnumWithAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EnumType Name=""Spicy"">
    <Annotation xmlns=""http://foo"">1</Annotation>
  </EnumType>
</Schema>");
        }

        public static EdmModel EnumWithAnnotationModel()
        {
            EdmModel model = new EdmModel();

            EdmEnumType spicy = new EdmEnumType("DefaultNamespace", "Spicy");
            model.AddElement(spicy);

            XElement annotationElement =
                new XElement("{http://foo}Annotation", "1");
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(spicy, "http://foo", "Annotation", annotation);

            return model;
        }

        public static IEnumerable<XElement> NavigationPropertyWithAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Friends"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""Self"">
      <Annotation xmlns=""http://foo"">1</Annotation>
    </NavigationProperty>
    <NavigationProperty Name=""Self"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""Friends"" />
  </EntityType>
</Schema>");
        }

        public static EdmModel NavigationPropertyWithAnnotationModel()
        {
            EdmModel model = new EdmModel();

            EdmEntityType person = new EdmEntityType("DefaultNamespace", "Person");
            EdmStructuralProperty personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            EdmNavigationProperty friend = person.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Friends", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "Self", TargetMultiplicity = EdmMultiplicity.One });
            IEdmNavigationProperty self = friend.Partner;

            XElement annotationElement =
                new XElement("{http://foo}Annotation", "1");
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(friend, "http://foo", "Annotation", annotation);

            return model;
        }

        public static IEnumerable<XElement> OutOfLineVocabularyAnnotationWithAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""SimpleType"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <Term Name=""Note"" Type=""Edm.String"" />
  <Annotations Target=""DefaultNamespace.SimpleType"">
    <Annotation Term=""DefaultNamespace.Note"" String=""ComplexTypeNote"">
      <Annotation xmlns=""http://foo"">1</Annotation>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static EdmModel OutOfLineVocabularyAnnotationWithAnnotationModel()
        {
            EdmModel model = new EdmModel();

            EdmComplexType simpleType = new EdmComplexType("DefaultNamespace", "SimpleType");
            simpleType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));
            model.AddElement(simpleType);

            EdmTerm note = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetString(true));
            model.AddElement(note);

            EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
                simpleType,
                note,
                new EdmStringConstant("ComplexTypeNote"));
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);
            
            XElement annotationElement =
                new XElement("{http://foo}Annotation", "1");
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(valueAnnotation, "http://foo", "Annotation", annotation);

            return model;
        }

        private static IEnumerable<XElement> ConvertCsdlsToXElements(params string[] csdls)
        {
            return csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        }
    }
}
