//---------------------------------------------------------------------
// <copyright file="InputBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;

    #endregion Namespaces

    /// <summary>
    /// Replaces references to resources - represented as either ParameterExpressions or one or more
    /// MemberExpressions over a ParameterExpression - with an appropriate InputReferenceExpression that
    /// indicates which resource is referenced; effective 'binds' the argument expression to the
    /// resources that it references.
    /// </summary>
    internal sealed class InputBinder : DataServiceALinqExpressionVisitor
    {
        #region Private fields

        /// <summary>Tracks which resources are referenced by the argument expression</summary>
        private readonly HashSet<ResourceExpression> referencedInputs = new HashSet<ResourceExpression>(EqualityComparer<ResourceExpression>.Default);

        /// <summary>Resource from which valid references must start; if no resource with a transparent scope is present, only direct references to this resource will be rebound</summary>
        private readonly ResourceExpression input;

        /// <summary>The input resource, as a queryable resource (may be null if the input is actually a NavigationPropertySingletonExpression)</summary>
        private readonly QueryableResourceExpression inputResource;

        /// <summary>The ParameterExpression that, if encountered, indicates a reference to the input resource</summary>
        private readonly ParameterExpression inputParameter;

        #endregion Private fields

        /// <summary>
        /// Constructs a new InputBinder based on the specified input resources, which is represented by the specified ParameterExpression.
        /// </summary>
        /// <param name="resource">The current input resource from which valid references must start</param>
        /// <param name="setReferenceParam">The parameter that must be referenced in order to refer to the specified input resources</param>
        private InputBinder(ResourceExpression resource, ParameterExpression setReferenceParam)
        {
            this.input = resource;
            this.inputResource = resource as QueryableResourceExpression;
            this.inputParameter = setReferenceParam;
        }

        /// <summary>
        /// Replaces Lambda parameter references or transparent scope property accesses over those Lambda
        /// parameter references with <see cref="InputReferenceExpression"/>s to the appropriate corresponding
        /// <see cref="QueryableResourceExpression"/>s, based on the 'input' QueryableResourceExpression to which the
        /// Lambda is logically applied and any enclosing transparent scope applied to that input resource.
        /// </summary>
        /// <param name="e">The expression to rebind</param>
        /// <param name="currentInput">
        /// The 'current input' resource - either the root resource or the
        /// rightmost resource in the navigation chain.</param>
        /// <param name="inputParameter">The Lambda parameter that represents a reference to the 'input'</param>
        /// <param name="referencedInputs">A list that will be populated with the resources that were referenced by the rebound expression</param>
        /// <returns>
        /// The rebound version of <paramref name="e"/> where MemberExpression/ParameterExpressions that
        /// represent resource references have been replaced with appropriate InputReferenceExpressions.
        /// </returns>
        internal static Expression Bind(Expression e, ResourceExpression currentInput, ParameterExpression inputParameter, List<ResourceExpression> referencedInputs)
        {
            Debug.Assert(e != null, "Expression cannot be null");
            Debug.Assert(currentInput != null, "A current input resource is required");
            Debug.Assert(inputParameter != null, "The input lambda parameter is required");
            Debug.Assert(referencedInputs != null, "The referenced inputs list is required");

            InputBinder binder = new InputBinder(currentInput, inputParameter);
            Expression result = binder.Visit(e);
            referencedInputs.AddRange(binder.referencedInputs);
            return result;
        }

        /// <summary>
        /// Resolves member accesses that represent transparent scope property accesses to the corresponding resource,
        /// iff the input resource is enclosed in a transparent scope and the specified MemberExpression represents
        /// such a property access.
        /// </summary>
        /// <param name="m">MemberExpression expression to visit</param>
        /// <returns>
        /// An InputReferenceExpression if the member access represents a transparent scope property
        /// access that can be resolved to a resource in the path that produces the input resource;
        /// otherwise the same MemberExpression is returned.
        /// </returns>
        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            // If the current input resource is not enclosed in a transparent scope, then this
            // MemberExpression cannot represent a valid transparent scope access based on the input parameter.
            if (this.inputResource == null ||
                !this.inputResource.HasTransparentScope)
            {
                return base.VisitMemberAccess(m);
            }

            ParameterExpression innerParamRef = null;
            Stack<PropertyInfo> nestedAccesses = new Stack<PropertyInfo>();
            MemberExpression memberRef = m;
            while (memberRef != null &&
                   PlatformHelper.IsProperty(memberRef.Member) &&
                   memberRef.Expression != null)
            {
                nestedAccesses.Push((PropertyInfo)memberRef.Member);

                if (memberRef.Expression.NodeType == ExpressionType.Parameter)
                {
                    innerParamRef = (ParameterExpression)memberRef.Expression;
                }

                memberRef = memberRef.Expression as MemberExpression;
            }

            // Only continue if the inner non-MemberExpression is the input reference ParameterExpression and
            // at least one property reference is present - otherwise this cannot be a transparent scope access.
            if (innerParamRef != this.inputParameter || nestedAccesses.Count == 0)
            {
                return m;
            }

            ResourceExpression target = this.input;
            QueryableResourceExpression targetResource = this.inputResource;
            bool transparentScopeTraversed = false;

            // Process all the traversals through transparent scopes.
            while (nestedAccesses.Count > 0)
            {
                if (targetResource == null || !targetResource.HasTransparentScope)
                {
                    break;
                }

                // Peek the property; pop it once it's consumed
                // (it could be a non-transparent-identifier access).
                PropertyInfo currentProp = nestedAccesses.Peek();

                // If this is the accessor for the target, then the member
                // refers to the target itself.
                if (currentProp.Name.Equals(targetResource.TransparentScope.Accessor, StringComparison.Ordinal))
                {
                    target = targetResource;
                    nestedAccesses.Pop();
                    transparentScopeTraversed = true;
                    continue;
                }

                // This member could also be one of the in-scope sources of the target.
                Expression source;
                if (!targetResource.TransparentScope.SourceAccessors.TryGetValue(currentProp.Name, out source))
                {
                    break;
                }

                transparentScopeTraversed = true;
                nestedAccesses.Pop();
                Debug.Assert(source != null, "source != null -- otherwise ResourceBinder created an accessor to nowhere");
                InputReferenceExpression sourceReference = source as InputReferenceExpression;
                if (sourceReference == null)
                {
                    targetResource = source as QueryableResourceExpression;
                    if (targetResource == null || !targetResource.HasTransparentScope)
                    {
                        target = (ResourceExpression)source;
                    }
                }
                else
                {
                    targetResource = sourceReference.Target as QueryableResourceExpression;
                    target = targetResource;
                }
            }

            // If no traversals were made, the original expression is OK.
            if (!transparentScopeTraversed)
            {
                return m;
            }

            // Process traversals after the transparent scope.
            Expression result = this.CreateReference(target);
            while (nestedAccesses.Count > 0)
            {
                result = Expression.Property(result, nestedAccesses.Pop());
            }

            return result;
        }

        /// <summary>
        /// Converts a parameter reference to the input resource into an InputReferenceExpression,
        /// iff the parameter reference is to the parameter expression that represents the input resource
        /// and the input resource is not enclosed in a transparent scope.
        /// </summary>
        /// <param name="p">The parameter reference expression</param>
        /// <returns>
        /// An InputReferenceExpression if the parameter reference is to the input parameter;
        /// otherwise the same parameter reference expression
        /// </returns>
        internal override Expression VisitParameter(ParameterExpression p)
        {
            // If the input resource is not enclosed in a transparent scope,
            // and the parameter reference is a reference to the Lambda parameter
            // that represents the input resource, then return an InputReferenceExpression.
            if ((this.inputResource == null || !this.inputResource.HasTransparentScope) &&
               p == this.inputParameter)
            {
                return this.CreateReference(this.input);
            }
            else
            {
                return base.VisitParameter(p);
            }
        }

        /// <summary>
        /// Returns an <see cref="InputReferenceExpression"/> that references the specified resource,
        /// and also adds the the resource to the hashset of resources that were referenced by the
        /// expression that is being rebound.
        /// </summary>
        /// <param name="resource">The resource for which a reference was found</param>
        /// <returns>An InputReferenceExpression that represents a reference to the specified resource</returns>
        private Expression CreateReference(ResourceExpression resource)
        {
            this.referencedInputs.Add(resource);
            return resource.CreateReference();
        }
    }
}

