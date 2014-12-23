//   OData .NET Libraries ver. 6.9
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
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlEnumType.
    /// </summary>
    internal class CsdlSemanticsEnumTypeDefinition : CsdlSemanticsTypeDefinition, IEdmEnumType
    {
        private readonly CsdlSemanticsSchema context;
        private readonly CsdlEnumType enumeration;

        private readonly Cache<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType> underlyingTypeCache = new Cache<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType>();
        private static readonly Func<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType> ComputeUnderlyingTypeFunc = (me) => me.ComputeUnderlyingType();

        private readonly Cache<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>> membersCache = new Cache<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>>();
        private static readonly Func<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>> ComputeMembersFunc = (me) => me.ComputeMembers();

        public CsdlSemanticsEnumTypeDefinition(CsdlSemanticsSchema context, CsdlEnumType enumeration)
            : base(enumeration)
        {
            this.context = context;
            this.enumeration = enumeration;
        }

        IEdmPrimitiveType IEdmEnumType.UnderlyingType
        {
            get { return this.underlyingTypeCache.GetValue(this, ComputeUnderlyingTypeFunc, null); }
        }

        IEnumerable<IEdmEnumMember> IEdmEnumType.Members
        {
            get { return this.membersCache.GetValue(this, ComputeMembersFunc, null); }
        }

        bool IEdmEnumType.IsFlags
        {
            get { return this.enumeration.IsFlags; }
        }

        EdmSchemaElementKind IEdmSchemaElement.SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }
        
        public string Namespace
        {
            get { return this.context.Namespace; }
        }

        string IEdmNamedElement.Name
        {
            get { return this.enumeration.Name; }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Enum; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.enumeration; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.context);
        }

        private IEdmPrimitiveType ComputeUnderlyingType()
        {
            if (this.enumeration.UnderlyingTypeName != null)
            {
                var underlyingTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(this.enumeration.UnderlyingTypeName);
                return underlyingTypeKind != EdmPrimitiveTypeKind.None ?
                    EdmCoreModel.Instance.GetPrimitiveType(underlyingTypeKind) :
                    new UnresolvedPrimitiveType(this.enumeration.UnderlyingTypeName, this.Location);
            }

            return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
        }

        private IEnumerable<IEdmEnumMember> ComputeMembers()
        {
            var members = new List<IEdmEnumMember>();

            // Walk the members and assign implicit values where needed.
            long previousValue = -1;
            foreach (CsdlEnumMember member in this.enumeration.Members)
            {
                IEdmEnumMember semanticsMember;
                long? implicitValue = null;
                if (!member.Value.HasValue)
                {
                    if (previousValue < long.MaxValue)
                    {
                        implicitValue = previousValue + 1;
                        previousValue = implicitValue.Value;
                        member.Value = implicitValue;
                        semanticsMember = new CsdlSemanticsEnumMember(this, member);
                    }
                    else
                    {
                        semanticsMember = new BadEnumMember(this, member.Name, new EdmError[] { new EdmError(member.Location ?? this.Location, EdmErrorCode.EnumMemberValueOutOfRange, Edm.Strings.CsdlSemantics_EnumMemberValueOutOfRange) });
                    }

                    semanticsMember.SetIsValueExplicit(this.Model, false);
                }
                else
                {
                    previousValue = member.Value.Value;
                    semanticsMember = new CsdlSemanticsEnumMember(this, member);
                    semanticsMember.SetIsValueExplicit(this.Model, true);
                }

                members.Add(semanticsMember);
            }

            return members;
        }
    }
}
