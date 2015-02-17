//---------------------------------------------------------------------
// <copyright file="EntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Entity set
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    [System.Diagnostics.DebuggerDisplay("EntitySet Name={this.Name} EntityType={this.EntityType.Name}")]
    public class EntitySet : AnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the EntitySet class without a name or a type.
        /// </summary>
        public EntitySet()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntitySet class with given name but without a type.
        /// </summary>
        /// <param name="name">Name of entity set.</param>
        public EntitySet(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntitySet class with given name and type.
        /// </summary>
        /// <param name="name">Name of entity set.</param>
        /// <param name="entityType">Base type of entity set elements.</param>
        public EntitySet(string name, EntityType entityType)
        {
            this.Name = name;
            this.EntityType = entityType;
        }

        /// <summary>
        /// Gets or sets EntitySet name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets type of entities belonging in the set.
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the entity container this set belongs to. If it is null, then the set has not been added to any container.
        /// </summary>
        public EntityContainer Container { get; set; }

        /// <summary>
        /// Gets the container-qualified name of this <see cref="EntitySet"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "InvalidOperationExceptions are allowed to be thrown from property getters according to this rule, and TaupoInvalidOperationExceptions are equivalent to these.")]
        public string ContainerQualifiedName
        {
            get
            {
                if (this.Container == null)
                {
                    throw new TaupoInvalidOperationException("Cannot determine the container-qualified name because this entity set is not associated with an entity container.");
                }

                return this.Container.Name + "." + this.Name;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.EntitySet"/>.
        /// </summary>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator EntitySet(string entitySetName)
        {
            return new EntitySetReference(entitySetName);
        }
    }
}
