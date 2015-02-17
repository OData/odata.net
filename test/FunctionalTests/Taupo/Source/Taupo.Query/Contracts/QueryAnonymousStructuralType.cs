//---------------------------------------------------------------------
// <copyright file="QueryAnonymousStructuralType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Structural type representing LINQ anonymous type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class QueryAnonymousStructuralType : QueryStructuralType
    {
        /// <summary>
        /// Initializes a new instance of the QueryAnonymousStructuralType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryAnonymousStructuralType(IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
        }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "AnonymousType(" + string.Join(", ", this.Properties.Select(c => c.Name + " : " + c.PropertyType.StringRepresentation).ToArray()) + ")";
            }
        }

        /// <summary>
        /// Determines whether the type can be assigned from another.
        /// </summary>
        /// <param name="queryType">Type to assign from.</param>
        /// <returns>True if assignment is possible, false otherwise.</returns>
        public override bool IsAssignableFrom(QueryType queryType)
        {
            if (object.ReferenceEquals(this, queryType))
            {
                return true;
            }

            // if it is not an anonymous type, then it is not assignable
            var anonymousType = queryType as QueryAnonymousStructuralType;
            if (anonymousType == null)
            {
                return false;
            }

            // if the number of member properties does not match, then it is not assignable
            if (this.Properties.Count != anonymousType.Properties.Count)
            {
                return false;
            }

            // if the member property names or types do not match, then it is not assignable
            for (int i = 0; i < this.Properties.Count; i++)
            {
                if (this.Properties[i].Name != anonymousType.Properties[i].Name || 
                    !this.Properties[i].PropertyType.IsAssignableFrom(anonymousType.Properties[i].PropertyType))
                {
                    return false;
                }
            }

            // otherwise it is assignable
            return true;
        }

        /// <summary>
        /// Returns value indicating whether this instance of anonymous type is equal to specified object.
        /// </summary>
        /// <param name="obj">Object to compare with this instance.</param>
        /// <returns>True if anonymous types are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            var anonymousObj = obj as QueryAnonymousStructuralType;

            if (anonymousObj == null)
            {
                return false;
            }

            if (anonymousObj.Properties.Count != this.Properties.Count)
            {
                return false;
            }

            for (int i = 0; i < anonymousObj.Properties.Count; i++)
            {
                if (anonymousObj.Properties[i].Name != this.Properties[i].Name)
                {
                    return false;
                }

                if (!anonymousObj.Properties[i].PropertyType.Equals(this.Properties[i].PropertyType))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Return the hash code for this anonymous type.
        /// </summary>
        /// <returns>Computed hash code.</returns>
        public override int GetHashCode()
        {
            int hashCode = 42;
            foreach (var member in this.Properties)
            {
                hashCode ^= member.Name.GetHashCode();
                hashCode ^= member.PropertyType.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query type.</param>
        /// <returns>The result of visiting this query type.</returns>
        public override TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
