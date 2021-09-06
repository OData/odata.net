//---------------------------------------------------------------------
// <copyright file="WriterCombinatiorialEngineProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine
{
    using System;
    using System.Collections.Generic;
    using ApprovalTests;
    using ApprovalTests.Core;
    using Microsoft.Test.OData.Utils.Common;
    using Approvals = ApprovalTests.Approvals;

    using Microsoft.Test.OData.Utils.CombinatorialEngine;

    public class WriterCombinatorialEngineProvider : ICombinatorialEngineProvider
    {
        private Func<string> baselineCallback;
        private Action<int> combinationCallback;

        // <summary>Verify result or not</summary>
        private bool skipVerify = false;
        private string approvalFileSoucePath = null;

        /// <summary>
        /// Set to skip verify
        /// </summary>
        public WriterCombinatorialEngineProvider SkipVerify(bool skip = true)
        {
            this.skipVerify = skip;
            return this;
        }

        /// <summary>
        /// Set approvalNamer
        /// </summary>
        public WriterCombinatorialEngineProvider SetApprovalFileSourcePath(string sourcePath)
        {
            this.approvalFileSoucePath = sourcePath;
            return this;
        }

        public ICombinatorialEngine CreateEngine(IEnumerable<CombinatorialDimension> dimensions)
        {
            return new WriterCombinatorialEngine(dimensions)
                .SetBaselineCallback(this.baselineCallback)
                .SetLogCombinationCallback(this.combinationCallback)
                .SkipVerify(skipVerify).SetApprovalFileSoucePath(approvalFileSoucePath);
        }

        /// <summary>
        /// Sets the callback function for retrieving the baseline
        /// </summary>
        /// <param name="callback">The baseline callback function.</param>
        /// <returns>This for more fluent usage.</returns>
        public WriterCombinatorialEngineProvider SetBaselineCallback(Func<string> callback)
        {
            this.baselineCallback = callback;
            return this;
        }

        /// <summary>
        /// Sets the callback function for logging the combination to the baseline.
        /// </summary>
        /// <param name="callback">The callback for the engine to log the combination number.</param>
        /// <returns>This for more fluent usage.</returns>
        public WriterCombinatorialEngineProvider SetLogCombinationCallback(Action<int> callback)
        {
            this.combinationCallback = callback;
            return this;
        }

        /// <summary>
        /// Implementation of combinatorial engine which iterates through all possible combinations
        /// </summary>
        public class WriterCombinatorialEngine : ICombinatorialEngine
        {
            /// <summary>
            /// List of dimensions
            /// </summary>
            private List<CombinatorialDimension> dimensions;

            /// <summary>
            /// List of indeces to values in all dimensions - this represents the current combination
            /// </summary>
            private int[] currentDimensionIndices;

            /// <summary>
            /// The dimensions and their values for the current combination
            /// </summary>
            private Dictionary<string, object> currentDimensionValues;
            private Func<string> baselineCallback;
            private Action<int> combinationCallback;

            // <summary>Verify result or not</summary>
            private bool skipVerify = false;
            private string approvalFileSoucePath = null;

            /// <summary>
            /// Set to skip verify
            /// </summary>
            public WriterCombinatorialEngine SkipVerify(bool skip = true)
            {
                this.skipVerify = skip;
                return this;
            }

            /// <summary>
            /// Set approvalNamer
            /// </summary>
            public WriterCombinatorialEngine SetApprovalFileSoucePath(string sourcePath)
            {
                this.approvalFileSoucePath = sourcePath;
                return this;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dimensions">The dimensions to use.</param>
            public WriterCombinatorialEngine(IEnumerable<CombinatorialDimension> dimensions)
            {
                this.dimensions = new List<CombinatorialDimension>(dimensions);
                this.currentDimensionIndices = new int[this.dimensions.Count];
                this.currentDimensionValues = null;

                if (this.dimensions.Count == 0)
                {
                    throw new ArgumentException("Must specify at least one dimension.", "dimensions");
                }

                foreach (var dimension in this.dimensions)
                {
                    if (dimension.Values.Count == 0)
                    {
                        throw new ArgumentException(string.Format("Dimension {0} doesn't contain any elements, the test would do nothing.", dimension.Name), "dimensions");
                    }
                }
            }

            public WriterCombinatorialEngine SetBaselineCallback(Func<string> callback)
            {
                this.baselineCallback = callback;
                return this;
            }

            public WriterCombinatorialEngine SetLogCombinationCallback(Action<int> callback)
            {
                this.combinationCallback = callback;
                return this;
            }

            /// <summary>
            /// List of dimensions - returns dimensions in the same order as the one specified when creating the engine.
            /// </summary>
            public IEnumerable<CombinatorialDimension> Dimensions
            {
                get { return this.dimensions; }
            }

            /// <summary>
            /// Returns the current values of all the dimensions, returns null if no combination is active
            /// </summary>
            public Dictionary<string, object> CurrentDimensionValues
            {
                get { return this.currentDimensionValues; }
            }

            /// <summary>
            /// Moves the engine to the next combination.
            /// </summary>
            /// <returns>True if there was another combination to return, false if no more combinations are available.</returns>
            public bool NextCombination()
            {
                if (this.currentDimensionValues == null)
                {
                    this.currentDimensionValues = new Dictionary<string, object>(this.dimensions.Count);
                    for (int i = 0; i < this.dimensions.Count; i++)
                    {
                        this.currentDimensionIndices[i] = 0;
                    }
                }
                else
                {
                    for (int index = this.currentDimensionIndices.Length - 1; index >= 0; index--)
                    {
                        this.currentDimensionIndices[index]++;
                        if (this.currentDimensionIndices[index] == this.dimensions[index].Values.Count)
                        {
                            this.currentDimensionIndices[index] = 0;
                            if (index == 0)
                            {
                                string baseline = baselineCallback();
                                if (!skipVerify && !string.IsNullOrEmpty(baseline))
                                {
                                    if (approvalFileSoucePath != null)
                                        Approvals.Verify(new ApprovalTextWriter(baseline), new CustomSourcePathNamer(approvalFileSoucePath), Approvals.GetReporter());
                                    else
                                        Approvals.Verify(baseline);
                                }

                                return false;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // Set the parameters
                for (int i = 0; i < this.dimensions.Count; i++)
                {
                    this.currentDimensionValues[this.dimensions[i].Name] = this.dimensions[i].Values[this.currentDimensionIndices[i]];
                }

                return true;
            }

            /// <summary>
            /// Logs the combination number if a callback is provided.
            /// </summary>
            /// <param name="combination">Current combination number.</param>
            public void LogCombinationNumber(int combination)
            {
                if (combinationCallback != null)
                {
                    combinationCallback(combination);
                }
            }
        }
    }
}
