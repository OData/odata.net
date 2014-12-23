//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl.Parsing.Ast;
    using Microsoft.OData.Edm.Library;

    /// <summary>
    /// Provides semantics for CsdlEntitySet.
    /// </summary>
    internal class CsdlSemanticsEntitySet : CsdlSemanticsNavigationSource, IEdmEntitySet
    {
        public CsdlSemanticsEntitySet(CsdlSemanticsEntityContainer container, CsdlEntitySet entitySet)
            : base(container, entitySet)
        {
        }

        public override IEdmType Type
        {
            get { return new EdmCollectionType(new EdmEntityTypeReference(this.typeCache.GetValue(this, ComputeElementTypeFunc, null), false)); }
        }

        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        protected override IEdmEntityType ComputeElementType()
        {
            string type = ((CsdlEntitySet)this.navigationSource).ElementType;
            return this.container.Context.FindType(type) as IEdmEntityType ?? new UnresolvedEntityType(type, this.Location);
        }
    }
}
