//---------------------------------------------------------------------
// <copyright file="IDataOracleResultConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataOracleService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    
    /// <summary>
    /// Converts WCF proxy classes from data oracle into classes common to the rest of Taupo
    /// </summary>
    [ImplementationSelector("DataOracleResultConverter", DefaultImplementation = "Default")]
    public interface IDataOracleResultConverter
    {
        /// <summary>
        /// Converts the serializable container (WCF proxy classes) into EntityContainerData.
        /// </summary>
        /// <param name="modelSchema">The model schema.</param>
        /// <param name="serializableContainer">The serializable container.</param>
        /// <returns>Instance of <see cref="EntityContainerData" /></returns>
        EntityContainerData Convert(EntityModelSchema modelSchema, SerializableContainer serializableContainer);

        /// <summary>
        /// Converts the serializable named value into a normal named value
        /// </summary>
        /// <param name="serializableNamedValue">The serializable named value</param>
        /// <returns>The value, but in types common to the rest of Taupo</returns>
        NamedValue Convert(SerializableNamedValue serializableNamedValue);
    }
}
