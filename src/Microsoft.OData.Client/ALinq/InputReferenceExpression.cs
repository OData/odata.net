//---------------------------------------------------------------------
// <copyright file="InputReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a reference to a bound resource set in the resource path.
    /// The type of the input reference is the element type of the set.
    /// </summary>
    /// <remarks>
    /// Because the type of the input reference is the element type of the set,
    /// it can be used to indicate what a range variable ranges over.
    ///
    /// For example, in input.Select(b => b.id), 'input' is an IQueryable of T,
    /// and 'b' is a parameter of type T. 'b' can be rebound as an input reference
    /// to 'input' by the InputBinder, which helps in query analysis and
    /// translation.
    /// </remarks>
    [DebuggerDisplay("InputReferenceExpression -> {Type}")]
    internal sealed class InputReferenceExpression : Expression
    {
        /// <summary>The resource or set referred to by this input reference expression</summary>
        private ResourceExpression target;

        /// <summary>
        /// Constructs a new input reference expression that refers to the specified resource set
        /// </summary>
        /// <param name="target">The target resource set that the new expression will reference</param>
        internal InputReferenceExpression(ResourceExpression target)
        {
            Debug.Assert(target != null, "Target resource set cannot be null");
            this.target = target;
        }

        /// <summary>
        /// The <see cref="Type"/> of the value represented by this <see cref="Expression"/>.
        /// </summary>
        public override Type Type
        {
            get { return this.target.ResourceType; }
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)ResourceExpressionType.InputReference; }
        }

        /// <summary>
        /// Retrieves the resource set referred to by this input reference expression
        /// </summary>
        internal ResourceExpression Target
        {
            get { return this.target; }
        }

        /// <summary>
        /// Retargets this input reference to point to the resource set specified by <paramref name="newTarget"/>.
        /// </summary>
        /// <param name="newTarget">The <see cref="QueryableResourceExpression"/> that this input reference should use as its target</param>
        internal void OverrideTarget(QueryableResourceExpression newTarget)
        {
            Debug.Assert(newTarget != null, "Resource set cannot be null");
            Debug.Assert(newTarget.ResourceType.Equals(this.Type), "Cannot reference a resource set with a different resource type");

            this.target = newTarget;
        }
    }
}
