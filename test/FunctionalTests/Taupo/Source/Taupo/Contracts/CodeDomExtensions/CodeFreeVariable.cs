//---------------------------------------------------------------------
// <copyright file="CodeFreeVariable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents free variable.
    /// </summary>
    public class CodeFreeVariable
    {
        /// <summary>
        /// Initializes a new instance of the CodeFreeVariable class.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="variableType">Variable type.</param>
        /// <param name="possibleValues">Values that can be assigned to the variable.</param>
        public CodeFreeVariable(string name, CodeTypeReference variableType, IEnumerable<CodeExpression> possibleValues)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");
            ExceptionUtilities.CheckArgumentNotNull(variableType, "variableType");
            ExceptionUtilities.CheckCollectionNotEmpty(possibleValues, "possibleValues");

            this.Name = name;
            this.VariableType = variableType;
            this.PossibleValues = possibleValues.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the variable name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the variable variableType.
        /// </summary>
        public CodeTypeReference VariableType { get; private set; }

        /// <summary>
        /// Gets the list of possible variable values.
        /// </summary>
        public ReadOnlyCollection<CodeExpression> PossibleValues { get; private set; }
    }
}