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
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    internal class AmbiguousPropertyBinding: AmbiguousBinding<IEdmProperty>, IEdmProperty
    {
        private readonly IEdmStructuredType declaringType;

        // Type cache.
        private readonly Cache<AmbiguousPropertyBinding, IEdmTypeReference> type = new Cache<AmbiguousPropertyBinding, IEdmTypeReference>();
        private readonly static Func<AmbiguousPropertyBinding, IEdmTypeReference> s_computeType = (me) => me.ComputeType();

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
            get { return this.type.GetValue(this, s_computeType, null); }
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
            // Any bad type reference will do, so using BadRowTypeDefinition because it doesn't require type name.
            return new EdmRowTypeReference(new BadRowType(Errors), true);
        }
    }
}
