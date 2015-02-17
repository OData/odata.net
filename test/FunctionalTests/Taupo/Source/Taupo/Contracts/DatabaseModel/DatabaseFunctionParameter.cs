//---------------------------------------------------------------------
// <copyright file="DatabaseFunctionParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Database function or procedure parameter.
    /// </summary>
    public class DatabaseFunctionParameter : IAnnotatedStoreItem
    {
        /// <summary>
        /// Initializes a new instance of the DatabaseFunctionParameter class without name or type and with <see cref="Mode" />
        /// set to <see cref="DatabaseParameterMode.In"/>
        /// </summary>
        public DatabaseFunctionParameter()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseFunctionParameter class with given name and <see cref="Mode"/> 
        /// set to <see cref="DatabaseParameterMode.In"/> but without a type.
        /// </summary>
        /// <param name="name">Parameter name</param>
        public DatabaseFunctionParameter(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseFunctionParameter class with given name, type and <see cref="Mode"/>
        /// set to <see cref="DatabaseParameterMode.In"/>.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="type">Parameter type.</param>
        public DatabaseFunctionParameter(string name, DataType type)
            : this(name, type, DatabaseParameterMode.In)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseFunctionParameter class with given name, type and mode.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="type">Parameter type</param>
        /// <param name="mode">Parameter mode.</param>
        public DatabaseFunctionParameter(string name, DataType type, DatabaseParameterMode mode)
        {
            this.Name = name;
            this.ParameterType = type;
            this.Mode = mode;
            this.Annotations = new List<Annotation>();
        }

        /// <summary>
        /// Gets or sets parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets parameter type.
        /// </summary>
        public DataType ParameterType { get; set; }

        /// <summary>
        /// Gets or sets parameter mode.
        /// </summary>
        public DatabaseParameterMode Mode { get; set; }

        /// <summary>
        /// Gets the list of column annotations.
        /// </summary>
        /// <value>The annotations.</value>
        public IList<Annotation> Annotations { get; private set; }
    }
}
