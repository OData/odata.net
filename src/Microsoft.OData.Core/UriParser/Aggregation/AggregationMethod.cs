//---------------------------------------------------------------------
// <copyright file="AggregationMethod.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.OData.UriParser.Aggregation
{
    /// <summary>
    /// Enumeration of methods used in the aggregation clause
    /// </summary>
    public enum AggregationMethod
    {
        /// <summary>The aggregation method Sum.</summary>
        Sum,

        /// <summary>The aggregation method Min.</summary>
        Min,

        /// <summary>The aggregation method Max.</summary>
        Max,

        /// <summary>The aggregation method Average.</summary>
        Average,

        /// <summary>The aggregation method CountDistinct.</summary>
        CountDistinct,

        /// <summary>The aggregation method Count. Used only internally to represent the virtual property $count.</summary>
        VirtualPropertyCount,

        /// <summary>A custom aggregation method.</summary>
        Custom
    }

    /// <summary>
    /// Class that encapsulates all the information needed to define a aggregation method.
    /// </summary>
    public sealed class AggregationMethodDefinition
    {
        /// <summary>Returns a definition for the sum aggregation method.</summary>
        public static AggregationMethodDefinition Sum = new AggregationMethodDefinition(AggregationMethod.Sum);

        /// <summary>Returns a definition for the min aggregation method.</summary>
        public static AggregationMethodDefinition Min = new AggregationMethodDefinition(AggregationMethod.Min);

        /// <summary>Returns a definition for the max aggregation method.</summary>
        public static AggregationMethodDefinition Max = new AggregationMethodDefinition(AggregationMethod.Max);

        /// <summary>Returns a definition for the average aggregation method.</summary>
        public static AggregationMethodDefinition Average = new AggregationMethodDefinition(AggregationMethod.Average);

        /// <summary>Returns a definition for the countdistinct aggregation method.</summary>
        public static AggregationMethodDefinition CountDistinct = new AggregationMethodDefinition(AggregationMethod.CountDistinct);

        /// <summary>Returns a definition for the aggregation method used to calculate $count.</summary>
        public static AggregationMethodDefinition VirtualPropertyCount = new AggregationMethodDefinition(AggregationMethod.VirtualPropertyCount);

        /// <summary>Private constructor. Instances should be acquired via static fields of via Custom method.</summary>
        /// <param name="aggregationMethodType">The <see cref="AggregationMethod"/> of this method definition.</param>
        private AggregationMethodDefinition(AggregationMethod aggregationMethodType)
        {
            this.MethodKind = aggregationMethodType;
        }

        /// <summary>Returns the <see cref="AggregationMethod"/> of this method definition.</summary>
        public AggregationMethod MethodKind { get; private set; }

        /// <summary>Returns the label of this method definition.</summary>
        public string MethodLabel { get; private set; }

        /// <summary>Creates a custom method definition from it's label.</summary>
        /// <param name="customMethodLabel">The label to call the custom method definition.</param>
        /// <returns>The custom method created.</returns>
        public static AggregationMethodDefinition Custom(string customMethodLabel)
        {
            ExceptionUtils.CheckArgumentNotNull(customMethodLabel, "customMethodLabel");

            // Custom aggregation methods MUST use a namespace-qualified name (see [OData-ABNF]), i.e. contain at least one dot.
            Debug.Assert(customMethodLabel.Contains(OData.ExpressionConstants.SymbolDot, StringComparison.Ordinal));

            var aggregationMethod = new AggregationMethodDefinition(AggregationMethod.Custom);
            aggregationMethod.MethodLabel = customMethodLabel;
            return aggregationMethod;
        }
    }
}
