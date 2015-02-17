//---------------------------------------------------------------------
// <copyright file="EntityModelSchema.cs" company="Microsoft">
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
    /// Represents a mutable Entity Model (based on EDM with extensions)
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class EntityModelSchema : AnnotatedItem
    {
        private IList<EntityContainer> entityContainersList = new List<EntityContainer>();
        private IList<EntityType> entityTypesList = new List<EntityType>();
        private IList<ComplexType> complexTypesList = new List<ComplexType>();
        private IList<AssociationType> associationsList = new List<AssociationType>();
        private IList<Function> functionsList = new List<Function>();
        private IList<EnumType> enumTypesList = new List<EnumType>();

        /// <summary>
        /// Initializes a new instance of the EntityModelSchema class
        /// </summary>
        public EntityModelSchema()
        {
        }

        /// <summary>
        /// Gets an enumeration of entity containers in the model.
        /// </summary>
        public IEnumerable<EntityContainer> EntityContainers 
        { 
            get { return this.entityContainersList.AsEnumerable(); } 
        }

        /// <summary>
        /// Gets an enumeration of entity types in the model.
        /// </summary>
        public IEnumerable<EntityType> EntityTypes 
        { 
            get { return this.entityTypesList.AsEnumerable(); }
        }

        /// <summary>
        /// Gets an enumeration of complex types in the model.
        /// </summary>
        public IEnumerable<ComplexType> ComplexTypes 
        { 
            get { return this.complexTypesList.AsEnumerable(); } 
        }

        /// <summary>
        /// Gets an enumeration of Associations in the model.
        /// </summary>
        public IEnumerable<AssociationType> Associations 
        {
            get { return this.associationsList.AsEnumerable(); } 
        }

        /// <summary>
        /// Gets an enumeration of Functions in the model.
        /// </summary>
        public IEnumerable<Function> Functions 
        { 
            get { return this.functionsList.AsEnumerable(); } 
        }

        /// <summary>
        /// Gets an enumeration of Enum types in the model
        /// </summary>
        public IEnumerable<EnumType> EnumTypes 
        { 
            get { return this.enumTypesList.AsEnumerable(); }
        }

        /// <summary>
        /// Adds new <see cref="EntityType"/> to the model.
        /// </summary>
        /// <param name="entityType">Entity type to be added.</param>
        public void Add(EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.Assert(entityType.Model == null, "Entity type was already added to another model");
            entityType.Model = this;
            this.entityTypesList.Add(entityType);
        }

        /// <summary>
        /// Adds new <see cref="ComplexType"/> to the model.
        /// </summary>
        /// <param name="complexType">Complex types to be added.</param>
        public void Add(ComplexType complexType)
        {
            ExceptionUtilities.CheckArgumentNotNull(complexType, "complexType");
            ExceptionUtilities.Assert(complexType.Model == null, "Complex type was already added to another model");
            complexType.Model = this;
            this.complexTypesList.Add(complexType);
        }

        /// <summary>
        /// Adds new <see cref="EntityContainer"/> to the model.
        /// </summary>
        /// <param name="entityContainer">Container to be added.</param>
        public void Add(EntityContainer entityContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityContainer, "entityContainer");
            ExceptionUtilities.Assert(entityContainer.Model == null, "Entity container was already added to another model");
            entityContainer.Model = this;
            this.entityContainersList.Add(entityContainer);
        }

        /// <summary>
        /// Adds new <see cref="AssociationType"/> to the model.
        /// </summary>
        /// <param name="associationType">Associatin type to be added</param>
        public void Add(AssociationType associationType)
        {
            ExceptionUtilities.CheckArgumentNotNull(associationType, "associationType");
            ExceptionUtilities.Assert(associationType.Model == null, "Association type was already added to another model");
            associationType.Model = this;
            this.associationsList.Add(associationType);
        }

        /// <summary>
        /// Adds new <see cref="Function"/> to the model.
        /// </summary>
        /// <param name="function">Function to be added</param>
        public void Add(Function function)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            ExceptionUtilities.Assert(function.Model == null, "Function was already added to another model");
            function.Model = this;
            this.functionsList.Add(function);
        }

        /// <summary>
        /// Adds new <see cref="EnumType"/> to the model
        /// </summary>
        /// <param name="enumType">Enum type to be added</param>
        public void Add(EnumType enumType)
        {
            ExceptionUtilities.CheckArgumentNotNull(enumType, "enumType");
            ExceptionUtilities.Assert(enumType.Model == null, "Enum type was already added to another model");
            enumType.Model = this;
            this.enumTypesList.Add(enumType);
        }

        /// <summary>
        /// Removes an <see cref="EntityType"/> from the model.
        /// </summary>
        /// <param name="entityType">Entity type to be removed.</param>
        public void Remove(EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.Assert(entityType.Model == this, "Entity type was not added to this model");
            ExceptionUtilities.Assert(this.entityTypesList.Remove(entityType), "Entity type was not added to this model");
            entityType.Model = null;
        }

        /// <summary>
        /// Removes a <see cref="ComplexType"/> from the model.
        /// </summary>
        /// <param name="complexType">Complex types to be removed.</param>
        public void Remove(ComplexType complexType)
        {
            ExceptionUtilities.CheckArgumentNotNull(complexType, "complexType");
            ExceptionUtilities.Assert(complexType.Model == this, "Complex type was not added to this model");
            ExceptionUtilities.Assert(this.complexTypesList.Remove(complexType), "Complex type was not added to this model");
            complexType.Model = null;
        }

        /// <summary>
        /// Removes an <see cref="EntityContainer"/> from the model.
        /// </summary>
        /// <param name="entityContainer">Container to be removed.</param>
        public void Remove(EntityContainer entityContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityContainer, "entityContainer");
            ExceptionUtilities.Assert(entityContainer.Model == this, "Entity container was not added to this model");
            ExceptionUtilities.Assert(this.entityContainersList.Remove(entityContainer), "Entity container was not added to this model");
            entityContainer.Model = null;
        }

        /// <summary>
        /// Removes an <see cref="AssociationType"/> from the model.
        /// </summary>
        /// <param name="associationType">Associatin type to be removed</param>
        public void Remove(AssociationType associationType)
        {
            ExceptionUtilities.CheckArgumentNotNull(associationType, "associationType");
            ExceptionUtilities.Assert(associationType.Model == this, "Association type was not added to this model");
            ExceptionUtilities.Assert(this.associationsList.Remove(associationType), "Association type was not added to this model");
            associationType.Model = null;
        }

        /// <summary>
        /// Removes a <see cref="Function"/> from the model.
        /// </summary>
        /// <param name="function">Function to be removed</param>
        public void Remove(Function function)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            ExceptionUtilities.Assert(function.Model == this, "Function was not added to this model");
            ExceptionUtilities.Assert(this.functionsList.Remove(function), "Function was not added to this model");
            function.Model = null;
        }

        /// <summary>
        /// Removes an <see cref="EnumType"/> from the model
        /// </summary>
        /// <param name="enumType">Enum type to be removed</param>
        public void Remove(EnumType enumType)
        {
            ExceptionUtilities.CheckArgumentNotNull(enumType, "enumType");
            ExceptionUtilities.Assert(enumType.Model == this, "Enum type was not added to this model");
            ExceptionUtilities.Assert(this.enumTypesList.Remove(enumType), "Enum type was not added to this model");
            enumType.Model = null;
        }
    }
}
