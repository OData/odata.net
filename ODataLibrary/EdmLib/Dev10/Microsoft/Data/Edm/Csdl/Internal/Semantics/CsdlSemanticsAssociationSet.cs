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
    /// Provides semantics for a CsdlAssociationSet.
    /// </summary>
    internal class CsdlSemanticsAssociationSet : CsdlSemanticsElement, IEdmAssociationSet, IEdmCheckable
    {
        private readonly CsdlSemanticsEntityContainer context;
        private readonly CsdlAssociationSet associationSet;

        private readonly Cache<CsdlSemanticsAssociationSet, TupleInternal<IEdmAssociationSetEnd, IEdmAssociationSetEnd>> endsCache = new Cache<CsdlSemanticsAssociationSet, TupleInternal<IEdmAssociationSetEnd, IEdmAssociationSetEnd>>();
        private readonly static Func<CsdlSemanticsAssociationSet, TupleInternal<IEdmAssociationSetEnd, IEdmAssociationSetEnd>> s_computeEnds = (me) => me.ComputeEnds();

        private readonly Cache<CsdlSemanticsAssociationSet, IEdmAssociation> elementTypeCache = new Cache<CsdlSemanticsAssociationSet, IEdmAssociation>();
        private readonly static Func<CsdlSemanticsAssociationSet, IEdmAssociation> s_computeElementType = (me) => me.ComputeElementType();

        public CsdlSemanticsAssociationSet(CsdlSemanticsEntityContainer context, CsdlAssociationSet associationSet)
        {
            this.context = context;
            this.associationSet = associationSet;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        internal CsdlSemanticsEntityContainer Context
        {
            get { return this.context; }
        }

        public override CsdlElement Element
        {
            get { return this.associationSet; }
        }

        public string Name
        {
            get { return this.associationSet.Name; }
        }

        private IEdmAssociation ComputeElementType()
        {
            return this.context.Model.FindAssociation(this.associationSet.Association) ?? new UnresolvedAssociation(this.associationSet.Association, this.associationSet.Location);
        }

        public IEdmAssociation Association
        {
            get { return this.elementTypeCache.GetValue(this, s_computeElementType, null); }
        }

        public IEdmAssociationSetEnd End1
        {
            get { return this.Ends.Item1; }
        }

        public IEdmAssociationSetEnd End2
        {
            get { return this.Ends.Item2; }
        }

        private TupleInternal<IEdmAssociationSetEnd, IEdmAssociationSetEnd> Ends
        {
            get { return this.endsCache.GetValue(this, s_computeEnds, null); }
        }

        private TupleInternal<IEdmAssociationSetEnd, IEdmAssociationSetEnd> ComputeEnds()
        {
            return TupleInternal.Create(
                (this.associationSet.End1 != null) ? new CsdlSemanticsAssociationSetEnd(this, this.associationSet.End1) : (IEdmAssociationSetEnd)new BadAssociationSetEnd(this, "End1", new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidAssociationSet, Edm.Strings.CsdlParser_InvalidAssociationSetIncorrectNumberOfEnds(this.Name))}) ,
                (this.associationSet.End2 != null) ? new CsdlSemanticsAssociationSetEnd(this, this.associationSet.End2) : (IEdmAssociationSetEnd)new BadAssociationSetEnd(this, "End2", new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidAssociationSet, Edm.Strings.CsdlParser_InvalidAssociationSetIncorrectNumberOfEnds(this.Name))}));
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.AssociationSet; }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.Association is UnresolvedAssociation)
                {
                    return this.Association.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
