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

using System.Collections.Generic;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library.Internal
{
    /// <summary>
    /// Represents a semantically invalid EDM named structured type definition.
    /// </summary>
    internal abstract class BadNamedStructuredType : BadStructuredType, IEdmSchemaElement
    {
        private readonly string namespaceName;
        private readonly string name;

        protected BadNamedStructuredType(string qualifiedName, IEnumerable<EdmError> errors)
            : base(errors)
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }
    }
}
