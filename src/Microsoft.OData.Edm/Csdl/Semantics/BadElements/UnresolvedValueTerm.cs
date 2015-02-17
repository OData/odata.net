//---------------------------------------------------------------------
// <copyright file="UnresolvedValueTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedValueTerm : UnresolvedVocabularyTerm, IEdmValueTerm
    {
        private readonly UnresolvedValueTermTypeReference type = new UnresolvedValueTermTypeReference();
        private readonly string appliesTo = null;
        private readonly string defaultValue = null;

        public UnresolvedValueTerm(string qualifiedName)
            : base(qualifiedName)
        {
        }

        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }

        public override EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        public string AppliesTo
        {
            get { return this.appliesTo; }
        }

        public string DefaultValue
        {
            get { return this.defaultValue; }
        }

        private class UnresolvedValueTermTypeReference : IEdmTypeReference
        {
            private readonly UnresolvedValueTermType definition = new UnresolvedValueTermType();

            public bool IsNullable
            {
                get { return false; }
            }

            public IEdmType Definition
            {
                get { return this.definition; }
            }

            private class UnresolvedValueTermType : IEdmType
            {
                public EdmTypeKind TypeKind
                {
                    get { return EdmTypeKind.None; }
                }
            }
        }
    }
}
