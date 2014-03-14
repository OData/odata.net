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
