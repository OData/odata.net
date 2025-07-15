//---------------------------------------------------------------------
// <copyright file="AnnotatableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class AnnotatableTests : EdmLibTestCaseBase
{
    private IEdmStringTypeReference stringRef = EdmCoreModel.Instance.GetString(false);

    [Theory]
    [InlineData(null, "bar")]
    [InlineData("foo", null)]
    [InlineData(null, null)]
    public void Should_ThrowArgumentNullException_When_SettingAnnotationWithNullValues(string nameSpace, string name)
    {
        // Arrange
        var model = new EdmModel();
        var qualifiedName = new MyQualifiedName(nameSpace, name);
        var annotatable = new EdmComplexType("", "");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => model.SetAnnotationValue(null, qualifiedName.NamespaceName, qualifiedName.Name, qualifiedName.MyFullName));
        model.GetAnnotationValue(annotatable, qualifiedName.NamespaceName, qualifiedName.Name);
    }

    public static TheoryData<IEdmElement> GetEdmAnnotatable()
    {
        return new TheoryData<IEdmElement>
        {
            new EdmComplexType("", ""),
            new EdmEntityContainer("", ""),
            new EdmEntityType("", ""),
            new EdmEntitySet(new EdmEntityContainer("", ""), "", new EdmEntityType("", "")),
            EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "", Target = new EdmEntityType("", ""), TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "", Target = new EdmEntityType("", ""), TargetMultiplicity = EdmMultiplicity.One }),
            new EdmStructuredValue(null, Enumerable.Empty<IEdmPropertyValue>()),
            new EdmStringConstant(null, ""),
            new EdmStructuralProperty(new EdmComplexType("", ""), "", EdmCoreModel.Instance.GetBoolean(true)),
        };
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnNull_When_AnnotationNotSet(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();

        // Act
        var result1 = model.GetAnnotationValue(annotatable, "foo", "foo");
        var result2 = model.GetAnnotationValue(annotatable, string.Empty, string.Empty);

        // Assert
        Assert.Empty(model.DirectValueAnnotations(annotatable));
        Assert.Null(result1);
        Assert.Null(result2);
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnSetAnnotationValue_When_AnnotationIsSet(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();
        var qualifiedNames = new MyQualifiedName[] {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        for (int i = 0; i < qualifiedNames.Length; i++)
        {
            MyQualifiedName q = qualifiedNames[i];
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName);

            Assert.Equal(i + 1, model.DirectValueAnnotations(annotatable).Count());
        }

        // get
        var result = model.GetAnnotationValue(annotatable, "_Not_Exist_", "_Not_Exist_");
        Assert.Null(result);

        foreach (MyQualifiedName q in qualifiedNames)
        {
            result = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
            Assert.Equal(q.MyFullName, result);

            var annotation = model.DirectValueAnnotations(annotatable).FirstOrDefault(a => a.NamespaceUri == q.NamespaceName && a.Name == q.Name);
            Assert.NotNull(annotation);
            Assert.Equal(q.MyFullName, ((IEdmDirectValueAnnotation)annotation).Value);
        }
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_UpdateAnnotationValue_When_AnnotationIsUpdated(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();
        var qualifiedNames = new MyQualifiedName[]
        {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        for (int i = 0; i < qualifiedNames.Count(); i++)
        {
            MyQualifiedName q = qualifiedNames[i];
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName);

            Assert.Equal(i + 1, model.DirectValueAnnotations(annotatable).Count());
        }

        // update
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName + q.MyFullName);
            object result = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
            Assert.Equal(q.MyFullName + q.MyFullName, result);
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnNull_When_AnnotationIsDeleted(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();
        var qualifiedNames = new MyQualifiedName[]
        {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        for (int i = 0; i < qualifiedNames.Count(); i++)
        {
            MyQualifiedName q = qualifiedNames[i];
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName);

            Assert.Equal(i + 1, model.DirectValueAnnotations(annotatable).Count());
        }

        // delete
        for (int i = 0; i < qualifiedNames.Length; i++)
        {
            MyQualifiedName q = qualifiedNames[i];
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, null);

            Assert.Equal(qualifiedNames.Length - i - 1, model.DirectValueAnnotations(annotatable).Count());
        }

        model.SetAnnotationValue(annotatable, "_Not_Exist_", "_Not_Exist_", null);

        foreach (MyQualifiedName q in qualifiedNames)
        {
            object result = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
            Assert.Null(result);
        }

        Assert.Empty(model.DirectValueAnnotations(annotatable));
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnNull_When_ExtensionAnnotationIsNotSet(IEdmElement annotatable)
    {
        // Assert
        var model = new EdmModel();

        Assert.Empty(model.DirectValueAnnotations(annotatable));

        var result2 = model.GetAnnotationValue<string>(annotatable);
        var result3 = model.GetAnnotationValue<string>(annotatable, "foo", "foo");
        var result4 = model.GetAnnotationValue<string>(annotatable, "foo", "foo");

        Assert.Null(result2);
        Assert.Null(result3);
        Assert.Null(result4);

        result3 = model.GetAnnotationValue<string>(annotatable, string.Empty, string.Empty);
        result4 = model.GetAnnotationValue<string>(annotatable, string.Empty, string.Empty);
        Assert.Null(result3);
        Assert.Null(result4);

        Assert.Empty(model.DirectValueAnnotations(annotatable));
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnSetValues_When_ExtensionInternalAnnotationIsSet(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();

        // set
        model.SetAnnotationValue(annotatable, "foo");
        model.SetAnnotationValue(annotatable, new MyQualifiedName("foo", "bar"));
        model.SetAnnotationValue<IEdmType>(annotatable, new EdmComplexType("foo", "foo", null, false));

        Assert.Equal(3, model.DirectValueAnnotations(annotatable).Count());

        // get
        string result1 = model.GetAnnotationValue<string>(annotatable);
        Assert.Equal("foo", result1);

        MyQualifiedName result2 = model.GetAnnotationValue<MyQualifiedName>(annotatable);
        Assert.Equal(new MyQualifiedName("foo", "bar"), result2);

        IEdmType result3 = model.GetAnnotationValue<IEdmType>(annotatable);
        Assert.True(result3 is EdmComplexType);
        Assert.Equal("foo.foo", (result3 as IEdmSchemaType).FullName());

        var result4 = model.GetAnnotationValue<Object>(annotatable);
        Assert.Null(result4);

        Assert.Equal(3, model.DirectValueAnnotations(annotatable).Count());
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_UpdateValues_When_ExtensionInternalAnnotationIsModified(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();

        // set
        model.SetAnnotationValue(annotatable, "foo");
        model.SetAnnotationValue(annotatable, new MyQualifiedName("foo", "bar"));
        model.SetAnnotationValue<IEdmType>(annotatable, new EdmComplexType("foo", "foo", null, false));

        Assert.Equal(3, model.DirectValueAnnotations(annotatable).Count());

        // update
        model.SetAnnotationValue<Object>(annotatable, "bar");
        var result0 = model.GetAnnotationValue<Object>(annotatable);
        Assert.Equal(result0, "bar");

        model.SetAnnotationValue<string>(annotatable, "updated");
        result0 = model.GetAnnotationValue<Object>(annotatable);
        Assert.Equal(result0, "bar");
        var result1 = model.GetAnnotationValue<string>(annotatable);
        Assert.Equal("updated", result1);

        model.SetAnnotationValue<IEdmType>(annotatable, new EdmEntityType("bar", "bar", null, false, false));
        var result3 = model.GetAnnotationValue<IEdmType>(annotatable);
        Assert.True(result3 is EdmEntityType);
        Assert.Equal("bar.bar", (result3 as IEdmSchemaType).FullName());

        Assert.Equal(4, model.DirectValueAnnotations(annotatable).Count());
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnNull_When_ExtensionInternalAnnotationIsDeleted(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();

        // set
        model.SetAnnotationValue(annotatable, "foo");
        model.SetAnnotationValue(annotatable, new MyQualifiedName("foo", "bar"));
        model.SetAnnotationValue<IEdmType>(annotatable, new EdmComplexType("foo", "foo", null, false));

        Assert.Equal(3, model.DirectValueAnnotations(annotatable).Count());

        // delete
        model.SetAnnotationValue<Object>(annotatable, null);
        model.SetAnnotationValue<MyQualifiedName>(annotatable, null);

        string result1 = model.GetAnnotationValue<string>(annotatable);
        Assert.Equal("foo", result1);

        MyQualifiedName result2 = model.GetAnnotationValue<MyQualifiedName>(annotatable);
        Assert.Null(result2);

        Assert.Equal(2, model.DirectValueAnnotations(annotatable).Count());
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnSetValues_When_ExtensionEdmVocabularyAnnotationIsSet(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();
        var qualifiedNames = new MyQualifiedName[] {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());

        // get
        foreach (MyQualifiedName q in qualifiedNames)
        {
            var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
            Assert.True(result1 is EdmStringConstant);
            Assert.Equal(q.MyFullName, (result1 as IEdmStringValue)?.Value);

            var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
            Assert.Equal(q.MyFullName, result2?.Value);
        }

        var result3 = model.GetAnnotationValue(annotatable, "_Not_Exist_", "_Not_Exist_");
        Assert.Null(result3);

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnSetValues_When_ExtensionEdmVocabularyAnnotationIsSetAsSets(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();
        var qualifiedNames = new MyQualifiedName[] {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        var annotations = new List<IEdmDirectValueAnnotationBinding>();
        foreach (MyQualifiedName q in qualifiedNames)
        {
            annotations.Add(new EdmDirectValueAnnotationBinding(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName)));
        }

        model.SetAnnotationValues(annotations);

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());

        // get
        foreach (MyQualifiedName q in qualifiedNames)
        {
            var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
            Assert.True(result1 is EdmStringConstant);
            Assert.Equal(q.MyFullName, (result1 as IEdmStringValue)?.Value);

            var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
            Assert.Equal(q.MyFullName, result2?.Value);
        }

        var result3 = model.GetAnnotationValue(annotatable, "_Not_Exist_", "_Not_Exist_");
        Assert.Null(result3);

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());

        object[] values = model.GetAnnotationValues(annotations);
        for (int index = 0; index < values.Length; index++)
        {
            Assert.Equal(annotations[index].NamespaceUri + ":::" + annotations[index].Name, ((IEdmStringValue)values[index]).Value);
        }
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_UpdateValues_When_ExtensionEdmVocabularyAnnotationIsModified(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();
        var qualifiedNames = new MyQualifiedName[] {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());

        // update
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName + q.MyFullName));

            var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
            Assert.True(result1 is EdmStringConstant);
            Assert.Equal(q.MyFullName + q.MyFullName, (result1 as IEdmStringValue)?.Value);

            var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
            Assert.Equal(q.MyFullName + q.MyFullName, result2.Value);
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnNull_When_ExtensionEdmVocabularyAnnotationIsDeleted(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();
        var qualifiedNames = new MyQualifiedName[] {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());

        // delete
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, null);
        }

        Assert.Empty(model.DirectValueAnnotations(annotatable));

        foreach (MyQualifiedName q in qualifiedNames)
        {
            var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
            Assert.Null(result1);

            var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
            Assert.Null(result2);
        }
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnSetValues_When_ExtensionGeneralAnnotationIsSet(IEdmElement annotatable)
    {
        var model = new EdmModel();

        var qualifiedNames = new MyQualifiedName[] {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());

        // get
        foreach (MyQualifiedName q in qualifiedNames)
        {
            var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
            Assert.True(result1 is EdmStringConstant);
            Assert.Equal(q.MyFullName, (result1 as IEdmStringValue)?.Value);

            var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
            Assert.Equal(q.MyFullName, result2.Value);

            var result3 = model.GetAnnotationValue<IEdmValue>(annotatable, q.NamespaceName, q.Name);
            Assert.True(result3 is EdmStringConstant);
            Assert.Equal(q.MyFullName, (result3 as IEdmStringValue)?.Value);
        }

        var result4 = model.GetAnnotationValue<IEdmValue>(annotatable, "_Not_Exist_", "_Not_Exist_");
        Assert.Null(result4);

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_UpdateValues_When_ExtensionGeneralAnnotationIsModified(IEdmElement annotatable)
    {
        var model = new EdmModel();

        var qualifiedNames = new MyQualifiedName[] {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());

        // update
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName + q.MyFullName));

            var result1 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
            Assert.Equal(q.MyFullName + q.MyFullName, result1.Value);

            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName + " Again");

            var result2 = model.GetAnnotationValue<string>(annotatable, q.NamespaceName, q.Name);
            Assert.Equal(q.MyFullName + " Again", result2);
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ReturnNull_When_ExtensionGeneralAnnotationIsDeleted(IEdmElement annotatable)
    {
        var model = new EdmModel();

        var qualifiedNames = new MyQualifiedName[] {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        // set
        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());

        // delete
        foreach (MyQualifiedName q in qualifiedNames)
        {
            // ?? this is somewhat weird!!
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, null);

            var result = model.GetAnnotationValue<IEdmValue>(annotatable, q.NamespaceName, q.Name);
            Assert.Null(result);
        }

        Assert.Empty(model.DirectValueAnnotations(annotatable));
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_ThrowInvalidOperationException_When_ExtensionGeneralAnnotationTypeDoesNotMatch(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();
        var qualifiedNames = new MyQualifiedName[]
        {
            new("", ""),
            new("", "bar"),
            new("foo", ""),
            new("foo", "bar"),
            new("foo2", "bar"),
            new("foo", "bar2")
        };

        foreach (MyQualifiedName q in qualifiedNames)
        {
            model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
            Assert.Throws<InvalidOperationException>(() => model.GetAnnotationValue<string>(annotatable, q.NamespaceName, q.Name));
        }

        Assert.Equal(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count());
    }

    [Theory]
    [MemberData(nameof(GetEdmAnnotatable))]
    public void Should_NotThrowException_When_UsingInternalUriForAnnotation(IEdmElement annotatable)
    {
        // Arrange
        var model = new EdmModel();

        model.SetAnnotationValue(annotatable, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "foo", "bar");
        var internalObj = model.GetAnnotationValue(annotatable, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "foo");

        Assert.NotNull(internalObj);
        Assert.Equal("bar", internalObj);
    }

    [Fact]
    public void Should_ValidateAnnotations_When_SettingInternalAnnotations()
    {
        // Arrange
        var model = new EdmModel();
        var fredFlintstone = new EdmComplexType("Flintstones", "Fred");
        fredFlintstone.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        model.AddElement(fredFlintstone);

        foreach (IEdmElement annotatable in model.SchemaElements)
        {
            // set
            model.SetAnnotationValue(annotatable, "foo");
            model.SetAnnotationValue(annotatable, new MyQualifiedName("foo", "bar"));
            model.SetAnnotationValue<IEdmType>(annotatable, new EdmComplexType("foo", "foo", null, false));
            model.SetAnnotationValue<Dictionary<string, int>>(annotatable, new Dictionary<string, int>());

            Assert.Equal(4, model.DirectValueAnnotations(annotatable).Count());
        }

        IEnumerable<EdmError> errors;
        model.Validate(out errors);
        Assert.Empty(errors);
    }

    private class MyQualifiedName : IEquatable<MyQualifiedName>
    {
        public MyQualifiedName(string namespaceName, string name)
        {
            this.NamespaceName = namespaceName;
            this.Name = name;
        }

        public string NamespaceName { get; set; }
        public string Name { get; set; }

        public string MyFullName
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(this.NamespaceName).Append(":::").Append(this.Name);
                return sb.ToString();
            }
        }

        public override bool Equals(object? obj)
        {
            var other = obj as MyQualifiedName;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public bool Equals(MyQualifiedName? other)
        {
            return this.NamespaceName == other.NamespaceName && this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return this.MyFullName.GetHashCode();
        }
    }
}
