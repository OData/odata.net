//   OData .NET Libraries ver. 5.6.3
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
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library.Internal
{
    internal class AmbiguousPropertyBinding : AmbiguousBinding<IEdmProperty>, IEdmProperty
    {
        private readonly IEdmStructuredType declaringType;

        // Type cache.
        private readonly Cache<AmbiguousPropertyBinding, IEdmTypeReference> type = new Cache<AmbiguousPropertyBinding, IEdmTypeReference>();
        private static readonly Func<AmbiguousPropertyBinding, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public AmbiguousPropertyBinding(IEdmStructuredType declaringType, IEdmProperty first, IEdmProperty second)
            : base(first, second)
        {
            this.declaringType = declaringType;
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.None; }
        }

        public IEdmTypeReference Type
        {
            get { return this.type.GetValue(this, ComputeTypeFunc, null); }
        }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmStructuredType DeclaringType 
        {
            get { return this.declaringType; }
        }

        private IEdmTypeReference ComputeType()
        {
            return new BadTypeReference(new BadType(Errors), true);
        }
    }
}
