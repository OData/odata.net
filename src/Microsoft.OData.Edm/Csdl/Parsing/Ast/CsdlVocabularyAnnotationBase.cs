//---------------------------------------------------------------------
// <copyright file="CsdlVocabularyAnnotationBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Common base type for a CSDL type or value annotation.
    /// </summary>
    internal abstract class CsdlVocabularyAnnotationBase : CsdlElement
    {
        private readonly string qualifier;
        private readonly string term;

        protected CsdlVocabularyAnnotationBase(string term, string qualifier, CsdlLocation location)
            : base(location)
        {
            this.qualifier = qualifier;
            this.term = term;
        }

        public string Qualifier
        {
            get { return this.qualifier; }
        }

        public string Term
        {
            get { return this.term; }
        }
    }
}
