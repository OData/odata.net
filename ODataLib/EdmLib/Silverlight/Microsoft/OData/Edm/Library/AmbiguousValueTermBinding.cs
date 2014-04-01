//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
