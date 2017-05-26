//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEnumMember.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlEnumMember.
    /// </summary>
    internal class CsdlSemanticsEnumMember : CsdlSemanticsElement, IEdmEnumMember
    {
        private readonly CsdlEnumMember member;
        private readonly CsdlSemanticsEnumTypeDefinition declaringType;

        private readonly Cache<CsdlSemanticsEnumMember, IEdmEnumMemberValue> valueCache = new Cache<CsdlSemanticsEnumMember, IEdmEnumMemberValue>();
        private static readonly Func<CsdlSemanticsEnumMember, IEdmEnumMemberValue> ComputeValueFunc = (me) => me.ComputeValue();

        public CsdlSemanticsEnumMember(CsdlSemanticsEnumTypeDefinition declaringType, CsdlEnumMember member)
            : base(member)
        {
            this.member = member;
            this.declaringType = declaringType;
        }

        public string Name
        {
            get { return this.member.Name; }
        }

        public IEdmEnumType DeclaringType
        {
            get { return this.declaringType; }
        }

        public IEdmEnumMemberValue Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.declaringType.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.member; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.declaringType.Context);
        }

        private IEdmEnumMemberValue ComputeValue()
        {
            if (this.member.Value == null)
            {
                return new BadEdmEnumMemberValue(
                    new EdmError[]
                    {
                        new EdmError(member.Location ?? this.Location, EdmErrorCode.EnumMemberMustHaveValue, Edm.Strings.CsdlSemantics_EnumMemberMustHaveValue)
                    });
            }
            else
            {
                return new EdmEnumMemberValue(this.member.Value.Value);
            }
        }
    }
}
