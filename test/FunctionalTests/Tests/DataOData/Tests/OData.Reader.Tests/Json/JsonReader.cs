//---------------------------------------------------------------------
// <copyright file="JsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using System.IO;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// The test wrapper of the JsonReader implementation in the product which is internal.
    /// </summary>
    public class JsonReader
    {
        /// <summary>
        /// The instance of the JsonReader from the product.
        /// </summary>
        protected object instance;

        /// <summary>
        /// Assertion handler used for validation.
        /// </summary>
        protected AssertionHandler assert;

        /// <summary>
        /// The type of the JsonReader from the product.
        /// </summary>
        private static readonly Type JsonReaderType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.Json.JsonReader");

        /// <summary>
        /// Creates new instance of the JsonReader.
        /// </summary>
        /// <param name="textReader">The text reader to read the input from.</param>
        /// <param name="assert">Optional assertion handler to use to verify the behavior of the reader.</param>
        /// <param name="isIeee754Compatible">If it is IEEE754Compatible</param>
        public JsonReader(TextReader textReader, AssertionHandler assert, bool isIeee754Compatible)
            : this(ReflectionUtils.CreateInstance(JsonReaderType, textReader, isIeee754Compatible), assert)
        {
        }

        /// <summary>
        /// Creates a new JsonReader wrapper with a given Microsoft.OData.Json.JsonReader instance.
        /// </summary>
        /// <param name="instance">The JsonReader instance to use.</param>
        /// <param name="assert">Optional assertion handler to use to verify the behavior of the reader.</param>
        protected JsonReader(object instance, AssertionHandler assert)
        {
            this.instance = instance;
            this.assert = assert;

            if (this.assert != null)
            {
                this.assert.IsNull(this.Value, "The Value should return null on newly created JsonReader.");
                this.assert.AreEqual(JsonNodeType.None, this.NodeType, "The initial NodeType of the JsonReader should be None.");
            }
        }

        /// <summary>
        /// Reads the next node from the input.
        /// </summary>
        /// <returns>true if next node was read, false if end of input was reached.</returns>
        public virtual bool Read()
        {
            bool result = (bool)ReflectionUtils.InvokeMethod(this.instance, "Read");
            if (this.assert != null)
            {
                this.assert.IsTrue(JsonNodeType.None != this.NodeType, "After Read NodeType should never be None.");
                if (result)
                {
                    this.assert.IsTrue(JsonNodeType.EndOfInput != this.NodeType, "After Read which returned true, the NodeType should never be EndOfInput.");
                }
                else
                {
                    this.assert.AreEqual(JsonNodeType.EndOfInput, this.NodeType, "After Read which returns false, the NodeType should be EndOfInput.");
                }
            }

            return result;
        }

        /// <summary>
        /// The type of the node read.
        /// </summary>
        public JsonNodeType NodeType
        {
            get
            {
                JsonNodeType nodeType = (JsonNodeType)(int)ReflectionUtils.GetProperty(this.instance, "NodeType");
                if (this.assert != null)
                {
                    object nodeValue = this.Value;
                    switch (nodeType)
                    {
                        case JsonNodeType.StartObject:
                        case JsonNodeType.StartArray:
                        case JsonNodeType.EndObject:
                        case JsonNodeType.EndArray:
                        case JsonNodeType.EndOfInput:
                        case JsonNodeType.None:
                            this.assert.IsNull(nodeValue, "The node type " + nodeType.ToString() + " should have a null NodeValue.");
                            break;
                        case JsonNodeType.Property:
                            this.assert.IsNotNull(nodeValue, "The Value for a Property must not be null.");
                            this.assert.AreEqual(typeof(string), nodeValue.GetType(), "The Value for a Property must be a string.");
                            break;
                        case JsonNodeType.PrimitiveValue:
                            if (nodeValue != null)
                            {
                                Type type = nodeValue.GetType();
                                if (type != typeof(bool) && type != typeof(string) && type != typeof(int) && type != typeof(double) && type != typeof(DateTime) && type != typeof(DateTimeOffset))
                                {
                                    this.assert.Fail("Value for PrimitiveValue must be either null or one of the expected primitive types. But a value '" + nodeValue.ToString() + "' of type '" + type.ToString() + "' was found.");
                                }
                            }
                            break;
                        default:
                            this.assert.Fail("The Read method returned unrecognized node type '" + nodeType.ToString() + "'. Was a new node type added without test update?");
                            break;
                    }
                }

                return nodeType;
            }
        }

        /// <summary>
        /// The value of the node.
        /// For PrimitiveValue this is the value found.
        /// For Property this is the name of the property.
        /// For all other types of nodes this is null.
        /// </summary>
        public object Value
        {
            get
            {
                return ReflectionUtils.GetProperty(this.instance, "Value");
            }
        }
    }
}
