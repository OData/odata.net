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
    /// Class to provide additional serialization information to the <see cref="ODataMessageWriter"/> for an <see cref="ODataEntityReferenceLinks"/>.
    /// </summary>
    public sealed class ODataEntityReferenceLinksSerializationInfo
    {
        /// <summary>
        /// The source entity set name of the navigation property. Should be fully qualified if the entity set is not in the default container.
        /// </summary>
        private string sourceEntitySetName;

        /// <summary>
        /// The fully qualified type name of the declaring entity type of the navigation property if the declaring entity type is not the base type of the source entity set.
        /// Should be null if the declaring entity type is the base type of the source entity set.
        /// </summary>
        private string typecast;

        /// <summary>
        /// The name of the navigation property to write the entity reference links for.
        /// </summary>
        private string navigationPropertyName;

        /// <summary>
        /// The source entity set name of the navigation property. Should be fully qualified if the entity set is not in the default container.
        /// </summary>
        public string SourceEntitySetName
        {
            get
            {
                return this.sourceEntitySetName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "SourceEntitySetName");
                this.sourceEntitySetName = value;
            }
        }

        /// <summary>
        /// The fully qualified type name of the declaring entity type of the navigation property if the declaring entity type is not the base type of the source entity set.
        /// Should be null if the declaring entity type is the base type of the source entity set.
        /// </summary>
        public string Typecast
        {
            get
            {
                return this.typecast;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotEmpty(value, "Typecast");
                this.typecast = value;
            }
        }

        /// <summary>
        /// The name of the navigation property to write the entity reference links for.
        /// </summary>
        public string NavigationPropertyName
        {
            get
            {
                return this.navigationPropertyName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "NavigationPropertyName");
                this.navigationPropertyName = value;
            }
        }

        /// <summary>
        /// Validates the <paramref name="serializationInfo"/> instance.
        /// </summary>
        /// <param name="serializationInfo">The serialization info instance to validate.</param>
        /// <returns>The <paramref name="serializationInfo"/> instance.</returns>
        internal static ODataEntityReferenceLinksSerializationInfo Validate(ODataEntityReferenceLinksSerializationInfo serializationInfo)
        {
            DebugUtils.CheckNoExternalCallers();
            if (serializationInfo != null)
            {
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.SourceEntitySetName, "serializationInfo.SourceEntitySetName");
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.NavigationPropertyName, "serializationInfo.NavigationPropertyName");
            }

            return serializationInfo;
        }
    }
}
