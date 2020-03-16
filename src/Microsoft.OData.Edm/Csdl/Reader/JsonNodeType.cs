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

        IJsonValue Parent { get; }

        // If the parent is an Array, we use this Index to figure the Number of the item in the Array
        int Index { get; }
    }

    // We can't use this Base class
    //internal abstract class JsonValueVase : IJsonValue
    //{
    //    protected JsonValueVase(IJsonValue parent, int index)
    //    {
    //        Parent = parent;
    //        Index = index;
    //    }

    //    public IJsonValue Parent { get; }

    //    public int Index { get; }

    //    public abstract JsonValueKind ValueKind { get;}
    //}

    internal class JsonPrimitiveValue : IJsonValue
    {
        public JsonPrimitiveValue(object value, IJsonValue parent, int index = -1)
        {
            Value = value;
            Parent = parent;
            Index = index;
        }

        public object Value { get; }

        public JsonValueKind ValueKind => JsonValueKind.JPrimitive;

        public IJsonValue Parent { get; }
        public int Index { get; }
    }

    internal class JsonObjectValue : Dictionary<string, IJsonValue>, IJsonValue
    {
        public JsonObjectValue(IJsonValue parent, int index = -1)
        {
            Parent = parent;
            Index = index;
        }

        public JsonValueKind ValueKind => JsonValueKind.JObject;

        public IJsonValue Parent { get; }
        public int Index { get; }
    }

    internal class JsonArrayValue : List<IJsonValue>, IJsonValue
    {
        public JsonArrayValue(IJsonValue parent, int index = -1)
        {
            Parent = parent;
            Index = index;
        }

        public JsonValueKind ValueKind => JsonValueKind.JArray;

        public IJsonValue Parent { get; }
        public int Index { get; }
    }
}
