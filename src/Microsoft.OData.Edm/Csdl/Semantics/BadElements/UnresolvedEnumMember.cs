//---------------------------------------------------------------------
// <copyright file="UnresolvedEnumMember.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedEnumMember : BadElement, IEdmEnumMember
    {
        private readonly string name;
        private readonly IEdmEnumType declaringType;

        // Value cache.
        private readonly Cache<UnresolvedEnumMember, IEdmEnumMemberValue> value = new Cache<UnresolvedEnumMember, IEdmEnumMemberValue>();
        private static readonly Func<UnresolvedEnumMember, IEdmEnumMemberValue> ComputeValueFunc = (me) => ComputeValue();

        public UnresolvedEnumMember(string name, IEdmEnumType declaringType, EdmLocation location)
            : base(new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedEnumMember, Edm.Strings.Bad_UnresolvedEnumMember(name)) })
        {
            this.name = name ?? string.Empty;
            this.declaringType = declaringType;
        }

        public string Name
        {
            get { return this.name; }
        }

        public IEdmEnumMemberValue Value
        {
            get { return this.value.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmEnumType DeclaringType
        {
            get { return this.declaringType; }
        }

        private static IEdmEnumMemberValue ComputeValue()
        {
            return new EdmEnumMemberValue(0);
        }
    }
}
