//---------------------------------------------------------------------
// <copyright file="LinqMemberMethodExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Expression node that represents linq member method call expression.
    /// </summary>
    public class LinqMemberMethodExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the LinqMemberMethodExpression class.
        /// </summary>
        /// <param name="source">The source expression.</param>
        /// <param name="memberMethod">The member method.</param>
        /// <param name="resultType">The result type.</param>
        /// <param name="arguments">The arguments for the method call.</param>
        internal LinqMemberMethodExpression(QueryExpression source, Function memberMethod, QueryType resultType, params QueryExpression[] arguments)
            : base(resultType)
        {
            this.Source = source;
            this.MemberMethod = memberMethod;
            this.Arguments = new ReadOnlyCollection<QueryExpression>(arguments.ToList());
        }

        /// <summary>
        /// Gets the source expression.
        /// </summary>
        public QueryExpression Source { get; private set; }

        /// <summary>
        /// Gets the member method.
        /// </summary>
        public Function MemberMethod { get; private set; }

        /// <summary>
        /// Gets the arguments for the method call.
        /// </summary>
        public ReadOnlyCollection<QueryExpression> Arguments { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            var methodAnnotation = this.MemberMethod.Annotations.OfType<MemberInSpatialTypeAnnotation>().FirstOrDefault();
            ExceptionUtilities.CheckObjectNotNull(methodAnnotation, "Member method should have annotation to define whether the method is static or not.");

            bool isStatic = methodAnnotation.IsStaticMember;
            var methodSignature = string.Empty;

            if (isStatic)
            {
                methodSignature = this.MemberMethod.NamespaceName + "." + this.MemberMethod.Name;
            }
            else
            {
                methodSignature = this.Source.ToString() + "." + this.MemberMethod.Name;
            }

            return methodSignature + "(" + string.Join(", ", this.Arguments.Select(x => x.ToString()).ToArray()) + ")";
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return ((ILinqExpressionVisitor<TResult>)visitor).Visit(this);
        }
    }
}