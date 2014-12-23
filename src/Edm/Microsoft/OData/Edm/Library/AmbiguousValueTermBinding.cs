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

using System;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a name binding to more than one item.
    /// </summary>
    internal class AmbiguousValueTermBinding : AmbiguousBinding<IEdmValueTerm>, IEdmValueTerm
    {
        private readonly IEdmValueTerm first;

        // Type cache.
        private readonly Cache<AmbiguousValueTermBinding, IEdmTypeReference> type = new Cache<AmbiguousValueTermBinding, IEdmTypeReference>();
        private static readonly Func<AmbiguousValueTermBinding, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        private readonly string appliesTo = null;
        private readonly string defaultValue = null;

        public AmbiguousValueTermBinding(IEdmValueTerm first, IEdmValueTerm second)
            : base(first, second)
        {
            this.first = first;
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }

        public string Namespace
        {
            get { return this.first.Namespace ?? string.Empty; }
        }

        public IEdmTypeReference Type
        {
            get { return this.type.GetValue(this, ComputeTypeFunc, null); }
        }

        public string AppliesTo
        {
            get { return this.appliesTo; }
        }

        public string DefaultValue
        {
            get { return this.defaultValue; }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        private IEdmTypeReference ComputeType()
        {
            return new BadTypeReference(new BadType(Errors), true);
        }
    }
}
