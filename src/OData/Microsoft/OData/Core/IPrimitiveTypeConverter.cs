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

namespace Microsoft.OData.Core
{
    #region Namespaces

    using System.IO;
    using System.Xml;
    using Microsoft.OData.Core.Json;
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
        /// Write the Atom representation of an instance of a primitive type to an TextWriter.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="writer">The text writer to use to write the instance.</param>
        void WriteAtom(object instance, TextWriter writer);

        /// <summary>
        /// Write the Json Lite representation of an instance of a primitive type to a json writer.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        void WriteJsonLight(object instance, IJsonWriter jsonWriter);
    }
}
