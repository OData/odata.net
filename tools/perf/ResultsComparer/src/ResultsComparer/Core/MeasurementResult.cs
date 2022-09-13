//---------------------------------------------------------------------
// <copyright file="MeasurementResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core
{
    /// <summary>
    /// Stores the result of a single test. The result could be based on a single measurement
    /// or an aggregate value (e.g. average) based on multiple measurement iterations for the same test.
    /// </summary>
    public class MeasurementResult
    {
        /// <summary>
        /// The measured result of the test.
        /// </summary>
        public double Result { get; set; }

        /// <summary>
        /// The modality of the distribution in case the <see cref="Result"/> is the average
        /// of multiple measurement iterations.
        /// </summary>
        public Modality Modality { get; set; }
    }
}
