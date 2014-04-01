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
    /// Provides semantics for a CsdlAssociationSetEnd.
    /// </summary>
    internal class CsdlSemanticsAssociationSetEnd : CsdlSemanticsElement, IEdmAssociationSetEnd, IEdmCheckable
    {
        private readonly CsdlSemanticsAssociationSet context;
        private readonly CsdlAssociationSetEnd end;
        private readonly IEdmAssociationEnd role;

        private readonly Cache<CsdlSemanticsAssociationSetEnd, IEdmEntitySet> entitySet = new Cache<CsdlSemanticsAssociationSetEnd, IEdmEntitySet>();
        private static readonly Func<CsdlSemanticsAssociationSetEnd, IEdmEntitySet> ComputeEntitySetFunc = (me) => me.ComputeEntitySet();

        private readonly Cache<CsdlSemanticsAssociationSetEnd, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsAssociationSetEnd, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsAssociationSetEnd, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsAssociationSetEnd(CsdlSemanticsAssociationSet context, CsdlAssociationSetEnd end, IEdmAssociationEnd role)
            : base(end)
        {
            this.context = context;
            this.end = end;
            this.role = role;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return (CsdlElement)this.end; }
        }

        public IEdmAssociationEnd Role
        {
            get
            {
                return this.role;
            }
        }

        public IEdmEntitySet EntitySet
        {
            get
            {
                return this.entitySet.GetValue(this, ComputeEntitySetFunc, null);
            }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private IEdmEntitySet ComputeEntitySet()
        {
            if (this.end != null)
            {
                return this.context.Container.FindEntitySet(this.end.EntitySet) ?? new UnresolvedEntitySet(this.end.EntitySet, this.context.Container, this.Location);
            }
            else
            {
                IEnumerable<EdmError> errors = new EdmError[]
                {
                    new EdmError(
                        this.Location,
                        EdmErrorCode.NoEntitySetsFoundForType,
                        Strings.EdmModel_Validator_Semantic_NoEntitySetsFoundForType(this.context.Container.FullName() + this.context.Name, this.role.EntityType.FullName(), this.Role.Name))
                };
                return this.context.Container.EntitySets().Where(set => set.ElementType == this.role.EntityType).FirstOrDefault() ?? new BadEntitySet("UnresolvedEntitySet", this.context.Container, errors);
            }
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            List<EdmError> errors = new List<EdmError>();
            if (this.Role is UnresolvedAssociationEnd)
            {
                errors.AddRange(this.Role.Errors());
            }

            if (this.EntitySet is UnresolvedEntitySet)
            {
                errors.AddRange(this.EntitySet.Errors());
            }

            if (this.end == null)
            {
                if (this.context.Container.EntitySets().Where(set => set.ElementType == this.role.EntityType).Count() > 1)
                {
                    errors.Add(new EdmError(
                       this.Location,
                       EdmErrorCode.CannotInferEntitySetWithMultipleSetsPerType,
                       Strings.EdmModel_Validator_Semantic_CannotInferEntitySetWithMultipleSetsPerType(this.context.Container.FullName() + this.context.Name, this.role.EntityType.FullName(), this.Role.Name)));
                }
            }

            return errors;
        }
    }
}
