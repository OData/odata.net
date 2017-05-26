//---------------------------------------------------------------------
// <copyright file="ODataMediaTypeFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces



    #endregion Namespaces

    /// <summary>
    /// A helper class to associate a <see cref="ODataFormat"/> with a media type.
    /// </summary>
    public sealed class ODataMediaTypeFormat
    {
        /// <summary>
        /// Constructor for<see cref="ODataMediaTypeFormat"/>
        /// </summary>
        /// <param name="mediaType">MediaType to be used.</param>
        /// <param name="format">Associated format.</param>
        public ODataMediaTypeFormat(ODataMediaType mediaType, ODataFormat format)
        {
            this.MediaType = mediaType;
            this.Format = format;
        }

        /// <summary>The media type.</summary>
        public ODataMediaType MediaType
        {
            get;
            internal set;
        }

        /// <summary>
        /// The <see cref="ODataFormat"/> for this media type.
        /// </summary>
        public ODataFormat Format
        {
            get;
            internal set;
        }
    }
}
