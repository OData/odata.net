//---------------------------------------------------------------------
// <copyright file="XElementAnnotationTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class XElementAnnotationTest : EdmLibTestCaseBase
    {
        [TestMethod]
        public void XElementAnnotationTestAnnotationWithoutChildrenCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation");

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "ComplexType", Type = AnnotatableElementType.ComplexType }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.AnnotationWithoutChildrenCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestAnnotationWithoutChildrenModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation");

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "ComplexType", Type = AnnotatableElementType.ComplexType }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.AnnotationWithoutChildrenCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.AnnotationWithoutChildrenModel());
        }

        [TestMethod]
        public void XElementAnnotationTestXElementWithWithoutNamespaceCsdl()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("DefaultNamespace", "ComplexType");
            complexType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));
            model.AddElement(complexType);

            XElement annotationElement = new XElement("EmptyAnnotation");
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            VerifyThrowsException(typeof(InvalidOperationException), () => annotation.SetIsSerializedAsElement(model, true));
        }

        [TestMethod]
        public void XElementAnnotationTestNestedXElementWithNoValueCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation",
                    new XElement("{http://foo}Child",
                        new XElement("{http://foo}GrandChild",
                            new XElement("{http://foo}GreatGrandChild",
                                new XElement("{http://foo}GreateGreatGrandChild")
                            )
                        )
                    )
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleType", Type = AnnotatableElementType.ComplexType },
                new ElementInfo() { Name = "Id", Type = AnnotatableElementType.Property }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.NestedXElementWithNoValueCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestNestedXElementWithNoValueModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation",
                    new XElement("{http://foo}Child",
                        new XElement("{http://foo}GrandChild",
                            new XElement("{http://foo}GreatGrandChild",
                                new XElement("{http://foo}GreateGreatGrandChild")
                            )
                        )
                    )
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleType", Type = AnnotatableElementType.ComplexType },
                new ElementInfo() { Name = "Id", Type = AnnotatableElementType.Property }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.NestedXElementWithNoValueCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.NestedXElementWithNoValueModel());
        }

        [TestMethod]
        public void XElementAnnotationTestNestedXElementWithValueCsdl()
        {
            XElement expectedAnnotation =
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

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleFunction", Type = AnnotatableElementType.Function }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.NestedXElementWithValueCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestNestedXElementWithValueModel()
        {
            XElement expectedAnnotation =
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

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleFunction", Type = AnnotatableElementType.Function }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.NestedXElementWithValueCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.NestedXElementWithValueModel());
        }

        [TestMethod]
        public void XElementAnnotationTestAnnotationWithValueCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation",
                    "Value 1.0"
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleFunction", Type = AnnotatableElementType.Function },
                new ElementInfo() { Name = "Id", Type = AnnotatableElementType.Parameter }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.AnnotationWithValueCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestAnnotationWithValueModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation",
                    "Value 1.0"
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleFunction", Type = AnnotatableElementType.Function },
                new ElementInfo() { Name = "Id", Type = AnnotatableElementType.Parameter }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.AnnotationWithValueCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.AnnotationWithValueModel());
        }

        [TestMethod]
        public void XElementAnnotationTestAnnotationWithSchemaTagValueCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation",
                    "</Schema>"
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleType", Type = AnnotatableElementType.EntityType }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.AnnotationWithSchemaTagValueCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestAnnotationWithSchemaTagValueModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation",
                    "</Schema>"
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleType", Type = AnnotatableElementType.EntityType }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.AnnotationWithSchemaTagValueCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.AnnotationWithSchemaTagValueModel());
        }

        [TestMethod]
        public void XElementAnnotationTestAnnotationWithEntitySetTagInEntityContainerCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}EntitySet",
                    "1"
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "EntitySet" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Container", Type = AnnotatableElementType.EntityContainer }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.AnnotationWithEntitySetTagInEntityContainerCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestAnnotationWithEntitySetTagInEntityContainerModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}EntitySet",
                    "1"
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "EntitySet" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Container", Type = AnnotatableElementType.EntityContainer }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.AnnotationWithEntitySetTagInEntityContainerCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.AnnotationWithEntitySetTagInEntityContainerModel());
        }

        [TestMethod]
        public void XElementAnnotationTestDifferentAnnotationNamespaceCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation",
                    new XElement("{http://foo1}Child",
                        new XElement("{http://foo2}GrandChild",
                            "1"
                        )
                    )
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Container", Type = AnnotatableElementType.EntityContainer },
                new ElementInfo() { Name = "SimpleSet", Type = AnnotatableElementType.EntitySet }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.DifferentAnnotationNamespaceCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestDifferentAnnotationNamespaceModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation",
                    new XElement("{http://foo1}Child",
                        new XElement("{http://foo2}GrandChild",
                            "1"
                        )
                    )
                );

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Container", Type = AnnotatableElementType.EntityContainer },
                new ElementInfo() { Name = "SimpleSet", Type = AnnotatableElementType.EntitySet }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.DifferentAnnotationNamespaceCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.DifferentAnnotationNamespaceModel());
        }

        [TestMethod]
        public void XElementAnnotationTestSetChildAnnotationAsAnnotationModel()
        {
            var model = XElementAnnotationModelBuilder.SetChildAnnotationAsAnnotationModel();
            var errors = new EdmLibTestErrors
            {
                {null, null, EdmErrorCode.InvalidElementAnnotation}
            };

            this.VerifySemanticValidation(model, EdmVersion.V40, errors);
        }

        [TestMethod]
        public void XElementAnnotationTestSetNullAnnotationNameModel()
        {
            try
            {
                SetNullAnnotationNameModel();
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            var errors = new MutableDirectValueAnnotation().Errors().ToArray();
            Assert.AreEqual(3, errors.Length, "error count");
            Assert.AreEqual(EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull, errors.Select(e => e.ErrorCode).Distinct().Single(), "InterfaceCriticalPropertyValueMustNotBeNull");
        }

        private static EdmModel SetNullAnnotationNameModel()
        {
            EdmModel model = new EdmModel();

            EdmEnumType spicy = new EdmEnumType("DefaultNamespace", "Spicy");
            model.AddElement(spicy);

            XElement annotationElement =
                new XElement("{http://foo}Annotation",
                    new XElement("{http://foo1}Child",
                      "1"
                    )
                );
            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), annotationElement.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(spicy, "http://foo", null, annotation);

            return model;
        }

        private sealed class MutableDirectValueAnnotation : Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation
        {
            public string NamespaceUri
            {
                get;
                set;
            }

            public object Value
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }

        [TestMethod]
        public void XElementAnnotationTestComplexNamespaceOverlappingCsdl()
        {
            XElement expectedAnnotation =
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

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Container", Type = AnnotatableElementType.EntityContainer },
                new ElementInfo() { Name = "SimpleFunction", Type = AnnotatableElementType.FunctionImport }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "Default", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.ComplexNamespaceOverlappingCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestComplexNamespaceOverlappingModel()
        {
            XElement expectedAnnotation =
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

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Container", Type = AnnotatableElementType.EntityContainer },
                new ElementInfo() { Name = "SimpleFunction", Type = AnnotatableElementType.FunctionImport }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "Default", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.ComplexNamespaceOverlappingCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.ComplexNamespaceOverlappingModel());
        }

        [TestMethod]
        public void XElementAnnotationTestFunctionImportParameterWithAnnotationCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation", 1);

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleFunction", Type = AnnotatableElementType.Action },
                new ElementInfo() { Name = "Name", Type = AnnotatableElementType.Parameter },
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "Default", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.ActionImportParameterWithAnnotationCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestFunctionImportParameterWithAnnotationModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation", 1);

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "SimpleFunction", Type = AnnotatableElementType.Function },
                new ElementInfo() { Name = "Name", Type = AnnotatableElementType.Parameter },
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "Default", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.ActionImportParameterWithAnnotationCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.OperationImportParameterWithAnnotationModel());
        }

        [TestMethod]
        public void XElementAnnotationTestTermWithAnnotationCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation", 1);

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Note", Type = AnnotatableElementType.Term }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.TermWithAnnotationCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestTermWithAnnotationModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation", 1);

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Note", Type = AnnotatableElementType.Term }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.TermWithAnnotationCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.TermWithAnnotationModel());
        }

        [TestMethod]
        public void XElementAnnotationTestEnumWithAnnotationCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation", 1);

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Spicy", Type = AnnotatableElementType.Enum }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.EnumWithAnnotationCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestEnumWithAnnotationModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation", 1);

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Spicy", Type = AnnotatableElementType.Enum }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.EnumWithAnnotationCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.EnumWithAnnotationModel());
        }

        [TestMethod]
        public void XElementAnnotationTestNavigationPropertyWithAnnotationCsdl()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation", 1);

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Person", Type = AnnotatableElementType.EntityType },
                new ElementInfo() { Name = "Friends", Type = AnnotatableElementType.NavigationProperty }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.NavigationPropertyWithAnnotationCsdl(), expectedAnnotation, annotationInfo, annotationLocation);
        }

        [TestMethod]
        public void XElementAnnotationTestNavigationPropertyWithAnnotationModel()
        {
            XElement expectedAnnotation =
                new XElement("{http://foo}Annotation", 1);

            AnnotationInfo annotationInfo = new AnnotationInfo() { Namespace = "http://foo", Name = "Annotation" };

            List<ElementInfo> annotationPath = new List<ElementInfo>()
            {
                new ElementInfo() { Name = "Person", Type = AnnotatableElementType.EntityType },
                new ElementInfo() { Name = "Friends", Type = AnnotatableElementType.NavigationProperty }
            };
            ElementLocation annotationLocation = new ElementLocation() { Namespace = "DefaultNamespace", ElementPath = annotationPath };

            this.AnnotationRoundTripCsdlCheck(XElementAnnotationModelBuilder.NavigationPropertyWithAnnotationCsdl(), expectedAnnotation, annotationInfo, annotationLocation, XElementAnnotationModelBuilder.NavigationPropertyWithAnnotationModel());
        }

        [TestMethod]
        public void XElementAnnotationTestOutOfLineVocabularyAnnotationWithAnnotationCsdl()
        {
            var csdls = XElementAnnotationModelBuilder.OutOfLineVocabularyAnnotationWithAnnotationCsdl();
            var model = this.GetParserResult(csdls);
            var seri = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
            var errors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.Latest, errors);

            var vocabularyAnnotation = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotation.Count(), "Invalid vocabulary count.");

            var actualAnnotationValue = (model.GetAnnotationValue(vocabularyAnnotation.Single(), "http://foo", "Annotation") as EdmStringConstant).Value;

            XElement expectedAnnotationValue =
                new XElement("{http://foo}Annotation", "1");

            Assert.IsTrue(expectedAnnotationValue.ToString().Equals(actualAnnotationValue), "XElement annotation are not the same.");

            var serializedModel = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(this.GetSerializerResult(model).Select(n => XElement.Parse(n)).ToArray(), EdmVersion.V40).ToList();
            new ConstructiveApiCsdlXElementComparer().Compare(csdls.ToList(), serializedModel);
        }

        [TestMethod]
        public void XElementAnnotationTestOutOfLineVocabularyAnnotationWithAnnotationModel()
        {
            var model = XElementAnnotationModelBuilder.OutOfLineVocabularyAnnotationWithAnnotationModel();

            var errors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.Latest, errors);

            var vocabularyAnnotation = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotation.Count(), "Invalid vocabulary count.");

            var actualAnnotationValue = (model.GetAnnotationValue(vocabularyAnnotation.Single(), "http://foo", "Annotation") as EdmStringConstant).Value;

            XElement expectedAnnotationValue =
                new XElement("{http://foo}Annotation", "1");

            Assert.IsTrue(expectedAnnotationValue.ToString().Equals(actualAnnotationValue), "XElement annotation are not the same.");

            var serializedModel = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(this.GetSerializerResult(model).Select(n => XElement.Parse(n)).ToArray(), EdmVersion.V40).ToList();
            new ConstructiveApiCsdlXElementComparer().Compare(XElementAnnotationModelBuilder.OutOfLineVocabularyAnnotationWithAnnotationCsdl().ToList(), serializedModel);
        }

        private void AnnotationRoundTripCsdlCheck(IEnumerable<XElement> csdls, XElement expectedAnnotation, AnnotationInfo annotationInfo, ElementLocation annotationLocation)
        {
            var model = this.GetParserResult(csdls);

            this.AnnotationRoundTripCsdlCheck(csdls, expectedAnnotation, annotationInfo, annotationLocation, model);
        }

        private void AnnotationRoundTripCsdlCheck(IEnumerable<XElement> csdls, XElement expectedAnnotation, AnnotationInfo annotationInfo, ElementLocation annotationLocation, IEdmModel model)
        {
            var errors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.Latest, errors);

            var isValid = this.ValidateAnnotation(model, expectedAnnotation, annotationInfo, annotationLocation);
            Assert.IsTrue(isValid, "Invalid XElement annotation.");
            IEnumerable<EdmError> serializationErrors;

            var serializedModelCsdls = this.GetSerializerResult(model, EdmVersion.Latest, out serializationErrors).Select(n => XElement.Parse(n));
            Assert.AreEqual(0, serializationErrors.Count(), "Round trip test should not have serialization errors: " + Environment.NewLine + String.Join(Environment.NewLine, serializationErrors));

            var csdlsWithLastestEdmVersion = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdls.ToArray(), EdmVersion.Latest).ToList();
            new ConstructiveApiCsdlXElementComparer().Compare(csdlsWithLastestEdmVersion, serializedModelCsdls.ToList());
        }

        private bool ValidateAnnotation(IEdmModel model, XElement expectedAnnotation, AnnotationInfo annotationInfo, ElementLocation annotationLocation)
        {
            string annotationValue = this.GetElementAnnotationValue(model, annotationInfo, annotationLocation);
            
            if (string.IsNullOrEmpty(annotationValue))
            {
                return false;
            }
            
            var actualAnnotation = XElement.Parse(annotationValue);
            
            return expectedAnnotation.ToString().Equals(actualAnnotation.ToString());
        }

        private string GetElementAnnotationValue(IEdmModel model, AnnotationInfo annotationInfo, ElementLocation annotationLocation)
        {
            IEdmElement element = GetElementByElementLocation(model, annotationLocation);

            if (null != element)
            {
                var annotation = model.GetAnnotationValue(element, annotationInfo.Namespace, annotationInfo.Name);

                if (null != annotation)
                {
                    return null != (annotation as EdmStringConstant) ? (annotation as EdmStringConstant).Value : string.Empty;
                }
            }

            return string.Empty;
        }

        private IEdmElement GetElementByElementLocation(IEdmModel model, ElementLocation elementLocation)
        {
            IEdmElement elementWithAnnotation = model;
            foreach (ElementInfo elementInfo in elementLocation.ElementPath)
            {
                elementWithAnnotation = this.GetAnnotatableElement(elementLocation.Namespace, elementInfo.Name, elementInfo.Type, elementWithAnnotation);

                if (null == elementWithAnnotation)
                {
                    break;
                }
            }
            return elementWithAnnotation;
        }

        class AnnotationInfo
        {
            public string Name { get; set; }
            public string Namespace { get; set; }
        }

        class ElementLocation
        {
            public string Namespace { get; set; }
            public List<ElementInfo> ElementPath { get; set; }
        }

        class ElementInfo
        {
            public string Name { get; set; }
            public AnnotatableElementType Type { get; set; }
        }

        private IEdmElement GetAnnotatableElement(string targetNamespace, string targetName, AnnotatableElementType targetType, IEdmElement element)
        {
            if (null != (element as IEdmModel)) 
            {
                return this.GetEdmModelAnnotatableElement(targetNamespace, targetName, targetType, element);
            }
            else if (null != (element as IEdmEntityContainer)) 
            {
                return this.GetEntityContainerAnnotatableElement(targetName, targetType, element);
            }
            else if (null != (element as IEdmEntityType))
            {
                return this.GetEntityTypeAnnotatableElement(targetName, targetType, element);
            }
            else if (null != (element as IEdmComplexType))
            {
                return this.GetComplexTypeAnnotatableElement(targetName, targetType, element);
            }
            else if (null != (element as IEdmOperation))
            {
                return this.GetFunctionAnnotatableElement(targetName, targetType, element);
            }
            else if (null != (element as IEdmOperationImport))
            {
                return this.GetFunctionImportAnnotatableElement(targetName, targetType, element);
            }

            return null;
        }

        private IEdmElement GetEdmModelAnnotatableElement(string targetNamespace, string targetName, AnnotatableElementType targetType, IEdmElement element)
        {
            var model = (element as IEdmModel);

            if (null != model)
            {
                if (targetType.Equals(AnnotatableElementType.ComplexType) || targetType.Equals(AnnotatableElementType.Enum))
                {
                    return model.FindType(this.GetFullName(targetNamespace, targetName));
                }
                else if (targetType.Equals(AnnotatableElementType.EntityType))
                {
                    return model.FindEntityType(this.GetFullName(targetNamespace, targetName));
                }
                else if (targetType.Equals(AnnotatableElementType.Action))
                {
                    var functions = model.FindOperations(this.GetFullName(targetNamespace, targetName));
                    return functions == null || functions.Count() == 0 ? null : functions.First();
                }
                else if (targetType.Equals(AnnotatableElementType.Function))
                {
                    var functions = model.FindOperations(this.GetFullName(targetNamespace, targetName));
                    return functions == null || functions.Count() == 0 ? null : functions.First();
                }
                else if (targetType.Equals(AnnotatableElementType.Term))
                {
                    return model.FindTerm(this.GetFullName(targetNamespace, targetName));
                }
                else if (targetType.Equals(AnnotatableElementType.EntityContainer))
                {
                    return model.FindEntityContainer(targetName);
                }
            }

            return null;
        }

        private IEdmElement GetEntityContainerAnnotatableElement(string targetName, AnnotatableElementType targetType, IEdmElement element)
        {
            var entityContainer = (element as IEdmEntityContainer);

            if (null != entityContainer)
            {
                if (targetType.Equals(AnnotatableElementType.EntitySet))
                {
                    return entityContainer.FindEntitySet(targetName);
                }
                else if (targetType.Equals(AnnotatableElementType.FunctionImport))
                {
                    var operationImports = entityContainer.FindOperationImports(targetName);
                    return operationImports == null || operationImports.Count() == 0 ? null : operationImports.First();
                }
            }

            return null;
        }

        private IEdmElement GetEntityTypeAnnotatableElement(string targetName, AnnotatableElementType targetType, IEdmElement element)
        {
            var entityType = (element as IEdmEntityType);

            if (null != entityType)
            {
                if (targetType.Equals(AnnotatableElementType.Property))
                {
                    return entityType.FindProperty(targetName);
                }
                else if (targetType.Equals(AnnotatableElementType.NavigationProperty))
                {
                    var navigationProperties = entityType.NavigationProperties().Where(n => n.Name.Equals(targetName));
                    return navigationProperties.Count() == 0 ? null : navigationProperties.First();
                }
            }

            return null;
        }

        private IEdmElement GetComplexTypeAnnotatableElement(string targetName, AnnotatableElementType targetType, IEdmElement element)
        {
            var complexType = (element as IEdmComplexType);

            if (null != complexType)
            {
                if (targetType.Equals(AnnotatableElementType.Property))
                {
                    return complexType.FindProperty(targetName);
                }
            }

            return null;
        }

        private IEdmElement GetFunctionAnnotatableElement(string targetName, AnnotatableElementType targetType, IEdmElement element)
        {
            var function = (element as IEdmOperation);

            if (null != function)
            {
                if (targetType.Equals(AnnotatableElementType.Parameter))
                {
                    return function.FindParameter(targetName);
                }
            }

            return null;
        }

        private IEdmElement GetFunctionImportAnnotatableElement(string targetName, AnnotatableElementType targetType, IEdmElement element)
        {
            var operationImport = (element as IEdmOperationImport);

            if (null != operationImport)
            {
                if (targetType.Equals(AnnotatableElementType.Parameter))
                {
                    return operationImport.Operation.FindParameter(targetName);
                }
            }

            return null;
        }

        private string GetFullName(string elementNamespace, string elementName)
        {
            return new StringBuilder().Append(elementNamespace).Append(".").Append(elementName).ToString();
        }

        enum AnnotatableElementType
        {
            ComplexType,
            EntityContainer,
            EntitySet,
            EntityType,
            Enum,
            Action,
            Function,
            FunctionImport,
            ActionImport,
            Model,
            NavigationProperty,
            Parameter,
            Property,
            Term
        }
    }
}
