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

#if ASTORIA_SERVER
namespace System.Data.Services.Parsing
#else
namespace Microsoft.Data.OData
#endif
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.IO;
    using System.Xml;
    #endregion

    /// <summary>
    /// Handles serialization and deserialization for a specified set of primitive types.
    /// </summary>
    internal sealed class PrimitiveConverter
    {
        /// <summary>Set of type converters that are known to this instance.</summary>
        private readonly Dictionary<Type, IPrimitiveTypeConverter> primitiveTypeConverters;

        /// <summary>
        /// Create a new instance of the converter.
        /// </summary>
        /// <param name="primitiveTypeConverters">Set of type converters to register.</param>
        internal PrimitiveConverter(KeyValuePair<Type, IPrimitiveTypeConverter>[] primitiveTypeConverters)
        {
#if !ASTORIA_SERVER
            DebugUtils.CheckNoExternalCallers();
#endif

            Debug.Assert(primitiveTypeConverters != null && primitiveTypeConverters.Length != 0, "PrimitiveConverter requires a non-null and non-empty array of type converters.");
            this.primitiveTypeConverters = new Dictionary<Type, IPrimitiveTypeConverter>(EqualityComparer<Type>.Default);
            foreach (KeyValuePair<Type, IPrimitiveTypeConverter> primitiveTypeConverter in primitiveTypeConverters)
            {
                this.primitiveTypeConverters.Add(primitiveTypeConverter.Key, primitiveTypeConverter.Value);
            }
        }

        /// <summary>
        /// Try to create an object of type <paramref name="targetType"/> from the value in <paramref name="reader" />.
        /// </summary>
        /// <param name="reader">XmlReader to use to read the value.</param>
        /// <param name="targetType">Expected type of the value in the reader.</param>
        /// <param name="tokenizedPropertyValue">Object of type <paramref name="targetType"/>, null if no object could be created.</param>
        /// <returns>True if the value was converted to the specified type, otherwise false.</returns>
        internal bool TryTokenizeFromXml(XmlReader reader, Type targetType, out object tokenizedPropertyValue)
        {
#if !ASTORIA_SERVER
            DebugUtils.CheckNoExternalCallers();
#endif
            tokenizedPropertyValue = null;

            Debug.Assert(reader != null, "Expected a non-null XmlReader.");
            Debug.Assert(reader.NodeType == XmlNodeType.Element, "Expected the reader to be at the start of an element.");
            Debug.Assert(targetType != null, "Expected a valid type to convert value.");

            IPrimitiveTypeConverter primitiveTypeConverter = null;
            if (this.TryGetConverter(targetType, out primitiveTypeConverter))
            {
                tokenizedPropertyValue = primitiveTypeConverter.TokenizeFromXml(reader);
                Debug.Assert(reader.NodeType == XmlNodeType.EndElement, "Expected reader to be at the end of an element");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to write the XML representation of <paramref name="instance"/> to the specified <paramref name="writer"/>
        /// </summary>
        /// <param name="instance">Object to convert to XML representation.</param>
        /// <param name="writer">XmlWriter to use to write the converted value.</param>
        /// <returns>True if the value was written, otherwise false.</returns>
        internal bool TryWriteAtom(object instance, XmlWriter writer)
        {
#if !ASTORIA_SERVER
            DebugUtils.CheckNoExternalCallers();
#endif
            Debug.Assert(instance != null, "Expected a non-null instance to write.");
            Debug.Assert(writer != null, "Expected a non-null XmlWriter.");

            return this.TryWriteValue(instance, (ptc) => ptc.WriteAtom(instance, writer));
        }

        /// <summary>
        /// Try to write the JSON representation of <paramref name="instance"/> to the specified <paramref name="writer"/>
        /// </summary>
        /// <param name="instance">Object to convert to JSON representation.</param>
        /// <param name="writer">TextWriter to use to write the converted value.</param>
        /// <returns>True if the value was written, otherwise false.</returns>
        internal bool TryWriteJson(object instance, TextWriter writer)
        {
#if !ASTORIA_SERVER
            DebugUtils.CheckNoExternalCallers();
#endif
            Debug.Assert(instance != null, "Expected a non-null instance to write.");
            Debug.Assert(writer != null, "Expected a non-null TextWriter.");

            return this.TryWriteValue(instance, (ptc) => ptc.WriteJson(instance, writer));
        }

#if ASTORIA_SERVER
        /// <summary>
        /// Determines whether or not there is a type converter registered for the specified type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True is there is a converter registed for <paramref name="type"/>, otherwise false.</returns>
        internal bool CanConvert(Type type)
        {
            return this.primitiveTypeConverters.Keys.Where(k => k.IsAssignableFrom(type)).FirstOrDefault() != null;
        }
#endif
        /// <summary>
        /// Tries to write the value of object instance using a registered primitive type converter.
        /// </summary>
        /// <param name="instance">Object to write.</param>
        /// <param name="writeMethod">Method to use when writing the value, if a registered converter is found for the type.</param>
        /// <returns>True if the value was written using a registered primitive type converter, otherwise false.</returns>
        private bool TryWriteValue(object instance, Action<IPrimitiveTypeConverter> writeMethod)
        {
            Type instanceType = instance.GetType();

            IPrimitiveTypeConverter primitiveTypeConverter = null;
            if (this.TryGetConverter(instanceType, out primitiveTypeConverter))
            {
                writeMethod(primitiveTypeConverter);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the primitive type converter for the given type.
        /// </summary>
        /// <param name="type">Clr type whose primitive type converter needs to be returned.</param>
        /// <param name="primitiveTypeConverter">Converter for the given clr type.</param>
        /// <returns>True if a converter was found for the given type, otherwise returns false.</returns>
        private bool TryGetConverter(Type type, out IPrimitiveTypeConverter primitiveTypeConverter)
        {
            KeyValuePair<Type, IPrimitiveTypeConverter> bestMatch = new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(object), null);
            foreach (KeyValuePair<Type, IPrimitiveTypeConverter> possibleMatch in this.primitiveTypeConverters)
            {
                // If the current primitive type is assignable from the given parameter type and
                // is a more derived type from the previous match, then the current type is a better match.
                if (possibleMatch.Key.IsAssignableFrom(type) && bestMatch.Key.IsAssignableFrom(possibleMatch.Key))
                {
                    bestMatch = possibleMatch;
                }
            }

            primitiveTypeConverter = bestMatch.Value;
            return bestMatch.Value != null;
        }
    }
}

