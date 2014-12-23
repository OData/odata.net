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
    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataDeltaWriter"/>.
    /// </summary>
    public sealed class ODataDeltaSerializationInfo
    {
        /// <summary>
        /// The navigation source name of the entry/source entry to be written. Should be fully qualified if the navigatio nsource is not in the default container.
        /// </summary>
        private string navigationSourceName;

        /// <summary>
        /// The navigation source name of the entry/source entry to be written. Should be fully qualified if the navigation source is not in the default container.
        /// </summary>
        public string NavigationSourceName
        {
            get
            {
                return this.navigationSourceName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "NavigationSourceName");
                this.navigationSourceName = value;
            }
        }

        /// <summary>
        /// Validates the <paramref name="serializationInfo"/> instance.
        /// </summary>
        /// <param name="serializationInfo">The serialization info instance to validate.</param>
        /// <returns>The <paramref name="serializationInfo"/> instance.</returns>
        internal static ODataDeltaSerializationInfo Validate(ODataDeltaSerializationInfo serializationInfo)
        {
            if (serializationInfo != null)
            {
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.NavigationSourceName, "serializationInfo.EntitySetName");
            }

            return serializationInfo;
        }
    }
}
