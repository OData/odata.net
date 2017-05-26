//---------------------------------------------------------------------
// <copyright file="StubPropertyConstructor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace EdmLibTests.VocabularyStubs
{
    public class StubPropertyConstructor : StubNonAnnotatedElement, IEdmPropertyConstructor
    {
        private string propertyName;
        private IEdmExpression expression;

        public StubPropertyConstructor (string propertyName, IEdmExpression expression)
        {
            this.propertyName = propertyName;
            this.expression = expression;
        }

        public string Name
        {
            get { return propertyName; }
        }

        public IEdmExpression Value
        {
            get { return expression; }
        }
    }
}
