//---------------------------------------------------------------------
// <copyright file="EntityContainer.cs" company="Microsoft">
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
    /// Entity Container
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class EntityContainer : AnnotatedItem, INamedItem, IEnumerable
    {
        private IList<EntitySet> entitySetsList = new List<EntitySet>();
        private IList<AssociationSet> associationSetsList = new List<AssociationSet>();
        private IList<FunctionImport> functionImportsList = new List<FunctionImport>();

        /// <summary>
        /// Initializes a new instance of the EntityContainer class without a name.
        /// </summary>
        public EntityContainer()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntityContainer class with a given name.
        /// </summary>
        /// <param name="name">Container name</param>
        public EntityContainer(string name)
            : this(null, name)
        {
            this.Name = name;
            this.entitySetsList = new List<EntitySet>();
            this.associationSetsList = new List<AssociationSet>();
            this.functionImportsList = new List<FunctionImport>();
        }

        /// <summary>
        /// Initializes a new instance of the EntityContainer class with a given name.
        /// </summary>
        /// <param name="namespaceName">Container namespace</param>
        /// <param name="name">Container name</param>
        public EntityContainer(string namespaceName, string name)
        {
            this.NamespaceName = namespaceName;
            this.Name = name;
            this.entitySetsList = new List<EntitySet>();
            this.associationSetsList = new List<AssociationSet>();
            this.functionImportsList = new List<FunctionImport>();
        }

        /// <summary>
        /// Gets or sets EntityContainer namespace
        /// </summary>
        public string NamespaceName { get; set; }

        /// <summary>
        /// Gets or sets EntityContainer name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets EntityContainer full name.
        /// </summary>
        public string FullName 
        {
            get
            {
                string fullName = string.Empty;

                if (!string.IsNullOrEmpty(this.NamespaceName))
                {
                    fullName = this.NamespaceName + ".";
                }

                fullName += this.Name;

                return fullName;
            }
        }

        /// <summary>
        /// Gets an enumeration of entity sets included in the container.
        /// </summary>
        public IEnumerable<EntitySet> EntitySets 
        { 
            get { return this.entitySetsList.AsEnumerable(); } 
        }

        /// <summary>
        /// Gets an enumeration of association sets included in the container.
        /// </summary>
        public IEnumerable<AssociationSet> AssociationSets 
        { 
            get { return this.associationSetsList.AsEnumerable(); } 
        }

        /// <summary>
        /// Gets an enumeration of function imports included in the container.
        /// </summary>
        public IEnumerable<FunctionImport> FunctionImports 
        { 
            get { return this.functionImportsList.AsEnumerable(); } 
        }

        /// <summary>
        /// Gets the model this container belongs to. If it is null, then the container has not been added to a model.
        /// </summary>
        public EntityModelSchema Model { get; internal set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.EntityContainer"/>.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator EntityContainer(string containerName)
        {
            return new EntityContainerReference(containerName);
        }

        /// <summary>
        /// Adds an <see cref="EntitySet"/> to <see cref="EntitySets" /> collection.
        /// </summary>
        /// <param name="entitySet">Entity set to add.</param>
        public void Add(EntitySet entitySet)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.Assert(entitySet.Container == null, "Entity set was already added to another container");
            entitySet.Container = this;
            this.entitySetsList.Add(entitySet);
        }

        /// <summary>
        /// Adds an <see cref="AssociationSet" /> to <see cref="AssociationSets" /> collection.
        /// </summary>
        /// <param name="associationSet">Association set to add.</param>
        public void Add(AssociationSet associationSet)
        {
            ExceptionUtilities.CheckArgumentNotNull(associationSet, "associationSet");
            ExceptionUtilities.Assert(associationSet.Container == null, "Association set was already added to another container");
            associationSet.Container = this;
            this.associationSetsList.Add(associationSet);
        }

        /// <summary>
        /// Adds an <see cref="FunctionImport"/> to <see cref="FunctionImports" /> collection.
        /// </summary>
        /// <param name="functionImport">FunctionImport to add.</param>
        public void Add(FunctionImport functionImport)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionImport, "functionImport");

            // NOTE: even though function import has an entity set property, it may not ever be set, so we need to record its container as well
            ExceptionUtilities.Assert(functionImport.Container == null, "Function import was already added to another container");
            functionImport.Container = this;
            this.functionImportsList.Add(functionImport);
        }

        /// <summary>
        /// Removes an <see cref="EntitySet"/> from <see cref="EntitySets" /> collection.
        /// </summary>
        /// <param name="entitySet">Entity set to remove.</param>
        public void Remove(EntitySet entitySet)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.Assert(entitySet.Container == this, "Entity set was not added to this container");
            ExceptionUtilities.Assert(this.entitySetsList.Remove(entitySet), "Entity set was not added to this model");
            entitySet.Container = null;
        }

        /// <summary>
        /// Removes an <see cref="AssociationSet" /> from <see cref="AssociationSets" /> collection.
        /// </summary>
        /// <param name="associationSet">Association set to remove.</param>
        public void Remove(AssociationSet associationSet)
        {
            ExceptionUtilities.CheckArgumentNotNull(associationSet, "associationSet");
            ExceptionUtilities.Assert(associationSet.Container == this, "Association set was not added to this container");
            ExceptionUtilities.Assert(this.associationSetsList.Remove(associationSet), "Association set was not added to this model");
            associationSet.Container = null;
        }

        /// <summary>
        /// Removes an <see cref="FunctionImport"/> from <see cref="FunctionImports" /> collection.
        /// </summary>
        /// <param name="functionImport">FunctionImport to remove.</param>
        public void Remove(FunctionImport functionImport)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionImport, "functionImport");
            ExceptionUtilities.Assert(functionImport.Container == this, "Function import set was not added to this container");
            ExceptionUtilities.Assert(this.functionImportsList.Remove(functionImport), "Function import set was not added to this model");
            functionImport.Container = null;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = 0x1234;

            if (this.Name != null)
            {
                hashCode ^= this.Name.GetHashCode();
            }

            if (this.NamespaceName != null)
            {
                hashCode ^= this.NamespaceName.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            INamedItem other = obj as INamedItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="INamedItem"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="INamedItem"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="INamedItem"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(INamedItem other)
        {
            var otherComplex = other as EntityContainer;
            if (otherComplex == null)
            {
                return false;
            }

            return (this.Name == otherComplex.Name) && (this.NamespaceName == otherComplex.NamespaceName);
        }
    }
}
