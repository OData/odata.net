//---------------------------------------------------------------------
// <copyright file="StubValueAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.VocabularyStubs
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Expressions;
    using EdmLibTests.StubEdm;

    public class StubValueAnnotation : StubEdmElement, IEdmValueAnnotation 
    {
        public string Qualifier { get; set; }

        public IEdmTerm Term { get; set; }

        public IEdmVocabularyAnnotatable Target { get; set; }

        public IEdmExpression Value { get; set; }
    }
}
