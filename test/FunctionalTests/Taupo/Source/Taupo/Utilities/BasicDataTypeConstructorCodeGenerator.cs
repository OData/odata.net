//---------------------------------------------------------------------
// <copyright file="BasicDataTypeConstructorCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Generates code to re-construct a minimally specified <see cref="DataType"/>
    /// </summary>
    public class BasicDataTypeConstructorCodeGenerator : IDataTypeConstructorCodeGenerator
    {
        /// <summary>
        /// Generate code to re-construct the specified <see cref="DataType"/> with minimal specification
        /// </summary>
        /// <param name="dataType">Type to be re-constructed</param>
        /// <returns>Code that will perform construction of the type</returns>
        public string GetDataTypeConstructorCode(DataType dataType)
        {
            // Code generation is currently convention based and the only works for primitive types
            if (!(dataType is PrimitiveDataType))
            {
                throw new TaupoNotSupportedException("Only primitive types are supported at this stage.");
            }

            string code = string.Format(
                CultureInfo.InvariantCulture, 
                "DataTypes.{0}", 
                dataType.GetType().Name.Replace("DataType", string.Empty));

            if (dataType.IsNullable)
            {
                code = string.Concat(code, ".Nullable(true)");
            }

            return code;
        }

        /// <summary>
        /// Namespaces required by the generated code
        /// </summary>
        /// <returns>Required namspaces</returns>
        public IEnumerable<string> GetCodeNamespaces()
        {
            return new string[] 
            { 
                typeof(DataTypes).Namespace,
            };
        }
    }
}
