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
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM property.
    /// </summary>
    internal class BadValueTerm : BadElement, IEdmValueTerm
    {
        private readonly string namespaceName;
        private readonly string namespaceUri;
        private readonly string name;

        // Type cache.
        private readonly Cache<BadValueTerm, IEdmTypeReference> type = new Cache<BadValueTerm, IEdmTypeReference>();
        private readonly static Func<BadValueTerm, IEdmTypeReference> s_computeType = (me) => ComputeType();

        public BadValueTerm(string namespaceName, string namespaceUri, string name, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.namespaceName = namespaceName ?? string.Empty;
            this.namespaceUri = namespaceUri ?? string.Empty;
            this.name = name ?? string.Empty;
        }

        public string Name
        {
            get { return this.name; }
        }

        public IEdmTypeReference Type
        {
            get { return this.type.GetValue(this, s_computeType, null); }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        public string NamespaceUri
        {
            get { return this.namespaceUri; }
        }

        private static IEdmTypeReference ComputeType()
        {
            // Any bad type reference will do, so using BadRowTypeDefinition because it doesn't require a name.
            return new EdmRowTypeReference(new BadRowType(new EdmError[]{new EdmError(null, EdmErrorCode.TermTypeIsBadBecauseTermIsBad, Edm.Strings.Bad_BadByAssociation)}), true);
        }
    }
}
