//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlReferentialConstraint.
    /// </summary>
    internal class CsdlSemanticsReferentialConstraint : CsdlSemanticsElement, IEdmReferentialConstraint, IEdmCheckable
    {
        private readonly CsdlSemanticsAssociation context;
        private readonly CsdlReferentialConstraint constraint;

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEdmAssociationEnd> principalCache = new Cache<CsdlSemanticsReferentialConstraint, IEdmAssociationEnd>();
        private readonly static Func<CsdlSemanticsReferentialConstraint, IEdmAssociationEnd> s_computePrincipal = (me) => me.ComputePrincipal();

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEnumerable<IEdmStructuralProperty>> dependentPropertiesCache = new Cache<CsdlSemanticsReferentialConstraint, IEnumerable<IEdmStructuralProperty>>();
        private readonly static Func<CsdlSemanticsReferentialConstraint, IEnumerable<IEdmStructuralProperty>> s_computeDependentProperties = (me) => me.ComputeDependentProperties();

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsReferentialConstraint, IEnumerable<EdmError>>();
        private readonly static Func<CsdlSemanticsReferentialConstraint, IEnumerable<EdmError>> s_computeErrors = (me) => me.ComputeErrors();

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEnumerable<string>> principalKeyPropertiesNotFoundInPrincipalPropertiesCache = new Cache<CsdlSemanticsReferentialConstraint, IEnumerable<string>>();
        private readonly static Func<CsdlSemanticsReferentialConstraint, IEnumerable<string>> s_computePrincipalKeyPropertiesNotFoundInPrincipalProperties = (me) => me.ComputePrincipalKeyPropertiesNotFoundInPrincipalProperties();

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEnumerable<string>> dependentPropertiesNotFoundInDependentTypeCache = new Cache<CsdlSemanticsReferentialConstraint, IEnumerable<string>>();
        private readonly static Func<CsdlSemanticsReferentialConstraint, IEnumerable<string>> s_computeDependentPropertiesNotFoundInDependentType = (me) => me.ComputeDependentPropertiesNotFoundInDependentType();

        public CsdlSemanticsReferentialConstraint(CsdlSemanticsAssociation context, CsdlReferentialConstraint constraint)
        {
            this.context = context;
            this.constraint = constraint;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.constraint; }
        }

        public IEdmAssociationEnd PrincipalEnd
        {
            get { return this.principalCache.GetValue(this, s_computePrincipal, null); }
        }

        public IEnumerable<IEdmStructuralProperty> DependentProperties
        {
            get { return this.dependentPropertiesCache.GetValue(this, s_computeDependentProperties, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, s_computeErrors, null); }
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            List<EdmError> errors = null;
            IEdmEntityType principalRoleType = this.PrincipalEnd.EntityType;
            CsdlReferentialConstraintRole principalEnd = this.constraint.Principal;
            CsdlReferentialConstraintRole dependentEnd = this.constraint.Dependent;

            if (this.constraint.Principal.Role == this.constraint.Dependent.Role)
            {
                errors = AllocateAndAdd(errors, new EdmError(this.Location, EdmErrorCode.SameRoleReferredInReferentialConstraint, Edm.Strings.EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint(this.constraint.Principal.Role)));
            }

            if (this.constraint.Dependent.Role != this.context.End1.Name && this.constraint.Dependent.Role != this.context.End2.Name)
            {
                errors = AllocateAndAdd(errors, new EdmError(this.Location, EdmErrorCode.InvalidRoleInRelationshipConstraint, Edm.Strings.CsdlParser_InvalidEndRoleInRelationshipConstraint(this.constraint.Dependent.Role, this.context.Name)));
            }

            if (this.constraint.Principal.Role != this.context.End1.Name && this.constraint.Principal.Role != this.context.End2.Name)
            {
                errors = AllocateAndAdd(errors, new EdmError(this.Location, EdmErrorCode.InvalidRoleInRelationshipConstraint, Edm.Strings.CsdlParser_InvalidEndRoleInRelationshipConstraint(this.constraint.Principal.Role, this.context.Name)));
            }

            // If there are already errors, property errors are likely to be meaningless.
            if (errors == null)
            {
                if (!(principalRoleType.Key().Count() == principalEnd.Properties.Count() && principalEnd.Properties.Count() == dependentEnd.Properties.Count()))
                {
                    errors = AllocateAndAdd(errors, new EdmError(this.Location,
                                   EdmErrorCode.MismatchNumberOfPropertiesInRelationshipConstraint,
                                   Edm.Strings.EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint));
                }

                foreach (string propertyName in this.PrincipalKeyPropertiesNotFoundInPrincipalProperties)
                {
                    errors = AllocateAndAdd(errors, new EdmError(this.Location,
                            EdmErrorCode.InvalidPropertyInRelationshipConstraint,
                            Edm.Strings.EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintPrimaryEnd(
                                                            propertyName,
                                                            this.constraint.Principal.Role)));
                }

                foreach (string propertyName in this.DependentPropertiesNotFoundInDependentType)
                {
                    errors = AllocateAndAdd(errors, new EdmError(this.Location,
                        EdmErrorCode.InvalidPropertyInRelationshipConstraint,
                        Edm.Strings.EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintDependentEnd(
                                                        propertyName,
                                                        this.constraint.Dependent.Role)));
                }
            }

            return errors ?? Enumerable.Empty<EdmError>();
        }

        private IEdmAssociationEnd ComputePrincipal()
        {
            IEdmAssociationEnd returnedEnd = this.context.End1;
            if (returnedEnd.Name != this.constraint.Principal.Role)
            {
                returnedEnd = this.context.End2;
            }

            if (returnedEnd.Name != this.constraint.Principal.Role)
            {
                returnedEnd = new BadAssociationEnd(this.context, this.constraint.Principal.Role, new EdmError[] { new EdmError(this.Location, EdmErrorCode.BadNonComputableAssociationEnd, Edm.Strings.Bad_UncomputableAssociationEnd(this.constraint.Principal.Role)) });
            }

            return returnedEnd;
        }

        private IEnumerable<IEdmStructuralProperty> ComputeDependentProperties()
        {
            // Sort the ReferentialConstraint principal/dependent property pairs into the order of the principal end.
            // If any part of the constraint is bad, return all dependent properties as bad.
            List<IEdmStructuralProperty> properties = new List<IEdmStructuralProperty>();
            IEdmEntityType dependentRoleType = this.DependentEnd().EntityType;
            IEdmEntityType principalRoleType = this.PrincipalEnd.EntityType;
            CsdlReferentialConstraintRole principalEnd = this.constraint.Principal;
            CsdlReferentialConstraintRole dependentEnd = this.constraint.Dependent;
            if ((principalRoleType.Key().Count() == principalEnd.Properties.Count() && principalEnd.Properties.Count() == dependentEnd.Properties.Count()) &&
                this.PrincipalKeyPropertiesNotFoundInPrincipalProperties.Count() == 0 &&
                this.DependentPropertiesNotFoundInDependentType.Count() == 0)
            {
                foreach (IEdmStructuralProperty principalKeyProperty in this.PrincipalEnd.EntityType.Key())
                {
                    CsdlPropertyReference correspondingPrimaryEndReference = (from reference in principalEnd.Properties
                                                                              where principalRoleType.FindProperty(reference.PropertyName).Equals(principalKeyProperty)
                                                                              select reference).FirstOrDefault();
                    int correspondingPrimaryEndIndex = principalEnd.IndexOf(correspondingPrimaryEndReference);
                    CsdlPropertyReference dependentEndReference = dependentEnd.Properties.ElementAt(correspondingPrimaryEndIndex);
                    IEdmStructuralProperty dependentProperty = dependentRoleType.FindProperty(dependentEndReference.PropertyName) as IEdmStructuralProperty;
                    properties.Add(dependentProperty);
                }
            }
            else
            {
                properties = new List<IEdmStructuralProperty>();
                foreach (CsdlPropertyReference prop in dependentEnd.Properties)
                {
                    properties.Add(new BadProperty(dependentRoleType, prop.PropertyName, new EdmError[] { new EdmError(this.Location, EdmErrorCode.TypeMismatchRelationshipConstraint, Edm.Strings.CsdlSemantics_ReferentialConstraintMismatch) }));
                }
            }

            return properties;
        }

        private IEnumerable<string> ComputePrincipalKeyPropertiesNotFoundInPrincipalProperties()
        {
            List<string> properties = null;
            foreach (IEdmStructuralProperty principalKeyProperty in this.PrincipalEnd.EntityType.Key())
            {
                CsdlReferentialConstraintRole principalEnd = this.constraint.Principal;
                CsdlPropertyReference correspondingPrimaryEndReference = (from reference in principalEnd.Properties
                                                                          where reference.PropertyName == principalKeyProperty.Name
                                                                          select reference).FirstOrDefault();
                if (correspondingPrimaryEndReference == null)
                {
                    properties = AllocateAndAdd(properties, principalKeyProperty.Name);
                }
            }

            return properties ?? Enumerable.Empty<string>();
        }

        private IEnumerable<string> PrincipalKeyPropertiesNotFoundInPrincipalProperties
        {
            get { return this.principalKeyPropertiesNotFoundInPrincipalPropertiesCache.GetValue(this, s_computePrincipalKeyPropertiesNotFoundInPrincipalProperties, null); }
        }

        private IEnumerable<string> ComputeDependentPropertiesNotFoundInDependentType()
        {
            List<string> properties = new List<string>();
            IEdmEntityType dependentRoleType = this.DependentEnd().EntityType;
            foreach (CsdlPropertyReference propRef in this.constraint.Dependent.Properties)
            {
                if (dependentRoleType.FindProperty(propRef.PropertyName) == null)
                {
                    properties = AllocateAndAdd(properties, propRef.PropertyName);
                }
            }

            return properties ?? Enumerable.Empty<string>();
        }

        private IEnumerable<string> DependentPropertiesNotFoundInDependentType
        {
            get { return this.dependentPropertiesNotFoundInDependentTypeCache.GetValue(this, s_computeDependentPropertiesNotFoundInDependentType, null); }
        }

        /// <summary>
        /// Allocates a new list if needed, and adds the item to the list.
        /// </summary>
        /// <typeparam name="T">Type of the list.</typeparam>
        /// <param name="list">List to add the item to.</param>
        /// <param name="item">Item being added.</param>
        /// <returns>List containing then new item.</returns>
        private static List<T> AllocateAndAdd<T>(List<T> list, T item)
        {
            if (list == null)
            {
                list = new List<T>();
            }

            list.Add(item);
            return list;
        }
    }
}
