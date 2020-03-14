//---------------------------------------------------------------------
// <copyright file="JsonNodeType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Reader
{
    /// <summary>
    /// Enumeration of all JSON node type.
    /// </summary>
    public enum JsonNodeType
    {
        /// <summary>
        /// No node - invalid value.
        /// </summary>
        None,

        /// <summary>
        /// Start of JSON object record, the { character.
        /// </summary>
        StartObject,

        /// <summary>
        /// End of JSON object record, the } character.
        /// </summary>
        EndObject,

        /// <summary>
        /// Start of JSON array, the [ character.
        /// </summary>
        StartArray,

        /// <summary>
        /// End of JSON array, the ] character.
        /// </summary>
        EndArray,

        /// <summary>
        /// Property, the name of the property (the value will be reported as a separate node or nodes)
        /// </summary>
        Property,

        /// <summary>
        /// Primitive value, that is either null, true, false, number or string.
        /// </summary>
        PrimitiveValue,

        /// <summary>
        /// End of input reached.
        /// </summary>
        EndOfInput
    }

    public enum JsonValueKind
    {
        JPrimitive,
        JObject,
        JArray
    }
    public interface IJsonValue
    {
        JsonValueKind ValueKind { get; }
    }

    internal class JsonPrimitiveValue : IJsonValue
    {
        public JsonPrimitiveValue(object value)
        {
            Value = value;
        }

        public object Value { get; }
        public JsonValueKind ValueKind => JsonValueKind.JPrimitive;
    }

    internal class JsonObjectValue : Dictionary<string, IJsonValue>, IJsonValue
    {
        public JsonObjectValue()
        { }

        public JsonValueKind ValueKind => JsonValueKind.JObject;

        public bool TryGetPropertyValue(string name, out IJsonValue value)
        {
            if (TryGetValue(name, out value))
            {
                return true;
            }

            return false;
        }
    }

    internal class JsonArrayValue : List<IJsonValue>, IJsonValue
    {
        public JsonValueKind ValueKind => JsonValueKind.JArray;
    }
}
