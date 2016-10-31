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

namespace Microsoft.Data.OData
{
    /// <summary>
    /// Annotation which stores the type name to serialize.
    /// </summary>
    /// <remarks>
    /// This annotation will be recognized on ODataEntry, ODataComplexValue, ODataCollectionValue and ODataPrimitiveValue.
    /// </remarks>
    public sealed class SerializationTypeNameAnnotation
    {
        /// <summary> Gets or sets the type name to serialize, for the annotated item. </summary>
        /// <returns>The type name to serialize, for the annotated item.</returns>
        /// <remarks>
        /// If this property is null, no type name will be written.
        /// If this property is non-null, the property value will be used as the type name written to the payload.
        /// If this annotation is present, it always overrides the type name specified on the annotated item.
        /// If this annotation is not present, the value of the TypeName property on the ODataEntry, ODataComplexValue or ODataCollectionValue
        /// is used as the type name in the payload.
        /// </remarks>
        public string TypeName { get; set; }
    }
}
