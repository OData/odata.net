//---------------------------------------------------------------------
// <copyright file="FullCombinatorialEngineProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.CombinatorialEngine
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of combinatorial engine provider which iterates through all possible combinations
    /// </summary>
    public class FullCombinatorialEngineProvider : ICombinatorialEngineProvider
    {
        /// <summary>
        /// Creates a new combinatorial engine with the specified dimensions.
        /// </summary>
        /// <param name="dimensions">Dimensions to create combinations from.</param>
        /// <returns>The new combinatorial engine to be used.</returns>
        public ICombinatorialEngine CreateEngine(IEnumerable<CombinatorialDimension> dimensions)
        {
            return new FullCombinatorialEngine(dimensions);
        }

        /// <summary>
        /// Implementation of combinatorial engine which iterates through all possible combinations
        /// </summary>
        private class FullCombinatorialEngine : ICombinatorialEngine
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

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dimensions">The dimensions to use.</param>
            public FullCombinatorialEngine(IEnumerable<CombinatorialDimension> dimensions)
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
                                // Done iterating
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
            /// This combinatorial engine does not support logging.
            /// </summary>
            /// <param name="combination">Current combination number.</param>
            public void LogCombinationNumber(int combination)
            {
            }
        }
    }
}
