//---------------------------------------------------------------------
// <copyright file="RemoveObjectLayerOnlyTypesFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// A fixup applied to object models that ensures every <see cref="EntityType"/>
    /// with an <see cref="ObjectLayerOnlyAnnotation"/> has its members pushed to
    /// derived entity types and the rest of the references to it in the model are fixed up.
    /// </summary>
    public sealed class RemoveObjectLayerOnlyTypesFixup : IEntityModelFixup
    {
        /// <summary>
        /// Performs the fixup that ensures every <see cref="EntityType"/>
        /// with an <see cref="ObjectLayerOnlyAnnotation"/> has its members pushed to
        /// derived entity types and the rest of the references to it in the model are fixed up.
        /// </summary>
        /// <param name="model">Model to perform fixup on.</param>
        public void Fixup(EntityModelSchema model)
        {
            var graph = this.BuildGenericTypeResolutionGraph(model);
            foreach (var baseType in model.EntityTypes.Where(e => e.Annotations.OfType<ObjectLayerOnlyAnnotation>().Any() || e.Annotations.OfType<GenericTypeAnnotation>().Any()).ToArray())
            {
                var derivedTypes = model.EntityTypes.Where(e => e.BaseType == baseType);
                var oldEntitySets = model.EntityContainers.SelectMany(e => e.EntitySets).Where(e => e.EntityType == baseType).ToArray();
                List<AssociationSet> oldAssociationSetsToRemove = new List<AssociationSet>();

                foreach (var derivedType in derivedTypes)
                {
                    // Update entity sets (if any exist).
                    var oldNewMap = new Dictionary<EntitySet, EntitySet>();
                    foreach (var oldEntitySet in oldEntitySets)
                    {
                        // In case of MEST, just using the entity type's name would not yield a unique entity set name,
                        // so we have to append the old EntitySet's name, too.
                        var entitySet = oldNewMap[oldEntitySet] = new EntitySet(derivedType.Name + "_" + oldEntitySet.Name, derivedType);

                        oldEntitySet.Container.Add(entitySet);
                    }

                    this.PushPropertiesToDerivedType(model, graph, baseType, oldAssociationSetsToRemove, derivedType, oldNewMap);

                    derivedType.BaseType = null;
                }

                foreach (var oldEntitySet in oldEntitySets)
                {
                    oldEntitySet.Container.Remove(oldEntitySet);
                }

                foreach (var oldAssociationSet in oldAssociationSetsToRemove)
                {
                    // Sometimes oldAssociationSetsToRemove can contain duplicates if association sets
                    // are copied for multiple derived entity types. If they are already removed
                    // they will be disassociated with the container.
                    if (oldAssociationSet.Container != null)
                    {
                        oldAssociationSet.Container.Remove(oldAssociationSet);
                    }
                }

                // These have already been fixed up, except for association types without navigation properties,
                // which doesn't make any sense in an unmapped base type anyway.
                foreach (var associationType in model.Associations.Where(a => a.Ends.Any(e => e.EntityType == baseType)).ToArray())
                {
                    ValidateAssociationType(baseType, associationType);

                    model.Remove(associationType);
                }

                model.Remove(baseType);
            }

            foreach (var complexType in model.ComplexTypes.Where(c => c.Annotations.OfType<ObjectLayerOnlyAnnotation>().Any()).ToArray())
            {
                model.Remove(complexType);
            }
        }

        private static void ValidateAssociationType(EntityType baseType, AssociationType associationType)
        {
            foreach (var end in associationType.Ends.Where(e => e.EntityType != baseType))
            {
                var navigationProperty = end.FromNavigationProperty();
                if (navigationProperty != null)
                {
                    throw new TaupoArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Entity type {0} cannot be an object-layer-only type because it has incoming navigation property {1} from entity type {2}.",
                            baseType.Name,
                            navigationProperty.Name,
                            end.EntityType.Name));
                }
            }
        }

        private void PushPropertiesToDerivedType(EntityModelSchema model, GenericTypeLookupGraph graph, EntityType unmappedBaseType, List<AssociationSet> oldAssociationSetsToRemove, EntityType derivedType, Dictionary<EntitySet, EntitySet> oldNewMap)
        {
            // Push primitive properties into derived classes.
            foreach (var property in unmappedBaseType.AllProperties)
            {
                var newProperty = this.ResolveProperty(graph, property, derivedType);
                derivedType.Properties.Add(newProperty);
            }

            // Push navigation properties into derived classes and add a new AssociationType.
            foreach (var navigationProperty in unmappedBaseType.AllNavigationProperties)
            {
                var oldAssociationType = navigationProperty.Association;
                var oldThisEnd = navigationProperty.FromAssociationEnd;
                var oldOtherEnd = navigationProperty.ToAssociationEnd;

                var associationTypeEndLookup = new Dictionary<AssociationEnd, AssociationEnd>();
                var thisEnd = associationTypeEndLookup[oldThisEnd] = new AssociationEnd(derivedType.Name, derivedType, oldThisEnd.Multiplicity, oldThisEnd.DeleteBehavior);
                var otherEnd = associationTypeEndLookup[oldOtherEnd] = new AssociationEnd(oldOtherEnd.RoleName, oldOtherEnd.EntityType, oldOtherEnd.Multiplicity, oldOtherEnd.DeleteBehavior);

                // Assumes combination of the derived type name and the old association type name is unique.
                var associationType = new AssociationType(oldAssociationType.NamespaceName, derivedType.Name + "_" + oldAssociationType.Name)
                {
                    associationTypeEndLookup[oldThisEnd],
                    associationTypeEndLookup[oldOtherEnd]
                };

                if (oldAssociationType.ReferentialConstraint != null)
                {
                    associationType.ReferentialConstraint = new ReferentialConstraint()
                        .WithDependentProperties(associationTypeEndLookup[oldAssociationType.ReferentialConstraint.DependentAssociationEnd], oldAssociationType.ReferentialConstraint.DependentProperties.ToArray())
                        .ReferencesPrincipalProperties(associationTypeEndLookup[oldAssociationType.ReferentialConstraint.PrincipalAssociationEnd], oldAssociationType.ReferentialConstraint.PrincipalProperties.ToArray());
                }

                var newNavigationProperty = new NavigationProperty(navigationProperty.Name, associationType, thisEnd, otherEnd);
                derivedType.NavigationProperties.Add(newNavigationProperty);

                model.Add(associationType);

                // Update the association sets for this association type.
                var oldAssociationSets = model.EntityContainers.SelectMany(e => e.AssociationSets).Where(a => a.AssociationType == oldAssociationType).ToArray();
                foreach (var oldAssociationSet in oldAssociationSets)
                {
                    EntitySet firstEntitySet;
                    if (!oldNewMap.TryGetValue(oldAssociationSet.Ends[0].EntitySet, out firstEntitySet))
                    {
                        firstEntitySet = oldAssociationSet.Ends[0].EntitySet;
                    }

                    EntitySet secondEntitySet;
                    if (!oldNewMap.TryGetValue(oldAssociationSet.Ends[1].EntitySet, out secondEntitySet))
                    {
                        secondEntitySet = oldAssociationSet.Ends[1].EntitySet;
                    }

                    var associationSet = new AssociationSet(associationType.Name + "_" + oldAssociationSet.Name, associationType)
                    {
                        new AssociationSetEnd(associationTypeEndLookup[oldAssociationSet.Ends[0].AssociationEnd], firstEntitySet),
                        new AssociationSetEnd(associationTypeEndLookup[oldAssociationSet.Ends[1].AssociationEnd], secondEntitySet),
                    };

                    oldAssociationSet.Container.Add(associationSet);
                }

                // Can't remove these yet, because we have to wait until the original has been copied for all derived entity types.
                oldAssociationSetsToRemove.AddRange(oldAssociationSets);
            }
        }

        private GenericTypeLookupGraph BuildGenericTypeResolutionGraph(EntityModelSchema model)
        {
            var graph = new GenericTypeLookupGraph();

            foreach (var unmappedBaseType in model.EntityTypes.Where(e => (e.Annotations.OfType<ObjectLayerOnlyAnnotation>().Any() || e.Annotations.OfType<GenericTypeAnnotation>().Any()) && e.BaseType == null))
            {
                var genericTypeAnnotation = unmappedBaseType.Annotations.OfType<GenericTypeAnnotation>().FirstOrDefault();
                if (genericTypeAnnotation == null)
                {
                    continue;
                }

                int count = genericTypeAnnotation.TypeParameters.Count;
                for (int i = 0; i < count; ++i)
                {
                    var lookups = new Dictionary<EntityType, GenericArgument>();
                    var typeParameterName = genericTypeAnnotation.TypeParameters[i];
                    graph.Add(new GenericTypeParameter(unmappedBaseType, typeParameterName), lookups);

                    this.WalkChildren(graph, model, unmappedBaseType, typeParameterName, lookups);
                }
            }

            return graph;
        }

        private void WalkChildren(GenericTypeLookupGraph graph, EntityModelSchema model, EntityType baseType, string typeParameterName, Dictionary<EntityType, GenericArgument> lookups)
        {
            foreach (var childType in model.EntityTypes.Where(e => e.BaseType == baseType))
            {
                var genericTypeAnnotation = childType.Annotations.OfType<GenericTypeAnnotation>().FirstOrDefault();
                var genericArgumentsAnnotation = childType.Annotations.OfType<GenericArgumentsAnnotation>().FirstOrDefault();

                GenericArgument argument = null;
                bool argumentMissing = genericArgumentsAnnotation == null ||
                    (argument = genericArgumentsAnnotation.GenericArguments.SingleOrDefault(ga => ga.TypeParameterName == typeParameterName)) == null;
                if (argumentMissing)
                {
                    var exception = new TaupoArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Entity type {0} cannot derive from entity type {1} because {0} does not have a type parameter {2} specified by a {3}.",
                            childType.Name,
                            baseType.Name,
                            typeParameterName,
                            typeof(GenericTypeAnnotation).Name));

                    if (genericTypeAnnotation == null)
                    {
                        throw exception;
                    }

                    var childTypeParameterPosition = genericTypeAnnotation.TypeParameters.IndexOf(typeParameterName);

                    if (childTypeParameterPosition == -1)
                    {
                        throw exception;
                    }

                    var childLookups = new Dictionary<EntityType, GenericArgument>();
                    graph.Add(new GenericTypeParameter(childType, typeParameterName), childLookups);

                    this.WalkChildren(graph, model, childType, typeParameterName, childLookups);

                    // Merge the child lookups back into lookups
                    foreach (var kvp in childLookups)
                    {
                        lookups[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    lookups[childType] = argument;
                }
            }
        }

        private MemberProperty ResolveProperty(GenericTypeLookupGraph graph, MemberProperty property, EntityType newOwner)
        {
            GenericPropertyTypeAnnotation propertyTypeAnnotation = property.Annotations.OfType<GenericPropertyTypeAnnotation>().FirstOrDefault();
            if (propertyTypeAnnotation != null)
            {
                // Have to find where the property type comes from
                var parameter = propertyTypeAnnotation.GenericTypeParameterName;

                EntityType oldOwner = null;
                for (oldOwner = newOwner.BaseType; oldOwner != null; oldOwner = oldOwner.BaseType)
                {
                    if (oldOwner.Properties.Contains(property))
                    {
                        break;
                    }
                }

                Dictionary<EntityType, GenericArgument> lookups = graph[new GenericTypeParameter(oldOwner, parameter)];
                GenericArgument genericArgument = null;

                EntityType resolvingType = null;
                for (resolvingType = newOwner; resolvingType != null && !lookups.TryGetValue(resolvingType, out genericArgument); resolvingType = resolvingType.BaseType)
                {
                }

                // Means we found a concrete generic argument for this property type.
                if (resolvingType != null)
                {
                    return new MemberProperty(property.Name, genericArgument.DataType)
                    {
                        IsPrimaryKey = property.IsPrimaryKey
                    };
                }

                return new MemberProperty(property.Name)
                {
                    IsPrimaryKey = property.IsPrimaryKey,

                    Annotations = 
                    {
                        new GenericPropertyTypeAnnotation(parameter)
                    }
                };
            }

            return new MemberProperty(property.Name, property.PropertyType)
            {
                IsPrimaryKey = property.IsPrimaryKey
            };
        }

        /// <summary>
        /// A graph that tracks type parameters on base types to their actual resolved arguments
        /// on derived types.
        /// </summary>
        private sealed class GenericTypeLookupGraph : Dictionary<GenericTypeParameter, Dictionary<EntityType, GenericArgument>>
        {
        }

        /// <summary>
        /// A combination of an <see cref="EntityType"/> and another type parameter.
        /// </summary>
        private sealed class GenericTypeParameter : Tuple<EntityType, string>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GenericTypeParameter"/> class.
            /// </summary>
            /// <param name="entityType">The <see cref="EntityType"/></param>
            /// <param name="typeParameter">The type parameter.</param>
            public GenericTypeParameter(EntityType entityType, string typeParameter) :
                base(entityType, typeParameter)
            {
            }
        }
    }
}
