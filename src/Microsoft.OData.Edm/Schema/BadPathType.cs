//---------------------------------------------------------------------
// <copyright file="BadPathType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM path type.
    /// </summary>
    internal class BadPathType : BadType, IEdmPathType, IEdmFullNamedElement
    {
        public BadPathType(string qualifiedName, IEnumerable<EdmError> errors)
            : base(errors)
        {
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Namespace
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public EdmPathTypeKind PathKind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Path; }
        }
    }
}
