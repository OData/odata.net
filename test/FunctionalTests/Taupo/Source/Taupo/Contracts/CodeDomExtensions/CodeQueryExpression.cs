//---------------------------------------------------------------------
// <copyright file="CodeQueryExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Expression representing query comprehension.
    /// </summary>
    public class CodeQueryExpression : CodeExpression
    {
        /// <summary>
        /// Initializes a new instance of the CodeQueryExpression class.
        /// </summary>
        /// <param name="inputParameterName">input variable name</param>
        /// <param name="groupParameterName">group variable name</param>
        /// <param name="from">'from' element</param>
        /// <param name="where">'where' element.</param>
        /// <param name="keySelectors">key selectors for 'order by' element.</param>
        /// <param name="areDescending">Ascending/descending information for 'order by' element.</param>
        /// <param name="groupBy">'group ... into' element</param>
        /// <param name="select">'select' element</param>
        public CodeQueryExpression(
            string inputParameterName, 
            string groupParameterName, 
            CodeExpression from, 
            CodeExpression where, 
            IEnumerable<CodeExpression> keySelectors, 
            IEnumerable<bool> areDescending, 
            CodeExpression groupBy, 
            CodeExpression select)
        {
            this.From = from;
            this.Where = where;
            this.OrderByKeySelectors = keySelectors.ToList().AsReadOnly();
            this.AreDescending = areDescending.ToList().AsReadOnly();
            this.GroupByKeySelector = groupBy;
            this.Select = select;
            this.InputParameterName = inputParameterName;
            this.GroupParameterName = groupParameterName;
        }

        /// <summary>
        /// Gets the variable name before 'group by' element
        /// </summary>
        public string InputParameterName { get; private set; }

        /// <summary>
        /// Gets the variable name after 'group by' element
        /// </summary>
        public string GroupParameterName { get; private set; }

        /// <summary>
        /// Gets the 'select' element
        /// </summary>
        public CodeExpression Select { get; private set; }

        /// <summary>
        /// Gets the 'where' element
        /// </summary>
        public CodeExpression Where { get; private set; }

        /// <summary>
        /// Gets key selectors for 'order by' element
        /// </summary>
        public ReadOnlyCollection<CodeExpression> OrderByKeySelectors { get; private set; }

        /// <summary>
        /// Gets asending/descending information for 'order by' element
        /// </summary>
        public ReadOnlyCollection<bool> AreDescending { get; private set; }

        /// <summary>
        /// Gets the 'group by' element
        /// </summary>
        public CodeExpression GroupByKeySelector { get; private set; }

        /// <summary>
        /// Gets the 'from' element.
        /// </summary>
        public CodeExpression From { get; private set; }
    }
}