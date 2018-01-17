//---------------------------------------------------------------------
// <copyright file="ODataDeltaSerializationInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataDeltaWriter"/>.
    /// </summary>
    public sealed class ODataDeltaSerializationInfo
    {
        /// <summary>
        /// The navigation source name of the resource/source resource to be written. Should be fully qualified if the navigatio nsource is not in the default container.
        /// </summary>
        private string navigationSourceName;

        /// <summary>
        /// The navigation source name of the resource/source resource to be written. Should be fully qualified if the navigation source is not in the default container.
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
