//---------------------------------------------------------------------
// <copyright file="CombinatorialEngine.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections;
    using System.Text;
    using AstoriaUnitTests.Data;

    /// <summary>
    /// Provides an engine to create all possible combinations for
    /// different values across different dimensions.
    /// </summary>
    /// <remarks>
    /// There are a few modes available:
    /// - FullCombinatorial:    all combinations are covered.
    /// - PairWise:             all pairwise combinations are covered.
    /// - EveryElement:         every element is covered.
    /// </remarks>
    public class CombinatorialEngine
    {
        #region Private fields.

        /// <summary>Dimensions to combine.</summary>
        private Dimension[] dimensions;

        /// <summary>Index of the current value of each dimension.</summary>
        private int[] valueIndexes;

        /// <summary>Before enumeration started flag.</summary>
        private bool beforeStart;

        /// <summary>The mode in which combinations are generated</summary>
        private CombinatorialEngineMode mode;

        /// <summary>Dimension being incremented in 'every element' mode.</summary>
        private int elementIncrementingDimension;

        #endregion Private fields.

        /// <summary>
        /// Initializes a new CombinatorialEngine from the specified
        /// dimensions.
        /// </summary>
        /// <param name='dimensions'>Dimensions to use in engine.</param>
        /// <returns>A new CombinatorialEngine instance.</returns>
        private CombinatorialEngine(Dimension[] dimensions)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException("dimensions");
            }
            if (dimensions.Length == 0)
            {
                throw new ArgumentException("Dimensions cannot be empty.");
            }
            if (Array.IndexOf(dimensions, null) != -1)
            {
                throw new ArgumentException("Dimensions cannot have null elements.");
            }

            SetupForDimensions(dimensions);
        }

        /// <summary>
        /// Initializes a new CombinatorialEngine from the specified
        /// dimensions.
        /// </summary>
        /// <param name='dimensions'>Dimensions to use in engine.</param>
        /// <returns>A new CombinatorialEngine instance.</returns>
        public static CombinatorialEngine FromDimensions(params Dimension[] dimensions)
        {
            return new CombinatorialEngine(dimensions);
        }

        /// <summary>
        /// Returns a description of the value for each dimension.
        /// </summary>
        /// <returns>
        /// A multiline description, [none] if the engine has not yet
        /// started to generate states.
        /// </returns>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void RunAllStates(CombinatorialEngine engine, ConfigurationSettings cs) {
        ///   while (engine.Next(cs)) {
        ///     Logger.Current.Log("Combination settings: " + engine.DescribeState());
        ///     /* do something with combinations in cs */
        ///   }
        /// }</code></example>
        public string DescribeState()
        {
            StringBuilder sb;   // Result of method.

            if (beforeStart)
                return "[none]";

            sb = new StringBuilder(dimensions.Length * 64);
            for (int i = 0; i < dimensions.Length; i++)
            {
                sb.Append(dimensions[i].Name);
                sb.Append(": [");
                object dimensionValue = dimensions[i].Values[valueIndexes[i]];
                if (dimensionValue == null)
                {
                    dimensionValue = "*null*";
                }
                else if (dimensionValue is string[])
                {
                    dimensionValue = "string[]:" + string.Join(",", (string[])dimensionValue);
                }
                sb.Append(dimensionValue);
                sb.Append("]");
                if (i < dimensions.Length - 1)
                {
                    sb.Append(System.Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a description of the value for each dimension that is suitable for a conditional breakpoint
        /// in Visual Studio using C# syntax.
        /// </summary>
        /// <returns>
        /// A single line description, [none] if the engine has not yet started to generate states.
        /// </returns>
        public string DescribeStateCondition()
        {
            if (beforeStart)
                return "[none]";

            var sb = new StringBuilder(dimensions.Length * 64);
            for (int i = 0; i < dimensions.Length; i++)
            {
                object dimensionValue = dimensions[i].Values[valueIndexes[i]];
                string valueExpression = "values[\"" + dimensions[i].Name + "\"]";
                if (dimensionValue == null)
                {
                    sb.Append(valueExpression + " == null");
                }
                else if (dimensionValue is string[])
                {
                    sb.Append(valueExpression + ".Join(',') == \"" + string.Join(",", (string[])dimensionValue) + "\"");
                }
                else if (dimensionValue is Int32)
                {
                    sb.Append("(int)" + valueExpression + " == " + dimensionValue.ToString());
                }
                else if (dimensionValue is bool)
                {
                    sb.Append("(bool)" + valueExpression + " == " + (((bool)dimensionValue) ? "true" : "false"));
                }
                else
                {
                    sb.Append(valueExpression + ".ToString() == \"" + dimensionValue.ToString() + "\"");
                }
                if (i < dimensions.Length - 1)
                {
                    sb.Append(" && ");
                }
            }
            return sb.ToString();
        }

        /// <summary>Whether there the engine has any content.</summary>
        public bool IsEmpty
        {
            get { return dimensions.Length > 0; }
        }

        /// <summary>The mode in which combinations will be generated.</summary>
        public CombinatorialEngineMode Mode
        {
            get { return this.mode; }
            set
            {
                if (this.mode != value)
                {
                    if (!beforeStart)
                    {
                        throw new InvalidOperationException("Cannot change combinatorial engine mode after enumeration has started.");
                    }

                    if (value == CombinatorialEngineMode.PairWise)
                    {
                        throw new NotImplementedException();
                    }

                    this.mode = value;
                }
            }
        }

        /// <summary>
        /// Sets the next set of values in the specified hash table.
        /// </summary>
        /// <param name='cs'>Hashtable with values to modify.</param>
        /// <returns>
        /// false if there are no more valid combinations, true otherwise.
        /// </returns>
        public bool Next(Hashtable cs)
        {
            int incrementingDimension;  // Index of dimension being incremented.
            bool validFound;            // Whether a valid combination was found.
            bool candidatesPending;     // Whether there are still combination
                                        // candidates to be evaluated.
            Dimension d;                // Dimension being evaluated.
            int valueIndex;             // Value being considered for dimension.

            if (this.dimensions.Length == 0)
            {
                return false;
            }

            // In full combinatorial mode, we start incrementing from
            // the left, and reset to the leftmost dimension whenever
            // we increment a value anywhere, to run all previous
            // combinations again.
            // 
            // In 'every element' mode, we start incrementing from the
            // left, but when we advance, we pick a single value for
            // the dimension we just finished with and keep moving.
            switch (this.mode)
            {
                case CombinatorialEngineMode.FullCombinatorial:
                    incrementingDimension = 0;
                    break;
                case CombinatorialEngineMode.EveryElement:
                    incrementingDimension = this.elementIncrementingDimension;
                    break;
                default:
                    throw new NotImplementedException();
            }

            validFound = false;
            do
            {
                d = this.dimensions[incrementingDimension];
                valueIndex = this.valueIndexes[incrementingDimension];

                // Consider the special case of 'before-start'.
                if (!this.beforeStart)
                {
                    valueIndex++;
                }
                else
                {
                    this.beforeStart = false;
                    if (this.mode == CombinatorialEngineMode.EveryElement)
                    {
                        for (int i = 0; i < dimensions.Length; i++)
                        {
                            this.valueIndexes[i] = dimensions[i].Values.Length / 2;
                        }
                    }
                }

                // Move to the next dimension if this one has no more values
                // to consider, otherwise verify acceptability.
                if (valueIndex == d.Values.Length)
                {
                    switch(this.mode)
                    {
                        case CombinatorialEngineMode.FullCombinatorial:
                            for (int i = incrementingDimension; i >= 0; i--)
                            {
                                valueIndexes[i] = 0;
                            }
                            break;
                        case CombinatorialEngineMode.EveryElement:
                            valueIndexes[incrementingDimension] = dimensions[incrementingDimension].Values.Length / 2;
                            elementIncrementingDimension++;
                            if (elementIncrementingDimension < valueIndexes.Length)
                            {
                                valueIndexes[elementIncrementingDimension] = -1;
                            }
                            break;
                    }
                    incrementingDimension++;
                }
                else
                {
                    valueIndexes[incrementingDimension] = valueIndex;
                    if (GetFiltersAreAcceptable())
                    {
                        validFound = true;
                    }
                    else
                    {
                        incrementingDimension = 0;
                    }
                }
                candidatesPending = incrementingDimension < dimensions.Length;
            } while (candidatesPending && !validFound);

            if (validFound)
            {
                PopulateWithDimensionValues(cs);
                return true;
            }
            else
                return false;
        }

        /// <summary>Resets the current engine to a state before enumeration.</summary>
        public void Reset()
        {
            this.elementIncrementingDimension = 0;
            this.beforeStart = true;
            for (int i = 0; i < this.valueIndexes.Length; i++)
            {
                this.valueIndexes[i] = 0;
            }
        }

        /// <summary>Filtering event fired when combination is evaluated.</summary>
        public FilteringEventHandler Filtering;

        /// <summary>Evaluates current filters.</summary>
        /// <returns>
        /// true if the current combination of filters is valid, false
        /// otherwise.
        /// </returns>
        private bool GetFiltersAreAcceptable()
        {
            bool result = true;

            if (Filtering != null)
            {
                Hashtable table = new Hashtable();
                PopulateWithDimensionValues(table);

                FilteringEventArgs e = new FilteringEventArgs(table, result);
                Filtering(this, e);

                result = e.IsAcceptable;
            }

            return result;
        }

        /// <summary>Populates the table with current dimension values.</summary>
        private void PopulateWithDimensionValues(Hashtable table)
        {
            int valueIndex; // Index of value for a given dimension.

            System.Diagnostics.Debug.Assert(table != null);
            for (int i = 0; i < dimensions.Length; i++)
            {
                valueIndex = valueIndexes[i];
                table[dimensions[i].Name] = dimensions[i].Values[valueIndex];
            }
        }

        /// <summary>
        /// Sets up the engine to use the specified dimensions in a
        /// combinatorial run.
        /// </summary>
        private void SetupForDimensions(Dimension[] dimensions)
        {
            System.Diagnostics.Debug.Assert(dimensions != null);
            
            this.dimensions = dimensions;
            valueIndexes = new int[dimensions.Length];
            beforeStart = true;
        }
    }

    /// <summary>Event arguments for combination state filtering.</summary>
    public class FilteringEventArgs: EventArgs
    {
        #region Constructors.

        /// <summary>Initializes a new FilteringEventArgs instance.</summary>
        internal FilteringEventArgs(Hashtable values, bool isAcceptable)
        {
            System.Diagnostics.Debug.Assert(values != null);

            this._values = values;
            this._isAcceptable = isAcceptable;
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>Whether the combination evaluated is acceptable.</summary>
        public bool IsAcceptable
        {
            get { return this._isAcceptable; }
            set { this._isAcceptable = value; }
        }

        /// <summary>Values being considered, indexed by dimension name.</summary>
        public Hashtable Values
        {
            get { return this._values; }
        }

        #endregion Public properties.

        #region Private fields.

        private bool _isAcceptable;
        private Hashtable _values;

        #endregion Private fields.
    }

    /// <summary>Filtering event.</summary>
    /// <param name='sender'>Object firing the event.</param>
    /// <param name='e'>Event arguments.</param>
    public delegate void FilteringEventHandler(object sender, FilteringEventArgs e);

    /// <summary>
    /// A dimension defines a named set of values, only one
    /// of which is selected at a time for specific combination.
    /// </summary>
    public class Dimension
    {
        /// <summary>Name of the dimension.</summary>
        private string name;

        /// <summary>Possible values in this dimension.</summary>
        private object[] values;

        /// <summary>Initializes a new Dimension instance.</summary>
        public Dimension(string name, IEnumerable values)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("No name defined for the dimension.", "name");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            var list = new System.Collections.Generic.List<object>();
            foreach (object o in values)
            {
                list.Add(o);
            }

            this.name = name;
            this.values = list.ToArray();
        }

        /// <summary>Initializes a new Dimension instance.</summary>
        public Dimension(string name, object[] values)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException(
                    "No name defined for the dimension.", "name");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException(
                    "No values have been defined for dimension " + name,
                    "values");
            }

            this.name = name;
            this.values = values;
        }

        /// <summary>Dimension name (the attribute).</summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>Possible values for dimension.</summary>
        public object[] Values
        {
            get { return this.values; }
        }

        /// <summary>Sets the Values property.</summary>
        /// <param name="values">New value for the Values property.</param>
        /// <remarks>
        /// This is a dangerous method to call, as most clients of Dimension
        /// expect that the Values property be immutable.
        /// </remarks>
        internal void SetValues(object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            this.values = values;
        }

        /// <summary>Given a value, returns a dimension value identifier.</summary>
        /// <param name="value">Object to convert.</param>
        /// <returns></returns>
        internal static string ValueToString(object value)
        {
            string result;

            if (value is string && value.ToString().Length == 0)
            {
                result = "EMPTY_STRING";
            }
            else if (value is Enum)
            {
                result = value.ToString();
            }
            else if (value is int)
            {
                result = ((int)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (value is double)
            {
                result = ((double)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (value is IStringIdentifierSupport)
            {
                result = ((IStringIdentifierSupport)value).StringIdentifier;
            }
            else if (value != null)
            {
                result = value.ToString();
            }
            else
            {
                result = null;
            }

            if (result == null)
            {
                result = "NULL";
            }

            return result.Replace(' ', '_').Replace('\\', '_');
        }
    }

    /// <summary>Specifies the combinatorial engine mode to run in.</summary>
    public enum CombinatorialEngineMode
    {
        // The values are listed in descending order of coverage.

        /// <summary>All combinations are covered.</summary>
        FullCombinatorial,
        
        /// <summary>All pairwise combinations are covered.</summary>
        PairWise,
        
        /// <summary>Every element is covered.</summary>
        EveryElement,
    }
}
