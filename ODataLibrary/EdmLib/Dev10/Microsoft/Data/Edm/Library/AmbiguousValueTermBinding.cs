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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a name binding to more than one item.
    /// </summary>
    internal class AmbiguousValueTermBinding : AmbiguousBinding<IEdmValueTerm>, IEdmValueTerm
    {
        private readonly IEdmValueTerm first;

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
            // ToDo JHamby: This isn't correct. This should be some sort of bad reference.
            get { return this.first.Type; }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        public string NamespaceUri
        {
            // ToDo JHamby: Should this be something like "http://Bad"?
            get { return this.first.NamespaceUri; }
        }
    }
}
