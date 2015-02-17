//---------------------------------------------------------------------
// <copyright file="StubValueTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.VocabularyStubs
{
    using Microsoft.OData.Edm;
    using EdmLibTests.StubEdm;

    public class StubValueTerm : StubEdmElement, IEdmValueTerm
    {
        public StubValueTerm(string namespaceName, string name)
        {
            this.Namespace = namespaceName;
            this.Name = name;
        }

        public string Namespace { get; set; }

        public string Name { get; set; }

        public IEdmTypeReference Type { get; set; }

        public string AppliesTo { get; set; }

        public string DefaultValue { get; set; }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        public override string ToString()
        {
            return this.GetCanonicalName(this);
        }

        public override bool Equals(object obj)
        {
            IEdmValueTerm otherValueTerm = obj as IEdmValueTerm;
            if (otherValueTerm == null)
            {
                return false;
            }

            return this.GetCanonicalName(this) == this.GetCanonicalName(otherValueTerm);
        }

        public override int GetHashCode()
        {
            return this.GetCanonicalName(this).GetHashCode();
        }

        private string GetCanonicalName(IEdmTerm t)
        {
            return t.Namespace + "." + t.Name;
        }
    }
}
