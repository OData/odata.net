//---------------------------------------------------------------------
// <copyright file="ODataLibPayloadTransformFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.PayloadTransformation
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Payload transform factory for generating payload transform instances.
    /// </summary>
    public class ODataLibPayloadTransformFactory : DefaultPayloadTransformFactory
    {
        /// <summary>
        /// Gets or sets injected test parameter AddWhiteSpaceJsonTransform.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AddWhiteSpaceJsonTransform AddWhiteSpaceJsonTransform { get; set; }

        /// <summary>
        /// Gets or sets injected test parameter ReorderPayloadElementPropertiesTransform.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ReorderPayloadElementPropertiesTransform ReorderPayloadElementPropertiesTransform { get; set; }
    }
}
