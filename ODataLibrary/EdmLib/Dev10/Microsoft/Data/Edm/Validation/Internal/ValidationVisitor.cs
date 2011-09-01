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

namespace Microsoft.Data.Edm.Validation.Internal
{
    internal class ValidationVisitor : EdmModelVisitor
    {
        private readonly ValidationContext context;

        public ValidationVisitor(ValidationContext context)
        {
            this.context = context;
        }

        internal void Validate(IEdmModel model)
        {
            this.VisitEdmModel(model);
        }

        protected void ProcessValidation<T>(T item, Func<T, bool> isBad, Func<T, IEnumerable<EdmError>> getErrors, Action<T> continueVisitation)
        {
            ProcessValidation(item, isBad, getErrors, continueVisitation, this.RunRules);
        }

        protected void ProcessValidation<T>(T item, Func<T, bool> isBad, Func<T, IEnumerable<EdmError>> getErrors, Action<T> continueVisitation, Action<T> runRules)
        {
            if (isBad(item))
            {
                foreach (EdmError error in getErrors(item))
                {
                    this.context.AddError(error);
                }
            }
            else
            {
                continueVisitation(item);
                runRules(item);
            }
        }

        protected override void ProcessElement(IEdmElement element)
        {
            this.ProcessValidation(element, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessElement);
        }

        protected void RunRules<TElement>(TElement element)
        {
            IEnumerable<ValidationRule> rules = this.context.RuleSet.GetRules(typeof(TElement));
            foreach (ValidationRule rule in rules)
            {
                rule.Evaluate(this.context, element);
            }
        }

        #region Process Methods

        protected override void ProcessModel(IEdmModel model)
        {
            this.ProcessValidation(model, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessModel);
        }

        protected override void ProcessNamedElement(IEdmNamedElement element)
        {
            this.ProcessValidation(element, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessNamedElement);
        }

        protected override void ProcessSchemaElement(IEdmSchemaElement element)
        {
            this.ProcessValidation(element, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessSchemaElement);
        }

        #region Type References

        protected override void ProcessComplexTypeReference(IEdmComplexTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessComplexTypeReference);
        }

        protected override void ProcessEntityTypeReference(IEdmEntityTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessEntityTypeReference);
        }

        protected override void ProcessEntityReferenceTypeReference(IEdmEntityReferenceTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessEntityReferenceTypeReference);
        }

        protected override void ProcessRowTypeReference(IEdmRowTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessRowTypeReference);
        }

        protected override void ProcessCollectionTypeReference(IEdmCollectionTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessCollectionTypeReference);
        }

        protected override void ProcessBinaryTypeReference(IEdmBinaryTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessBinaryTypeReference);
        }

        protected override void ProcessDecimalTypeReference(IEdmDecimalTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessDecimalTypeReference);
        }

        protected override void ProcessSpatialTypeReference(IEdmSpatialTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessSpatialTypeReference);
        }

        protected override void ProcessStringTypeReference(IEdmStringTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessStringTypeReference);
        }

        protected override void ProcessTemporalTypeReference(IEdmTemporalTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessTemporalTypeReference);
        }

        protected override void ProcessPrimitiveTypeReference(IEdmPrimitiveTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessPrimitiveTypeReference);
        }

        protected override void ProcessStructuredTypeReference(IEdmStructuredTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessStructuredTypeReference);
        }

        protected override void ProcessTypeReference(IEdmTypeReference reference)
        {
            this.ProcessValidation(reference, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessTypeReference);
        }

        #endregion

        #region Type Definitions

        protected override void ProcessAssociation(IEdmAssociation definition)
        {
            this.ProcessValidation(definition, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessAssociation);
        }

        protected override void ProcessComplexTypeDefinition(IEdmComplexType definition)
        {
            Action<IEdmComplexType> runRulesAction = (d) =>
            {
                RunRules(d);
                RunRules((IEdmSchemaType)d);
            };
            this.ProcessValidation(definition, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessComplexTypeDefinition, runRulesAction);
        }

        protected override void ProcessEntityTypeDefinition(IEdmEntityType definition)
        {
            Action<IEdmEntityType> runRulesAction = (d) =>
            {
                RunRules(d);
                RunRules((IEdmSchemaType)d);
            };
            this.ProcessValidation(definition, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessEntityTypeDefinition, runRulesAction);
        }

        protected override void ProcessRowTypeDefinition(IEdmRowType definition)
        {
            this.ProcessValidation(definition, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessRowTypeDefinition);
        }

        protected override void ProcessEnumTypeDefinition(IEdmEnumType definition)
        {
            this.ProcessValidation(definition, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessEnumTypeDefinition);
        }

        protected override void ProcessCollectionTypeDefinition(IEdmCollectionType definition)
        {
            this.ProcessValidation(definition, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessCollectionTypeDefinition);
        }

        protected override void ProcessEntityReferenceTypeDefinition(IEdmEntityReferenceType definition)
        {
            this.ProcessValidation(definition, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessEntityReferenceTypeDefinition);
        }

        protected override void ProcessStructuredTypeDefinition(IEdmStructuredType definition)
        {
            this.ProcessValidation(definition, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessStructuredTypeDefinition);
        }

        #endregion

        #region Definition Components

        protected override void ProcessAssociationEnd(IEdmAssociationEnd end)
        {
            this.ProcessValidation(end, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessAssociationEnd);
        }

        protected override void ProcessReferentialConstraint(IEdmReferentialConstraint constraint)
        {
            this.ProcessValidation(constraint, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessReferentialConstraint);
        }

        protected override void ProcessNavigationProperty(IEdmNavigationProperty property)
        {
            this.ProcessValidation(property, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessNavigationProperty);
        }

        protected override void ProcessStructuralProperty(IEdmStructuralProperty property)
        {
            this.ProcessValidation(property, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessStructuralProperty);
        }

        protected override void ProcessProperty(IEdmProperty property)
        {
            this.ProcessValidation(property, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessProperty);
        }

        protected override void ProcessEnumMember(IEdmEnumMember enumMember)
        {
            this.ProcessValidation(enumMember, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessEnumMember);
        }

        #endregion

        #region Data Model

        protected override void ProcessEntityContainer(IEdmEntityContainer container)
        {
            this.ProcessValidation(container, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessEntityContainer);
        }

        protected override void ProcessEntitySet(IEdmEntitySet set)
        {
            this.ProcessValidation(set, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessEntitySet);
        }

        protected override void ProcessAssociationSet(IEdmAssociationSet set)
        {
            this.ProcessValidation(set, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessAssociationSet);
        }

        protected override void ProcessAssociationSetEnd(IEdmAssociationSetEnd end)
        {
            this.ProcessValidation(end, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessAssociationSetEnd);
        }
        #endregion

        #region Function Related
        protected override void ProcessFunction(IEdmFunction function)
        {
            this.ProcessValidation(function, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessFunction);
        }

        protected override void ProcessFunctionImport(IEdmFunctionImport functionImport)
        {
            this.ProcessValidation(functionImport, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessFunctionImport);
        }

        protected override void ProcessFunctionBase(IEdmFunctionBase functionBase)
        {
            this.ProcessValidation(functionBase, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessFunctionBase);
        }

        protected override void ProcessFunctionParameter(IEdmFunctionParameter parameter)
        {
            this.ProcessValidation(parameter, ExtensionMethods.IsBad, ExtensionMethods.Errors, base.ProcessFunctionParameter);
        }

        #endregion

        #endregion
    }
}
