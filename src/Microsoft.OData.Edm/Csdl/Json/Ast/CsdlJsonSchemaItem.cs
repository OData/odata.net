//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Json.Value;

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal enum SchemaMemberKind
    {
        Entity,
        Complex,
        Enum,
        TypeDefinition,
        Action,
        Function,
        Term,
        EntityContainer,
        OutOfLineAnnotations
    }

    internal abstract class CsdlJsonSchemaItem
    {
        public CsdlJsonSchemaItem()
        {
            Members = new Dictionary<string, IJsonValue>();
        }

        private string _fullName;
        public string FullName
        {
            get
            {
                if (_fullName == null)
                {
                    _fullName = Namespace + "." + Name;
                }
                return _fullName;
            }
        }
        public string Namespace { get; set; }

        public string Name { get; set; }

        public abstract SchemaMemberKind Kind { get; }

        public IJsonValue JsonValue { get; set; }

        public IDictionary<string, IJsonValue> Members { get; }

        public IJsonPath JsonPath { get; set; }

        public void AddMember(string name, IJsonValue value)
        {
            Members[name] = value;
        }
    }
}
