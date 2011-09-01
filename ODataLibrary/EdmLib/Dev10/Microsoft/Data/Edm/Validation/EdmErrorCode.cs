//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.Edm.Validation
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
        // unused 1,
        //SecurityError = 2,
        // unused 3,
        //IOException = 4,
        //XmlError = 5,
        //TooManyErrors = 6,
        //MalformedXml = 7,
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
        //XsdError = 13,
        //InvalidAlias = 14,
        /// <summary>
        /// An XML element was missing a required attribute
        /// </summary>
        MissingAttribute = 15,
        //IntegerExpected = 16,
        /// <summary>
        /// Invalid Name
        /// </summary>
        InvalidName = 17,
        // unused 18,
        /// <summary>
        /// Element name is already defined in this context.
        /// </summary>
        AlreadyDefined = 19,
        //ElementNotInSchema = 20,
        // unused 21,
        //InvalidBaseType = 22,
        //NoConcreteDescendants = 23,
        //CycleInTypeHierarchy = 24,
        /// <summary>
        /// The specified version number is not valid.
        /// </summary>
        InvalidVersionNumber = 25,
        //InvalidSize = 26,
        /// <summary>
        /// Malformed boolean value.
        /// </summary>
        InvalidBoolean = 27,
        // unused 28,
        //BadType = 29,
        // unused 30,
        // unused 31,
        //InvalidVersioningClass = 32,
        //InvalidVersionIntroduced = 33,
        //BadNamespace = 34,
        // unused 35,
        // unused 36,
        // unused 37,
        //UnresolvedReferenceSchema = 38,
        // unused 39,
        //NotInNamespace = 40,
        //NotUnnestedType = 41,
        /// <summary>
        /// The property contains an error.
        /// </summary>
        BadProperty = 42,
        //UndefinedProperty = 43,
        /// <summary>
        /// The type of this property is invalid for the given context.
        /// </summary>
        InvalidPropertyType = 44,
        //InvalidAsNestedType = 45,
        //InvalidChangeUnit = 46,
        //UnauthorizedAccessException = 47,
        // unused 48,
        // unused 49,
        // unused 50,
        /// <summary> Precision out of range </summary>
        PrecisionOutOfRange = 51,
        /// <summary> Scale out of range </summary>
        ScaleOutOfRange = 52,
        //DefaultNotAllowed = 53,
        //InvalidDefault = 54,
        //// <summary>One of the required facets is missing</summary>
        //RequiredFacetMissing = 55,
        //BadImageFormatException = 56,
        //MissingSchemaXml = 57,
        //BadPrecisionAndScale = 58,
        //InvalidChangeUnitUsage = 59,
        /// <summary>
        /// Name is too long.
        /// </summary>
        NameTooLong = 60,
        //CircularlyDefinedType = 61,
        /// <summary>
        /// The provided association is invalid
        /// </summary>
        InvalidAssociation = 62,
        //// <summary>
        //// The facet isn't allow by the property type.
        //// </summary>
        //FacetNotAllowedByType = 63,
        //// <summary>
        //// This facet value is constant and is specified in the schema
        //// </summary>
        //ConstantFacetSpecifiedInSchema = 64,
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
        // unused 76,
        // unused 77,
        // unused 78,
        // unused 79,
        // unused 80,
        // unused 81,
        // unused 82,
        // unused 83,
        // unused 84,
        // unused 85,
        // unused 86,
        // unused 87,
        // unused 88,
        // unused 89,
        // unused 90,
        // unused 91,
        /// <summary>Multiplicity value was malformed</summary>
        InvalidMultiplicity = 92,
        // unused 93,
        // unused 94,
        // unused 95,
        /// <summary>The value for the Action attribute is invalid or not allowed in the current context</summary>
        InvalidAction = 96,
        /// <summary>An error occured processing the On Operation elements</summary>
        InvalidOperation = 97,
        // unused 98,
        //// <summary>Ends were given for the Property element of a EntityContainer that is not a RelationshipSet</summary>
        //InvalidContainerTypeForEnd = 99,
        /// <summary>The extent name used in the EntittyContainerType End does not match the name of any of the EntityContainerProperties in the containing EntityContainer</summary>
        InvalidEndEntitySet = 100,
        //// <summary>An end element was not given, and cannot be inferred because too many EntityContainerEntitySet elements that are good possibilities.</summary>
        //AmbiguousEntityContainerEnd = 101,
        //// <summary>An end element was not given, and cannot be infered because there is no EntityContainerEntitySets that are the correct type to be used as an EntitySet.</summary>
        //MissingExtentEntityContainerEnd = 102,
        // unused 103,
        // unused 104,
        // unused 105,
        //// <summary>Not a valid parameter direction for the parameter in a function</summary>
        //BadParameterDirection = 106,
        //// <summary>Unable to infer an optional schema part, to resolve this, be more explicit</summary>
        //FailedInference = 107,
        // unused = 108,
        //// <summary> Invalid facet attribute(s) specified in provider manifest</summary>
        //InvalidFacetInProviderManifest = 109,
        /// <summary> Invalid role value in the relationship constraint</summary>
        InvalidRoleInRelationshipConstraint = 110,
        /// <summary> Invalid Property in relationship constraint</summary>
        InvalidPropertyInRelationshipConstraint = 111,
        /// <summary> Type mismatch between ToProperty and FromProperty in the relationship constraint</summary>
        TypeMismatchRelationshipConstraint = 112,
        /// <summary> Invalid multiplicty in FromRole in the relationship constraint</summary>
        InvalidMultiplicityInRoleInRelationshipConstraint = 113,
        /// <summary> The number of properties in the FromProperty and ToProperty in the relationship constraint must be identical</summary>
        MismatchNumberOfPropertiesInRelationshipConstraint = 114,
        //// <summary> No Properties defined in either FromProperty or ToProperty in the relationship constraint</summary>
        //MissingPropertyInRelationshipConstraint = 115,
        //// <summary> Missing constraint in relationship type in ssdl</summary>
        //MissingConstraintOnRelationshipType = 116,
        // unused 117,
        // unused 118,
        /// <summary> Same role referred in the ToRole and FromRole of a referential constraint </summary>
        SameRoleReferredInReferentialConstraint = 119,
        //// <summary> Invalid value for attribute ParameterTypeSemantics </summary>
        //InvalidValueForParameterTypeSemantics = 120,
        //// <summary> Invalid type used for a Relationship End Type</summary>
        //InvalidRelationshipEndType = 121,
        //// <summary> Invalid PrimitiveTypeKind</summary>
        //InvalidPrimitiveTypeKind = 122,
        // unused 123,
        //// <summary> Invalid TypeConversion DestinationType</summary>
        //InvalidTypeConversionDestinationType = 124,
        //// <summary>Expected a integer value between 0 - 255</summary>
        // ByteValueExpected = 125,
        //// <summary> Invalid Type specified in function</summary>
        //FunctionWithNonScalarTypeNotSupported = 126,
        //// <summary> Precision must not be greater than 28 </summary>
        //PrecisionMoreThanAllowedMax = 127,
        /// <summary> Properties that are part of entity key must be of scalar type</summary>
        EntityKeyMustBeScalar = 128,
        /// <summary> Binary type properties which are part of entity key are currently supported before V2.0 </summary>
        EntityKeyMustNotBeBinary = 129,
        //// <summary>The primitive type kind does not have a prefered mapping</summary>
        //NoPreferredMappingForPrimitiveTypeKind = 130,
        //// <summary>More than one PreferredMapping for a PrimitiveTypeKind</summary>
        //TooManyPreferredMappingsForPrimitiveTypeKind = 131,
        /// <summary>End with * multiplicity cannot have operations specified</summary>
        EndWithManyMultiplicityCannotHaveOperationsSpecified = 132,
        /// <summary>EntitySet type has no keys</summary>
        EntitySetTypeHasNoKeys = 133,
        //// <summary>Invalid Number Of Parameters For Aggregate Function</summary>
        //InvalidNumberOfParametersForAggregateFunction = 134,
        //// <summary>Invalid Parameter Type For Aggregate Function</summary>
        //InvalidParameterTypeForAggregateFunction = 135,
        //// <summary>Composable functions must declare a return type.</summary>
        //ComposableFunctionWithoutReturnType = 136,
        //// <summary>Non-composable functions must not declare a return type.</summary>
        //NonComposableFunctionWithReturnType = 137,
        //// <summary>Non-composable functions do not permit the aggregate, niladic, or built-in attributes.</summary>
        //NonComposableFunctionAttributesNotValid = 138,
        //// <summary>Composable functions can not include command text attribute.</summary>
        //ComposableFunctionWithCommandText = 139,
        //// <summary>Functions should not declare both a store name and command text (only one or the other
        //// can be used).</summary>
        //FunctionDeclaresCommandTextAndStoreFunctionName = 140,
        //// <summary>System Namespace</summary>
        //SystemNamespace = 141,
        //// <summary>Empty DefiningQuery text</summary>
        //EmptyDefiningQuery = 142,
        //// <summary>Schema, Table and DefiningQuery are all specified, and are mutualy exlusive</summary>
        //TableAndSchemaAreMutuallyExclusiveWithDefiningQuery = 143,
        /// <summary>ConcurrencyMode value was malformed</summary>
        InvalidConcurrencyMode = 144,
        /// <summary>Conurency can't change for any sub types of an EntitySet type.</summary>
        ConcurrencyRedefinedOnSubtypeOfEntitySetType = 145,
        /// <summary>Function import return type must be either empty, a collection of entities, or a singleton scalar.</summary>
        FunctionImportUnsupportedReturnType = 146,
        /// <summary>Composable function import cannot be side-effecting.</summary>
        ComposableFunctionImportCannotBeSideEffecting = 147,
        /// <summary>Function import specifies entity type return but no entity set.</summary>
        FunctionImportReturnsEntitiesButDoesNotSpecifyEntitySet = 148,
        /// <summary>Function import specifies entity type that does not derive from element type of entity set.</summary>
        FunctionImportEntityTypeDoesNotMatchEntitySet = 149,
        /// <summary>Function import specifies a binding to an entity set but does not return entities.</summary>
        FunctionImportSpecifiesEntitySetButDoesNotReturnEntityType = 150,
        //// <summary>Internal Error</summary>
        //InternalError = 152,
        /// <summary>Same Entity Set Taking part in the same role of the relationship set in two different relationship sets</summary>
        SimilarRelationshipEnd = 153,
        /// <summary> Entity key refers to the same property twice</summary>
        DuplicatePropertySpecifiedInEntityKey = 154,
        //// <summary> Function declares a ReturnType attribute and element</summary>
        //AmbiguousFunctionReturnType = 156,
        /// <summary> Nullable Complex Type not supported in Edm V1</summary>
        NullableComplexType = 157,
        //// <summary> Only Complex Collections supported in Edm V1.1</summary>
        //NonComplexCollections = 158,
        /// <summary>No Key defined on Entity Type </summary>
        KeyMissingOnEntityType = 159,
        //// <summary> Invalid namespace specified in using element</summary>
        //InvalidNamespaceInUsing = 160,
        /// <summary>
        /// Need not specify system namespace in using 
        /// </summary>
        SystemNamespaceEncountered = 161,
        //// <summary> Cannot use a reserved/system namespace as alias </summary>
        //CannotUseSystemNamespaceAsAlias = 162,
        /// <summary> Invalid qualification specified for type </summary>
        InvalidNamespaceName = 163,
        //// <summary> Invalid Entity Container Name in extends attribute </summary>
        //InvalidEntityContainerNameInExtends = 164,
        //// <summary> Invalid CollectionKind value in property CollectionKind attribute</summary>
        //InvalidCollectionKind = 165,
        //// <summary> Must specify namespace or alias of the schema in which this type is defined </summary>
        //InvalidNamespaceOrAliasSpecified = 166,
        //// <summary> Entity Container cannot extend itself </summary>
        //EntityContainerCannotExtendItself = 167,
        //// <summary> Failed to retrieve provider manifest </summary>
        //FailedToRetrieveProviderManifest = 168,
        //// <summary> Mismatched Provider Manifest token values in SSDL artifacts </summary>
        //ProviderManifestTokenMismatch = 169,
        //// <summary> Missing Provider Manifest token value in SSDL artifact(s) </summary>
        //ProviderManifestTokenNotFound = 170,
        //// <summary>Empty CommandText element</summary>
        //EmptyCommandText = 171,
        //// <summary> Inconsistent Provider values in SSDL artifacts </summary>
        //InconsistentProvider = 172,
        //// <summary> Inconsistent Provider Manifest token values in SSDL artifacts </summary>
        //InconsistentProviderManifestToken = 173,
        //// <summary> Duplicated Function overloads </summary>
        //DuplicatedFunctionoverloads = 174,
        //// <summary>Invalid Provider</summary>
        //InvalidProvider = 175,
        //// <summary>Function With Non Edm Type Not Supported</summary>
        //FunctionWithNonEdmTypeNotSupported = 176,
        //// <summary>Complex Type As Return Type And Defined Entity Set</summary>
        //ComplexTypeAsReturnTypeAndDefinedEntitySet = 177,
        //// <summary>Complex Type As Return Type And Defined Entity Set</summary>
        //ComplexTypeAsReturnTypeAndNestedComplexProperty = 178,
        //// unused 179,
        //// unused 180,
        //// unused 181,
        //// <summary>In model functions facet attribute is allowed only on ScalarTypes</summary>
        //FacetOnNonScalarType = 182,
        //// <summary>Captures several conditions where facets are placed on element where it should not exist.</summary>
        //IncorrectlyPlacedFacet = 183,
        //// <summary>Return type has not been declared</summary>
        //ReturnTypeNotDeclared = 184,
        //TypeNotDeclared = 185,
        //RowTypeWithoutProperty = 186,
        //ReturnTypeDeclaredAsAttributeAndElement = 187,
        //TypeDeclaredAsAttributeAndElement = 188,
        //ReferenceToNonEntityType = 189,
        //// <summary>Invalid value in the EnumTypeOption</summary>
        //InvalidValueInEnumOption = 190,
        //IncompatibleSchemaVersion = 191,
        //// <summary> The structural annotation cannot use codegen namespaces </summary>
        //NoCodeGenNamespaceInStructuralAnnotation = 192,
        //// <summary> Function and type cannot have the same fully qualified name</summary>
        //AmbiguousFunctionAndType = 193,
        //// <summary> Cannot load different version of schema in the same ItemCollection</summary>
        //CannotLoadDifferentVersionOfSchemaInTheSameItemCollection = 194,
        //// <summary> Expected bool value</summary>
        //BoolValueExpected = 195,
        //// <summary> End without Multiplicity specified</summary>
        //EndWithoutMultiplicity = 196,
        //// <summary>In SSDL, if composable function returns a collection of rows (TVF), all row properties must be of scalar types.</summary>
        //TVFReturnTypeRowHasNonScalarProperty = 197,
        //// <summary> The name of NamedEdmItem must not be empty or white space only</summary>
        //EdmModel_NameMustNotBeEmptyOrWhiteSpace = 198,
        //// <summary> EdmTypeReference is empty</summary>
        //// Unused 199,
        //EdmAssociationType_AssocationEndMustNotBeNull = 200,
        //EdmAssociationConstraint_DependentEndMustNotBeNull = 201,
        //EdmAssociationConstraint_DependentPropertiesMustNotBeEmpty = 202,
        //EdmNavigationProperty_AssocationMustNotBeNull = 203,
        //EdmNavigationProperty_ResultEndMustNotBeNull = 204,
        //EdmAssociationEnd_EntityTypeMustNotBeNull = 205,
        /// <summary>
        /// The value for an enumeration type member is ouf of range.
        /// </summary>
        EnumMemberValueOutOfRange = 206,
        //EdmAssociationSet_ElementTypeMustNotBeNull = 207,
        //EdmAssociationSet_SourceSetMustNotBeNull = 208,
        //EdmAssociationSet_TargetSetMustNotBeNull = 209,
        //EdmFunctionImport_ReturnTypeMustBeCollectionType = 210,
        //EdmModel_NameIsNotAllowed = 211,
        //EdmTypeReferenceNotValid = 212,
        //EdmFunctionNotExistsInV1 = 213,
        //EdmFunctionNotExistsInV1_1 = 214,
        //Serializer_OneNamespaceAndOneContainer = 215,
        //EdmModel_Validator_Semantic_InvalidEdmTypeReference = 216,
        //EdmModel_Validator_TypeNameAlreadyDefinedDuplicate = 217,
        /// <summary>
        /// The entity container name has already been assigned to a different entity container.
        /// </summary>
        DuplicateEntityContainerMemberName = 218,
        //EdmFunction_UnsupportedParameterType = 219,
        /// <summary>
        /// Complex types were not allowed to be abstract here.
        /// </summary>
        InvalidAbstractComplexType = 220,
        /// <summary>
        /// Complex types cannot have base types in this version.
        /// </summary>
        InvalidPolymorphicComplexType = 221,
        //// <summary> A property cannot have a type of multivalue and have a collection type defined</summary>
        //EdmProperty_CannotHaveMultiValueAndCollection = 222,
        //// <summary> Parameter mode cannot have a value other than "In" "Out" or "InOut"</summary>
        //EdmFunctionParameter_InvalidMode=223,
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
        // Unassigned 239
        /// <summary>
        /// Rows cannot have base types.
        /// </summary>
        RowTypeMustNotHaveBaseType = 240,
        /// <summary>
        /// The role of an association set end must be an association end belonging to the association type that defines the associaiton set.
        /// </summary>
        AssociationSetEndRoleMustBelongToSetElementType = 241,
        /// <summary>
        /// Every property in an entity key must be a property of the entity.
        /// </summary>
        KeyPropertyMustBelongToEntity = 242,
        /// <summary>
        /// The principle end of a referential constraint must be one of the ends of the association that defined the referential constraint.
        /// </summary>
        ReferentialConstraintPrincipleEndMustBelongToAssociation = 243,
        /// <summary>
        /// Dependent properties of a referential constraint must belong to the dependent entity set.
        /// </summary>
        DependentPropertiesMustBelongToDependentEntity = 244,
        /// <summary>
        /// If a structured type declares a property, that properties declaring type must be the declaring structured type.
        /// </summary>
        DeclaringTypeMustBeCorrect = 245,
        /// <summary>
        /// Functions are not supported before version 2.0.
        /// </summary>
        FunctionsNotSupportedBeforeV2 = 256,
        /// <summary>
        /// Abstract entities cannot be used as the type of an entity set because they cannot be instantiated.
        /// </summary>
        AbstractEntitiesCannotBeInstantiated = 257,
        /// <summary>
        /// Navigation property has a type that is not an entity or collection of entities.
        /// </summary>
        InvalidNavigationPropertyType = 258,
        /// <summary>
        /// Type of term is bad because the term is bad.
        /// </summary>
        TermTypeIsBadBecauseTermIsBad = 259,
        /// <summary>
        /// TODO: placeholder for external annotations parser.
        /// </summary>
        FailedToParseExternalAnnotations = 260,
        /// <summary>
        /// Underlying type of the enumeration type is bad because the enumeration type is bad.
        /// </summary>
        UnderlyingTypeIsBadBecauseEnumTypeIsBad = 261,
        /// <summary>
        /// The type of the entity set on this association end is inconsistent with the association end.
        /// </summary>
        InvalidAssociationSetEndSetWrongType = 262,
        /// <summary>
        /// Only function parameters with mode of In are allowed in function imports.
        /// </summary>
        OnlyInputParametersAllowedInFunctions = 263,
        /// <summary>
        /// Function parameter was of a non-allowed type.
        /// </summary>
        FunctionParameterIncorrectType = 264,
        /// <summary>
        /// Function return type was of a non-allowed type.
        /// </summary>
        FunctionReturnTypeIncorrectType = 265,
        /// <summary>
        /// A row type must contain at least one property.
        /// </summary>
        RowTypeMustHaveProperties = 266,
        /// <summary>
        /// A referential constraint cannot have multiple dependent properties with the same name.
        /// </summary>
        DuplicateDependentProperty = 267,
        /// <summary>
        /// Bindable function import must have at least one parameter.
        /// </summary>
        BindableFunctionImportMustHaveParameters = 268,
        /// <summary>
        /// Function imports with side-effecting setting are not supported before version 3.0.
        /// </summary>
        FunctionImportSideEffectingNotSupportedBeforeV3 = 269,
        /// <summary>
        /// Function imports with composable setting are not supported before version 3.0.
        /// </summary>
        FunctionImportComposableNotSupportedBeforeV3 = 270,
        /// <summary>
        /// Function imports with bindable setting are not supported before version 3.0.
        /// </summary>
        FunctionImportBindableNotSupportedBeforeV3 = 271,
        /// <summary>
        /// Max length is out of range.
        /// </summary>
        MaxLengthOutOfRange = 272,
        /// <summary>
        /// Could not find a ValueTerm with that name
        /// </summary>
        BadUnresolvedValueTerm = 273,
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
    }
}
