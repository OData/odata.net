//---------------------------------------------------------------------
// <copyright file="BatchBoundaryAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Annotations
{
    using System;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Annotation on payloads to represent the overall batch boundary
    /// </summary>
    public class BatchBoundaryAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Constructor that takes a batch boundary.
        /// </summary>
        /// <param name="batchBoundary">The (possibly quoted) batch boundary to use.</param>
        public BatchBoundaryAnnotation(string batchBoundary)
        {
            ExceptionUtilities.CheckArgumentNotNull(batchBoundary, "batchBoundary");

            this.BatchBoundaryInHeader = batchBoundary;

            // Check whether the batch boundary is quoted or not.
            if (batchBoundary != null && 
                batchBoundary.Length > 1 && 
                batchBoundary[0] == '"' && 
                batchBoundary[batchBoundary.Length - 1] == '"')
            {
                // Quoted boundary string
                batchBoundary = batchBoundary.Substring(1, batchBoundary.Length - 2);

                // Now also un-escape any escaped characters
                if (batchBoundary.IndexOf('\\') >= 0)
                {
                    StringBuilder builder = new StringBuilder(batchBoundary.Length);
                    for (int i = 0; i < batchBoundary.Length; ++i)
                    {
                        char c = batchBoundary[i];
                        if (c == '\\' && i < batchBoundary.Length - 1)
                        {
                            // unconditionally add the next character
                            c = batchBoundary[++i];
                        }

                        builder.Append(c);
                    }

                    batchBoundary = builder.ToString();
                }
            }

            this.BatchBoundaryInPayload = batchBoundary;
        }

        /// <summary>
        /// Property for Batch boundary as used in the payload.
        /// </summary>
        public string BatchBoundaryInPayload { get; private set; }

        /// <summary>
        /// Property for Batch boundary as used in the header.
        /// </summary>
        public string BatchBoundaryInHeader { get; private set; }

        /// <summary>
        /// String representation of annotation
        /// </summary>
        public override string StringRepresentation
        {
            get { return this.BatchBoundaryInHeader; }
        }

        /// <summary>
        /// Clone to create a copy
        /// </summary>
        /// <returns></returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new BatchBoundaryAnnotation(this.BatchBoundaryInHeader);
        }
    }
}
