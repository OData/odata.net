//---------------------------------------------------------------------
// <copyright file="EmptyUntypedCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents an empty collection where the type cannot be inferred from the payload
    /// This is only used when deserializing certain payloads that don't have enough information to generate something more specific
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Type represents a collection")]
    public class EmptyUntypedCollection : ODataPayloadElementCollection, ITypedValue
    {
        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return this.ElementType.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the Type Name of the collection
        /// </summary>
        public string FullTypeName
        {
            get { return null; }
            set { throw new TaupoInvalidOperationException("Cannot set type name on empty un-typed collection. It is always un-typed."); }
        }

        /// <summary>
        /// Gets or sets the Type Name of the collection
        /// </summary>
        public long? InlineCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Type Name of the collection
        /// </summary>
        public string NextLink
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the value is null
        /// </summary>
        public bool IsNull
        {
            get { return false; }
            set { throw new TaupoInvalidOperationException("Empty un-typed collection is always non-null. Otherwise it is not 'empty'."); }
        }

        /// <summary>
        /// Throws an invalid operation exception. By definition, one cannot add elements to an empty collection
        /// </summary>
        /// <param name="element">The element to add</param>
        public override void Add(ODataPayloadElement element)
        {
            throw new TaupoInvalidOperationException("Cannot add elements to an empty un-typed collection");
        }

        /// <summary>
        /// Returns an enumerator for an empty list
        /// </summary>
        /// <returns>The enumerator for an empty list</returns>
        public IEnumerator<ODataPayloadElement> GetEnumerator()
        {
            return new List<ODataPayloadElement>().GetEnumerator();
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(IODataPayloadElementVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that does not return a result.
        /// </summary>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        public override void Accept(IODataPayloadElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
