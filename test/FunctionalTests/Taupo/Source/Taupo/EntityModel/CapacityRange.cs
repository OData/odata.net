//---------------------------------------------------------------------
// <copyright file="CapacityRange.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents capacity range.
    /// </summary>
    public class CapacityRange
    {
        private static readonly CapacityRange zeroRange = new CapacityRange(0, 0);
        private static readonly CapacityRange anyRange = new CapacityRange(-1, -1);
        private static readonly CapacityRange exactlyOneRange = new CapacityRange(1, 1);
        private static readonly CapacityRange oneOrMoreRange = new CapacityRange(1, -1);
        private static readonly CapacityRange noMoreThanOneRange = new CapacityRange(-1, 1);

        private CapacityRange(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Gets CapacityRange with 0 capacity.
        /// </summary>
        /// <returns>CapacityRange with 0 capacity.</returns>
        public static CapacityRange Zero
        {
            get { return zeroRange; }
        }

        /// <summary>
        /// Gets CapacityRange with no boundary on minimum or maximum capacity.
        /// </summary>
        /// <returns>CapacityRange with no boundary on minimum or maximum capacity.</returns>
        public static CapacityRange Any
        {
            get { return anyRange; }
        }

        /// <summary>
        /// Gets CapacityRange with exactly 1 element.
        /// </summary>
        /// <returns>CapacityRange with at least 1 element.</returns>
        public static CapacityRange ExactlyOne
        {
            get { return exactlyOneRange; }
        }

        /// <summary>
        /// Gets CapacityRange with at least 1 element.
        /// </summary>
        /// <returns>CapacityRange with at least 1 element.</returns>   
        public static CapacityRange AtLeastOne
        {
            get { return oneOrMoreRange; }
        }

        /// <summary>
        /// Gets CapacityRange with at most 1 element.
        /// </summary>
        /// <returns>CapacityRange with at most 1 element.</returns>   
        public static CapacityRange AtMostOne
        {
            get { return noMoreThanOneRange; }
        }

        /// <summary>
        /// Gets the minimum capacity.
        /// </summary>
        /// <value>The minimum capacity.</value>
        public int Min { get; private set; }

        /// <summary>
        /// Gets the maximum capacity.
        /// </summary>
        /// <value>The maximum capacity.</value>
        public int Max { get; private set; }

        /// <summary>
        /// Creates CapacityRange with the specified minimum and maximum.
        /// </summary>
        /// <param name="min">The minimum capacity.</param>
        /// <param name="max">The maximum capacity.</param>
        /// <returns>CapacityRange with specified minimum and maximum.</returns>      
        public static CapacityRange Within(int min, int max)
        {
            ValidateBoundary(min, "min");

            ExceptionUtilities.CheckValidRange(min, "min", max, "max");

            return new CapacityRange(min, max);
        }

        /// <summary>
        /// Creates CapacityRange with the specified minimum and no boundary for maximum.
        /// </summary>
        /// <param name="min">The minimum capacity.</param>
        /// <returns>CapacityRange with the specified minimum and no boundary for maximum.</returns>        
        public static CapacityRange AtLeast(int min)
        {
            ValidateBoundary(min, "min");

            return new CapacityRange(min, -1);
        }

        /// <summary>
        /// Creates CapacityRange with the specified maximum and no boundary for minimum.
        /// </summary>
        /// <param name="max">The maximum capacity.</param>
        /// <returns>CapacityRange with the specified maximum and no boundary for minimum.</returns>
        public static CapacityRange AtMost(int max)
        {
            ValidateBoundary(max, "max");

            return new CapacityRange(-1, max);
        }

        /// <summary>
        /// Creates CapacityRange with the same maximum and minimum equal to the specified count.
        /// </summary>
        /// <param name="count">The capacity.</param>
        /// <returns>CapacityRange with the same maximum and minimum equal to the specified count.</returns>
        public static CapacityRange Exactly(int count)
        {
            if (count == 0)
            {
                return Zero;
            }

            ValidateBoundary(count, "count");

            return new CapacityRange(count, count);
        }

        private static void ValidateBoundary(int boundary, string name)
        {
            if (boundary < 0)
            {
                throw new TaupoArgumentException("'" + name + "' argument cannot be less than zero.");
            }
        }
    }
}
