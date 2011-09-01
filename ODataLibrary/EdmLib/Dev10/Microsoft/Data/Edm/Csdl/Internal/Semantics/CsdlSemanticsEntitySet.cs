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
    /// Provides semantics for CsdlEntitySet.
    /// </summary>
    internal class CsdlSemanticsEntitySet : CsdlSemanticsElement, IEdmEntitySet, IEdmCheckable
    {
        private readonly CsdlEntitySet entitySet;
        private readonly CsdlSemanticsEntityContainer container;

        private readonly Cache<CsdlSemanticsEntitySet, IEdmEntityType> elementTypeCache = new Cache<CsdlSemanticsEntitySet, IEdmEntityType>();
        private readonly static Func<CsdlSemanticsEntitySet, IEdmEntityType> s_computeElementType = (me) => me.ComputeElementType();

        public CsdlSemanticsEntitySet(CsdlSemanticsEntityContainer container, CsdlEntitySet entitySet)
        {
            this.container = container;
            this.entitySet = entitySet;
        }

        public CsdlSemanticsEntityContainer Container
        {
            get { return this.container; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.container.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.entitySet; }
        }

        public string Name
        {
            get { return this.entitySet.Name; }
        }

        private IEdmEntityType ComputeElementType()
        {
            return this.container.Context.FindType(this.entitySet.EntityType) as IEdmEntityType ?? new UnresolvedEntityType(this.entitySet.EntityType, this.entitySet.Location);
        }

        public IEdmEntityType ElementType
        {
            get
            {
               return this.elementTypeCache.GetValue(this, s_computeElementType, null);
            }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                return this.ElementType is UnresolvedEntityType
                           ? this.ElementType.Errors()
                           : Enumerable.Empty<EdmError>();
            }
        }
    }
}
