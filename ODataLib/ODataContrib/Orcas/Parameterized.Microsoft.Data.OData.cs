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

namespace Microsoft.Data.OData {
    using System;
    using System.Resources;

    /// <summary>
    ///    Strongly-typed and parameterized string resources.
    /// </summary>
    internal static class Strings {
        /// <summary>
        /// A string like "A service operation with name '{0}' could not be found in the provided model."
        /// </summary>
        internal static string ODataQueryUtils_DidNotFindServiceOperation(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataQueryUtils_DidNotFindServiceOperation,p0);
        }

        /// <summary>
        /// A string like "Found multiple service operations with name '{0}' in a single entity container. Service operation overloads are not supported."
        /// </summary>
        internal static string ODataQueryUtils_FoundMultipleServiceOperations(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataQueryUtils_FoundMultipleServiceOperations,p0);
        }

        /// <summary>
        /// A string like "Setting a metadata annotation on a primitive type is not supported."
        /// </summary>
        internal static string ODataQueryUtils_CannotSetMetadataAnnotationOnPrimitiveType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataQueryUtils_CannotSetMetadataAnnotationOnPrimitiveType);
            }
        }

        /// <summary>
        /// A string like "An entity set with name '{0}' could not be found in the provided model."
        /// </summary>
        internal static string ODataQueryUtils_DidNotFindEntitySet(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataQueryUtils_DidNotFindEntitySet,p0);
        }

        /// <summary>
        /// A string like "Only operands with primitive types are allowed in binary operators. Found operand types '{0}' and '{1}'."
        /// </summary>
        internal static string BinaryOperatorQueryNode_InvalidOperandType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.BinaryOperatorQueryNode_InvalidOperandType,p0,p1);
        }

        /// <summary>
        /// A string like "Both operands of a binary operators must have the same type. Found different operand types '{0}' and '{1}'."
        /// </summary>
        internal static string BinaryOperatorQueryNode_OperandsMustHaveSameTypes(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.BinaryOperatorQueryNode_OperandsMustHaveSameTypes,p0,p1);
        }

        /// <summary>
        /// A string like "An unsupported query node kind '{0}' was found."
        /// </summary>
        internal static string QueryExpressionTranslator_UnsupportedQueryNodeKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_UnsupportedQueryNodeKind,p0);
        }

        /// <summary>
        /// A string like "An unsupported extension query node was found."
        /// </summary>
        internal static string QueryExpressionTranslator_UnsupportedExtensionNode {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_UnsupportedExtensionNode);
            }
        }

        /// <summary>
        /// A string like "A query node of kind '{0}' was translated to a null expression. Translation of any query node must return a non-null expression."
        /// </summary>
        internal static string QueryExpressionTranslator_NodeTranslatedToNull(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_NodeTranslatedToNull,p0);
        }

        /// <summary>
        /// A string like "A query node of kind '{0}' was translated to an expression of type '{1}' but an expression of type '{2}' was expected."
        /// </summary>
        internal static string QueryExpressionTranslator_NodeTranslatedToWrongType(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_NodeTranslatedToWrongType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A CollectionQueryNode of kind '{0}' with null ItemType was found. Only a CollectionQueryNode with non-null ItemType can be translated into an expression."
        /// </summary>
        internal static string QueryExpressionTranslator_CollectionQueryNodeWithoutItemType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_CollectionQueryNodeWithoutItemType,p0);
        }

        /// <summary>
        /// A string like "A SingleValueQueryNode of kind '{0}' with null TypeReference was found. A SingleValueQueryNode can only be translated into an expression if it has a non-null type or statically represents the null value."
        /// </summary>
        internal static string QueryExpressionTranslator_SingleValueQueryNodeWithoutTypeReference(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_SingleValueQueryNodeWithoutTypeReference,p0);
        }

        /// <summary>
        /// A string like "A ConstantQueryNode of type '{0}' was found. Only a ConstantQueryNode of a primitive type can be translated to an expression."
        /// </summary>
        internal static string QueryExpressionTranslator_ConstantNonPrimitive(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_ConstantNonPrimitive,p0);
        }

        /// <summary>
        /// A string like "A KeyLookupQueryNode is being applied to a collection of type '{0}' which is of kind '{1}'. KeyLookupQueryNode can only be applied to a collection of entity types."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyLookupOnlyOnEntities(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_KeyLookupOnlyOnEntities,p0,p1);
        }

        /// <summary>
        /// A string like "A KeyLookupQueryNode is being applied to an expression of incompatible type '{0}'. This KeyLookupQueryNode can only be applied to a collection which translates to an expression of type '{1}'."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyLookupOnlyOnQueryable(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_KeyLookupOnlyOnQueryable,p0,p1);
        }

        /// <summary>
        /// A string like "A KeyLookupQueryNode is either missing or has more than one value for a key property '{0}' on type '{1}'. There must be exactly one value for the key property."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyLookupWithoutKeyProperty(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_KeyLookupWithoutKeyProperty,p0,p1);
        }

        /// <summary>
        /// A string like "A KeyLookupQueryNode with no key property values was found. Only a KeyLookupQueryNode with at least one key property value can be translated into an expression."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyLookupWithNoKeyValues {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_KeyLookupWithNoKeyValues);
            }
        }

        /// <summary>
        /// A string like "A KeyPropertyValue instance without a valid key property was found. The KeyPropertyValue.KeyProperty must specify a key property."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyPropertyValueWithoutProperty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_KeyPropertyValueWithoutProperty);
            }
        }

        /// <summary>
        /// A string like "A KeyPropertyValue instance for key property '{0}' has a value of a wrong type. The KeyPropertyValue.KeyValue must be of the same type as the key property."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyPropertyValueWithWrongValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_KeyPropertyValueWithWrongValue,p0);
        }

        /// <summary>
        /// A string like "A FilterQueryNode input collection was translated to an expression of type '{0}', but type '{1}' is expected."
        /// </summary>
        internal static string QueryExpressionTranslator_FilterCollectionOfWrongType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_FilterCollectionOfWrongType,p0,p1);
        }

        /// <summary>
        /// A string like "A FilterQueryNode.Expression was translated to an expression of type '{0}', but the expression must evaluate to a boolean value."
        /// </summary>
        internal static string QueryExpressionTranslator_FilterExpressionOfWrongType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_FilterExpressionOfWrongType,p0);
        }

        /// <summary>
        /// A string like "The operand for the unary not operator is being applied to an expression of type '{0}'. A unary not operator can only be applied to an operand of type bool or bool?."
        /// </summary>
        internal static string QueryExpressionTranslator_UnaryNotOperandNotBoolean(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_UnaryNotOperandNotBoolean,p0);
        }

        /// <summary>
        /// A string like "The source of a PropertyAccessQueryNode was translated to an expression of type '{0}', but type '{1}' is required in order to translate the property access."
        /// </summary>
        internal static string QueryExpressionTranslator_PropertyAccessSourceWrongType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_PropertyAccessSourceWrongType,p0,p1);
        }

        /// <summary>
        /// A string like "The source of a PropertyAccessQueryNode is of kind '{0}'. Properties are only supported for type kinds 'Complex' and 'Entity'."
        /// </summary>
        internal static string QueryExpressionTranslator_PropertyAccessSourceNotStructured(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_PropertyAccessSourceNotStructured,p0);
        }

        /// <summary>
        /// A string like "A ParameterQueryNode which is not defined in the current scope was found."
        /// </summary>
        internal static string QueryExpressionTranslator_ParameterNotDefinedInScope {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_ParameterNotDefinedInScope);
            }
        }

        /// <summary>
        /// A string like "An OrderByQueryNode input collection was translated to an expression of type '{0}', but type '{1}' is expected."
        /// </summary>
        internal static string QueryExpressionTranslator_OrderByCollectionOfWrongType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_OrderByCollectionOfWrongType,p0,p1);
        }

        /// <summary>
        /// A string like "An unknown function with name '{0}' was found."
        /// </summary>
        internal static string QueryExpressionTranslator_UnknownFunction(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_UnknownFunction,p0);
        }

        /// <summary>
        /// A string like "The argument for an invocation of a function with name '{0}' is not a single value. All arguments for this function must be single values."
        /// </summary>
        internal static string QueryExpressionTranslator_FunctionArgumentNotSingleValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_FunctionArgumentNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "No function signature for the function with name '{0}' matches the specified arguments. The function signatures considered are: {1}."
        /// </summary>
        internal static string QueryExpressionTranslator_NoApplicableFunctionFound(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryExpressionTranslator_NoApplicableFunctionFound,p0,p1);
        }

        /// <summary>
        /// A string like "The specified URI '{0}' must be absolute."
        /// </summary>
        internal static string QueryDescriptorQueryToken_UriMustBeAbsolute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryDescriptorQueryToken_UriMustBeAbsolute,p0);
        }

        /// <summary>
        /// A string like "The maximum depth setting must be a number greater than zero."
        /// </summary>
        internal static string QueryDescriptorQueryToken_MaxDepthInvalid {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryDescriptorQueryToken_MaxDepthInvalid);
            }
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $skip query option found. The $skip query option requires a non-negative integer value."
        /// </summary>
        internal static string QueryDescriptorQueryToken_InvalidSkipQueryOptionValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryDescriptorQueryToken_InvalidSkipQueryOptionValue,p0);
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $top query option found. The $top query option requires a non-negative integer value."
        /// </summary>
        internal static string QueryDescriptorQueryToken_InvalidTopQueryOptionValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryDescriptorQueryToken_InvalidTopQueryOptionValue,p0);
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $inlinecount query option found. Valid values are '{1}'."
        /// </summary>
        internal static string QueryDescriptorQueryToken_InvalidInlineCountQueryOptionValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryDescriptorQueryToken_InvalidInlineCountQueryOptionValue,p0,p1);
        }

        /// <summary>
        /// A string like "Query option '{0}' was specified more than once, but it must be specified at most once."
        /// </summary>
        internal static string QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce,p0);
        }

        /// <summary>
        /// A string like "The CLR literal of type '{0}' is not supported to be written as a Uri part."
        /// </summary>
        internal static string UriBuilder_NotSupportedClrLiteral(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriBuilder_NotSupportedClrLiteral,p0);
        }

        /// <summary>
        /// A string like "QueryToken '{0}' is not supported to be written as a Uri part."
        /// </summary>
        internal static string UriBuilder_NotSupportedQueryToken(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriBuilder_NotSupportedQueryToken,p0);
        }

        /// <summary>
        /// A string like "Recursion depth exceeded allowed limit."
        /// </summary>
        internal static string UriQueryExpressionParser_TooDeep {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_TooDeep);
            }
        }

        /// <summary>
        /// A string like "Expression expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_ExpressionExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_ExpressionExpected,p0,p1);
        }

        /// <summary>
        /// A string like "'(' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_OpenParenExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_OpenParenExpected,p0,p1);
        }

        /// <summary>
        /// A string like "')' or ',' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CloseParenOrCommaExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_CloseParenOrCommaExpected,p0,p1);
        }

        /// <summary>
        /// A string like "')' or operator expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CloseParenOrOperatorExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_CloseParenOrOperatorExpected,p0,p1);
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid because it is not based on '{1}'."
        /// </summary>
        internal static string UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri,p0,p1);
        }

        /// <summary>
        /// A string like "Bad Request: there was an error in the query syntax."
        /// </summary>
        internal static string UriQueryPathParser_SyntaxError {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryPathParser_SyntaxError);
            }
        }

        /// <summary>
        /// A string like "Too many segments in URI."
        /// </summary>
        internal static string UriQueryPathParser_TooManySegments {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryPathParser_TooManySegments);
            }
        }

        /// <summary>
        /// A string like "The key value '{0}' was not recognized as a valid literal."
        /// </summary>
        internal static string UriQueryPathParser_InvalidKeyValueLiteral(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryPathParser_InvalidKeyValueLiteral,p0);
        }

        /// <summary>
        /// A string like "Unable to find property '{2}' on the instance type '{1}' of the structured type '{0}'."
        /// </summary>
        internal static string PropertyInfoTypeAnnotation_CannotFindProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.PropertyInfoTypeAnnotation_CannotFindProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "An unsupported query token kind '{0}' was found."
        /// </summary>
        internal static string MetadataBinder_UnsupportedQueryTokenKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnsupportedQueryTokenKind,p0);
        }

        /// <summary>
        /// A string like "An unsupported extension query token was found."
        /// </summary>
        internal static string MetadataBinder_UnsupportedExtensionToken {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnsupportedExtensionToken);
            }
        }

        /// <summary>
        /// A string like "Could not find an entity set for root segment '{0}'."
        /// </summary>
        internal static string MetadataBinder_RootSegmentResourceNotFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_RootSegmentResourceNotFound,p0);
        }

        /// <summary>
        /// A string like "Type '{0}' is not an entity type. Key value can only be applied to an entity type."
        /// </summary>
        internal static string MetadataBinder_KeyValueApplicableOnlyToEntityType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_KeyValueApplicableOnlyToEntityType,p0);
        }

        /// <summary>
        /// A string like "Type '{0}' does not have a property '{1}'."
        /// </summary>
        internal static string MetadataBinder_PropertyNotDeclared(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_PropertyNotDeclared,p0,p1);
        }

        /// <summary>
        /// A string like "Property '{0}' is not declared on type '{1}' or is not a key property. Only key properties can be used in key lookups."
        /// </summary>
        internal static string MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue,p0,p1);
        }

        /// <summary>
        /// A string like "An unnamed key value was used in a key lookup on a type '{0}' which has more than one key property. Unnamed key value can only be used on a type with one key property."
        /// </summary>
        internal static string MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties,p0);
        }

        /// <summary>
        /// A string like "A key property '{0}' was found twice in a key lookup. Each key property can be specified just once in a key lookup."
        /// </summary>
        internal static string MetadataBinder_DuplicitKeyPropertyInKeyValues(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_DuplicitKeyPropertyInKeyValues,p0);
        }

        /// <summary>
        /// A string like "A key lookup on type '{0}' didn't specify values for all key properties. All key properties must be specified in a key lookup."
        /// </summary>
        internal static string MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues,p0);
        }

        /// <summary>
        /// A string like "Expression of type '{0}' cannot be converted to type '{1}'."
        /// </summary>
        internal static string MetadataBinder_CannotConvertToType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_CannotConvertToType,p0,p1);
        }

        /// <summary>
        /// A string like "Segment '{0}' which is a service operation returning non-queryable result has a key lookup. Only service operations returning queryable results can have a key lookup applied to them."
        /// </summary>
        internal static string MetadataBinder_NonQueryableServiceOperationWithKeyLookup(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NonQueryableServiceOperationWithKeyLookup,p0);
        }

        /// <summary>
        /// A string like "Service operation '{0}' of kind '{1}' returns type '{2}' which is not an entity type. Service operations of kind QueryWithMultipleResults or QueryWithSingleResult can only return entity types."
        /// </summary>
        internal static string MetadataBinder_QueryServiceOperationOfNonEntityType(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_QueryServiceOperationOfNonEntityType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Service operation '{0}' is missing the required parameter '{1}'."
        /// </summary>
        internal static string MetadataBinder_ServiceOperationParameterMissing(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_ServiceOperationParameterMissing,p0,p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' with value '{1}' for the service operation '{2}' is not a valid literal of type '{3}'."
        /// </summary>
        internal static string MetadataBinder_ServiceOperationParameterInvalidType(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_ServiceOperationParameterInvalidType,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The $filter query option cannot be applied to the query path. Filter can only be applied to a collection of entities."
        /// </summary>
        internal static string MetadataBinder_FilterNotApplicable {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_FilterNotApplicable);
            }
        }

        /// <summary>
        /// A string like "The $filter expression must evaluate to a single boolean value."
        /// </summary>
        internal static string MetadataBinder_FilterExpressionNotSingleValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_FilterExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "The $orderby query option cannot be applied to the query path. Ordering can only be applied to a collection of entities."
        /// </summary>
        internal static string MetadataBinder_OrderByNotApplicable {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_OrderByNotApplicable);
            }
        }

        /// <summary>
        /// A string like "The $orderby expression must evaluate to a single value of primitive type."
        /// </summary>
        internal static string MetadataBinder_OrderByExpressionNotSingleValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_OrderByExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "The $skip query option cannot be applied to the query path. Skip can only be applied to a collection of entities."
        /// </summary>
        internal static string MetadataBinder_SkipNotApplicable {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_SkipNotApplicable);
            }
        }

        /// <summary>
        /// A string like "The $top query option cannot be applied to the query path. Top can only be applied to a collection of entities."
        /// </summary>
        internal static string MetadataBinder_TopNotApplicable {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_TopNotApplicable);
            }
        }

        /// <summary>
        /// A string like "A PropertyAccessQueryToken without a parent was encountered outside of $filter or $orderby expression. The PropertyAccessQueryToken without a parent token is only allowed inside $filter or $orderby expressions."
        /// </summary>
        internal static string MetadataBinder_PropertyAccessWithoutParentParameter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_PropertyAccessWithoutParentParameter);
            }
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' cannot be used in $filter or $orderby query expression. MultiValue properties are not supported with these query options."
        /// </summary>
        internal static string MetadataBinder_MultiValuePropertyNotSupportedInExpression(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_MultiValuePropertyNotSupportedInExpression,p0);
        }

        /// <summary>
        /// A string like "The operand for a binary operator '{0}' is not a single value. Binary operators require both operands to be single values."
        /// </summary>
        internal static string MetadataBinder_BinaryOperatorOperandNotSingleValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_BinaryOperatorOperandNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "The operand for a unary operator '{0}' is not a single value. Unary operators require the operand to be a single value."
        /// </summary>
        internal static string MetadataBinder_UnaryOperatorOperandNotSingleValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnaryOperatorOperandNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "The parent value for a property access of a property '{0}' is not a single value. Property access can only be applied to a single value."
        /// </summary>
        internal static string MetadataBinder_PropertyAccessSourceNotSingleValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_PropertyAccessSourceNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "A binary operator with incompatible types was detected. Found operand types '{0}' and '{1}' for operator kind '{2}'."
        /// </summary>
        internal static string MetadataBinder_IncompatibleOperandsError(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_IncompatibleOperandsError,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A unary operator with an incompatible type was detected. Found operand type '{0}' for operator kind '{1}'."
        /// </summary>
        internal static string MetadataBinder_IncompatibleOperandError(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_IncompatibleOperandError,p0,p1);
        }

        /// <summary>
        /// A string like "An unknown function with name '{0}' was found."
        /// </summary>
        internal static string MetadataBinder_UnknownFunction(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnknownFunction,p0);
        }

        /// <summary>
        /// A string like "The argument for an invocation of a function with name '{0}' is not a single value. All arguments for this function must be single values."
        /// </summary>
        internal static string MetadataBinder_FunctionArgumentNotSingleValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_FunctionArgumentNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "No function signature for the function with name '{0}' matches the specified arguments. The function signatures considered are: {1}."
        /// </summary>
        internal static string MetadataBinder_NoApplicableFunctionFound(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NoApplicableFunctionFound,p0,p1);
        }

        /// <summary>
        /// A string like "The system query option '{0}' is not supported."
        /// </summary>
        internal static string MetadataBinder_UnsupportedSystemQueryOption(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnsupportedSystemQueryOption,p0);
        }

        /// <summary>
        /// A string like "A token of kind '{0}' was bound to the value null; this is invalid. A query token must always be bound to a non-null query node."
        /// </summary>
        internal static string MetadataBinder_BoundNodeCannotBeNull(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_BoundNodeCannotBeNull,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a non-negative integer value. In OData, the $top query option must specify a non-negative integer value."
        /// </summary>
        internal static string MetadataBinder_TopRequiresNonNegativeInteger(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_TopRequiresNonNegativeInteger,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a non-negative integer value. In OData, the $skip query option must specify a non-negative integer value."
        /// </summary>
        internal static string MetadataBinder_SkipRequiresNonNegativeInteger(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_SkipRequiresNonNegativeInteger,p0);
        }

        /// <summary>
        /// A string like " The service operation '{0}' does not have an associated result kind. Without a result kind, a service operation cannot be bound."
        /// </summary>
        internal static string MetadataBinder_ServiceOperationWithoutResultKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_ServiceOperationWithoutResultKind,p0);
        }

        /// <summary>
        /// A string like "Encountered invalid type cast. '{0}' is not assignable from '{1}'."
        /// </summary>
        internal static string MetadataBinder_HierarchyNotFollowed(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_HierarchyNotFollowed,p0,p1);
        }

        /// <summary>
        /// A string like "Encountered Root segment in non-root location."
        /// </summary>
        internal static string MetadataBinder_MustBeCalledOnRoot {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_MustBeCalledOnRoot);
            }
        }

        /// <summary>
        /// A string like "A segment without an associated type was given as input."
        /// </summary>
        internal static string MetadataBinder_NoTypeSupported {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NoTypeSupported);
            }
        }

        /// <summary>
        /// A string like "Only collection navigation properties may head Any/All queries."
        /// </summary>
        internal static string MetadataBinder_InvalidAnyAllHead {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_InvalidAnyAllHead);
            }
        }

        /// <summary>
        /// A string like "An internal error '{0}' occurred."
        /// </summary>
        internal static string General_InternalError(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.General_InternalError,p0);
        }

        /// <summary>
        /// A string like "A non-negative integer value was expected, but the value '{0}' is not a valid non-negative integer."
        /// </summary>
        internal static string ExceptionUtils_CheckIntegerNotNegative(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_CheckIntegerNotNegative,p0);
        }

        /// <summary>
        /// A string like "A positive integer value was expected, but the value '{0}' is not a valid positive integer."
        /// </summary>
        internal static string ExceptionUtils_CheckIntegerPositive(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_CheckIntegerPositive,p0);
        }

        /// <summary>
        /// A string like "A positive long value was expected; however, the value '{0}' is not a valid positive long value."
        /// </summary>
        internal static string ExceptionUtils_CheckLongPositive(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_CheckLongPositive,p0);
        }

        /// <summary>
        /// A string like "Value cannot be null or empty."
        /// </summary>
        internal static string ExceptionUtils_ArgumentStringNullOrEmpty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_ArgumentStringNullOrEmpty);
            }
        }

        /// <summary>
        /// A string like "An identifier was expected at position {0}."
        /// </summary>
        internal static string ExpressionToken_IdentifierExpected(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionToken_IdentifierExpected,p0);
        }

        /// <summary>
        /// A string like "There is an unterminated string literal at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_UnterminatedStringLiteral(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_UnterminatedStringLiteral,p0,p1);
        }

        /// <summary>
        /// A string like "Syntax error: character '{0}' is not valid at position {1} in '{2}'."
        /// </summary>
        internal static string ExpressionLexer_InvalidCharacter(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_InvalidCharacter,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Syntax error at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_SyntaxError(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_SyntaxError,p0,p1);
        }

        /// <summary>
        /// A string like "There is an unterminated literal at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_UnterminatedLiteral(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_UnterminatedLiteral,p0,p1);
        }

        /// <summary>
        /// A string like "A digit was expected at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_DigitExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_DigitExpected,p0,p1);
        }

        /// <summary>
        /// A string like "Unrecognized '{0}' literal '{1}' at '{2}' in '{3}'."
        /// </summary>
        internal static string UriQueryExpressionParser_UnrecognizedLiteral(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_UnrecognizedLiteral,p0,p1,p2,p3);
        }

    }

    /// <summary>
    ///    Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error {

        /// <summary>
        /// The exception that is thrown when a null reference (Nothing in Visual Basic) is passed to a method that does not accept it as a valid argument.
        /// </summary>
        internal static Exception ArgumentNull(string paramName) {
            return new ArgumentNullException(paramName);
        }
        
        /// <summary>
        /// The exception that is thrown when the value of an argument is outside the allowable range of values as defined by the invoked method.
        /// </summary>
        internal static Exception ArgumentOutOfRange(string paramName) {
            return new ArgumentOutOfRangeException(paramName);
        }

        /// <summary>
        /// The exception that is thrown when the author has yet to implement the logic at this point in the program. This can act as an exception based TODO tag.
        /// </summary>
        internal static Exception NotImplemented() {
            return new NotImplementedException();
        }

        /// <summary>
        /// The exception that is thrown when an invoked method is not supported, or when there is an attempt to read, seek, or write to a stream that does not support the invoked functionality. 
        /// </summary>
        internal static Exception NotSupported() {
            return new NotSupportedException();
        }        
    }
}
