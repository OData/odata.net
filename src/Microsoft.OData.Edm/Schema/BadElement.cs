//---------------------------------------------------------------------
// <copyright file="BadElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an invalid EDM element.
    /// </summary>
    internal class BadElement : IEdmElement, IEdmCheckable, IEdmVocabularyAnnotatable
    {
        private readonly IEnumerable<EdmError> errors;

        public BadElement(IEnumerable<EdmError> errors)
        {
            this.errors = errors;
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errors; }
        }
    }
}
