//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Represents a semantically invalid EDM association end.
    /// </summary>
    internal class BadAssociationEnd : BadElement, IEdmAssociationEnd
    {
        private readonly string role;
        private readonly IEdmAssociation declaringAssociation;

        // Type cache.
        private readonly Cache<BadAssociationEnd, BadEntityType> type = new Cache<BadAssociationEnd, BadEntityType>();
        private static readonly Func<BadAssociationEnd, BadEntityType> ComputeTypeFunc = (me) => me.ComputeType();

        public BadAssociationEnd(IEdmAssociation declaringAssociation, string role, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.role = role ?? string.Empty;
            this.declaringAssociation = declaringAssociation;
        }

        public IEdmAssociation DeclaringAssociation
        {
            get { return this.declaringAssociation; }
        }

        public EdmMultiplicity Multiplicity
        {
            get { return EdmMultiplicity.Unknown; }
        }

        public EdmOnDeleteAction OnDelete
        {
            get { return EdmOnDeleteAction.None; }
        }

        public IEdmEntityType EntityType
        {
            get { return this.type.GetValue(this, ComputeTypeFunc, null); }
        }

        public string Name
        {
            get { return this.role; }
        }

        private BadEntityType ComputeType()
        {
            return new BadEntityType(this.declaringAssociation.Name + "." + this.role, this.Errors);
        }
    }
}
