//   OData .NET Libraries ver. 6.8.1
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

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library
{
    internal class AmbiguousOperationBinding : AmbiguousBinding<IEdmOperation>, IEdmOperation
    {
        private IEdmOperation first;

        public AmbiguousOperationBinding(IEdmOperation first, IEdmOperation second)
            : base(first, second)
        {
            this.first = first;
        }

        public IEdmTypeReference ReturnType
        {
            // Not using the typical behavior for first.ReturnType as returning null is the old behavior.
            get { return null; }
        }

        public string Namespace
        {
            get { return this.first.Namespace; }
        }

        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.first.Parameters; }
        }

        public bool IsBound
        {
            get { return this.first.IsBound; }
        }

        public IEdmPathExpression EntitySetPath
        {
            get { return this.first.EntitySetPath; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return this.first.SchemaElementKind; }
        }

        public IEdmOperationParameter FindParameter(string name)
        {
            return this.first.FindParameter(name);
        }
    }
}
