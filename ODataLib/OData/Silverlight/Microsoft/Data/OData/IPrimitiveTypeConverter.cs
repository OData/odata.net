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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System.Xml;
    using Microsoft.Data.OData.Json;
    #endregion

    /// <summary>
    /// Interface used for serialization and deserialization of primitive types.
    /// </summary>
    internal interface IPrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of a primitive type from the value in an Xml reader.
        /// </summary>
        /// <param name="reader">The Xml reader to use to read the value.</param>
        /// <returns>An instance of the primitive type.</returns>
        object TokenizeFromXml(XmlReader reader);

        /// <summary>
        /// Write the Atom representation of an instance of a primitive type to an XmlWriter.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="writer">The Xml writer to use to write the instance.</param>
        void WriteAtom(object instance, XmlWriter writer);

        /// <summary>
        /// Write the Json representation of an instance of a primitive type to a json writer.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        /// <param name="typeName">Type name of the instance to write. If the type name is null, the type name will not be written in the payload.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        void WriteJson(object instance, JsonWriter jsonWriter, string typeName, ODataVersion odataVersion);
    }
}
