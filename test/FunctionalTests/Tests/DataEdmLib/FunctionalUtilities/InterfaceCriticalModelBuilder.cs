//---------------------------------------------------------------------
// <copyright file="InterfaceCriticalModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.StubEdm;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    class InterfaceCriticalModelBuilder
    {
        public static IEnumerable<XElement> InterfaceCriticalPropertyValueMustNotBeNullOnlyCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.String"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" />
    </Annotations>
</Schema>");
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullOnlyModel()
        {
            var model = new EdmModel();
            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetString(true));
            model.AddElement(valueTerm);

            var valueAnnotation = new MutableVocabularyAnnotation()
            {
                Target = valueTerm
            };
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEnumerable<XElement> InterfaceCriticalNavigationPartnerInvalidOnlyCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""AToB"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""BToA"" />
        <NavigationProperty Name=""BToA"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""Bad"" />
    </EntityType>
</Schema>");
        }

        public static IEdmModel EdmExpressionKindInterfaceCriticalKindValueUnexpectedOnlyModel()
        {
            var model = new EdmModel();
            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetString(true));
            model.AddElement(valueTerm);

            var badString = new CustomStringConstant("foo", EdmExpressionKind.None, EdmValueKind.String);

            var valueAnnotation = new EdmVocabularyAnnotation(
                valueTerm,
                valueTerm,
                badString);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEdmModel EdmValueKindInterfaceCriticalKindValueUnexpectedOnlyModel()
        {
            var model = new EdmModel();
            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetString(true));
            model.AddElement(valueTerm);

            var badString = new CustomStringConstant("foo", EdmExpressionKind.StringConstant, (EdmValueKind)123);

            var valueAnnotation = new EdmVocabularyAnnotation(
                valueTerm,
                valueTerm,
                badString);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEdmModel EdmTypeKindInterfaceCriticalKindValueUnexpectedOnlyModel()
        {
            var model = new EdmModel();

            var customEntity = new CustomEntityType((EdmTypeKind)123);
            model.AddElement(customEntity);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueMismatchOnlyModel()
        {
            var model = new EdmModel();

            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetString(false));
            model.AddElement(valueTerm);

            var badString = new CustomStringConstant("foo", EdmExpressionKind.StringConstant, EdmValueKind.Integer);
            var valueAnnotation = new EdmVocabularyAnnotation(
                valueTerm,
                valueTerm,
                badString);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueMismatchOnlyUsingComplexTypeReferenceModel()
        {
            var model = new EdmModel();

            var badTypeRef = new CustomComplexTypeReference(new EdmEntityType("NS", "Entity"), true);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueMismatchOnlyUsingEntityTypeReferenceModel()
        {
            var model = new EdmModel();

            var badTypeRef = new CustomEntityTypeReference(new EdmComplexType("NS", "Complex"), true);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueMismatchOnlyUsingEntityReferenceTypeReferenceModel()
        {
            var model = new EdmModel();

            var badTypeRef = new CustomEntityReferenceTypeReference(new EdmComplexType("NS", "Complex"), true);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingEntityReferenceTypeModel()
        {
            var model = new EdmModel();
            var badTypeRef = new EdmEntityReferenceTypeReference(new CustomEntityReferenceType(null), true);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueMismatchOnlyUsingBinaryTypeReferenceModel()
        {
            var model = new EdmModel();

            var badTypeRef = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingBinaryValueModel()
        {
            var model = new EdmModel();

            var valueTerm = new EdmTerm("NS", "Note", EdmCoreModel.Instance.GetBinary(true));
            model.AddElement(valueTerm);

            var badValue = new CustomBinaryConstant(null);
            var valueAnnotation = new EdmVocabularyAnnotation(
                valueTerm,
                valueTerm,
                badValue);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueMismatchOnlyUsingDecimalTypeReferenceModel()
        {
            var model = new EdmModel();

            var badTypeRef = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueMismatchOnlyUsingStringTypeReferenceModel()
        {
            var model = new EdmModel();

            var badTypeRef = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), true);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueMismatchOnlyUsingEnumTypeReferenceModel()
        {
            var model = new EdmModel();

            var badType = new CustomEnumType("NS", "Enum", EdmTypeKind.Complex);
            var badTypeRef = new EdmEnumTypeReference(badType, true);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingEnumTypeReferenceModel()
        {
            var model = new EdmModel();

            var badType = new CustomEnumType("NS", "Enum", (IEdmPrimitiveType)null);
            var badTypeRef = new EdmEnumTypeReference(badType, true);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingEnumMemberValueModel()
        {
            var model = new EdmModel();

            var enumType = new EdmEnumType("NS", "Enum");
            enumType.AddMember(new CustomEnumMember(enumType, "foo", null));
            var enumTypeRef = new EdmEnumTypeReference(enumType, true);
            var valueTerm = new EdmTerm("NS", "Note", enumTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingEnumMemberDeclaredTypeModel()
        {
            var model = new EdmModel();

            var enumType = new EdmEnumType("NS", "Enum");
            enumType.AddMember(new CustomEnumMember(null, "foo", new EdmEnumMemberValue(5)));
            var enumTypeRef = new EdmEnumTypeReference(enumType, true);
            var valueTerm = new EdmTerm("NS", "Note", enumTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingEnumValueNullMemberModel()
        {
            var model = new EdmModel();

            var enumType = new EdmEnumType("NS", "Enum");
            var enumMember = new CustomEnumMember(enumType, "foo", null);
            var enumTypeRef = new EdmEnumTypeReference(enumType, true);
            enumType.AddMember(enumMember);
            var valueTerm = new EdmTerm("NS", "Note", enumTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueMismatchOnlyUsingCollectionTypeReferenceModel()
        {
            var model = new EdmModel();

            var badType = new CustomCollectionType(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), EdmTypeKind.Enum);
            var badTypeRef = new EdmCollectionTypeReference(badType);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingCollectionTypeModel()
        {
            var model = new EdmModel();

            var badType = new CustomCollectionType(null, EdmTypeKind.Collection);
            var badTypeRef = new EdmCollectionTypeReference(badType);
            var valueTerm = new EdmTerm("NS", "Note", badTypeRef);
            model.AddElement(valueTerm);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingEntitySetElementTypeModel()
        {
            var model = new EdmModel();

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);

            var badSet = new CustomEntitySet(entityContainer, "Set", null);
            entityContainer.AddElement(badSet);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingEntitySetNullNavigationModel()
        {
            var model = new EdmModel();

            var entity = new EdmEntityType("NS", "Entity");
            var entityId = entity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            entity.AddKeys(entityId);
            model.AddElement(entity);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);

            var goodSet = new EdmEntitySet(entityContainer, "GoodSet", entity);
            entityContainer.AddElement(goodSet);

            var badSet = new CustomEntitySet(entityContainer, "BadSet", entity);
            badSet.AddNavigationTarget(null, goodSet);
            entityContainer.AddElement(badSet);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingEntitySetNullNavigationSetModel()
        {
            var model = new EdmModel();

            var entity = new EdmEntityType("NS", "Entity");
            var entityId = entity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            entity.AddKeys(entityId);
            var nav = entity.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Nav", Target = entity, TargetMultiplicity = EdmMultiplicity.One });
            model.AddElement(entity);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);

            var badSet = new CustomEntitySet(entityContainer, "BadSet", entity);
            badSet.AddNavigationTarget(nav, null);
            entityContainer.AddElement(badSet);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingOperationParameterTypeModel()
        {
            var model = new EdmModel();

            var operation = new EdmAction("NS", "Function", EdmCoreModel.Instance.GetInt32(true));
            var parameter = new CustomOperationParameter(operation, "Parameter", null);
            operation.AddParameter(parameter);
            model.AddElement(operation);

            return model;
        }

        public static IEdmModel InterfaceCriticalPropertyValueMustNotBeNullUsingOperationParameterDeclaredTypeModel()
        {
            var model = new EdmModel();

            var operation = new EdmFunction("NS", "Function", EdmCoreModel.Instance.GetInt32(true));
            var parameter = new CustomOperationParameter(null, "Parameter", EdmCoreModel.Instance.GetInt32(true));
            operation.AddParameter(parameter);
            model.AddElement(operation);

            return model;
        }

        public static IEdmModel InterfaceCriticalEnumerableMustNotHaveNullElementsOnlyModel()
        {
            var model = new EdmModel();

            var customEntity = new CustomEntityType(new List<IEdmProperty>() { null });
            model.AddElement(customEntity);

            return model;
        }

        public static IEdmModel AllInterfaceCriticalModel()
        {
            var model = new EdmModel();
            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetString(true));
            model.AddElement(valueTerm);

            var badString = new CustomStringConstant("foo", EdmExpressionKind.None, EdmValueKind.Integer);

            var valueAnnotation = new EdmVocabularyAnnotation(
                valueTerm,
                valueTerm,
                badString);
            model.AddVocabularyAnnotation(valueAnnotation);

            var mutableValueAnnotationueAnnotation = new MutableVocabularyAnnotation()
            {
                Target = valueTerm
            };

            model.AddVocabularyAnnotation(mutableValueAnnotationueAnnotation);

            var customEntity = new CustomEntityType(new List<IEdmProperty>() { null });
            model.AddElement(customEntity);

            var entity = new EdmEntityType("DefaultNamespace", "bar");
            var entity2 = new EdmEntityType("DefaultNamespace", "bar2");
            var navProperty = new StubEdmNavigationProperty("Nav")
            {
                DeclaringType = entity,
                Type = new EdmEntityTypeReference(entity2, false)
            };

            navProperty.Partner = navProperty;
            entity.AddProperty(navProperty);
            model.AddElement(entity);
            model.AddElement(entity2);

            return model;
        }

        public static IEdmModel InterfaceCriticalKindValueUnexpectedWithOtherErrorsModel()
        {
            var model = new EdmModel();
            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetString(true));
            model.AddElement(valueTerm);
            model.AddElement(valueTerm);

            var entity = new EdmEntityType("DefaultNamespace", "foo");
            model.AddElement(entity);

            var entityContainer = new EdmEntityContainer("DefaultNamespace", "container");
            model.AddElement(entityContainer);

            var badString = new CustomStringConstant("foo", EdmExpressionKind.None, EdmValueKind.String);

            var valueAnnotation = new EdmVocabularyAnnotation(
                valueTerm,
                valueTerm,
                badString);
            model.AddVocabularyAnnotation(valueAnnotation);

            var valueAnnotation2 = new EdmVocabularyAnnotation(
                valueTerm,
                valueTerm,
                new EdmStringConstant("foo"));
            model.AddVocabularyAnnotation(valueAnnotation2);

            return model;
        }

        public class CustomStringConstant : EdmValue, IEdmStringConstantExpression
        {
            private string value;
            private EdmValueKind valueKind;
            public CustomStringConstant(string value, EdmExpressionKind expressionKind, EdmValueKind valueKind)
                : base(null)
            {
                this.value = value;
                this.ExpressionKind = expressionKind;
                this.valueKind = valueKind;
            }

            public string Value
            {
                get { return this.value; }
                set { this.value = value; }
            }

            public EdmExpressionKind ExpressionKind { get; set; }

            public override EdmValueKind ValueKind
            {
                get { return this.valueKind; }
            }
        }

        public class CustomEntityType : EdmEntityType
        {
            private EdmTypeKind typeKind;
            private List<IEdmProperty> declaredProperties;

            public CustomEntityType(EdmTypeKind typeKind) : base("", "")
            {
                this.typeKind = typeKind;
                this.declaredProperties = new List<IEdmProperty>();
            }

            public CustomEntityType(IEnumerable<IEdmProperty> properties) : base("", "")
            {
                this.typeKind = EdmTypeKind.Entity;
                this.declaredProperties = properties.ToList<IEdmProperty>();
            }

            public override IEnumerable<IEdmProperty> DeclaredProperties
            {
                get { return this.declaredProperties; }
            }

            public override EdmTypeKind TypeKind
            {
                get { return this.typeKind; }
            }
        }

        private static IEnumerable<XElement> ConvertCsdlsToXElements(params string[] csdls)
        {
            return csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        }

        private sealed class CustomComplexTypeReference : EdmTypeReference, IEdmComplexTypeReference
        {
            public CustomComplexTypeReference(IEdmType definition, bool isNullable)
                : base(definition, isNullable)
            {
            }
        }

        private sealed class CustomEntityTypeReference : EdmTypeReference, IEdmEntityTypeReference
        {
            public CustomEntityTypeReference(IEdmType definition, bool isNullable)
                : base(definition, isNullable)
            {
            }
        }

        private sealed class CustomEntityReferenceTypeReference : EdmTypeReference, IEdmEntityReferenceTypeReference
        {
            public CustomEntityReferenceTypeReference(IEdmType type, bool isNullable)
                : base(type, isNullable)
            {
            }

            public IEdmEntityReferenceType EntityReferenceDefinition
            {
                get { return (IEdmEntityReferenceType)Definition; }
            }
        }

        private sealed class CustomEntityReferenceType : EdmType, IEdmEntityReferenceType
        {
            private readonly IEdmEntityType entityType;

            public CustomEntityReferenceType(IEdmEntityType entityType)
            {
                this.entityType = entityType;
            }

            public IEdmEntityType EntityType
            {
                get { return this.entityType; }
            }

            public override EdmTypeKind TypeKind
            {
                get { return EdmTypeKind.EntityReference; }
            }
        }

        private sealed class CustomBinaryConstant : EdmValue, IEdmBinaryConstantExpression
        {
            private readonly byte[] value;

            public CustomBinaryConstant(byte[] value)
                : base(null)
            {
                this.value = value;
            }

            public byte[] Value
            {
                get { return this.value; }
            }

            public EdmExpressionKind ExpressionKind
            {
                get { return EdmExpressionKind.BinaryConstant; }
            }

            public override EdmValueKind ValueKind
            {
                get { return EdmValueKind.Binary; }
            }
        }

        private sealed class CustomEnumType : EdmType, IEdmEnumType
        {
            private readonly EdmTypeKind typeKind;
            private readonly IEdmPrimitiveType underlyingType;
            private readonly string namespaceName;
            private readonly string name;
            private readonly bool isFlags;
            private readonly List<IEdmEnumMember> members = new List<IEdmEnumMember>();

            public CustomEnumType(string namespaceName, string name, EdmTypeKind typeKind)
            {
                this.typeKind = typeKind;
                this.name = name;
                this.namespaceName = namespaceName;
                this.isFlags = false;
                this.underlyingType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
            }

            public CustomEnumType(string namespaceName, string name, IEdmPrimitiveType underlyingType)
            {
                this.typeKind = EdmTypeKind.Enum;
                this.name = name;
                this.namespaceName = namespaceName;
                this.isFlags = false;
                this.underlyingType = underlyingType;
            }

            public override EdmTypeKind TypeKind
            {
                get { return typeKind; }
            }

            public IEdmPrimitiveType UnderlyingType
            {
                get { return this.underlyingType; }
            }

            public IEnumerable<IEdmEnumMember> Members
            {
                get { return this.members; }
            }

            public bool IsFlags
            {
                get { return this.isFlags; }
            }

            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.TypeDefinition; }
            }

            public string Namespace
            {
                get { return this.namespaceName; }
            }

            public string Name
            {
                get { return this.name; }
            }
        }

        private sealed class CustomEnumMember : EdmNamedElement, IEdmEnumMember
        {
            private readonly IEdmEnumType declaringType;
            private IEdmEnumMemberValue value;

            public CustomEnumMember(IEdmEnumType declaringType, string name, IEdmEnumMemberValue value)
                : base(name)
            {
                this.declaringType = declaringType;
                this.value = value;
            }

            public IEdmEnumType DeclaringType
            {
                get { return this.declaringType; }
            }

            public IEdmEnumMemberValue Value
            {
                get { return this.value; }
            }
        }

        private sealed class CustomCollectionType : EdmType, IEdmCollectionType
        {
            private readonly EdmTypeKind typeKind;
            private readonly IEdmTypeReference elementType;

            public CustomCollectionType(IEdmTypeReference elementType, EdmTypeKind typeKind)
            {
                this.elementType = elementType;
                this.typeKind = typeKind;
            }

            public override EdmTypeKind TypeKind
            {
                get { return this.typeKind; }
            }

            public IEdmTypeReference ElementType
            {
                get { return this.elementType; }
            }
        }

        private sealed class CustomEntitySet : EdmNamedElement, IEdmEntitySet
        {
            private readonly IEdmEntityContainer container;
            private IEdmType type;
            private readonly List<IEdmNavigationPropertyBinding> navigationPropertyBindings = new List<IEdmNavigationPropertyBinding>();

            public CustomEntitySet(IEdmEntityContainer container, string name, IEdmEntityType elementType)
                 : this(container, name, elementType, false)
            {
            }

            public CustomEntitySet(IEdmEntityContainer container, string name, IEdmEntityType elementType, bool includeInServiceDocument)
                : base(name)
            {
                this.container = container;
                if (elementType != null)
                {
                    this.type = new EdmCollectionType(new EdmEntityTypeReference(elementType, false));
                }
                this.IncludeInServiceDocument = includeInServiceDocument;
            }

            public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
            {
                get { return this.navigationPropertyBindings; }
            }

            public void AddNavigationTarget(IEdmNavigationProperty property, IEdmEntitySet target)
            {
                this.navigationPropertyBindings.Add(new EdmNavigationPropertyBinding(property, target));
            }

            public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
            {
                return null;
            }

            public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
            {
                return null;
            }

            public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
            {
                return null;
            }

            public EdmContainerElementKind ContainerElementKind
            {
                get { return EdmContainerElementKind.EntitySet; }
            }

            public IEdmEntityContainer Container
            {
                get { return this.container; }
            }

            public IEdmPathExpression Path
            {
                get { return null; }
            }

            public IEdmType Type
            {
                get { return type; }
            }

            public bool IncludeInServiceDocument
            {
                get; internal set;
            }
        }

        private sealed class CustomOperationParameter : EdmNamedElement, IEdmOperationParameter
        {
            public CustomOperationParameter(IEdmOperation declaringOperation, string name, IEdmTypeReference type)
                : base(name)
            {
                this.Type = type;
                this.DeclaringOperation = declaringOperation;
            }

            public IEdmTypeReference Type { get; private set; }

            public IEdmOperation DeclaringOperation { get; private set; }
        }

        private sealed class CustomPropertyValueBinding : EdmElement, IEdmPropertyValueBinding
        {
            private readonly IEdmProperty boundProperty;
            private readonly IEdmExpression value;

            public CustomPropertyValueBinding(IEdmProperty boundProperty, IEdmExpression value)
            {
                this.boundProperty = boundProperty;
                this.value = value;
            }

            public IEdmProperty BoundProperty
            {
                get { return this.boundProperty; }
            }

            public IEdmExpression Value
            {
                get { return this.value; }
            }
        }
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
    }
}
