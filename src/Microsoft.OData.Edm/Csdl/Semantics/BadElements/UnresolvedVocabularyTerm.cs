//---------------------------------------------------------------------
// <copyright file="UnresolvedVocabularyTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal abstract class UnresolvedVocabularyTerm : EdmElement, IEdmTerm, IUnresolvedElement
    {
        private readonly string namespaceName;
        private readonly string name;

        protected UnresolvedVocabularyTerm(string qualifiedName)
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public abstract EdmTermKind TermKind
        {
            get;
        }

        public abstract EdmSchemaElementKind SchemaElementKind
        {
            get;
        }
    }
}
