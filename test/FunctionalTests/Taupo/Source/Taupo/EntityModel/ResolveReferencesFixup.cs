//---------------------------------------------------------------------
// <copyright file="ResolveReferencesFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Performs type resolution and replaces <see cref="EntityTypeReference"/> and <see cref="ComplexTypeReference"/> with actual 
    /// types.
    /// </summary>
    public class ResolveReferencesFixup : IEntityModelFixup
    {
        /// <summary>
        /// Performs type resolution and replaces <see cref="EntityTypeReference"/> and <see cref="ComplexTypeReference"/> with actual 
        /// types.
        /// </summary>
        /// <param name="model">Model to fixup.</param>
        public void Fixup(EntityModelSchema model)
        {
            this.FixupEntityTypes(model, model.EntityTypes);
            this.FixupComplexTypes(model, model.ComplexTypes);
            this.FixupAssociationTypes(model, model.Associations);
            this.FixupFunctions(model, model.Functions);
            this.FixupEntityContainers(model, model.EntityContainers);
        }

        private void FixupEntityTypes(EntityModelSchema model, IEnumerable<EntityType> entityTypes)
        {
            foreach (EntityType type in entityTypes)
            {
                type.BaseType = this.ResolveEntityTypeReference(model, type.BaseType);

                foreach (MemberProperty prop in type.Properties)
                {
                    prop.PropertyType = this.ResolveReferencesInDataType(model, prop.PropertyType);
                }

                foreach (NavigationProperty nav in type.NavigationProperties)
                {
                    this.ResolveNavigationProperty(model, nav);
                }

                foreach (EdmUniqueConstraint edmUniqueConstraint in type.EdmUniqueConstraints)
                {
                    this.ResolvePropertyReferences(type, edmUniqueConstraint.Properties);
                }
            }
        }

        private void ResolveNavigationProperty(EntityModelSchema model, NavigationProperty nav)
        {
            nav.Association = this.ResolveAssociationTypeReference(model, nav.Association);

            nav.FromAssociationEnd = this.ResolveAssociationEndReference(nav.Association, nav.FromAssociationEnd);
            nav.ToAssociationEnd = this.ResolveAssociationEndReference(nav.Association, nav.ToAssociationEnd);
        }

        private AssociationEnd ResolveAssociationEndReference(AssociationType association, AssociationEnd associationEnd)
        {
            AssociationEndReference endRef = associationEnd as AssociationEndReference;
            if (endRef != null)
            {
                AssociationEnd end = association.Ends.SingleOrDefault(e => e.RoleName == endRef.RoleName);
                if (end == null)
                {
                    throw new TaupoInvalidOperationException("The association end with role name: '" + endRef.RoleName + "' was not found.");
                }

                return end;
            }
            else
            {
                return associationEnd;
            }
        }

        private void FixupComplexTypes(EntityModelSchema model, IEnumerable<ComplexType> complexTypes)
        {
            foreach (ComplexType type in complexTypes)
            {
                type.BaseType = this.ResolveComplexTypeReference(model, type.BaseType);

                foreach (MemberProperty prop in type.Properties)
                {
                    prop.PropertyType = this.ResolveReferencesInDataType(model, prop.PropertyType);
                }
            }
        }

        private void FixupAssociationTypes(EntityModelSchema model, IEnumerable<AssociationType> associations)
        {
            foreach (AssociationType association in associations)
            {
                foreach (AssociationEnd end in association.Ends)
                {
                    end.EntityType = this.ResolveEntityTypeReference(model, end.EntityType);
                }

                if (association.ReferentialConstraint != null)
                {
                    this.ResolveReferentialConstraint(association, association.ReferentialConstraint);
                }
            }
        }

        private void ResolveReferentialConstraint(AssociationType association, ReferentialConstraint constraint)
        {
            constraint.DependentAssociationEnd = this.ResolveAssociationEndReference(association, constraint.DependentAssociationEnd);
            if (constraint.DependentAssociationEnd != null)
            {
                this.ResolvePropertyReferences(constraint.DependentAssociationEnd.EntityType, constraint.DependentProperties);
            }

            constraint.PrincipalAssociationEnd = this.ResolveAssociationEndReference(association, constraint.PrincipalAssociationEnd);
            if (constraint.PrincipalAssociationEnd != null)
            {
                this.ResolvePropertyReferences(constraint.PrincipalAssociationEnd.EntityType, constraint.PrincipalProperties);
            }
        }

        private void ResolvePropertyReferences(EntityType entityType, IList<MemberProperty> properties)
        {
            for (int i = 0; i < properties.Count; ++i)
            {
                properties[i] = this.ResolveMemberPropertyReference(entityType, properties[i]);
            }
        }

        private MemberProperty ResolveMemberPropertyReference(EntityType type, MemberProperty prop)
        {
            MemberPropertyReference propRef = prop as MemberPropertyReference;
            if (propRef != null)
            {
                MemberProperty realProperty = type.AllProperties.SingleOrDefault(p => p.Name == propRef.Name);
                if (realProperty == null)
                {
                    throw new TaupoInvalidOperationException("Property " + propRef.Name + " was not found.");
                }

                return realProperty;
            }
            else
            {
                return prop;
            }
        }

        private void FixupEntityContainers(EntityModelSchema model, IEnumerable<EntityContainer> containers)
        {
            foreach (EntityContainer container in containers)
            {
                foreach (EntitySet eset in container.EntitySets)
                {
                    eset.EntityType = this.ResolveEntityTypeReference(model, eset.EntityType);
                }

                foreach (AssociationSet aset in container.AssociationSets)
                {
                    this.ResolveAssociationSet(model, container, aset);
                }

                foreach (FunctionImport functionImport in container.FunctionImports)
                {
                    foreach (var returnType in functionImport.ReturnTypes)
                    {
                        returnType.DataType = this.ResolveReferencesInDataType(model, returnType.DataType);
                        if (returnType.EntitySet != null)
                        {
                            returnType.EntitySet = this.ResolveEntitySetReference(container, returnType.EntitySet);
                        }
                    }

                    this.FixupFunctionParameters(model, functionImport.Parameters);
                }
            }
        }

        private void FixupFunctions(EntityModelSchema model, IEnumerable<Function> functions)
        {
            foreach (Function f in functions)
            {
                if (f.ReturnType != null)
                {
                    f.ReturnType = this.ResolveReferencesInDataType(model, f.ReturnType);
                }

                this.FixupFunctionParameters(model, f.Parameters);
            }
        }

        private void FixupFunctionParameters(EntityModelSchema model, IEnumerable<FunctionParameter> parameters)
        {
            foreach (FunctionParameter p in parameters)
            {
                p.DataType = this.ResolveReferencesInDataType(model, p.DataType);
            }
        }

        private DataType ResolveReferencesInDataType(EntityModelSchema model, DataType dataType)
        {
            if (dataType != null)
            {
                var visitor = new ResolveReferencesDataTypeVisitor(this, model);
                return visitor.ResolveReferencesIn(dataType);
            }

            return null;
        }

        private void ResolveAssociationSet(EntityModelSchema model, EntityContainer container, AssociationSet aset)
        {
            aset.AssociationType = this.ResolveAssociationTypeReference(model, aset.AssociationType);
            foreach (AssociationSetEnd end in aset.Ends)
            {
                this.ResolveAssociationSetEnd(container, aset, end);
            }
        }

        private void ResolveAssociationSetEnd(EntityContainer container, AssociationSet aset, AssociationSetEnd end)
        {
            end.EntitySet = this.ResolveEntitySetReference(container, end.EntitySet);
            end.AssociationEnd = this.ResolveAssociationEndReference(aset.AssociationType, end.AssociationEnd);
        }

        private EntitySet ResolveEntitySetReference(EntityContainer container, EntitySet entitySet)
        {
            EntitySetReference esr = entitySet as EntitySetReference;
            if (esr == null)
            {
                return entitySet;
            }

            var result = container.EntitySets.SingleOrDefault(s => s.Name == esr.Name);
            if (result == null)
            {
                throw new TaupoInvalidOperationException("The set: '" + esr.Name + "' was not found.");
            }

            return result;
        }

        private AssociationType ResolveAssociationTypeReference(EntityModelSchema model, AssociationType associationType)
        {
            AssociationTypeReference associationTypeRef = associationType as AssociationTypeReference;
            if (associationTypeRef == null)
            {
                return associationType;
            }

            var result = model.Associations.SingleOrDefault(c => c.Name == associationTypeRef.Name && (c.NamespaceName == associationTypeRef.NamespaceName || associationTypeRef.NamespaceName == null));
            if (result == null)
            {
                throw new TaupoInvalidOperationException("The type: '" + associationTypeRef.FullName + "' was not found.");
            }

            return result;
        }

        private ComplexType ResolveComplexTypeReference(EntityModelSchema model, ComplexType complexType)
        {
            ComplexTypeReference ctr = complexType as ComplexTypeReference;
            if (ctr == null)
            {
                return complexType;
            }

            var result = model.ComplexTypes.SingleOrDefault(c => c.Name == ctr.Name && (c.NamespaceName == ctr.NamespaceName || ctr.NamespaceName == null));
            if (result == null)
            {
                throw new TaupoInvalidOperationException("The type: '" + ctr.FullName + "' was not found.");
            }

            return result;
        }

        private EntityType ResolveEntityTypeReference(EntityModelSchema model, EntityType entityType)
        {
            EntityTypeReference etr = entityType as EntityTypeReference;
            if (etr == null)
            {
                return entityType;
            }

            var result = model.EntityTypes.SingleOrDefault(c => c.Name == etr.Name && (c.NamespaceName == etr.NamespaceName || etr.NamespaceName == null));
            if (result == null)
            {
                throw new TaupoInvalidOperationException("The type: '" + etr.FullName + "' was not found.");
            }

            return result;
        }

        private EnumType ResolveEnumTypeReference(EntityModelSchema model, EnumType enumType)
        {
            EnumTypeReference etr = enumType as EnumTypeReference;
            if (etr == null)
            {
                return enumType;
            }

            var result = model.EnumTypes.SingleOrDefault(c => c.Name == etr.Name && (c.NamespaceName == etr.NamespaceName || etr.NamespaceName == null));
            if (result == null)
            {
                throw new TaupoInvalidOperationException("The type: '" + etr.FullName + "' was not found.");
            }

            return result;
        }

        /// <summary>
        /// Visitor to resolve all references inside a data type
        /// </summary>
        private class ResolveReferencesDataTypeVisitor : IDataTypeVisitor<DataType>
        {
            private ResolveReferencesFixup parent;
            private EntityModelSchema model;

            /// <summary>
            /// Initializes a new instance of the ResolveReferencesDataTypeVisitor class.
            /// </summary>
            /// <param name="parent">The parent resolve reference fixup</param>
            /// <param name="model">the entity schema model</param>
            public ResolveReferencesDataTypeVisitor(ResolveReferencesFixup parent, EntityModelSchema model)
            {
                this.parent = parent;
                this.model = model;
            }

            /// <summary>
            /// Resolved all references inside a given data type
            /// </summary>
            /// <param name="dataType">The given data type</param>
            /// <returns>The data type with all references resolved</returns>
            public DataType ResolveReferencesIn(DataType dataType)
            {
                DataType resolvedType = dataType.Accept(this);
                if (resolvedType != dataType)
                {
                    this.CopyDataTypeAnnotations(dataType, resolvedType);
                }

                return resolvedType;
            }

            /// <summary>
            /// Visits the specified collection type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the data type with all references resolved</returns>
            public DataType Visit(CollectionDataType dataType)
            {
                return DataTypes.CollectionType
                                .Nullable(dataType.IsNullable)
                                .WithElementDataType(this.ResolveReferencesIn(dataType.ElementDataType));
            }

            /// <summary>
            /// Visits the specified complex type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the data type with all references resolved</returns>
            public DataType Visit(ComplexDataType dataType)
            {
                ComplexType resolved = this.parent.ResolveComplexTypeReference(this.model, dataType.Definition);
                return DataTypes.ComplexType
                                .Nullable(dataType.IsNullable)
                                .WithDefinition(resolved);
            }

            /// <summary>
            /// Visits the specified entity type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the data type with all references resolved</returns>
            public DataType Visit(EntityDataType dataType)
            {
                EntityType resolved = this.parent.ResolveEntityTypeReference(this.model, dataType.Definition);
                return DataTypes.EntityType
                                .Nullable(dataType.IsNullable)
                                .WithDefinition(resolved);
            }

            /// <summary>
            /// Visits the specified primitive type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the data type with all references resolved</returns>
            public DataType Visit(PrimitiveDataType dataType)
            {
                EnumDataType enumDataType = dataType as EnumDataType;
                if (enumDataType != null)
                {
                    EnumType resolved = this.parent.ResolveEnumTypeReference(this.model, enumDataType.Definition);
                    return enumDataType.WithDefinition(resolved);
                }
                else
                {
                    return dataType;
                }
            }

            /// <summary>
            /// Visits the specified reference type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the data type with all references resolved</returns>
            public DataType Visit(ReferenceDataType dataType)
            {
                return DataTypes.ReferenceType
                                .Nullable(dataType.IsNullable)
                                .WithEntityType(this.parent.ResolveEntityTypeReference(this.model, dataType.EntityType));
            }
            
            private void CopyDataTypeAnnotations(DataType fromDataType, DataType toDataType)
            {
                foreach (var annotation in fromDataType.Annotations)
                {
                    toDataType.SetAnnotation(annotation);
                }
            }
        }
    }
}
