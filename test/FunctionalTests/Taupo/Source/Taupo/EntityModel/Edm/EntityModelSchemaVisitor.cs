//---------------------------------------------------------------------
// <copyright file="EntityModelSchemaVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Edm
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Traverses the entire Taupo model
    /// Override appropriate methods to give meaningful behavior
    /// </summary>
    public abstract class EntityModelSchemaVisitor
    {
        /// <summary>
        /// Visits a model
        /// </summary>
        /// <param name="model">model to visit</param>
        public void Visit(EntityModelSchema model)
        {
            foreach (var entity in model.EntityTypes)
            {
                this.VisitEntityType(entity);
            }

            foreach (var complex in model.ComplexTypes)
            {
                this.VisitComplexType(complex);
            }

            foreach (var association in model.Associations)
            {
                this.VisitAssociationType(association);
            }

            foreach (var container in model.EntityContainers)
            {
                this.VisitEntityContainer(container);
            }

            foreach (var function in model.Functions)
            {
                this.VisitFunction(function);
            }

            foreach (var enumType in model.EnumTypes)
            {
                this.VisitEnumType(enumType);
            }
        }

        /// <summary>
        /// Visits an entity
        /// </summary>
        /// <param name="entity">entity to visit</param>
        protected virtual void VisitEntityType(EntityType entity)
        {
            this.VisitAnnotatedItem(entity);

            foreach (var property in entity.Properties)
            {
                this.VisitMemberProperty(property);
            }

            foreach (var navigationProperty in entity.NavigationProperties)
            {
                this.VisitNavigationProperty(navigationProperty);
            }
        }

        /// <summary>
        /// Visits an annotated item
        /// </summary>
        /// <param name="item">item to visit</param>
        protected virtual void VisitAnnotatedItem(AnnotatedItem item)
        {
            // do nothing
        }

        /// <summary>
        /// Visits member property
        /// </summary>
        /// <param name="memberProperty">member property to visit</param>
        protected virtual void VisitMemberProperty(MemberProperty memberProperty)
        {
            this.VisitAnnotatedItem(memberProperty);
        }

        /// <summary>
        /// Visits navigation property
        /// </summary>
        /// <param name="navigationProperty">navigation property to visit</param>
        protected virtual void VisitNavigationProperty(NavigationProperty navigationProperty)
        {
            this.VisitAnnotatedItem(navigationProperty);
        }

        /// <summary>
        /// Visits complex type
        /// </summary>
        /// <param name="complex">complex type to visit</param>
        protected virtual void VisitComplexType(ComplexType complex)
        {
            this.VisitAnnotatedItem(complex);

            foreach (var property in complex.Properties)
            {
                this.VisitMemberProperty(property);
            }
        }

        /// <summary>
        /// Visits association type
        /// </summary>
        /// <param name="association">association type to visit</param>
        protected virtual void VisitAssociationType(AssociationType association)
        {
            this.VisitAnnotatedItem(association);

            foreach (var associationEnd in association.Ends)
            {
                this.VisitAssociationEnd(associationEnd);
            }

            if (association.ReferentialConstraint != null)
            {
                this.VisitReferentialConstraint(association.ReferentialConstraint);
            }
        }

        /// <summary>
        /// Visit association end
        /// </summary>
        /// <param name="associationEnd">association end to visit</param>
        protected virtual void VisitAssociationEnd(AssociationEnd associationEnd)
        {
            this.VisitAnnotatedItem(associationEnd);
        }

        /// <summary>
        /// Visits referential constraint
        /// </summary>
        /// <param name="referentialConstraint">referential constraint to visit</param>
        protected virtual void VisitReferentialConstraint(ReferentialConstraint referentialConstraint)
        {
            this.VisitAnnotatedItem(referentialConstraint);
        }

        /// <summary>
        /// Visits entity container
        /// </summary>
        /// <param name="container">entity container to visit</param>
        protected virtual void VisitEntityContainer(EntityContainer container)
        {
            this.VisitAnnotatedItem(container);

            foreach (var entitySet in container.EntitySets)
            {
                this.VisitEntitySet(entitySet);
            }

            foreach (var associationSet in container.AssociationSets)
            {
                this.VisitAssociationSet(associationSet);
            }

            foreach (var functionImport in container.FunctionImports)
            {
                this.VisitFunctionImport(functionImport);
            }
        }

        /// <summary>
        /// Visit entity set
        /// </summary>
        /// <param name="entitySet">entity set to visit</param>
        protected virtual void VisitEntitySet(EntitySet entitySet)
        {
            this.VisitAnnotatedItem(entitySet);
        }

        /// <summary>
        /// Visit association set
        /// </summary>
        /// <param name="associationSet">association set to visit</param>
        protected virtual void VisitAssociationSet(AssociationSet associationSet)
        {
            this.VisitAnnotatedItem(associationSet);

            foreach (var associationSetEnd in associationSet.Ends)
            {
                this.VisitAssociationSetEnd(associationSetEnd);
            }
        }

        /// <summary>
        /// Visit association set end
        /// </summary>
        /// <param name="associationSetEnd">association set end to visit</param>
        protected virtual void VisitAssociationSetEnd(AssociationSetEnd associationSetEnd)
        {
            this.VisitAnnotatedItem(associationSetEnd);
        }

        /// <summary>
        /// Visit function import
        /// </summary>
        /// <param name="functionImport">function import to visit</param>
        protected virtual void VisitFunctionImport(FunctionImport functionImport)
        {
            this.VisitAnnotatedItem(functionImport);

            foreach (var parameter in functionImport.Parameters)
            {
                this.VisitFunctionParameter(parameter);
            }
        }

        /// <summary>
        /// Visit function parameter
        /// </summary>
        /// <param name="parameter">function parameter to visit</param>
        protected virtual void VisitFunctionParameter(FunctionParameter parameter)
        {
            this.VisitAnnotatedItem(parameter);
        }

        /// <summary>
        /// Visit function
        /// </summary>
        /// <param name="function">function to visit</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Function", Justification = "Seems like the most appropriate name")]
        protected virtual void VisitFunction(Function function)
        {
            this.VisitAnnotatedItem(function);

            foreach (var parameter in function.Parameters)
            {
                this.VisitFunctionParameter(parameter);
            }
        }

        /// <summary>
        /// Visit Enum type
        /// </summary>
        /// <param name="enumType">enum type to visit</param>
        protected virtual void VisitEnumType(EnumType enumType)
        {
            this.VisitAnnotatedItem(enumType);

            foreach (var enumMember in enumType.Members)
            {
                this.VisitEnumMember(enumMember);
            }
        }

        /// <summary>
        /// Visit Enum member
        /// </summary>
        /// <param name="enumMember">enum member to visit</param>
        protected virtual void VisitEnumMember(EnumMember enumMember)
        {
            this.VisitAnnotatedItem(enumMember);
        }
    }
}
