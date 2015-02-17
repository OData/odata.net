//---------------------------------------------------------------------
// <copyright file="CombinatorialDimension.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.CombinatorialEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.OData.Utils.Common;

    /// <summary>
    /// Represents a single dimension in combinatorial engine
    /// </summary>
    public class CombinatorialDimension
    {
        /// <summary>
        /// The name of the dimension
        /// </summary>
        private string name;

        /// <summary>
        /// Values of the dimension
        /// </summary>
        private ReadOnlyCollection<object> values;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the dimension</param>
        /// <param name="values">The values of the dimension. This is enumerated in this constructor and the values are stored in a seprate list.</param>
        public CombinatorialDimension(string name, IEnumerable values)
        {
            ExceptionUtilities.CheckStringNotNullOrEmpty(name, "Name of the dimension needs to be specified and must not be empty.");
            ExceptionUtilities.CheckArgumentNotNull(values, "values");

            this.name = name;
            this.values = new List<object>(values.Cast<object>()).AsReadOnly();
        }

        /// <summary>
        /// The name of the dimension
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// The values
        /// </summary>
        public ReadOnlyCollection<object> Values
        {
            get { return this.values; }
        }
    }
}
