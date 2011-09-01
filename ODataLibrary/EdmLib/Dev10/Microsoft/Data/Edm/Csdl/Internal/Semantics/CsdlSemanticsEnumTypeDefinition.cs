//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlEnumType.
    /// </summary>
    internal class CsdlSemanticsEnumTypeDefinition : CsdlSemanticsTypeDefinition, IEdmEnumType
    {
        private readonly CsdlSemanticsSchema context;
        private readonly CsdlEnumType enumeration;

        private readonly Cache<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType> underlyingTypeCache = new Cache<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType>();
        private readonly static Func<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType> s_computeUnderlyingType = (me) => me.ComputeUnderlyingType();

        private readonly Cache<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>> membersCache = new Cache<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>>();
        private readonly static Func<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>> s_computeMembers = (me) => me.ComputeMembers();

        public CsdlSemanticsEnumTypeDefinition(CsdlSemanticsSchema context, CsdlEnumType enumeration)
        {
            this.context = context;
            this.enumeration = enumeration;
        }

        IEdmPrimitiveType IEdmEnumType.UnderlyingType
        {
            get { return this.underlyingTypeCache.GetValue(this, s_computeUnderlyingType, null); }
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

        IEnumerable<IEdmEnumMember> IEdmEnumType.Members
        {
            get { return this.membersCache.GetValue(this, s_computeMembers, null); }
        }

        private IEnumerable<IEdmEnumMember> ComputeMembers()
        {
            var members = new List<IEdmEnumMember>();

            // Walk the members and assign implicit values where needed.
            long previousValue = -1;
            foreach (var member in this.enumeration.Members)
            {
                long? implicitValue = null;
                if (!member.Value.HasValue)
                {
                    if (previousValue < long.MaxValue)
                    {
                        implicitValue = previousValue + 1;
                        previousValue = implicitValue.Value;
                    }
                    else
                    {
                        // TODO: pass in non-null IEdmIntegerValue here when it gets checked in.
                        members.Add(new BadEnumMember(this, member.Name, /*TODO ^ */null, new EdmError[] { new EdmError(member.Location, EdmErrorCode.EnumMemberValueOutOfRange, Edm.Strings.CsdlSemantics_EnumMemberValueOutOfRange) }));
                        continue;
                    }
                }
                else
                {
                    previousValue = member.Value.Value;
                }

                members.Add(new CsdlSemanticsEnumMember(this, member));
            }

            return members;
        }

        bool IEdmEnumType.TreatAsBits
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
    }
}
