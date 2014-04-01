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
