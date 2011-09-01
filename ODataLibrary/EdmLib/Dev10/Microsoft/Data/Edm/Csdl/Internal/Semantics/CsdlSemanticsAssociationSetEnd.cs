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
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlAssociationSetEnd.
    /// </summary>
    internal class CsdlSemanticsAssociationSetEnd : CsdlSemanticsElement, IEdmAssociationSetEnd, IEdmCheckable
    {
        private readonly CsdlSemanticsAssociationSet context;
        private readonly CsdlAssociationSetEnd end;

        private readonly Cache<CsdlSemanticsAssociationSetEnd, IEdmAssociationEnd> role = new Cache<CsdlSemanticsAssociationSetEnd, IEdmAssociationEnd>();
        private readonly static Func<CsdlSemanticsAssociationSetEnd, IEdmAssociationEnd> s_computeRole = (me) => me.ComputeRole();

        private readonly Cache<CsdlSemanticsAssociationSetEnd, IEdmEntitySet> entitySet = new Cache<CsdlSemanticsAssociationSetEnd, IEdmEntitySet>();
        private readonly static Func<CsdlSemanticsAssociationSetEnd, IEdmEntitySet> s_computeEntitySet = (me) => me.ComputeEntitySet();

        public CsdlSemanticsAssociationSetEnd(CsdlSemanticsAssociationSet context, CsdlAssociationSetEnd end)
        {
            this.context = context;
            this.end = end;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.end; }
        }

        private IEdmAssociationEnd ComputeRole()
        {
            if (this.end.Role != null)
            {
                Func<IEdmAssociationEnd, bool> match = (endCandidate) => endCandidate != null && endCandidate.Name == this.end.Role;
                if (match(this.context.Association.End1))
                {
                    return this.context.Association.End1;
                }

                if (match(this.context.Association.End2))
                {
                    return this.context.Association.End2;
                }
            }

            return new UnresolvedAssociationEnd(this.context.Association, this.end.Role, this.Location);
        }

        public IEdmAssociationEnd Role
        {
            get
            {
                return this.role.GetValue(this, s_computeRole, null);
            }
        }

        private IEdmEntitySet ComputeEntitySet()
        {
            return this.context.Context.FindEntitySet(this.end.EntitySet) ?? new UnresolvedEntitySet(this.end.EntitySet, this.end.Location);
        }

        public IEdmEntitySet EntitySet
        {
            get
            {
                return this.entitySet.GetValue(this, s_computeEntitySet, null);
            }
        }

        public IEnumerable<EdmError> Errors
        {
            get
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

                return errors;
            }
        }
    }
}
