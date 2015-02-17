//---------------------------------------------------------------------
// <copyright file="RemoveOperationsNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Common
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    public sealed class RemoveOperationsNormalizer : ODataPayloadElementVisitorBase
    {
        public static ODataPayloadElement Normalize(ODataPayloadElement payloadElement)
        {
            new RemoveOperationsNormalizer().Recurse(payloadElement);
            return payloadElement;
        }

        public override void Visit(EntityInstance payloadElement)
        {
            base.Visit(payloadElement);

            // Remove the actions and functions
            payloadElement.ServiceOperationDescriptors.Clear();
        }
    }
}