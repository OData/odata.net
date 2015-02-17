//---------------------------------------------------------------------
// <copyright file="FunctionParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Parameter of Function
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class FunctionParameter : AnnotatedItem
    {
        /// <summary>
        /// Initializes a new instance of the FunctionParameter class
        /// </summary>
        public FunctionParameter()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FunctionParameter class, with specified name, mode and data type
        /// </summary>
        /// <param name="name">The name of parameter</param>
        /// <param name="dataType">the DataType of parameter</param>
        public FunctionParameter(string name, DataType dataType)
            : this(name, dataType, FunctionParameterMode.In)
        {
        }

         /// <summary>
        /// Initializes a new instance of the FunctionParameter class with given name, type and mode.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dataType">Parameter type</param>
        /// <param name="mode">Parameter mode.</param>
        public FunctionParameter(string name, DataType dataType, FunctionParameterMode mode)
        {
            this.Name = name;
            this.DataType = dataType;
            this.Mode = mode;
        }

        /// <summary>
        /// Gets or sets the Name of the parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the DataType of the parameter.
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the mode of the parameter
        /// </summary>
        public FunctionParameterMode Mode { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.FunctionParameter"/>.
        /// </summary>
        /// <param name="parameterName">Name of the function parameter.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator FunctionParameter(string parameterName)
        {
            return new FunctionParameterReference(parameterName);
        }
    }
}