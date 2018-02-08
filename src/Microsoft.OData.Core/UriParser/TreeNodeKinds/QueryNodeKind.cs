//---------------------------------------------------------------------
// <copyright file="QueryNodeKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Public enumeration of kinds of query nodes. A subset of InternalQueryNodeKind
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "QueryNodeKind is not a flag.")]
    public enum QueryNodeKind
    {
        /// <summary>
        /// No query node kind...  the default value.
        /// </summary>
        None = InternalQueryNodeKind.None,

        /// <summary>
        /// A constant value.
        /// </summary>
        Constant = InternalQueryNodeKind.Constant,

        /// <summary>
        /// A node that represents conversion from one type to another.
        /// </summary>
        Convert = InternalQueryNodeKind.Convert,

        /// <summary>
        /// Non-resource node referencing a range variable.
        /// </summary>
        NonResourceRangeVariableReference = InternalQueryNodeKind.NonResourceRangeVariableReference,

        /// <summary>
        /// Node used to represent a binary operator.
        /// </summary>
        BinaryOperator = InternalQueryNodeKind.BinaryOperator,

        /// <summary>
        /// Node used to represent a unary operator.
        /// </summary>
        UnaryOperator = InternalQueryNodeKind.UnaryOperator,

        /// <summary>
        /// Node describing access to a property which is a single (non-collection) non-entity value.
        /// </summary>
        SingleValuePropertyAccess = InternalQueryNodeKind.SingleValuePropertyAccess,

        /// <summary>
        /// Node describing access to a property which is a non-entity collection value.
        /// </summary>
        CollectionPropertyAccess = InternalQueryNodeKind.CollectionPropertyAccess,

        /// <summary>
        /// Function call returning a single value.
        /// </summary>
        SingleValueFunctionCall = InternalQueryNodeKind.SingleValueFunctionCall,

        /// <summary>
        /// Any query.
        /// </summary>
        Any = InternalQueryNodeKind.Any,

        /// <summary>
        /// Node for a navigation property with target multiplicity Many.
        /// </summary>
        CollectionNavigationNode = InternalQueryNodeKind.CollectionNavigationNode,

        /// <summary>
        /// Node for a navigation property with target multiplicity ZeroOrOne or One.
        /// </summary>
        SingleNavigationNode = InternalQueryNodeKind.SingleNavigationNode,

        /// <summary>
        /// Single-value property access that refers to an open property.
        /// </summary>
        SingleValueOpenPropertyAccess = InternalQueryNodeKind.SingleValueOpenPropertyAccess,

        /// <summary>
        /// Cast on a single resource.
        /// </summary>
        SingleResourceCast = InternalQueryNodeKind.SingleResourceCast,

        /// <summary>
        /// All query.
        /// </summary>
        All = InternalQueryNodeKind.All,

        /// <summary>
        /// Cast on a collection of resources.
        /// </summary>
        CollectionResourceCast = InternalQueryNodeKind.CollectionResourceCast,

        /// <summary>
        /// Placeholder node referencing a rangeVariable on the binding stack that references an entity or a complex.
        /// </summary>
        ResourceRangeVariableReference = InternalQueryNodeKind.ResourceRangeVariableReference,

        /// <summary>
        /// Node the represents a function call that returns a single entity or complex.
        /// </summary>
        SingleResourceFunctionCall = InternalQueryNodeKind.SingleResourceFunctionCall,

        /// <summary>
        /// Node that represents a function call that returns a collection.
        /// </summary>
        CollectionFunctionCall = InternalQueryNodeKind.CollectionFunctionCall,

        /// <summary>
        /// Node that represents a function call that returns a collection of resources.
        /// </summary>
        CollectionResourceFunctionCall = InternalQueryNodeKind.CollectionResourceFunctionCall,

        /// <summary>
        /// Node that represents a named function parameter.
        /// </summary>
        NamedFunctionParameter = InternalQueryNodeKind.NamedFunctionParameter,

        /// <summary>
        /// The parameter alias node.
        /// </summary>
        ParameterAlias = InternalQueryNodeKind.ParameterAlias,

        /// <summary>
        /// The entity set node.
        /// </summary>
        EntitySet = InternalQueryNodeKind.EntitySet,

        /// <summary>
        /// The key lookup on a collection.
        /// </summary>
        KeyLookup = InternalQueryNodeKind.KeyLookup,

        /// <summary>
        /// The search term node.
        /// </summary>
        SearchTerm = InternalQueryNodeKind.SearchTerm,

        /// <summary>
        /// Node describing access to a open property which is a non-entity collection value.
        /// </summary>
        CollectionOpenPropertyAccess = InternalQueryNodeKind.CollectionOpenPropertyAccess,

        /// <summary>
        /// Node represents a collection of complex property.
        /// </summary>
        CollectionComplexNode = InternalQueryNodeKind.CollectionComplexNode,

        /// <summary>
        /// Node represents a single complex property.
        /// </summary>
        SingleComplexNode = InternalQueryNodeKind.SingleComplexNode,

        /// <summary>
        /// Count of a collection contains primitive or enum or complex or entity type.
        /// </summary>
        Count = InternalQueryNodeKind.Count,

        /// <summary>
        /// Cast on a single value.
        /// </summary>
        SingleValueCast = InternalQueryNodeKind.SingleValueCast,
    }

    /// <summary>
    /// Internal enumeration of kinds of query nodes. A superset of QueryNodeKind
    /// </summary>
    internal enum InternalQueryNodeKind
    {
        /// <summary>
        /// none... default value.
        /// </summary>
        None = 0,

        /// <summary>
        /// The constant value.
        /// </summary>
        Constant = 1,

        /// <summary>
        /// A node that signifies the promotion of a primitive type.
        /// </summary>
        Convert = 2,

        /// <summary>
        /// Non-resource node referencing a range variable.
        /// </summary>
        NonResourceRangeVariableReference = 3,

        /// <summary>
        /// Parameter node used to represent a binary operator.
        /// </summary>
        BinaryOperator = 4,

        /// <summary>
        /// Parameter node used to represent a unary operator.
        /// </summary>
        UnaryOperator = 5,

        /// <summary>
        /// Node describing access to a property which is a single (non-collection) non-entity value.
        /// </summary>
        SingleValuePropertyAccess = 6,

        /// <summary>
        /// Node describing access to a property which is a non-entity collection value.
        /// </summary>
        CollectionPropertyAccess = 7,

        /// <summary>
        /// Function call returning a single value.
        /// </summary>
        SingleValueFunctionCall = 8,

        /// <summary>
        /// Any query.
        /// </summary>
        Any = 9,

        /// <summary>
        /// Node for a navigation property with target multiplicity Many.
        /// </summary>
        CollectionNavigationNode = 10,

        /// <summary>
        /// Node for a navigation property with target multiplicity ZeroOrOne or One.
        /// </summary>
        SingleNavigationNode = 11,

        /// <summary>
        /// Single-value property access that refers to an open property.
        /// </summary>
        SingleValueOpenPropertyAccess = 12,

        /// <summary>
        /// Cast on a single resource.
        /// </summary>
        SingleResourceCast = 13,

        /// <summary>
        /// All query.
        /// </summary>
        All = 14,

        /// <summary>
        /// Cast on a resource collection.
        /// </summary>
        CollectionResourceCast = 15,

        /// <summary>
        /// Resource node referencing a range variable.
        /// </summary>
        ResourceRangeVariableReference = 16,

        /// <summary>
        /// SingleResourceFunctionCall node.
        /// </summary>
        SingleResourceFunctionCall = 17,

        /// <summary>
        /// Node that represents a function call that returns a collection.
        /// </summary>
        CollectionFunctionCall = 18,

        /// <summary>
        /// Node that represents a function call that returns a collection of resources.
        /// </summary>
        CollectionResourceFunctionCall = 19,

        /// <summary>
        /// Node that represents a named function parameter.
        /// </summary>
        NamedFunctionParameter = 20,

        /// <summary>
        /// The parameter alias node.
        /// </summary>
        ParameterAlias = 21,

        /// <summary>
        /// The entity set node.
        /// </summary>
        EntitySet = 22,

        /// <summary>
        /// The key lookup on a collection.
        /// </summary>
        KeyLookup = 23,

        /// <summary>
        /// The search Term.
        /// </summary>
        SearchTerm = 24,

        /// <summary>
        /// Node describing access to a open property which is a non-entity collection value.
        /// </summary>
        CollectionOpenPropertyAccess = 25,

        /// <summary>
        /// Node represents a collection of complex property.
        /// </summary>
        CollectionComplexNode = 26,

        /// <summary>
        /// Node represents a single complex property.
        /// </summary>
        SingleComplexNode = 27,

        /// <summary>
        /// Node describing count of a collection contains primitive or enum or complex or entity type.
        /// </summary>
        Count = 28,

        /// <summary>
        /// Cast on a single value.
        /// </summary>
        SingleValueCast = 29,
    }
}