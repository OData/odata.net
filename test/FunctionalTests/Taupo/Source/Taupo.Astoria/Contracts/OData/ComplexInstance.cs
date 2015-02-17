//---------------------------------------------------------------------
// <copyright file="ComplexInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents an instance of a complex type
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class ComplexInstance : TypedValue, IEnumerable
    {
        /// <summary>
        /// The collection of property values. 
        /// This is not exposed publicly to enforce use of Add/Remove methods
        /// </summary>
        private List<PropertyInstance> properties = new List<PropertyInstance>();

        /// <summary>
        /// Initializes a new instance of the ComplexInstance class
        /// </summary>
        public ComplexInstance()
            : this(null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComplexInstance class with the given values
        /// </summary>
        /// <param name="fullTypeName">The full type name of the value</param>
        /// <param name="isNull">Whether or not the value is null</param>
        public ComplexInstance(string fullTypeName, bool isNull)
            : base(fullTypeName, isNull)
        {
        }

        /// <summary>
        /// Gets or sets the property values for this complex type
        /// </summary>
        public IEnumerable<PropertyInstance> Properties
        {
            get
            {
                return this.properties.AsEnumerable();
            }

            set
            {
                ExceptionUtilities.CheckArgumentNotNull(value, "value");
                this.properties.Clear();
                value.ForEach(p => this.Add(p));
            }
        }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} {{ {1} }}", this.FullTypeName, string.Join(", ", this.Properties.Select(p => p.StringRepresentation)));
            }
        }

        /// <summary>
        /// Adds the given property to the complex instance, and sets it as the property's parent
        /// </summary>
        /// <param name="toAdd">The property to add</param>
        public void Add(PropertyInstance toAdd)
        {
            this.properties.Add(toAdd);
        }

        /// <summary>
        /// Removes the given property, if present, and sets it's parent to null
        /// </summary>
        /// <param name="toRemove">The property to remove</param>
        public void Remove(PropertyInstance toRemove)
        {
            this.properties.Remove(toRemove);
        }

        /// <summary>
        /// Replace the old property with a new property
        /// </summary>
        /// <param name="oldInstance">The old property to be replaced</param>
        /// <param name="newInstance">The property to replace the old one</param>
        public void Replace(PropertyInstance oldInstance, PropertyInstance newInstance)
        {
            this.properties[this.properties.IndexOf(oldInstance)] = newInstance;
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
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
