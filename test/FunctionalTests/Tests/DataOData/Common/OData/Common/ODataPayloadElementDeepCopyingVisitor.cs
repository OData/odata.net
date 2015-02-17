//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementDeepCopyingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;

    /// <summary>
    /// A visitor to make DeepCopy of ODataPayloadElement including the annotations.
    /// </summary>
    public class ODataPayloadElementDeepCopyingVisitor : ODataPayloadElementReplacingVisitor, IODataPayloadElementVisitor<ODataPayloadElement>
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        public ODataPayloadElementDeepCopyingVisitor() :
            base(true)
        {
        }

        /// <summary>
        /// Creates a deep copy of the payload and returns it
        /// </summary>
        /// <param name="payload"> the payload to create a deep copy of</param>
        /// <returns>deep copy of the payload</returns>
        public ODataPayloadElement DeepCopy(ODataPayloadElement payload)
        {
            return payload.Accept(this).RemoveAnnotations(typeof(ReplacedElementAnnotation));
        }
        
        /// <summary>
        /// Override to strip the replaced element annotation
        /// </summary>
        /// <param name="element">the copy of the original element</param>
        /// <returns>the copy with ReplacedElement annotation removed</returns>
        protected override ODataPayloadElement Recurse(ODataPayloadElement element)
        {
            return base.Recurse(element).RemoveAnnotations(typeof(ReplacedElementAnnotation));
        }
    }
}
