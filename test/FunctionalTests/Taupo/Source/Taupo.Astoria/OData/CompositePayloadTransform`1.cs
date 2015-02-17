//---------------------------------------------------------------------
// <copyright file="CompositePayloadTransform`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Payload transform class which composes a set of transforms.
    /// </summary>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public class CompositePayloadTransform<TPayload> : IPayloadTransform<TPayload>
    {
        /// <summary>
        /// Holds all the transforms passed to the class constructor.
        /// </summary>
        private IPayloadTransform<TPayload>[] payloadTransforms;

        /// <summary>
        /// Initializes a new instance of the CompositePayloadTransform class
        /// and sets the payload transforms.
        /// </summary>
        /// <param name="transforms">Array of transform instances.</param>
        public CompositePayloadTransform(params IPayloadTransform<TPayload>[] transforms)
        {
            this.payloadTransforms = transforms;
        }

        /// <summary>
        /// Transforms the original payload by applying all composable transforms to it.
        /// </summary>
        /// <param name="originalPayload">Original payload.</param>
        /// <param name="transformedPayload">Transformed payload.</param>
        /// <returns>True if the transformation can be applied on the payload else returns false.</returns>
        public bool TryTransform(TPayload originalPayload, out TPayload transformedPayload)
        {
            ExceptionUtilities.CheckArgumentNotNull(this.payloadTransforms, "payloadTransforms");

            transformedPayload = default(TPayload);
            TPayload payload = originalPayload;

            foreach (var transform in this.payloadTransforms)
            {
                if (transform.TryTransform(payload, out transformedPayload))
                {
                    payload = transformedPayload;
                }
                else
                {
                    return false;
                }
            }

            return (transformedPayload != null) ? true : false;
        }
    }
}
