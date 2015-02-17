//---------------------------------------------------------------------
// <copyright file="RemoveMLEAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Class for removing the MLE annotation from an EntityInstance
    /// </summary>
    class RemoveMLEAnnotation : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Removes the MLE annotation from an EntityInstance
        /// <param name="payloadElement">The entity instance to be modified</param>
        /// </summary>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var annotation = payloadElement.Annotations.Where(a => a is IsMediaLinkEntryAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);
            base.Visit(payloadElement);
        }

    }
}
