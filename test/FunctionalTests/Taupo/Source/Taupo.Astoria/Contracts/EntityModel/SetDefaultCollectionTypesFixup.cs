//---------------------------------------------------------------------
// <copyright file="SetDefaultCollectionTypesFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    
    /// <summary>
    /// Fixup for adding default contract and instance types to all collection properties
    /// </summary>
    public class SetDefaultCollectionTypesFixup : IEntityModelFixup
    {
        /// <summary>
        /// Initializes a new instance of the SetDefaultCollectionTypesFixup class
        /// </summary>
        public SetDefaultCollectionTypesFixup()
            : this("System.Collections.Generic.List", "System.Collections.Generic.List", true, true)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the SetDefaultCollectionTypesFixup class
        /// </summary>
        /// <param name="fullContractTypeName">The full name of the contract type</param>
        /// <param name="fullInstanceTypeName">The full name of the instance type</param>
        /// <param name="isContractGeneric">Whether or not the contract type is generic</param>
        /// <param name="isInstanceGeneric">Whether or not the instance type is generic</param>
        public SetDefaultCollectionTypesFixup(string fullContractTypeName, string fullInstanceTypeName, bool isContractGeneric, bool isInstanceGeneric)
        {
            this.FullContractTypeName = fullContractTypeName;
            this.FullInstanceTypeName = fullInstanceTypeName;
            this.IsContractGeneric = isContractGeneric;
            this.IsInstanceGeneric = isInstanceGeneric;
        }

        /// <summary>
        /// Gets the full name of the contract type
        /// </summary>
        internal string FullContractTypeName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the contract type is generic
        /// </summary>
        internal bool IsContractGeneric { get; private set; }

        /// <summary>
        /// Gets the full name of the instance type
        /// </summary>
        internal string FullInstanceTypeName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the instance type is generic
        /// </summary>
        internal bool IsInstanceGeneric { get; private set; }

        /// <summary>
        /// Adds contract and instance annotations to all collection properties in the model
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            this.ApplyEntityTypeCollectionTypes(model);

            this.ApplyComplexTypeCollectionTypes(model);

            this.ApplyFunctionCollectionTypes(model);
        }

        /// <summary>
        /// Constructs a new contract type annotation
        /// </summary>
        /// <returns>A new contract type annotation</returns>
        protected virtual CollectionContractTypeAnnotation CreateContractTypeAnnotation()
        {
            return new CollectionContractTypeAnnotation() { FullTypeName = this.FullContractTypeName, IsGeneric = this.IsContractGeneric };
        }

        /// <summary>
        /// Constructs a new instance type annotation
        /// </summary>
        /// <returns>A new instance type annotation</returns>
        protected virtual CollectionInstanceTypeAnnotation CreateInstanceTypeAnnotation()
        {
            return new CollectionInstanceTypeAnnotation() { FullTypeName = this.FullInstanceTypeName, IsGeneric = this.IsInstanceGeneric };
        }

        private void ApplyEntityTypeCollectionTypes(EntityModelSchema model)
        {
            foreach (var entityType in model.EntityTypes)
            {
                foreach (var property in entityType.Properties.Where(p => p.PropertyType is CollectionDataType))
                {
                    if (!property.Annotations.OfType<CollectionContractTypeAnnotation>().Any())
                    {
                        property.Annotations.Add(this.CreateContractTypeAnnotation());
                    }

                    if (!property.Annotations.OfType<CollectionInstanceTypeAnnotation>().Any())
                    {
                        property.Annotations.Add(this.CreateInstanceTypeAnnotation());
                    }
                }

                foreach (var navigation in entityType.NavigationProperties.Where(p => p.ToAssociationEnd.Multiplicity == EndMultiplicity.Many))
                {
                    if (!navigation.Annotations.OfType<CollectionContractTypeAnnotation>().Any())
                    {
                        navigation.Annotations.Add(this.CreateContractTypeAnnotation());
                    }

                    if (!navigation.Annotations.OfType<CollectionInstanceTypeAnnotation>().Any())
                    {
                        navigation.Annotations.Add(this.CreateInstanceTypeAnnotation());
                    }
                }
            }
        }

        private void ApplyComplexTypeCollectionTypes(EntityModelSchema model)
        {
            foreach (var complexType in model.ComplexTypes)
            {
                foreach (var property in complexType.Properties.Where(p => p.PropertyType is CollectionDataType))
                {
                    if (!property.Annotations.OfType<CollectionContractTypeAnnotation>().Any())
                    {
                        property.Annotations.Add(this.CreateContractTypeAnnotation());
                    }

                    if (!property.Annotations.OfType<CollectionInstanceTypeAnnotation>().Any())
                    {
                        property.Annotations.Add(this.CreateInstanceTypeAnnotation());
                    }
                }
            }
        }

        private void ApplyFunctionCollectionTypes(EntityModelSchema model)
        {
            foreach (var function in model.Functions)
            {
                foreach (var parameter in function.Parameters)
                {
                    if (!parameter.Annotations.OfType<CollectionContractTypeAnnotation>().Any())
                    {
                        parameter.Annotations.Add(this.CreateContractTypeAnnotation());
                    }
                }

                if (function.ReturnType is CollectionDataType)
                {
                    if (!function.Annotations.OfType<CollectionContractTypeAnnotation>().Any())
                    {
                        function.Annotations.Add(this.CreateContractTypeAnnotation());
                    }
                }
            }
        }
    }
}