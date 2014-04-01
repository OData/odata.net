//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.OData.Edm.Internal;
using Microsoft.OData.Edm.Library.Internal;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlReferentialConstraint.
    /// </summary>
    internal class CsdlSemanticsReferentialConstraint : CsdlSemanticsElement, IEdmCheckable
    {
        private readonly CsdlSemanticsAssociation context;
        private readonly CsdlReferentialConstraint constraint;

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEdmAssociationEnd> principalCache = new Cache<CsdlSemanticsReferentialConstraint, IEdmAssociationEnd>();
        private static readonly Func<CsdlSemanticsReferentialConstraint, IEdmAssociationEnd> ComputePrincipalFunc = (me) => me.ComputePrincipal();

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEnumerable<IEdmStructuralProperty>> dependentPropertiesCache = new Cache<CsdlSemanticsReferentialConstraint, IEnumerable<IEdmStructuralProperty>>();
        private static readonly Func<CsdlSemanticsReferentialConstraint, IEnumerable<IEdmStructuralProperty>> ComputeDependentPropertiesFunc = (me) => me.ComputeDependentProperties();

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsReferentialConstraint, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsReferentialConstraint, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEnumerable<string>> principalKeyPropertiesNotFoundInPrincipalPropertiesCache = new Cache<CsdlSemanticsReferentialConstraint, IEnumerable<string>>();
        private static readonly Func<CsdlSemanticsReferentialConstraint, IEnumerable<string>> ComputePrincipalKeyPropertiesNotFoundInPrincipalPropertiesFunc = (me) => me.ComputePrincipalKeyPropertiesNotFoundInPrincipalProperties();

        private readonly Cache<CsdlSemanticsReferentialConstraint, IEnumerable<string>> dependentPropertiesNotFoundInDependentTypeCache = new Cache<CsdlSemanticsReferentialConstraint, IEnumerable<string>>();
        private static readonly Func<CsdlSemanticsReferentialConstraint, IEnumerable<string>> ComputeDependentPropertiesNotFoundInDependentTypeFunc = (me) => me.ComputeDependentPropertiesNotFoundInDependentType();

        public CsdlSemanticsReferentialConstraint(CsdlSemanticsAssociation context, CsdlReferentialConstraint constraint)
            : base(constraint)
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
            get { return this.principalCache.GetValue(this, ComputePrincipalFunc, null); }
        }

        public IEnumerable<IEdmStructuralProperty> DependentProperties
        {
            get { return this.dependentPropertiesCache.GetValue(this, ComputeDependentPropertiesFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private IEdmAssociationEnd DependentEnd
        {
            get
            {
                IEdmAssociation association = this.PrincipalEnd.DeclaringAssociation;
                return this.PrincipalEnd == association.End1 ? association.End2 : association.End1;
            }
        }

        private IEnumerable<string> PrincipalKeyPropertiesNotFoundInPrincipalProperties
        {
            get { return this.principalKeyPropertiesNotFoundInPrincipalPropertiesCache.GetValue(this, ComputePrincipalKeyPropertiesNotFoundInPrincipalPropertiesFunc, null); }
        }

        private IEnumerable<string> DependentPropertiesNotFoundInDependentType
        {
            get { return this.dependentPropertiesNotFoundInDependentTypeCache.GetValue(this, ComputeDependentPropertiesNotFoundInDependentTypeFunc, null); }
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
                if (principalEnd.Properties.Count() != dependentEnd.Properties.Count())
                {
                    errors = AllocateAndAdd(
                        errors,
                        new EdmError(
                            this.Location,
                            EdmErrorCode.MismatchNumberOfPropertiesInRelationshipConstraint,
                            Edm.Strings.EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint));
                }

                if (principalRoleType.Key().Count() != principalEnd.Properties.Count() || this.PrincipalKeyPropertiesNotFoundInPrincipalProperties.Count() != 0)
                {
                    string errorMessage =
                        Edm.Strings.EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintPrimaryEnd(
                            this.DependentEnd.DeclaringAssociation.Namespace + '.' + this.DependentEnd.DeclaringAssociation.Name,
                            this.constraint.Principal.Role);
                    errors = AllocateAndAdd(
                        errors,
                        new EdmError(
                            this.Location,
                            EdmErrorCode.BadPrincipalPropertiesInReferentialConstraint,
                            errorMessage));
                }

                foreach (string propertyName in this.DependentPropertiesNotFoundInDependentType)
                {
                    string errorMessage = Edm.Strings.EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintDependentEnd(
                        propertyName,
                        this.constraint.Dependent.Role);

                    errors = AllocateAndAdd(
                        errors,
                        new EdmError(
                            this.Location,
                            EdmErrorCode.InvalidPropertyInRelationshipConstraint,
                            errorMessage));
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
            IEdmEntityType dependentRoleType = this.DependentEnd.EntityType;
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

        private IEnumerable<string> ComputeDependentPropertiesNotFoundInDependentType()
        {
            List<string> properties = new List<string>();
            IEdmEntityType dependentRoleType = this.DependentEnd.EntityType;
            foreach (CsdlPropertyReference propRef in this.constraint.Dependent.Properties)
            {
                if (dependentRoleType.FindProperty(propRef.PropertyName) == null)
                {
                    properties = AllocateAndAdd(properties, propRef.PropertyName);
                }
            }

            return properties ?? Enumerable.Empty<string>();
        }
    }
}
