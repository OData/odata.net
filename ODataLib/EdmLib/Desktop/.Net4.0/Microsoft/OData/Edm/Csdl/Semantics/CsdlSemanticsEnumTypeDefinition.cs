//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
