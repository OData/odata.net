//---------------------------------------------------------------------
// <copyright file="EdmErrorCode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// EdmLib validation error codes
    /// </summary>
    public enum EdmErrorCode
    {
        /// <summary>
        /// Invalid error code
        /// </summary>
        InvalidErrorCodeValue = 0,

        // IOException = 4,

        /// <summary>
        /// An exception was thrown by the underlying xml reader.
        /// </summary>
        XmlError = 5,

        // TooManyErrors = 6,
        // MalformedXml = 7,

        /// <summary>
        /// Encountered an XML node that was never used
        /// </summary>
        UnexpectedXmlNodeType = 8,

        /// <summary>
        /// Encountered an XML attribute that was never used
        /// </summary>
        UnexpectedXmlAttribute = 9,

        /// <summary>
        /// Encountered an XML element that was never used
        /// </summary>
        UnexpectedXmlElement = 10,

        /// <summary>
        /// Text was found in a location it was not allowed in
        /// </summary>
        TextNotAllowed = 11,

        /// <summary>
        /// An empty file was provided to the parser
        /// </summary>
        EmptyFile = 12,

        // XsdError = 13,
        // InvalidAlias = 14,

        /// <summary>
        /// An XML element was missing a required attribute
        /// </summary>
        MissingAttribute = 15,

        // IntegerExpected = 16,

        /// <summary>
        /// Invalid Name
        /// </summary>
        InvalidName = 17,

        /// <summary>
        /// An XML attribute or element representing EDM type is missing.
        /// </summary>
        MissingType = 18,

        /// <summary>
        /// Element name is already defined in this context.
        /// </summary>
        AlreadyDefined = 19,

        // ElementNotInSchema = 20,
        // unused 21,
        // InvalidBaseType = 22,
        // NoConcreteDescendants = 23,
        // 24,

        /// <summary>
        /// The specified version number is not valid.
        /// </summary>
        InvalidVersionNumber = 25,

        // InvalidSize = 26,

        /// <summary>
        /// Malformed boolean value.
        /// </summary>
        InvalidBoolean = 27,

        // unused 28,
        // BadType = 29,
        // unused 30,
        // unused 31,
        // InvalidVersioningClass = 32,
        // InvalidVersionIntroduced = 33,
        // BadNamespace = 34,
        // unused 35,
        // unused 36,
        // unused 37,
        // UnresolvedReferenceSchema = 38,
        // unused 39,
        // NotInNamespace = 40,
        // NotUnnestedType = 41,

        /// <summary>
        /// The property contains an error.
        /// </summary>
        BadProperty = 42,

        // UndefinedProperty = 43,

        /// <summary>
        /// The type of this property is invalid for the given context.
        /// </summary>
        InvalidPropertyType = 44,

        // InvalidAsNestedType = 45,
        // InvalidChangeUnit = 46,
        // UnauthorizedAccessException = 47,
        // unused 48,
        // unused 49,
        // unused 50,

        /// <summary>
        /// Precision out of range
        /// </summary>
        PrecisionOutOfRange = 51,

        /// <summary>
        /// Scale out of range
        /// </summary>
        ScaleOutOfRange = 52,

        // DefaultNotAllowed = 53,
        // InvalidDefault = 54,
        //// <summary>One of the required facets is missing</summary>
        // RequiredFacetMissing = 55,
        // BadImageFormatException = 56,
        // MissingSchemaXml = 57,
        // BadPrecisionAndScale = 58,
        // InvalidChangeUnitUsage = 59,

        /// <summary>
        /// Name is too long.
        /// </summary>
        NameTooLong = 60,

        // CircularlyDefinedType = 61,

        /// <summary>
        /// The provided association is invalid
        /// </summary>
        InvalidAssociation = 62,

        //// <summary>
        //// The facet isn't allow by the property type.
        //// </summary>
        // FacetNotAllowedByType = 63,
        //// <summary>
        //// This facet value is constant and is specified in the schema
        //// </summary>
        // ConstantFacetSpecifiedInSchema = 64,
        // unused 65,
        // unused 66,
        // unused 67,
        // unused 68,
        // unused 69,
        // unused 70,
        // unused 71,
        // unused 72,
        // unused 73,

        /// <summary>
        /// Navigation property contains errors.
        /// </summary>
        BadNavigationProperty = 74,

        /// <summary>
        /// Entity key is invalid.
        /// </summary>
        InvalidKey = 75,

        /// <summary>
        /// The value of the property must not be null.
        /// </summary>
        InterfaceCriticalPropertyValueMustNotBeNull = 76,

        /// <summary>
        /// An object with an interface kind property does not implement the interface corresponding to the value of that property.
        /// For example this error will be reported for an object that implements <see cref="IEdmType"/> interface with kind property reporting <see cref="EdmTypeKind"/>.Entity,
        /// but does not implement <see cref="IEdmEntityType"/> interface.
        /// </summary>
        InterfaceCriticalKindValueMismatch = 77,

        /// <summary>
        /// The value of an interface kind property is not semantically valid. A semantically valid model must not contain elements of kind 'None'.
        /// </summary>
        InterfaceCriticalKindValueUnexpected = 78,

        /// <summary>
        /// An enumeration property must not contain null elements.
        /// </summary>
        InterfaceCriticalEnumerableMustNotHaveNullElements = 79,

        /// <summary>
        /// The value of the enum type property is out of range.
        /// </summary>
        InterfaceCriticalEnumPropertyValueOutOfRange = 80,

        /// <summary>
        /// If property P1 is a navigation property and P2 is its parnter, then partner property of P2 must be P1.
        /// </summary>
        InterfaceCriticalNavigationPartnerInvalid = 81,

        /// <summary>
        /// A chain of base types is cyclic.
        /// </summary>
        InterfaceCriticalCycleInTypeHierarchy = 82,

        // reserved for critical structural errors 83,
        // reserved for critical structural errors 84,
        // reserved for critical structural errors 85,
        // reserved for critical structural errors 86,
        // reserved for critical structural errors 87,
        // reserved for critical structural errors 88,
        // reserved for critical structural errors 89,
        // reserved for critical structural errors 90,
        // reserved for critical structural errors 91,

        /// <summary>
        /// Multiplicity value was malformed
        /// </summary>
        InvalidMultiplicity = 92,

        // unused = 93
        // unused = 94
        // unused = 95

        /// <summary>
        /// The value for the Action attribute is invalid or not allowed in the current context
        /// </summary>
        InvalidAction = 96,

        /// <summary>
        /// An error occured processing the OnDelete element
        /// </summary>
        InvalidOnDelete = 97,

        /// <summary>
        /// No complex type with that name exists.
        /// </summary>
        BadUnresolvedComplexType = 98,

        //// <summary>Ends were given for the Property element of a EntityContainer that is not a RelationshipSet</summary>
        // InvalidContainerTypeForEnd = 99,

        /// <summary>
        /// The extent name used in the EntittyContainerType End does not match the name of any of the EntityContainerProperties in the containing EntityContainer
        /// </summary>
        InvalidEndEntitySet = 100,

        //// <summary>An end element was not given, and cannot be inferred because too many EntityContainerEntitySet elements that are good possibilities.</summary>
        // AmbiguousEntityContainerEnd = 101,
        //// <summary>An end element was not given, and cannot be infered because there is no EntityContainerEntitySets that are the correct type to be used as an EntitySet.</summary>
        // MissingExtentEntityContainerEnd = 102,

        /// <summary>
        /// Operation import specifies an entity set expression which is not supported in this context. Operation import entity set expression can be either an entity set reference or a path starting with a operation import parameter and traversing navigation properties.
        /// </summary>
        OperationImportEntitySetExpressionIsInvalid = 103,

        //// unused 104,
        //// unused 105,
        //// <summary>Not a valid parameter direction for the parameter in a function</summary>
        // BadParameterDirection = 106,
        //// <summary>Unable to infer an optional schema part, to resolve this, be more explicit</summary>
        // FailedInference = 107,
        //// unused = 108,

        /// <summary>
        /// The target entity set must be able to hold an entity that is valid for the navigation property of a mapping.
        /// </summary>
        NavigationPropertyMappingMustPointToValidTargetForProperty = 109,

        /// <summary>
        /// Invalid role value in the relationship constraint
        /// </summary>
        InvalidRoleInRelationshipConstraint = 110,

        /// <summary>
        /// Invalid Property in relationship constraint
        /// </summary>
        InvalidPropertyInRelationshipConstraint = 111,

        /// <summary>
        /// Type mismatch between ToProperty and FromProperty in the relationship constraint
        /// </summary>
        TypeMismatchRelationshipConstraint = 112,

        /// <summary>
        /// Invalid multiplicty of the principal end of a navigation.
        /// </summary>
        InvalidMultiplicityOfPrincipalEnd = 113,

        /// <summary>
        /// The number of properties in the FromProperty and ToProperty in the relationship constraint must be identical
        /// </summary>
        MismatchNumberOfPropertiesInRelationshipConstraint = 114,

        //// <summary> No Properties defined in either FromProperty or ToProperty in the relationship constraint</summary>
        // MissingPropertyInRelationshipConstraint = 115,

        /// <summary>
        /// Invalid multiplicty of the dependent end of a navigation.
        /// </summary>
        InvalidMultiplicityOfDependentEnd = 116,

        /// <summary>
        /// Open types are supported only in version 1.2 and after version 2.0. Only entity types can be open.
        /// </summary>
        OpenTypeNotSupported = 117,

        /// <summary>
        /// Same role referred in the ToRole and FromRole of a referential constraint
        /// </summary>
        SameRoleReferredInReferentialConstraint = 119,

        //// <summary> Invalid value for attribute ParameterTypeSemantics </summary>
        // InvalidValueForParameterTypeSemantics = 120,
        //// <summary> Invalid type used for a Relationship End Type</summary>
        // InvalidRelationshipEndType = 121,
        //// <summary> Invalid PrimitiveTypeKind</summary>
        // InvalidPrimitiveTypeKind = 122,
        // unused 123,
        //// <summary> Invalid TypeConversion DestinationType</summary>
        // InvalidTypeConversionDestinationType = 124,
        //// <summary>Expected a integer value between 0 - 255</summary>
        // ByteValueExpected = 125,
        //// <summary> Invalid Type specified in function</summary>
        // FunctionWithNonScalarTypeNotSupported = 126,
        //// <summary> Precision must not be greater than 28 </summary>
        // PrecisionMoreThanAllowedMax = 127,

        /// <summary>
        /// Properties that are part of entity key must be of scalar type
        /// </summary>
        EntityKeyMustBeScalar = 128,

        /// <summary>
        /// Binary type properties which are part of entity key are currently supported before V2.0
        /// </summary>
        EntityKeyMustNotBeBinary = 129,

        //// <summary>The primitive type kind does not have a prefered mapping</summary>
        // NoPreferredMappingForPrimitiveTypeKind = 130,
        //// <summary>More than one PreferredMapping for a PrimitiveTypeKind</summary>
        // TooManyPreferredMappingsForPrimitiveTypeKind = 131,

        /// <summary>
        /// End with * multiplicity cannot have operations specified
        /// </summary>
        EndWithManyMultiplicityCannotHaveOperationsSpecified = 132,

        /// <summary>
        /// Navigation source type has no keys
        /// </summary>
        NavigationSourceTypeHasNoKeys = 133,

        //// <summary>Invalid Number Of Parameters For Aggregate Function</summary>
        // InvalidNumberOfParametersForAggregateFunction = 134,
        //// <summary>Invalid Parameter Type For Aggregate Function</summary>
        // InvalidParameterTypeForAggregateFunction = 135,
        //// <summary>Composable functions must declare a return type.</summary>
        // ComposableFunctionWithoutReturnType = 136,
        //// <summary>Non-composable functions must not declare a return type.</summary>
        // NonComposableFunctionWithReturnType = 137,
        //// <summary>Non-composable functions do not permit the aggregate, niladic, or built-in attributes.</summary>
        // NonComposableFunctionAttributesNotValid = 138,
        //// <summary>Composable functions can not include command text attribute.</summary>
        // ComposableFunctionWithCommandText = 139,
        //// <summary>Functions should not declare both a store name and command text (only one or the other
        //// can be used).</summary>
        // FunctionDeclaresCommandTextAndStoreFunctionName = 140,
        //// <summary>System Namespace</summary>
        // SystemNamespace = 141,
        //// <summary>Empty DefiningQuery text</summary>
        // EmptyDefiningQuery = 142,
        //// <summary>Schema, Table and DefiningQuery are all specified, and are mutualy exlusive</summary>
        // TableAndSchemaAreMutuallyExclusiveWithDefiningQuery = 143,

        /// <summary>
        /// Conurency can't change for any sub types of an EntitySet type.
        /// </summary>
        ConcurrencyRedefinedOnSubtypeOfEntitySetType = 145,

        /// <summary>
        /// In version 1.0 operation import can have no return type or return a collection of scalars or a collection of entities.
        /// In all other versions operation import can have no return type or return a scalar, a complex type, an entity type or a collection of those.
        /// </summary>
        OperationImportUnsupportedReturnType = 146,

        /// <summary>
        /// Operation import specifies entity type return but no entity set.
        /// </summary>
        OperationImportReturnsEntitiesButDoesNotSpecifyEntitySet = 148,

        /// <summary>
        /// Operation import specifies entity type that does not derive from element type of entity set.
        /// </summary>
        OperationImportEntityTypeDoesNotMatchEntitySet = 149,

        /// <summary>
        /// Operation import specifies a binding to an entity set but does not return entities.
        /// </summary>
        OperationImportSpecifiesEntitySetButDoesNotReturnEntityType = 150,

        /// <summary>
        /// Operation import cannot import a bound function.
        /// </summary>
        OperationImportCannotImportBoundOperation = 151,

        /// <summary>
        /// A function must have return type.
        /// </summary>
        FunctionMustHaveReturnType = 152,

        /// <summary>
        /// Same Entity Set Taking part in the same role of the relationship set in two different relationship sets
        /// </summary>
        SimilarRelationshipEnd = 153,

        /// <summary>
        /// Entity key refers to the same property twice
        /// </summary>
        DuplicatePropertySpecifiedInEntityKey = 154,

        //// <summary> Function declares a ReturnType attribute and element</summary>
        // AmbiguousFunctionReturnType = 156,

        /// <summary>
        /// Nullable complex Type not supported in version 1.0 and 2.0.
        /// </summary>
        NullableComplexTypeProperty = 157,

        //// <summary> Only Complex Collections supported in Edm V1.1</summary>
        // NonComplexCollections = 158,

        /// <summary>
        /// No Key defined on Entity Type
        /// </summary>
        KeyMissingOnEntityType = 159,

        //// <summary> Invalid namespace specified in using element</summary>
        // InvalidNamespaceInUsing = 160,

        /// <summary>
        /// Need not specify system namespace in using
        /// </summary>
        SystemNamespaceEncountered = 161,

        //// <summary> Cannot use a reserved/system namespace as alias </summary>
        // CannotUseSystemNamespaceAsAlias = 162,

        /// <summary>
        /// Invalid qualification specified for type
        /// </summary>
        InvalidNamespaceName = 163,

        //// <summary> Invalid Entity Container Name in extends attribute </summary>
        // InvalidEntityContainerNameInExtends = 164,
        //// <summary> Invalid CollectionKind value in property CollectionKind attribute</summary>
        // InvalidCollectionKind = 165,
        //// <summary> Must specify namespace or alias of the schema in which this type is defined </summary>
        // InvalidNamespaceOrAliasSpecified = 166,
        //// <summary> Entity Container cannot extend itself </summary>
        // EntityContainerCannotExtendItself = 167,
        //// <summary> Failed to retrieve provider manifest </summary>
        // FailedToRetrieveProviderManifest = 168,
        //// <summary> Mismatched Provider Manifest token values in SSDL artifacts </summary>
        // ProviderManifestTokenMismatch = 169,
        //// <summary> Missing Provider Manifest token value in SSDL artifact(s) </summary>
        // ProviderManifestTokenNotFound = 170,
        //// <summary>Empty CommandText element</summary>
        // EmptyCommandText = 171,
        //// <summary> Inconsistent Provider values in SSDL artifacts </summary>
        // InconsistentProvider = 172,
        //// <summary> Inconsistent Provider Manifest token values in SSDL artifacts </summary>
        // InconsistentProviderManifestToken = 173,
        //// <summary> Duplicated Function overloads </summary>
        // DuplicatedFunctionoverloads = 174,
        //// <summary>Invalid Provider</summary>
        // InvalidProvider = 175,
        //// <summary>Function With Non Edm Type Not Supported</summary>
        // FunctionWithNonEdmTypeNotSupported = 176,
        //// <summary>Complex Type As Return Type And Defined Entity Set</summary>
        // ComplexTypeAsReturnTypeAndDefinedEntitySet = 177,
        //// <summary>Complex Type As Return Type And Defined Entity Set</summary>
        // ComplexTypeAsReturnTypeAndNestedComplexProperty = 178,
        //// unused 179,
        //// unused 180,
        //// unused 181,
        //// <summary>In model functions facet attribute is allowed only on ScalarTypes</summary>
        // FacetOnNonScalarType = 182,
        //// <summary>Captures several conditions where facets are placed on element where it should not exist.</summary>
        // IncorrectlyPlacedFacet = 183,
        //// <summary>Return type has not been declared</summary>
        // ReturnTypeNotDeclared = 184,
        // TypeNotDeclared = 185,
        // RowTypeWithoutProperty = 186,
        // ReturnTypeDeclaredAsAttributeAndElement = 187,
        // TypeDeclaredAsAttributeAndElement = 188,
        // ReferenceToNonEntityType = 189,
        //// <summary>Invalid value in the EnumTypeOption</summary>
        // InvalidValueInEnumOption = 190,
        // IncompatibleSchemaVersion = 191,
        //// <summary> The structural annotation cannot use codegen namespaces </summary>
        // NoCodeGenNamespaceInStructuralAnnotation = 192,
        //// <summary> Function and type cannot have the same fully qualified name</summary>
        // AmbiguousFunctionAndType = 193,
        //// <summary> Cannot load different version of schema in the same ItemCollection</summary>
        // CannotLoadDifferentVersionOfSchemaInTheSameItemCollection = 194,
        //// <summary> Expected bool value</summary>
        // BoolValueExpected = 195,
        //// <summary> End without Multiplicity specified</summary>
        // EndWithoutMultiplicity = 196,
        //// <summary>In SSDL, if composable function returns a collection of rows (TVF), all row properties must be of scalar types.</summary>
        // TVFReturnTypeRowHasNonScalarProperty = 197,
        //// <summary> The name of NamedEdmItem must not be empty or white space only</summary>
        // EdmModel_NameMustNotBeEmptyOrWhiteSpace = 198,
        //// <summary> EdmTypeReference is empty</summary>
        //// Unused 199,
        // EdmAssociationType_AssociationEndMustNotBeNull = 200,
        // EdmAssociationConstraint_DependentEndMustNotBeNull = 201,
        // EdmAssociationConstraint_DependentPropertiesMustNotBeEmpty = 202,
        // EdmNavigationProperty_AssociationMustNotBeNull = 203,
        // EdmNavigationProperty_ResultEndMustNotBeNull = 204,
        // EdmAssociationEnd_EntityTypeMustNotBeNull = 205,

        /// <summary>
        /// The enumeration member must have a value.
        /// </summary>
        EnumMemberMustHaveValue = 206,

        // EdmFunctionImport_ReturnTypeMustBeCollectionType = 210,
        // EdmModel_NameIsNotAllowed = 211,
        // EdmTypeReferenceNotValid = 212,
        // EdmFunctionNotExistsInV1 = 213,
        // EdmFunctionNotExistsInV1_1 = 214,
        // Serializer_OneNamespaceAndOneContainer = 215,
        // EdmModel_Validator_Semantic_InvalidEdmTypeReference = 216,
        // EdmModel_Validator_TypeNameAlreadyDefinedDuplicate = 217,

        /// <summary>
        /// The entity container name has already been assigned to a different entity container.
        /// </summary>
        DuplicateEntityContainerMemberName = 218,

        /// <summary>
        /// An unbound function overload has a different return type.
        /// </summary>
        UnboundFunctionOverloadHasIncorrectReturnType = 219,

        // EdmFunction_UnsupportedParameterType = 219,

        /// <summary>
        /// Complex types were not allowed to be abstract here.
        /// </summary>
        InvalidAbstractComplexType = 220,

        /// <summary>
        /// Complex types cannot have base types in this version.
        /// </summary>
        InvalidPolymorphicComplexType = 221,

        /// <summary>
        /// A navigation property without direct containment cannot contain its declaring entity indirectly.
        /// </summary>
        NavigationPropertyEntityMustNotIndirectlyContainItself = 222,

        /// <summary>
        /// If a navigation property mapping is of a recursive navigation property, the mapping must point back to the same entity set.
        /// </summary>
        EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet = 223,

        /// <summary>
        /// Name collision makes this name ambiguous.
        /// </summary>
        BadAmbiguousElementBinding = 224,

        /// <summary>
        /// Could not find a type with this name.
        /// </summary>
        BadUnresolvedType = 225,

        /// <summary>
        /// Could not find a primitive type with this name.
        /// </summary>
        BadUnresolvedPrimitiveType = 226,

        /// <summary>
        /// This complex type is part of a cycle.
        /// </summary>
        BadCyclicComplex = 227,

        /// <summary>
        /// This Entity Container is bad because some part of its extends hierarchy is part of a cycle.
        /// </summary>
        BadCyclicEntityContainer = 228,

        /// <summary>
        /// This entity type is part of a cycle.
        /// </summary>
        BadCyclicEntity = 229,

        /// <summary>
        /// Could not convert type reference to the requested type.
        /// </summary>
        TypeSemanticsCouldNotConvertTypeReference = 230,

        /// <summary>
        /// This entity set became invalid because the entity that it was of the type of was removed from the model.
        /// </summary>
        ConstructibleEntitySetTypeInvalidFromEntityTypeRemoval = 231,

        /// <summary>
        /// Could not find an EntityContainer with that name.
        /// </summary>
        BadUnresolvedEntityContainer = 232,

        /// <summary>
        /// Could not find an EntitySet with that name.
        /// </summary>
        BadUnresolvedEntitySet = 233,

        /// <summary>
        /// Could not find a property with that name
        /// </summary>
        BadUnresolvedProperty = 234,

        /// <summary>
        /// Could not find an association end with that name
        /// </summary>
        BadNonComputableAssociationEnd = 235,

        /// <summary>
        /// Type of the navigation property was invalid because the association of the navigation property was invalid.
        /// </summary>
        NavigationPropertyTypeInvalidBecauseOfBadAssociation = 236,

        /// <summary>
        /// The base type of an entity must also be an entity.
        /// </summary>
        EntityMustHaveEntityBaseType = 237,

        /// <summary>
        /// The base type of a complex type must also be complex.
        /// </summary>
        ComplexTypeMustHaveComplexBaseType = 238,

        /// <summary>
        /// Could not find a operation with this name.
        /// </summary>
        BadUnresolvedOperation = 239,

        /// <summary>
        /// Every property in an entity key must be a property of the entity.
        /// </summary>
        KeyPropertyMustBelongToEntity = 242,

        /// <summary>
        /// The principal end of a referential constraint must be one of the ends of the association that defined the referential constraint.
        /// </summary>
        ReferentialConstraintPrincipalEndMustBelongToAssociation = 243,

        /// <summary>
        /// Dependent properties of a referential constraint must belong to the dependent entity set.
        /// </summary>
        DependentPropertiesMustBelongToDependentEntity = 244,

        /// <summary>
        /// If a structured type declares a property, that properties declaring type must be the declaring structured type.
        /// </summary>
        DeclaringTypeMustBeCorrect = 245,

        /// <summary>
        /// Navigation property has a type that is not an entity or collection of entities.
        /// </summary>
        InvalidNavigationPropertyType = 258,

        // unused 259

        // unused 260

        /// <summary>
        /// Underlying type of the enumeration type is bad because the enumeration type is bad.
        /// </summary>
        UnderlyingTypeIsBadBecauseEnumTypeIsBad = 261,

        /// <summary>
        /// Complex types must contain at least one property.
        /// </summary>
        ComplexTypeMustHaveProperties = 264,

        /// <summary>
        /// Unsupported operation import parameter type.
        /// </summary>
        OperationImportParameterIncorrectType = 265,

        /// <summary>
        /// A referential constraint cannot have multiple dependent properties with the same name.
        /// </summary>
        DuplicateDependentProperty = 267,

        /// <summary>
        /// Bindable operation must have at least one parameter.
        /// </summary>
        BoundOperationMustHaveParameters = 268,

        /// <summary>
        /// Operation with an EntitySetPath must be on a bound operation.
        /// </summary>
        OperationCannotHaveEntitySetPathWithUnBoundOperation = 269,

        /// <summary>
        /// Operation with an EntitySetPath must have the first path item be the same name as the binding parameter.
        /// </summary>
        InvalidPathFirstPathParameterNotMatchingFirstParameterName = 271,

        /// <summary>
        /// Operation with an EntitySetPath references a binding parameter that is not an entity type.
        /// </summary>
        InvalidPathWithNonEntityBindingParameter = 246,

        /// <summary>
        /// Operation with an EntitySetPath segment is invalid as it has less than two items in the path.
        /// </summary>
        OperationWithInvalidEntitySetPathMissingCompletePath = 248,

        /// <summary>
        /// Operation with an EntitySetPath segment has an unknown type cast segment.
        /// </summary>
        InvalidPathUnknownTypeCastSegment = 249,

        /// <summary>
        /// Operation with an EntitySetPath segment has an invalid type cast segment.
        /// </summary>
        InvalidPathInvalidTypeCastSegment = 250,

        /// <summary>
        /// Operation with an EntitySetPath segment has an invalid type cast segment, it must be an EntityType.
        /// </summary>
        InvalidPathTypeCastSegmentMustBeEntityType = 251,

        /// <summary>
        /// Operation with an EntitySetPath segment has an unknown navigation property.
        /// </summary>
        InvalidPathUnknownNavigationProperty = 252,

        /// <summary>
        /// Operation with an EntitySetPath has a return type that is not assignable to the resulting determined type from the entity set path.
        /// </summary>
        OperationWithEntitySetPathAndReturnTypeTypeNotAssignable = 253,

        /// <summary>
        /// Operation entity set path resolves to a collection entity type when an entity type is expected
        /// </summary>
        OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType = 254,

        /// <summary>
        /// Operation entity set path resolves to an entity type when a collection of entity type is expected.
        /// </summary>
        OperationWithEntitySetPathResolvesToEntityTypeMismatchesCollectionEntityTypeReturnType = 255,

        /// <summary>
        /// Operation with an EntitySetPath has an invalid return type. The return type must be an entity type or collection of entity type.
        /// </summary>
        OperationWithEntitySetPathReturnTypeInvalid = 256,

        /// <summary>
        /// Max length is out of range.
        /// </summary>
        MaxLengthOutOfRange = 272,

        /// <summary>
        /// Binding context for Path expression does not supply an entity type
        /// </summary>
        PathExpressionHasNoEntityContext = 274,

        /// <summary>
        /// Invalid value for SRID
        /// </summary>
        InvalidSrid = 275,

        /// <summary>
        /// Invalid value for max length
        /// </summary>
        InvalidMaxLength = 276,

        /// <summary>
        /// Invalid value for long
        /// </summary>
        InvalidLong = 277,

        /// <summary>
        /// Invalid value for integer
        /// </summary>
        InvalidInteger = 278,

        /// <summary>
        /// Invalid association set
        /// </summary>
        InvalidAssociationSet = 279,

        /// <summary>
        /// Invalid parameter mode
        /// </summary>
        InvalidParameterMode = 280,

        /// <summary>
        /// No entity type with that name exists.
        /// </summary>
        BadUnresolvedEntityType = 281,

        /// <summary>
        /// Value is invalid
        /// </summary>
        InvalidValue = 282,

        /// <summary>
        /// Binary value is invalid.
        /// </summary>
        InvalidBinary = 283,

        /// <summary>
        /// Floating point value is invalid.
        /// </summary>
        InvalidFloatingPoint = 284,

        /// <summary>
        /// DateTime value is invalid.
        /// </summary>
        InvalidDateTime = 285,

        /// <summary>
        /// DateTimeOffset value is invalid.
        /// </summary>
        InvalidDateTimeOffset = 286,

        /// <summary>
        /// Decimal value is invalid.
        /// </summary>
        InvalidDecimal = 287,

        /// <summary>
        /// Guid value is invalid.
        /// </summary>
        InvalidGuid = 288,

        /// <summary>
        /// The type kind None is not semantically valid. A semantically valid model must not contain elements of type kind None.
        /// </summary>
        InvalidTypeKindNone = 289,

        /// <summary>
        /// The if expression is invalid because it does not have 3 elements.
        /// </summary>
        InvalidIfExpressionIncorrectNumberOfOperands = 290,

        /// <summary>
        /// The enum member value is out of range of its underlying type.
        /// </summary>
        EnumMemberValueOutOfRange = 292,

        /// <summary>
        /// The IsType expression is invalid because it does not have 1 element.
        /// </summary>
        InvalidIsTypeExpressionIncorrectNumberOfOperands = 293,

        /// <summary>
        /// The type name is not fully qualified and not a primitive.
        /// </summary>
        InvalidTypeName = 294,

        /// <summary>
        /// The term name is not fully qualified.
        /// </summary>
        InvalidQualifiedName = 295,

        /// <summary>
        /// No model was parsed because no XmlReaders were provided.
        /// </summary>
        NoReadersProvided = 296,

        /// <summary>
        /// Model could not be parsed because one of the XmlReaders was null.
        /// </summary>
        NullXmlReader = 297,

        /// <summary>
        /// IsUnbounded cannot be true if MaxLength is non-null.
        /// </summary>
        IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull = 298,

        /// <summary>
        /// ImmediateValueAnnotation is invalid as an element annotation.
        /// </summary>
        InvalidElementAnnotation = 299,

        /// <summary>
        /// The LabeledElement expression is invalid because it does not have 1 element.
        /// </summary>
        InvalidLabeledElementExpressionIncorrectNumberOfOperands = 300,

        /// <summary>
        /// Could not find a LabeledElement with that name
        /// </summary>
        BadUnresolvedLabeledElement = 301,

        /// <summary>
        /// Could not find a enum member with that name
        /// </summary>
        BadUnresolvedEnumMember = 302,

        /// <summary>
        /// The Cast expression is invalid because it does not have 1 element.
        /// </summary>
        InvalidCastExpressionIncorrectNumberOfOperands = 303,

        /// <summary>
        /// Could not find a Parameter with that name
        /// </summary>
        BadUnresolvedParameter = 304,

        /// <summary>
        /// A navigation property with <see cref="IEdmNavigationProperty.ContainsTarget"/> = true must point to an optional target.
        /// </summary>
        NavigationPropertyWithRecursiveContainmentTargetMustBeOptional = 305,

        /// <summary>
        /// If a navigation property has <see cref="IEdmNavigationProperty.ContainsTarget"/> = true and the target entity type is the same as
        /// the declaring type of the property, then the multiplicity of the source of navigation is Zero-Or-One.
        /// </summary>
        NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne = 306,

        /// <summary>
        /// If a navigation property has <see cref="IEdmNavigationProperty.ContainsTarget"/> = true and the target entity type is defferent than
        /// the declaring type of the property, then the multiplicity of the source of navigation is One.
        /// </summary>
        NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne = 307,

        /// <summary>
        /// The annotation target path cannot possibly refer to an annotable element.
        /// </summary>
        ImpossibleAnnotationsTarget = 309,

        /// <summary>
        /// A nullable type is not valid if a non-nullable type is required.
        /// </summary>
        CannotAssertNullableTypeAsNonNullableType = 310,

        /// <summary>
        /// The expression is a primitive constant, and cannot be valid for an non-primitive type.
        /// </summary>
        CannotAssertPrimitiveExpressionAsNonPrimitiveType = 311,

        /// <summary>
        /// The primitive type is not valid for the requested type.
        /// </summary>
        ExpressionPrimitiveKindNotValidForAssertedType = 312,

        /// <summary>
        /// Null is not valid in a non nullable expression.
        /// </summary>
        NullCannotBeAssertedToBeANonNullableType = 313,

        /// <summary>
        /// The expression is not valid for the asserted type.
        /// </summary>
        ExpressionNotValidForTheAssertedType = 314,

        /// <summary>
        /// A collection expression is not valid for a non-collection type.
        /// </summary>
        CollectionExpressionNotValidForNonCollectionType = 315,

        /// <summary>
        /// A record expression is not valid for a non-structured type.
        /// </summary>
        RecordExpressionNotValidForNonStructuredType = 316,

        /// <summary>
        /// The record expression does not have all of the properties required for the specified type.
        /// </summary>
        RecordExpressionMissingRequiredProperty = 317,

        /// <summary>
        /// The record expression's type is not open, but the record expression has extra properties.
        /// </summary>
        RecordExpressionHasExtraProperties = 318,

        /// <summary>
        /// Target has multiple annotations with the same term and same qualifier.
        /// </summary>
        DuplicateAnnotation = 319,

        /// <summary>
        /// Function application has wrong number of arguments for the function being applied.
        /// </summary>
        IncorrectNumberOfArguments = 320,

        /// <summary>
        /// Is it invalid to have duplicate alias in a single schema model.
        /// </summary>
        DuplicateAlias = 321,

        /// <summary>
        /// A model cannot be serialized to CSDL if it has references to types without fully qualified names.
        /// </summary>
        ReferencedTypeMustHaveValidName = 322,

        /// <summary>
        /// The model could not be serialized because multiple schemas were produced and only a single output stream was found.
        /// </summary>
        SingleFileExpected = 323,

        /// <summary>
        /// The Edmx version is not valid.
        /// </summary>
        UnknownEdmxVersion = 324,

        /// <summary>
        /// The EdmVersion is not valid.
        /// </summary>
        UnknownEdmVersion = 325,

        /// <summary>
        /// Nothing was written because no schemas were produced.
        /// </summary>
        NoSchemasProduced = 326,

        /// <summary>
        /// Model has multiple entity containers with the same name.
        /// </summary>
        DuplicateEntityContainerName = 327,

        /// <summary>
        /// The container name of a container element must be the full name of the container entity container.
        /// </summary>
        ContainerElementContainerNameIncorrect = 328,

        /// <summary>
        /// A primitive constant expression is not valid for a non-primitive type.
        /// </summary>
        PrimitiveConstantExpressionNotValidForNonPrimitiveType = 329,

        /// <summary>
        /// The value of the integer constant is out of range for the asserted type.
        /// </summary>
        IntegerConstantValueOutOfRange = 330,

        /// <summary>
        /// The length of the string constant is too large for the asserted type.
        /// </summary>
        StringConstantLengthOutOfRange = 331,

        /// <summary>
        /// The length of the binary constant is too large for the asserted type.
        /// </summary>
        BinaryConstantLengthOutOfRange = 332,

        /// <summary>
        /// None is not a valid mode for a operation import parameter.
        /// </summary>
        InvalidOperationImportParameterMode = 333,

        /// <summary>
        /// A type without other errors must not have kind of none.
        /// </summary>
        TypeMustNotHaveKindOfNone = 334,

        /// <summary>
        /// A primitive type without other errors must not have kind of none.
        /// </summary>
        PrimitiveTypeMustNotHaveKindOfNone = 335,

        /// <summary>
        /// A property without other errors must not have kind of none.
        /// </summary>
        PropertyMustNotHaveKindOfNone = 336,

        /// <summary>
        /// A schema element without other errors must not have kind of none.
        /// </summary>
        SchemaElementMustNotHaveKindOfNone = 338,

        /// <summary>
        /// An entity container element without other errors must not have kind of none.
        /// </summary>
        EntityContainerElementMustNotHaveKindOfNone = 339,

        /// <summary>
        /// A binary value must have content.
        /// </summary>
        BinaryValueCannotHaveEmptyValue = 340,

        /// <summary>
        /// There can only be a single navigation property mapping with containment that targets a particular entity set.
        /// </summary>
        EntitySetCanOnlyBeContainedByASingleNavigationProperty = 341,

        /// <summary>
        /// The navigation properties partner does not point back to the correct type.
        /// </summary>
        InconsistentNavigationPropertyPartner = 342,

        /// <summary>
        /// An entity set can only have one navigation property with containment.
        /// </summary>
        EntitySetCanOnlyHaveSingleNavigationPropertyWithContainment = 343,

        /// <summary>
        /// If a navigation property is traversed from an entity set/singleton, and then it's partner is traversed from the target of the first mapping, the destination should be the originating entity set/singleton.
        /// </summary>
        NavigationMappingMustBeBidirectional = 344,

        /// <summary>
        /// There can only be a single mapping from a given EntitySet with a particular navigation property.
        /// </summary>
        DuplicateNavigationPropertyMapping = 345,

        /// <summary>
        /// An entity set must have a mapping for all of the navigation properties in its element type.
        /// </summary>
        AllNavigationPropertiesMustBeMapped = 346,

        /// <summary>
        /// Type annotation does not have a property binding for all required properties.
        /// </summary>
        TypeAnnotationMissingRequiredProperty = 347,

        /// <summary>
        /// Type annotation has a property binding for a non-existant property and its type is not open.
        /// </summary>
        TypeAnnotationHasExtraProperties = 348,

        /// <summary>
        /// Duration value is invalid.
        /// </summary>
        InvalidDuration = 349,

        /// <summary>
        /// The primitive type is invalid.
        /// </summary>
        InvalidPrimitiveValue = 350,

        /// <summary>
        /// An Enum type must have an underlying type of integer.
        /// </summary>
        EnumMustHaveIntegerUnderlyingType = 351,

        /// <summary>
        /// Could not find a term with this name.
        /// </summary>
        BadUnresolvedTerm = 352,

        /// <summary>
        /// The principal properties of a referential constraint must match the key of the referential constraint.
        /// </summary>
        BadPrincipalPropertiesInReferentialConstraint = 353,

        /// <summary>
        /// A direct annotation with the same name and namespace already exists.
        /// </summary>
        DuplicateDirectValueAnnotationFullName = 354,

        /// <summary>
        /// Cannot infer an entity set because no set exists of the given type.
        /// </summary>
        NoEntitySetsFoundForType = 355,

        /// <summary>
        /// Cannot infer an entity set because more than one set exists of the given type.
        /// </summary>
        CannotInferEntitySetWithMultipleSetsPerType = 356,

        /// <summary>
        /// Invalid entity set path.
        /// </summary>
        InvalidEntitySetPath = 357,

        /// <summary>
        /// Invalid enum member path.
        /// </summary>
        InvalidEnumMemberPath = 358,

        /// <summary>
        /// An annotation qualifier must be a simple name.
        /// </summary>
        QualifierMustBeSimpleName = 359,

        /// <summary>
        /// Enum type could not be resolved.
        /// </summary>
        BadUnresolvedEnumType = 360,

        /// <summary>
        /// Could not find a target with this name.
        /// </summary>
        BadUnresolvedTarget = 361,

        /// <summary>
        /// Path cannot be resolved in the given context.
        /// </summary>
        PathIsNotValidForTheGivenContext = 362,

        /// <summary>
        /// Could not find a navigation property with this name.
        /// </summary>
        BadUnresolvedNavigationPropertyPath = 363,

        /// <summary>
        /// The 'Nullable' attribute cannot be specified for a navigation property with collection type.
        /// </summary>
        NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute = 364,

        /// <summary>
        /// Metadata document cannot have more than one entity container.
        /// </summary>
        MetadataDocumentCannotHaveMoreThanOneEntityContainer = 365,

        /// <summary>
        /// Model has multiple functions that are the same definitions.
        /// </summary>
        DuplicateFunctions = 366,

        /// <summary>
        /// Model has multiple functions that are the same definitions.
        /// </summary>
        DuplicateActions = 367,

        /// <summary>
        /// Bound Function overloads must have the same return type.
        /// </summary>
        BoundFunctionOverloadsMustHaveSameReturnType = 368,

        /// <summary>
        /// The type of singleton must be entity type.
        /// </summary>
        SingletonTypeMustBeEntityType = 369,

        /// <summary>
        /// The type of entity set must be collection of entity type.
        /// </summary>
        EntitySetTypeMustBeCollectionOfEntityType = 370,

        /// <summary>
        /// The binding on navigation property of collection type must not target to singleton.
        /// </summary>
        NavigationPropertyOfCollectionTypeMustNotTargetToSingleton = 371,

        /// <summary>
        /// Reference must contatin at least one Include or IncludeAnnotations
        /// </summary>
        ReferenceElementMustContainAtLeastOneIncludeOrIncludeAnnotationsElement = 372,

        /// <summary>
        /// Function import must not have parameters if included in service document.
        /// </summary>
        FunctionImportWithParameterShouldNotBeIncludedInServiceDocument = 373,

        /// <summary>
        /// Unresolved Uri found in edmx:Reference, getReferencedModelReaderFunc should not return null when the URI is not a well-known schema.
        /// </summary>
        UnresolvedReferenceUriInEdmxReference = 374,

        /// <summary>
        /// Date value is invalid.
        /// </summary>
        InvalidDate = 375,

        /// <summary>
        /// TimeOfDay value is invalid.
        /// </summary>
        InvalidTimeOfDay = 376,

        /// <summary>
        /// Navigation property partner path cannot be resolved.
        /// </summary>
        UnresolvedNavigationPropertyPartnerPath = 377,

        /// <summary>
        /// Navigation property binding path cannot be resolved.
        /// </summary>
        UnresolvedNavigationPropertyBindingPath = 378,

        /// <summary>
        /// A required parameter followed an optional parameter.
        /// </summary>
        RequiredParametersMustPrecedeOptional = 379,

        /// <summary>
        /// The enum type is not valid for the requested type.
        /// </summary>
        ExpressionEnumKindNotValidForAssertedType = 380,
    }
}
