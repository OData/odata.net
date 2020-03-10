//---------------------------------------------------------------------
// <copyright file="QueryableResourceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>Queryable Resource Expression, the base class for ResourceSetExpression and SingletonExpression</summary>
    [DebuggerDisplay("QueryableResourceExpression {Source}.{MemberExpression}")]
    internal abstract class QueryableResourceExpression : ResourceExpression
    {
        /// <summary>Key Predicate conjuncts that will make a key predicate</summary>
        private readonly List<Expression> keyPredicateConjuncts;

        /// <summary>
        /// The (static) type of the resources in this navigation resource.
        /// The resource type can differ from this.Type if this expression represents a transparent scope.
        /// For example, in TransparentScope{Category, Product}, the true element type is Product.
        /// </summary>
        private readonly Type resourceType;

        /// <summary>property member name</summary>
        private readonly Expression member;

        /// <summary>key predicate</summary>
        private Dictionary<PropertyInfo, ConstantExpression> keyFilter;

        /// <summary>sequence query options</summary>
        private List<QueryOptionExpression> sequenceQueryOptions;

        /// <summary>enclosing transparent scope</summary>
        private TransparentAccessors transparentScope;

        /// <summary>
        /// Creates a navigation resource expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="source">the source expression</param>
        /// <param name="memberExpression">property member name</param>
        /// <param name="resourceType">the element type of the resource</param>
        /// <param name="expandPaths">expand paths for resource set</param>
        /// <param name="countOption">count query option for the resource</param>
        /// <param name="customQueryOptions">custom query options for resource</param>
        /// <param name="projection">the projection expression</param>
        /// <param name="resourceTypeAs">TypeAs type</param>
        /// <param name="uriVersion">version of the Uri from the expand and projection paths</param>
        internal QueryableResourceExpression(Type type, Expression source, Expression memberExpression, Type resourceType, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion)
            : this(type, source, memberExpression, resourceType, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion, null, null, false)
        {
        }

        /// <summary>
        /// Creates a navigation resource expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="source">the source expression</param>
        /// <param name="memberExpression">property member name</param>
        /// <param name="resourceType">the element type of the resource</param>
        /// <param name="expandPaths">expand paths for resource set</param>
        /// <param name="countOption">count query option for the resource</param>
        /// <param name="customQueryOptions">custom query options for resource</param>
        /// <param name="projection">the projection expression</param>
        /// <param name="resourceTypeAs">TypeAs type</param>
        /// <param name="uriVersion">version of the Uri from the expand and projection paths</param>
        /// <param name="operationName">name of function</param>
        /// <param name="operationParameters">parameters' names and values of function</param>
        /// <param name="isAction">action flag</param>
        internal QueryableResourceExpression(Type type, Expression source, Expression memberExpression, Type resourceType, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion, string operationName, Dictionary<string, string> operationParameters, bool isAction)
            : base(source, type, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion, operationName, operationParameters, isAction)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(
                (source == null && memberExpression is ConstantExpression) ||
                (source != null && memberExpression is MemberExpression) ||
                (memberExpression == null),
                "source is null with constant entity set name, or not null with member expression, or memberExpression is null for function import.");

            this.member = memberExpression;
            this.resourceType = resourceType;
            this.sequenceQueryOptions = new List<QueryOptionExpression>();
            this.keyPredicateConjuncts = new List<Expression>();
        }

        /// <summary>
        /// Member for ResourceSet
        /// </summary>
        internal Expression MemberExpression
        {
            get { return this.member; }
        }

        /// <summary>
        /// Type of resources contained in this ResourceSet - it's element type.
        /// </summary>
        internal override Type ResourceType
        {
            get { return this.resourceType; }
        }

        /// <summary>
        /// Is this ResourceSet enclosed in an anonymously-typed transparent scope produced by a SelectMany operation?
        /// Applies to navigation ResourceSets.
        /// </summary>
        internal bool HasTransparentScope
        {
            get { return this.transparentScope != null; }
        }

        /// <summary>
        /// The property accesses required to reference this ResourceSet and its source ResourceSet if a transparent scope is present.
        /// May be null. Use <see cref="HasTransparentScope"/> to test for the presence of a value.
        /// </summary>
        internal TransparentAccessors TransparentScope
        {
            get { return this.transparentScope; }
            set { this.transparentScope = value; }
        }

        /// <summary>
        /// The list of key expressions that comprise the key predicate (if any) applied to this ResourceSet.
        /// </summary>
        internal ReadOnlyCollection<Expression> KeyPredicateConjuncts
        {
            get
            {
                return new ReadOnlyCollection<Expression>(this.keyPredicateConjuncts);
            }
        }

        /// <summary>
        /// Have sequence query options (filter, orderby, skip, take), expand paths, projection
        /// or custom query options been applied to this resource set?
        /// </summary>
        internal override bool HasQueryOptions
        {
            get
            {
                return this.sequenceQueryOptions.Count > 0 ||
                    this.ExpandPaths.Count > 0 ||
                    this.CountOption == CountOption.CountQueryTrue ||        // value only count is not an option
                    this.CountOption == CountOption.CountQueryFalse ||       // value only count is not an option
                    this.CustomQueryOptions.Count > 0 ||
                    this.Projection != null;
            }
        }

        /// <summary>
        /// If this expresssion contains at least one non-key predicate
        /// This indicates that a filter should be used
        /// </summary>
        internal bool UseFilterAsPredicate { get; set; }

        /// <summary>
        /// Filter query option for ResourceSet
        /// </summary>
        internal FilterQueryOptionExpression Filter
        {
            get
            {
                return this.sequenceQueryOptions.OfType<FilterQueryOptionExpression>().SingleOrDefault();
            }
        }

        /// <summary>
        /// OrderBy query option for ResourceSet
        /// </summary>
        internal OrderByQueryOptionExpression OrderBy
        {
            get { return this.sequenceQueryOptions.OfType<OrderByQueryOptionExpression>().SingleOrDefault(); }
        }

        /// <summary>
        /// Skip query option for ResourceSet
        /// </summary>
        internal SkipQueryOptionExpression Skip
        {
            get { return this.sequenceQueryOptions.OfType<SkipQueryOptionExpression>().SingleOrDefault(); }
        }

        /// <summary>
        /// Take query option for ResourceSet
        /// </summary>
        internal TakeQueryOptionExpression Take
        {
            get { return this.sequenceQueryOptions.OfType<TakeQueryOptionExpression>().SingleOrDefault(); }
        }

        /// <summary>
        /// Gets sequence query options for ResourceSet
        /// </summary>
        internal IEnumerable<QueryOptionExpression> SequenceQueryOptions
        {
            get { return this.sequenceQueryOptions.ToList(); }
        }

        /// <summary>Whether there are any query options for the sequence.</summary>
        internal bool HasSequenceQueryOptions
        {
            get { return this.sequenceQueryOptions.Count > 0; }
        }

        /// <summary>
        /// Create a clone with new type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The new clone.</returns>
        internal override ResourceExpression CreateCloneWithNewType(Type type)
        {
            QueryableResourceExpression clone = this.CreateCloneWithNewTypes(type, TypeSystem.GetElementType(type));

            if (this.keyPredicateConjuncts != null && this.keyPredicateConjuncts.Count > 0)
            {
                clone.SetKeyPredicate(this.keyPredicateConjuncts);
            }

            clone.keyFilter = this.keyFilter;
            clone.sequenceQueryOptions = this.sequenceQueryOptions;
            clone.transparentScope = this.transparentScope;
            return clone;
        }

        /// <summary>
        /// Creates a navigation resource expression
        /// </summary>
        /// <param name="expressionType">The expression type.</param>
        /// <param name="type">the return type of the expression</param>
        /// <param name="source">the source expression</param>
        /// <param name="memberExpression">property member name</param>
        /// <param name="resourceType">the element type of the resource</param>
        /// <param name="expandPaths">expand paths for resource set</param>
        /// <param name="countOption">count query option for the resource</param>
        /// <param name="customQueryOptions">custom query options for resource</param>
        /// <param name="projection">the projection expression</param>
        /// <param name="resourceTypeAs">TypeAs type</param>
        /// <param name="uriVersion">version of the Uri from the expand and projection paths</param>
        /// <param name="operationName">The operation name.</param>
        /// <param name="operationParameters">The operation parameter names and parameters pair for Resource</param>
        /// <returns>The navigation resource expression.</returns>
        internal static QueryableResourceExpression CreateNavigationResourceExpression(ExpressionType expressionType, Type type, Expression source, Expression memberExpression, Type resourceType, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion, string operationName, Dictionary<string, string> operationParameters)
        {
            Debug.Assert(
                expressionType == (ExpressionType)ResourceExpressionType.RootResourceSet || expressionType == (ExpressionType)ResourceExpressionType.ResourceNavigationProperty || expressionType == (ExpressionType)ResourceExpressionType.RootSingleResource,
                "Expression type is not one of following: RootResourceSet, ResourceNavigationProperty, RootSingleResource.");

            QueryableResourceExpression expression = null;

            if (expressionType == (ExpressionType)ResourceExpressionType.RootResourceSet || expressionType == (ExpressionType)ResourceExpressionType.ResourceNavigationProperty)
            {
                expression = new ResourceSetExpression(type, source, memberExpression, resourceType, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion);
            }

            if (expressionType == (ExpressionType)ResourceExpressionType.RootSingleResource)
            {
                expression = new SingletonResourceExpression(type, source, memberExpression, resourceType, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion);
            }

            if (expression != null)
            {
                expression.OperationName = operationName;
                expression.OperationParameters = operationParameters;
                return expression;
            }

            return null;
        }

        /// <summary>
        /// Cast QueryableResourceExpression to new type without affecting member type
        /// </summary>
        /// <param name="type">The new expression type</param>
        /// <returns>A copy of this with the new types</returns>
        internal QueryableResourceExpression CreateCloneForTransparentScope(Type type)
        {
            // QueryableResourceExpression can always have order information,
            // so return them as IOrderedQueryable<> always. Necessary to allow
            // OrderBy results that get aliased to a previous expression work
            // with ThenBy.
            Type elementType = TypeSystem.GetElementType(type);
            Debug.Assert(elementType != null, "elementType != null -- otherwise the set isn't going to act like a collection");
            Type newType = typeof(IOrderedQueryable<>).MakeGenericType(elementType);

            QueryableResourceExpression clone = this.CreateCloneWithNewTypes(newType, this.ResourceType);

            if (this.keyPredicateConjuncts != null && this.keyPredicateConjuncts.Count > 0)
            {
                clone.SetKeyPredicate(this.keyPredicateConjuncts);
            }

            clone.keyFilter = this.keyFilter;
            clone.sequenceQueryOptions = this.sequenceQueryOptions;
            clone.transparentScope = this.transparentScope;
            return clone;
        }

        /// <summary>
        /// Converts the key expression to filter expression
        /// </summary>
        internal void ConvertKeyToFilterExpression()
        {
            if (this.keyPredicateConjuncts.Count > 0)
            {
                this.AddFilter(this.keyPredicateConjuncts);
            }
        }

        /// <summary>
        /// Adds a filter to this ResourceSetExpression.
        /// If filter is already presents, adds the predicateConjuncts to the
        /// PredicateConjuncts of the filter
        /// </summary>
        /// <param name="predicateConjuncts">The predicate conjuncts.</param>
        internal void AddFilter(IEnumerable<Expression> predicateConjuncts)
        {
            if (this.Skip != null)
            {
                throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("filter", "skip"));
            }
            else if (this.Take != null)
            {
                throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("filter", "top"));
            }
            else if (this.Projection != null)
            {
                throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("filter", "select"));
            }

            if (this.Filter == null)
            {
                this.AddSequenceQueryOption(new FilterQueryOptionExpression(this.Type));
            }

            this.Filter.AddPredicateConjuncts(predicateConjuncts);

            this.keyPredicateConjuncts.Clear();
        }

        /// <summary>
        /// Add query option to resource expression
        /// </summary>
        /// <param name="qoe">The query option expression.</param>
        internal void AddSequenceQueryOption(QueryOptionExpression qoe)
        {
            Debug.Assert(qoe != null, "qoe != null");
            QueryOptionExpression old = this.sequenceQueryOptions.Where(o => o.GetType() == qoe.GetType()).FirstOrDefault();
            if (old != null)
            {
                qoe = qoe.ComposeMultipleSpecification(old);
                this.sequenceQueryOptions.Remove(old);
            }

            this.sequenceQueryOptions.Add(qoe);
        }

        /// <summary>
        /// Removes the filter expression from current resource set.
        /// This happens when a separate Where clause is specified in a LINQ
        /// expression for every key property.
        /// </summary>
        internal void RemoveFilterExpression()
        {
            if (this.Filter != null)
            {
                this.sequenceQueryOptions.Remove(this.Filter);
            }
        }

        /// <summary>
        /// Instructs this resource set expression to use the input reference expression from <paramref name="newInput"/> as it's
        /// own input reference, and to retarget the input reference from <paramref name="newInput"/> to this resource set expression.
        /// </summary>
        /// <param name="newInput">The resource set expression from which to take the input reference.</param>
        /// <remarks>Used exclusively by ResourceBinder.RemoveTransparentScope.</remarks>
        internal void OverrideInputReference(QueryableResourceExpression newInput)
        {
            Debug.Assert(newInput != null, "Original resource set cannot be null");
            Debug.Assert(this.inputRef == null, "OverrideInputReference cannot be called if the target has already been referenced");

            InputReferenceExpression inputRef = newInput.inputRef;
            if (inputRef != null)
            {
                this.inputRef = inputRef;
                inputRef.OverrideTarget(this);
            }
        }

        /// <summary>
        /// Sets key predicate from given values
        /// </summary>
        /// <param name="keyValues">The key values</param>
        internal void SetKeyPredicate(IEnumerable<Expression> keyValues)
        {
            Debug.Assert(keyValues != null, "keyValues != null");

            this.keyPredicateConjuncts.Clear();

            foreach (Expression ex in keyValues)
            {
                this.keyPredicateConjuncts.Add(ex);
            }
        }

        /// <summary>
        /// Gets the key properties from KeyPredicateConjuncts
        /// </summary>
        /// <returns>The key properties.</returns>
        internal Dictionary<PropertyInfo, ConstantExpression> GetKeyProperties()
        {
            var keyValues = new Dictionary<PropertyInfo, ConstantExpression>(EqualityComparer<PropertyInfo>.Default);
            if (this.keyPredicateConjuncts.Count > 0)
            {
                foreach (Expression predicate in this.keyPredicateConjuncts)
                {
                    PropertyInfo property;
                    ConstantExpression constantValue;
                    if (ResourceBinder.PatternRules.MatchKeyComparison(predicate, out property, out constantValue))
                    {
                        keyValues.Add(property, constantValue);
                    }
                }
            }

            return keyValues;
        }

        /// <summary>
        /// Creates a clone of the current QueryableResourceExpression with the specified expression and resource types
        /// </summary>
        /// <param name="newType">The new expression type</param>
        /// <param name="newResourceType">The new resource type</param>
        /// <returns>A copy of this with the new types</returns>
        protected abstract QueryableResourceExpression CreateCloneWithNewTypes(Type newType, Type newResourceType);

        /// <summary>
        /// Represents the property accesses required to access both
        /// this resource set and its source resource/set (for navigations).
        ///
        /// These accesses are required to reference resource sets enclosed
        /// in transparent scopes introduced by use of SelectMany.
        /// </summary>
        /// <remarks>
        /// For example, this query:
        ///  from c in Custs where c.id == 1
        ///  from o in c.Orders from od in o.OrderDetails select od
        ///
        /// Translates to:
        ///  c.Where(c => c.id == 1)
        ///   .SelectMany(c => o, (c, o) => new $(c=c, o=o))
        ///   .SelectMany($ => $.o, ($, od) => od)
        ///
        /// PatternRules.MatchPropertyProjectionSet identifies Orders as the target of the collector.
        /// PatternRules.MatchTransparentScopeSelector identifies the introduction of a transparent identifier.
        ///
        /// A transparent accessor is associated with Orders, with 'c' being the source accessor,
        /// and 'o' being the (introduced) accessor.
        /// </remarks>
        [DebuggerDisplay("{ToString()}")]
        internal class TransparentAccessors
        {
            #region Internal fields.

            /// <summary>
            /// The property reference that must be applied to reference this resource set
            /// </summary>
            internal readonly string Accessor;

            /// <summary>
            /// The property reference that must be applied to reference the source resource set.
            /// Note that this set's Accessor is NOT required to access the source set, but the
            /// source set MAY impose it's own Transparent Accessors
            /// </summary>
            internal readonly Dictionary<string, Expression> SourceAccessors;

            #endregion Internal fields.

            /// <summary>
            /// Constructs a new transparent scope with the specified set and source set accessors
            /// </summary>
            /// <param name="acc">The name of the property required to access the resource set</param>
            /// <param name="sourceAccessors">The names of the property required to access the resource set's sources.</param>
            internal TransparentAccessors(string acc, Dictionary<string, Expression> sourceAccessors)
            {
                Debug.Assert(!string.IsNullOrEmpty(acc), "Set accessor cannot be null or empty");
                Debug.Assert(sourceAccessors != null, "sourceAccessors != null");

                this.Accessor = acc;
                this.SourceAccessors = sourceAccessors;
            }

            /// <summary>Provides a string representation of this accessor.</summary>
            /// <returns>The text representation of this accessor.</returns>
            public override string ToString()
            {
                string result = "SourceAccessors=[" + string.Join(",", this.SourceAccessors.Keys.ToArray());
                result += "] ->* Accessor=" + this.Accessor;
                return result;
            }
        }
    }
}
