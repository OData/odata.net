//   OData .NET Libraries ver. 5.6.3
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

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.Data.OData.Json
#endif
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Enumeration of all JSON node type.
    /// </summary>
#if ODATALIB_PUBLICJSONREADER
    public
#else
    internal
#endif
    enum JsonNodeType
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
}
