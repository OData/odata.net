//---------------------------------------------------------------------
// <copyright file="LinqNewInstanceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Expression node representing the construction of a new instance of a particular type in a Linq query.
    /// </summary>
    public class LinqNewInstanceExpression : QueryExpression
    {
        internal LinqNewInstanceExpression(
            IEnumerable<QueryExpression> constructorArguments, 
            IEnumerable<string> memberNames, 
            IEnumerable<QueryExpression> members, 
            QueryType type)
            : base(type)
        {
            this.ConstructorArguments = constructorArguments.ToList().AsReadOnly();
            this.MemberNames = memberNames.ToList().AsReadOnly();
            this.Members = members.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the list of arguments to be passed to the constructor.
        /// </summary>
        public ReadOnlyCollection<QueryExpression> ConstructorArguments { get; private set; }

        /// <summary>
        /// Gets the list of member names for the initializer pattern.
        /// </summary>
        public ReadOnlyCollection<string> MemberNames { get; private set; }

        /// <summary>
        /// Gets the list of members for the initializer pattern
        /// </summary>
        public ReadOnlyCollection<QueryExpression> Members { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            var constructorArguments = string.Join(", ", this.ConstructorArguments.Select(a => a.ToString()).ToArray());
            var result = string.Format(CultureInfo.InvariantCulture, "new {0}({1})", ((IQueryClrType)this.ExpressionType).ClrType.Name, constructorArguments);

            if (this.Members.Count > 0)
            {
                var initializerArguments = this.Members.Select((t, i) => this.MemberNames[i] + " = " + t).ToList();
                result += string.Format(CultureInfo.InvariantCulture, " {{ {0} }}", string.Join(", ", initializerArguments.ToArray()));
            }

            return result;
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
