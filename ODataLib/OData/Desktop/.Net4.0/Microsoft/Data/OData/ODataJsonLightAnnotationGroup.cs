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
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Class representing an annotation group in the JSON Light format.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    internal sealed class ODataJsonLightAnnotationGroup
    {
        /// <summary>The name of the annotation group.</summary>
        private string name;

        /// <summary>The (instance and property) annotations included in this annotation group.</summary>
        private IDictionary<string, object> annotations;

        /// <summary>
        /// The name of the annotation group.
        /// </summary>
        /// <remarks>The name has to be unique across the whole JSON Light payload.</remarks>
        public string Name
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.name;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.name = value;
            }
        }

        /// <summary>
        /// The (instance and property) annotations included in this annotation group.
        /// </summary>
        /// <remarks>The keys in the dictionary are the names of the annotations, the values are their values.</remarks>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We allow setting of all properties on public ODataLib OM classes.")]
        public IDictionary<string, object> Annotations
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.annotations; 
            }

            set 
            {
                DebugUtils.CheckNoExternalCallers();
                this.annotations = value; 
            }
        }
    }
}
