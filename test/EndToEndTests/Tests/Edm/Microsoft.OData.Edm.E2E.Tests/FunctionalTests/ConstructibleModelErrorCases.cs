//---------------------------------------------------------------------
// <copyright file="ConstructibleModelErrorCases.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ConstructibleModelErrorCases : EdmLibTestCaseBase
{
    [Fact]
    public void CreateAmbiguousBinding_ShouldReturnBadAmbiguousElementBindingError()
    {
        // Arrange
        EdmModel model = new EdmModel();

        EdmComplexType c1 = new EdmComplexType("Ambiguous", "Binding");
        EdmComplexType c2 = new EdmComplexType("Ambiguous", "Binding");
        EdmComplexType c3 = new EdmComplexType("Ambiguous", "Binding");


        // Act
        model.AddElement(c1);
        Assert.Equal(c1, model.FindType("Ambiguous.Binding"));

        model.AddElement(c2);
        model.AddElement(c3);

        IEdmNamedElement ambiguous = model.FindType("Ambiguous.Binding");

        // Assert
        Assert.True(ambiguous.IsBad());
        Assert.Equal(EdmErrorCode.BadAmbiguousElementBinding, ambiguous.Errors().First().ErrorCode);

        c1 = null;
        c2 = new EdmComplexType("Ambiguous", "Binding2");

        Assert.True
        (
            model.SchemaElements.OfType<IEdmComplexType>().Count() == 3 && model.SchemaElements.OfType<IEdmComplexType>().All(n => n.FullName() == "Ambiguous.Binding")
        );
    }

    [Fact]
    public void AddOperationWithSameNameAsType_ShouldAllowMultipleOperationsAndType()
    {
        // Arrange
        EdmModel model = new EdmModel();

        EdmComplexType c1 = new EdmComplexType("Ambiguous", "Binding");
        IEdmOperation o1 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));
        IEdmOperation o2 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));
        IEdmOperation o3 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));

        // Act & Assert
        model.AddElement(o1);
        Assert.Single(model.FindOperations("Ambiguous.Binding"));
        model.AddElement(o2);
        Assert.Equal(2, model.FindOperations("Ambiguous.Binding").Count());
        model.AddElement(c1);
        model.AddElement(o3);
        Assert.Equal(3, model.FindOperations("Ambiguous.Binding").Count());

        Assert.Equal(c1, model.FindType("Ambiguous.Binding"));
    }

    [Fact]
    public void AddOperationWithSameNameAsTypeIncludingAction_ShouldAllowOperationsAndType()
    {
        // Arrange
        EdmModel model = new EdmModel();

        EdmComplexType c1 = new EdmComplexType("Ambiguous", "Binding");
        IEdmOperation o1 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));
        IEdmOperation o2 = new EdmAction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));
        IEdmOperation o3 = new EdmFunction("Ambiguous", "Binding", EdmCoreModel.Instance.GetInt16(true));

        // Act & Assert
        model.AddElement(o1);
        Assert.Single(model.FindOperations("Ambiguous.Binding"));
        model.AddElement(o2);
        Assert.Equal(2, model.FindOperations("Ambiguous.Binding").Count());
        model.AddElement(c1);
        model.AddElement(o3);
        Assert.Equal(3, model.FindOperations("Ambiguous.Binding").Count());

        Assert.Equal(c1, model.FindType("Ambiguous.Binding"));
    }

    [Fact]
    public void CreatingPrimitiveWithInvalidType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        EdmCoreModel coreModel = EdmCoreModel.Instance;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => coreModel.GetPrimitive(EdmPrimitiveTypeKind.None, false));
    }

    [Fact]
    public void CreatingTemporalTypeWithoutFacetsWithInvalidType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        EdmCoreModel coreModel = EdmCoreModel.Instance;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => coreModel.GetTemporal(EdmPrimitiveTypeKind.Int32, false));
    }

    [Fact]
    public void CreatingTemporalTypeWithFacetsWithInvalidType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        EdmCoreModel coreModel = EdmCoreModel.Instance;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => coreModel.GetTemporal(EdmPrimitiveTypeKind.Int32, 1, false));
    }

    [Fact]
    public void CreatingSpatialTypeWithoutFacetsWithInvalidType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        EdmCoreModel coreModel = EdmCoreModel.Instance;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => coreModel.GetSpatial(EdmPrimitiveTypeKind.Int32, false));
    }

    [Fact]
    public void CreatingSpatialTypeWithFacetsWithInvalidType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        EdmCoreModel coreModel = EdmCoreModel.Instance;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => coreModel.GetSpatial(EdmPrimitiveTypeKind.Int32, 1337, false));
    }

    [Fact]
    public void AddCustomElementToModel_ShouldThrowForInvalidElementAndAllowValidElement()
    {
        // Negative test
        Assert.Throws<InvalidCastException>(() => (new EdmModel()).AddElement(new AnEdmElement()));

        // Positive test
        var edmModel = new EdmModel();
        edmModel.AddElement(new AnEdmOperationElement());
        Assert.True(edmModel.FindOperations("MyNamespace.MyName").Any());
    }

    [Fact]
    public void AddAnnotationWithoutTarget_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act & Assert
        var edmModel = new EdmModel();
        var annotation = new MutableVocabularyAnnotation();

        Assert.Throws<InvalidOperationException>(() => edmModel.AddVocabularyAnnotation(annotation));
    }

    [Fact]
    public void NullNamespaceOrName_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EdmComplexType(null, "null"));
        Assert.Throws<ArgumentNullException>(() => new EdmComplexType("null", null));

        Assert.Throws<ArgumentNullException>(() => new EdmEntityType(null, "null"));
        Assert.Throws<ArgumentNullException>(() => new EdmEntityType("null", null));

        Assert.Throws<ArgumentNullException>(() => new EdmUntypedStructuredType(null, "null"));
        Assert.Throws<ArgumentNullException>(() => new EdmUntypedStructuredType("null", null));
    }

    [Fact]
    public void CreatingStringTypeWithUnboundedAndMaxLength_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act & Assert
        EdmCoreModel coreModel = EdmCoreModel.Instance;
        Assert.Throws<InvalidOperationException>(() => coreModel.GetString(true, 255, true, false));
    }

    [Fact]
    public void CreatingBinaryTypeWithUnboundedAndMaxLength_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act & Assert
        EdmCoreModel coreModel = EdmCoreModel.Instance;
        Assert.Throws<InvalidOperationException>(() => coreModel.GetBinary(true, 255, false));
    }

    [Fact]
    public void ConstructBadElementAnnotations_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act & Assert
        var model = new EdmModel();

        Assert.Throws<InvalidOperationException>(() => new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "Derp").SetIsSerializedAsElement(model, true));
        Assert.Throws<InvalidOperationException>(() => new EdmIntegerConstant(EdmCoreModel.Instance.GetString(false), 3).SetIsSerializedAsElement(model, true));
        Assert.Throws<InvalidOperationException>(() => new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "<Derp>").SetIsSerializedAsElement(model, true));
    }

    [Fact]
    public void AddMultipleEntityContainersToModel_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act & Assert
        string @namespace = "Westwind";
        EdmModel model = new EdmModel();

        EdmEntityContainer container1 = new EdmEntityContainer(@namespace, "Container1");
        EdmEntityContainer container2 = new EdmEntityContainer(@namespace, "Container2");
        model.AddElement(container1);
        var exception = Assert.Throws<InvalidOperationException>(() => model.AddElement(container2));
        Assert.Equal("Cannot add more than one entity container to an edm model.", exception.Message);
    }

    #region Private

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

        public IEnumerable<Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation> AttachedAnnotations
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

        public IEnumerable<IEdmDirectValueAnnotation> AttachedAnnotations
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

    #endregion
}

internal sealed class MutableVocabularyAnnotation : IEdmVocabularyAnnotation
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

    public bool UsesDefault => throw new NotImplementedException();
}
