//---------------------------------------------------------------------
// <copyright file="IDataTypeConstructorCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Generates code to re-construct a <see cref="DataType"/>
    /// </summary>
    public interface IDataTypeConstructorCodeGenerator
    {
        /// <summary>
        /// Generate code to re-construct the specified <see cref="DataType"/>
        /// </summary>
        /// <param name="dataType">Type to be re-constructed</param>
        /// <returns>Code that will perform construction of the type</returns>
        string GetDataTypeConstructorCode(DataType dataType);

        /// <summary>
        /// Namespaces required by the generated code
        /// </summary>
        /// <returns>Required namspaces</returns>
        IEnumerable<string> GetCodeNamespaces();
    }
}
