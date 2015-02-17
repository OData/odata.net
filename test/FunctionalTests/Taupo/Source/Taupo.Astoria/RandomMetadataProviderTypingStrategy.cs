//---------------------------------------------------------------------
// <copyright file="RandomMetadataProviderTypingStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Metadata provider typing strategy that randomly makes some types open and some weakly vs strongly type-backed.
    /// </summary>
    [ImplementationName(typeof(IMetadataProviderTypingStrategy), "Random")]
    public class RandomMetadataProviderTypingStrategy : IMetadataProviderTypingStrategy, IEntityModelFixup
    {
        /// <summary>
        /// Gets or sets the random number generator to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Gets a fixup that changes the model's metadata typing annotations randomly
        /// </summary>
        /// <returns>The model fixup</returns>
        public IEntityModelFixup GetModelFixup()
        {
            return this;
        }

        /// <summary>
        /// Changes the model's metadata typing annotations randomly
        /// </summary>
        /// <param name="model">The model to fixup</param>
        public void Fixup(EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.SetupOpenTypes(model);

            this.SetupTypeBackedTypes(model);

            this.SetupLazyLoadingTypes(model);

            foreach (var entityType in model.EntityTypes)
            {
                this.AddPropertyAnnotations(entityType);
            }

            foreach (var complexType in model.ComplexTypes)
            {
                this.AddPropertyAnnotations(complexType);
            }

            this.CombineEquivalentProperties(model);
        }

        private void SetupOpenTypes(EntityModelSchema model)
        {
            foreach (var entityType in model.EntityTypes.Where(t => t.BaseType == null))
            {
                var derivedTypes = model.EntityTypes.Where(t => t.IsKindOf(entityType)).ToList();
                if (this.Random.Next(2) == 0)
                {
                    var openType = this.Random.ChooseFrom(derivedTypes);
                    derivedTypes.Where(t => t.IsKindOf(openType)).ForEach(t => t.IsOpen = true);
                }
            }
        }

        private void SetupTypeBackedTypes(EntityModelSchema model)
        {
            foreach (var type in model.ComplexTypes.Where(t => t.BaseType == null))
            {
                var derivedTypes = model.ComplexTypes.Where(t => t.IsKindOf(type)).ToList();
                if (this.Random.Next(2) == 0)
                {
                    var backedType = this.Random.ChooseFrom(derivedTypes);
                    derivedTypes.Where(t => t.IsKindOf(backedType)).ForEach(t => t.MakeTypeBacked());
                }
            }

            foreach (var type in model.EntityTypes.Where(t => t.BaseType == null))
            {
                var derivedTypes = model.EntityTypes.Where(t => t.IsKindOf(type)).ToList();
                if (this.Random.Next(2) == 0)
                {
                    var backedType = this.Random.ChooseFrom(derivedTypes);
                    derivedTypes.Where(t => t.IsKindOf(backedType)).ForEach(t => t.MakeTypeBacked());
                }
            }
        }

        private void SetupLazyLoadingTypes(EntityModelSchema model)
        {
            foreach (var type in model.EntityTypes.Cast<NamedStructuralType>().Concat(model.ComplexTypes.Cast<NamedStructuralType>()))
            {
                if (this.Random.Next(2) == 0)
                {
                    type.MakeLazyLoadedType();
                }
            }
        }

        private void AddPropertyAnnotations(EntityType entityType)
        {
            entityType.MakeAllRequiredPropertiesMetadataDeclared();
            this.MarkTypeBackedProperties(entityType);
        }
        
        private void MarkTypeBackedProperties(EntityType entityType)
        {
            if (!entityType.IsTypeBacked())
            {
                return;
            }

            foreach (var property in entityType.Properties.Where(p => p.IsMetadataDeclaredProperty()))
            {
                if (this.Random.Next(2) == 0)
                {
                    property.MakeTypeBacked();
                }
            }

            foreach (var property in entityType.NavigationProperties)
            {
                if (this.Random.Next(2) == 0)
                {
                    property.MakeTypeBacked();
                }
            }
        }

        private void AddPropertyAnnotations(ComplexType complexType)
        {
            complexType.Properties.ForEach(p => p.MakeMetadataDeclared());

            if (complexType.IsTypeBacked())
            {
                foreach (var property in complexType.Properties)
                {
                    if (this.Random.Next(2) == 0)
                    {
                        property.MakeTypeBacked();
                    }
                }
            }
        }

        private void CombineEquivalentProperties(EntityModelSchema model)
        {
            var propertyDictionary = new Dictionary<MemberProperty, Guid>(
                new DelegateBasedEqualityComparer<MemberProperty>(
                (p1, p2) =>
                {
                    ExceptionUtilities.Assert(p1.IsMetadataDeclaredProperty(), "Property '{0}' was not metadata declared", p1.Name);
                    ExceptionUtilities.Assert(p2.IsMetadataDeclaredProperty(), "Property '{0}' was not metadata declared", p2.Name);

                    if (p1.Name != p2.Name)
                    {
                        return false;
                    }

                    if (!p1.PropertyType.Equals(p2.PropertyType))
                    {
                        return false;
                    }

                    if (p1.IsPrimaryKey != p2.IsPrimaryKey)
                    {
                        return false;
                    }

                    if (p1.Annotations.OfType<ConcurrencyTokenAnnotation>().Any() != p2.Annotations.OfType<ConcurrencyTokenAnnotation>().Any())
                    {
                        return false;
                    }

                    if (p1.Annotations.OfType<MimeTypeAnnotation>().Any() != p2.Annotations.OfType<MimeTypeAnnotation>().Any())
                    {
                        return false;
                    }

                    if (p1.IsTypeBacked() != p2.IsTypeBacked())
                    {
                        return false;
                    }

                    return true;
                },
                p =>
                {
                    ExceptionUtilities.Assert(p.IsMetadataDeclaredProperty(), "Property '{0}' was not metadata declared", p.Name);
                    return (p.Name + p.PropertyType).GetHashCode();
                }));

            foreach (var memberProperty in model.EntityTypes.SelectMany(t => t.Properties).Concat(model.ComplexTypes.SelectMany(t => t.Properties)))
            {
                if (!memberProperty.IsMetadataDeclaredProperty())
                {
                    continue;
                }

                Guid linkGuid;
                if (!propertyDictionary.TryGetValue(memberProperty, out linkGuid))
                {
                    propertyDictionary[memberProperty] = linkGuid = Guid.NewGuid();
                }

                memberProperty.Annotations.Add(new DictionaryResourcePropertyInstanceLink() { UniqueId = linkGuid });
            }
        }
    }
}