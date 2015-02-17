//---------------------------------------------------------------------
// <copyright file="EdmUniqueConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Edm Unique Constraint
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class EdmUniqueConstraint : AnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the EdmUniqueConstraint class.
        /// </summary>
        public EdmUniqueConstraint()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EdmUniqueConstraint class with a given name.
        /// </summary>
        /// <param name="name">Edm Unique Constraint name.</param>
        public EdmUniqueConstraint(string name) 
        {
            this.Name = name;
            this.Properties = new List<MemberProperty>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets properties that are part of this Edm Unique Constraint.
        /// </summary>
        public IList<MemberProperty> Properties { get; private set; }

        /// <summary>
        /// Adds the specified property.
        /// </summary>
        /// <param name="property">The property to add.</param>
        public void Add(MemberProperty property)
        {
            this.Properties.Add(property);
        }

        /// <summary>
        /// Creates <see cref=" MemberPropertyReference"/> for each property name and adds it to <see cref="Properties"/> collection.
        /// </summary>
        /// <param name="propertyNames">Names of properties.</param>
        /// <returns>This object (useful for chaining multiple calls).</returns>
        public EdmUniqueConstraint WithProperties(params string[] propertyNames)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyNames, "propertyNames");
            return this.WithProperties(propertyNames.Select(c => new MemberPropertyReference(c)).ToArray());
        }

        /// <summary>
        /// Adds given properties to <see cref="Properties"/> collection.
        /// </summary>
        /// <param name="properties">Properties to add.</param>
        /// <returns>This object (useful for chaining multiple calls).</returns>
        public EdmUniqueConstraint WithProperties(params MemberProperty[] properties)
        {
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(properties, "properties");
            ExceptionUtilities.Assert(this.Properties.Count == 0, "This method can only be called on Edm Unique Constraint without any properties defined.");

            foreach (var p in properties)
            {
                this.Add(p);
            }

            return this;
        }
    }
}
