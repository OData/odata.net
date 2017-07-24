//---------------------------------------------------------------------
// <copyright file="ConstructibleModelErrorCases.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    #if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstructibleModelErrorCases : EdmLibTestCaseBase
    {
        [TestMethod]
        public void CreateAmbiguousBinding()
        {
            EdmModel model = new EdmModel();

            EdmComplexType c1 = new EdmComplexType("Ambiguous", "Binding");
            EdmComplexType c2 = new EdmComplexType("Ambiguous", "Binding");
            EdmComplexType c3 = new EdmComplexType("Ambiguous", "Binding");

            model.AddElement(c1);
            Assert.AreEqual(c1, model.FindType("Ambiguous.Binding"), "Single item resolved");

            model.AddElement(c2);
            model.AddElement(c3);

            IEdmNamedElement ambiguous = model.FindType("Ambiguous.Binding");
            Assert.IsTrue(ambiguous.IsBad(), "Ambiguous binding is bad");
            Assert.AreEqual(EdmErrorCode.BadAmbiguousElementBinding, ambiguous.Errors().First().ErrorCode, "Ambiguous");

            c1 = null;
            c2 = new EdmComplexType("Ambiguous", "Binding2");

            Assert.IsTrue
                            (
                                model.SchemaElements.OfType<IEdmComplexType>().Count() == 3 && model.SchemaElements.OfType<IEdmComplexType>().All(n => n.FullName() == "Ambiguous.Binding"),
                                "The model must be immutable."
                            );
        }

        [TestMethod]
        public void AddOperationWhileTypeHasSameName()
        {
            EdmModel model = new EdmModel();

            EdmComplexType c1 = new EdmComplexType("Ambiguous", "Binding");
            IEdmOperation o1 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));
            IEdmOperation o2 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));
            IEdmOperation o3 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));

            model.AddElement(o1);
            Assert.AreEqual(1, model.FindOperations("Ambiguous.Binding").Count(), "First function was correctly added to operation group");
            model.AddElement(o2);
            Assert.AreEqual(2, model.FindOperations("Ambiguous.Binding").Count(), "Second function was correctly added to operation group");
            model.AddElement(c1);
            model.AddElement(o3);
            Assert.AreEqual(3, model.FindOperations("Ambiguous.Binding").Count(), "Third function was correctly added to operation group");

            Assert.AreEqual(c1, model.FindType("Ambiguous.Binding"), "Single item resolved");
        }

        [TestMethod]
        public void AddOperationWhileTypeHasSameNameButOneIsAction()
        {
            EdmModel model = new EdmModel();

            EdmComplexType c1 = new EdmComplexType("Ambiguous", "Binding");
            IEdmOperation o1 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));
            IEdmOperation o2 = new EdmAction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));
            IEdmOperation o3 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));

            model.AddElement(o1);
            Assert.AreEqual(1, model.FindOperations("Ambiguous.Binding").Count(), "First function was correctly added to operation group");
            model.AddElement(o2);
            Assert.AreEqual(2, model.FindOperations("Ambiguous.Binding").Count(), "Second function was correctly added to operation group");
            model.AddElement(c1);
            model.AddElement(o3);
            Assert.AreEqual(3, model.FindOperations("Ambiguous.Binding").Count(), "Third function was correctly added to operation group");

            Assert.AreEqual(c1, model.FindType("Ambiguous.Binding"), "Single item resolved");
        }

        [TestMethod]
        public void CreatingPrimitiveWithInvalidTypeCausesException()
        {
            EdmCoreModel coreModel = EdmCoreModel.Instance;
            this.VerifyThrowsException(typeof(InvalidOperationException), () => coreModel.GetPrimitive(EdmPrimitiveTypeKind.None, false));
        }

        [TestMethod]
        public void CreatingTemporalTypeWithoutFacetsWithInvalidTypeCausesException()
        {
            EdmCoreModel coreModel = EdmCoreModel.Instance;
            this.VerifyThrowsException(typeof(InvalidOperationException), () => coreModel.GetTemporal(EdmPrimitiveTypeKind.Int32, false));
        }

        [TestMethod]
        public void CreatingTemporalTypeWithFacetsWithInvalidTypeCausesException()
        {
            EdmCoreModel coreModel = EdmCoreModel.Instance;
            this.VerifyThrowsException(typeof(InvalidOperationException), () => coreModel.GetTemporal(EdmPrimitiveTypeKind.Int32, 1, false));
        }

        [TestMethod]
        public void CreatingSpatialTypeWithoutFacetsWithInvalidTypeCausesException()
        {
            EdmCoreModel coreModel = EdmCoreModel.Instance;
            this.VerifyThrowsException(typeof(InvalidOperationException), () => coreModel.GetSpatial(EdmPrimitiveTypeKind.Int32, false));
        }

        [TestMethod]
        public void CreatingSpatialTypeWithFacetsWithInvalidTypeCausesException()
        {
            EdmCoreModel coreModel = EdmCoreModel.Instance;
            this.VerifyThrowsException(typeof(InvalidOperationException), () => coreModel.GetSpatial(EdmPrimitiveTypeKind.Int32, 1337, false));
        }

        [TestMethod]
        public void AddCustomElementToAmodel()
        {
            // Negative test
            try
            {
                (new EdmModel()).AddElement(new AnEdmElement());
            }
            catch (InvalidCastException e)
            {
                Assert.IsTrue(e is InvalidCastException, "It should fail when casting to IEdmFuncition.");
            }

            // Positive test
            var edmModel = new EdmModel();
            edmModel.AddElement(new AnEdmOperationElement());
            Assert.IsTrue(edmModel.FindOperations("MyNamespace.MyName").Any(), "Failed to find the newly added element.");
        }

        [TestMethod]
        public void ValidateCustomType()
        {
            // Negative test
            try
            {
                new EdmModel().AddElement(new AnEdmType());
            }
            catch (InvalidCastException e)
            {
                Assert.IsTrue(e.Message.Contains("IEdmEntityType"), "It should fail when casting to IEdmEntityType.");
            }

            // Positive test
            var edmModel = new EdmModel();
            edmModel.AddElement(new AnEdmEntityType());
            Assert.IsTrue(edmModel.SchemaElements.Any(n => n.FullName() == "MyNamespace.MyName"), "Failed to find the newly added element.");
        }

        [TestMethod]
        public void AddAnnotationWithNoTarget()
        {
            var edmModel = new EdmModel();
            var annotation = new MutableVocabularyAnnotation();
            this.VerifyThrowsException(typeof(InvalidOperationException), () => edmModel.AddVocabularyAnnotation(annotation));
        }

        [TestMethod]
        public void NullNamespaceAndNamesThrow()
        {
            try
            {
                new EdmComplexType(null, "null");
                Assert.Fail("exception expected");
            }
            catch (Exception e)
            {
                Assert.AreEqual("ArgumentNullException", e.GetType().Name, "null namespace");
            }
            
            try
            {
                new EdmComplexType("null", null);
                Assert.Fail("exception expected");
            }
            catch (Exception e)
            {
                Assert.AreEqual("ArgumentNullException", e.GetType().Name, "null name");
            }

            try
            {
                new EdmEntityType(null, "null");
                Assert.Fail("exception expected");
            }
            catch (Exception e)
            {
                Assert.AreEqual("ArgumentNullException", e.GetType().Name, "null namespace");
            }
            
            try
            {
                new EdmEntityType("null", null);
                Assert.Fail("exception expected");
            }
            catch (Exception e)
            {
                Assert.AreEqual("ArgumentNullException", e.GetType().Name, "null name");
            }

            try
            {
                new EdmUntypedStructuredType("null", null);
                Assert.Fail("exception expected");
            }
            catch (Exception e)
            {
                Assert.AreEqual("ArgumentNullException", e.GetType().Name, "null name");
            }

            try
            {
                new EdmUntypedStructuredType(null, "null");
                Assert.Fail("exception expected");
            }
            catch (Exception e)
            {
                Assert.AreEqual("ArgumentNullException", e.GetType().Name, "null name");
            }
        }

        private class AnEdmOperationElement : IEdmOperation, IEdmAction
        {
            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.Action; }
            }

            public string Namespace
            {
                get { return "MyNamespace"; }
            }

            public string Name
            {
                get { return "MyName"; }
            }

            public bool IsBound
            {
                get { return false; }
            }

            public IEdmPathExpression EntitySetPath
            {
                get { return null; }
            }

            public System.Collections.Generic.IEnumerable<Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation> AttachedAnnotations
            {
                get { throw new NotImplementedException(); }
            }

            public void SetAnnotation(string namespaceName, string localName, object value)
            {
                throw new NotImplementedException();
            }

            public object GetAnnotation(string namespaceName, string localName)
            {
                throw new NotImplementedException();
            }

            public IEdmTypeReference ReturnType
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerable<IEdmOperationParameter> Parameters
            {
                get { throw new NotImplementedException(); }
            }

            public IEdmOperationParameter FindParameter(string name)
            {
                throw new NotImplementedException();
            }

            bool IEdmOperation.IsBound
            {
                get { throw new NotImplementedException(); }
            }

            IEdmPathExpression IEdmOperation.EntitySetPath
            {
                get { throw new NotImplementedException(); }
            }

            IEdmTypeReference IEdmOperation.ReturnType
            {
                get { throw new NotImplementedException(); }
            }

            IEnumerable<IEdmOperationParameter> IEdmOperation.Parameters
            {
                get { throw new NotImplementedException(); }
            }

            IEdmOperationParameter IEdmOperation.FindParameter(string name)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void CreatingStringTypeWithIsUnboundedAndMaxLengthCausesException()
        {
            EdmCoreModel coreModel = EdmCoreModel.Instance;
            this.VerifyThrowsException(typeof(InvalidOperationException), () => coreModel.GetBinary(true, 255, false));
        }

        [TestMethod]
        public void CreatingBinaryTypeWithIsUnboundedAndMaxLengthCausesException()
        {
            EdmCoreModel coreModel = EdmCoreModel.Instance;
            this.VerifyThrowsException(typeof(InvalidOperationException), () => coreModel.GetString(true, 255, true, false));
        }

        [TestMethod]
        public void ConstructBadElementAnnotations()
        {
            var model = new EdmModel();

            this.VerifyThrowsException(typeof(InvalidOperationException), () => new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "Derp").SetIsSerializedAsElement(model, true));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => new EdmIntegerConstant(EdmCoreModel.Instance.GetString(false), 3).SetIsSerializedAsElement(model, true));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "<Derp>").SetIsSerializedAsElement(model, true));
        }

        [TestMethod]
        public void AddMultipleEntityContainersToModel()
        {
            string @namespace = "Westwind";
            EdmModel model = new EdmModel();

            EdmEntityContainer container1 = new EdmEntityContainer(@namespace, "Container1");
            EdmEntityContainer container2 = new EdmEntityContainer(@namespace, "Container2");
            model.AddElement(container1);
            this.VerifyThrowsException(typeof(InvalidOperationException), () => model.AddElement(container2), "EdmModel_CannotAddMoreThanOneEntityContainerToOneEdmModel");
        }

        private class AnEdmElement : IEdmSchemaElement
        {
            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.Action; }
            }

            public string Namespace
            {
                get { return "MyNamespace"; }
            }

            public string Name
            {
                get { return "MyName"; }
            }

            public System.Collections.Generic.IEnumerable<Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation> AttachedAnnotations
            {
                get { throw new NotImplementedException(); }
            }

            public void SetAnnotation(string namespaceName, string localName, object value)
            {
                throw new NotImplementedException();
            }

            public object GetAnnotation(string namespaceName, string localName)
            {
                throw new NotImplementedException();
            }
        }

        private class AnEdmEntityType : IEdmEntityType
        {
            public EdmTypeKind TypeKind
            {
                get { return EdmTypeKind.Entity; }
            }

            public System.Collections.Generic.IEnumerable<Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation> AttachedAnnotations
            {
                get { throw new NotImplementedException(); }
            }

            public void SetAnnotation(string namespaceName, string localName, object value)
            {
                throw new NotImplementedException();
            }

            public object GetAnnotation(string namespaceName, string localName)
            {
                throw new NotImplementedException();
            }

            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.TypeDefinition; }
            }

            public string Namespace
            {
                get { return "MyNamespace"; }
            }

            public string Name
            {
                get { return "MyName"; }
            }

            public IEnumerable<IEdmStructuralProperty> DeclaredKey
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsAbstract
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsOpen
            {
                get { throw new NotImplementedException(); }
            }

            public bool HasStream
            {
                get { throw new NotImplementedException(); }
            }

            public IEdmStructuredType BaseType
            {
                get { return null; }
            }

            public IEnumerable<IEdmProperty> DeclaredProperties
            {
                get { throw new NotImplementedException(); }
            }

            public IEdmProperty FindProperty(string name)
            {
                throw new NotImplementedException();
            }

            public string NamespaceUri
            {
                get { throw new NotImplementedException(); }
            }
        }

        private class AnEdmType : IEdmSchemaType
        {

            public EdmTypeKind TypeKind
            {
                get { return EdmTypeKind.Entity; }
            }

            public System.Collections.Generic.IEnumerable<Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation> AttachedAnnotations
            {
                get { throw new NotImplementedException(); }
            }

            public void SetAnnotation(string namespaceName, string localName, object value)
            {
                throw new NotImplementedException();
            }

            public object GetAnnotation(string namespaceName, string localName)
            {
                throw new NotImplementedException();
            }

            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.TypeDefinition; }
            }

            public string Namespace
            {
                get { return "MyNamepsace"; }
            }

            public string Name
            {
                get { return "MyName"; }
            }
        }
    }

    internal sealed class MutableVocabularyAnnotation : Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation
    {
        public IEdmExpression Value
        {
            get;
            set;
        }

        public string Qualifier
        {
            get;
            set;
        }

        public IEdmTerm Term
        {
            get;
            set;
        }

        public IEdmVocabularyAnnotatable Target
        {
            get;
            set;
        }
    }
}
