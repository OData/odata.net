//---------------------------------------------------------------------
// <copyright file="BadStructuredType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM structured type definition.
    /// </summary>
    internal abstract class BadStructuredType : BadType, IEdmStructuredType, IEdmCheckable
    {
        protected BadStructuredType(IEnumerable<EdmError> errors)
            : base(errors)
        {
        }

        public IEdmStructuredType BaseType
        {
            get { return null; }
        }

        public IEnumerable<IEdmProperty> DeclaredProperties
        {
            get { return Enumerable.Empty<IEdmProperty>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsOpen
        {
            get { return false; }
        }

        public IEdmProperty FindProperty(string name)
        {
            return null;
        }
    }
}
