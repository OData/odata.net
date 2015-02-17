//---------------------------------------------------------------------
// <copyright file="PredefinedModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumerates pre-defined EDM models used in Astoria testing.
    /// </summary>
    [DataContract]
    public enum PredefinedModel
    {
        /// <summary>
        /// Aruba model.
        /// </summary>
        [EnumMember]
        Aruba,

        /// <summary>
        /// Northwind model.
        /// </summary>
        [EnumMember]
        Northwind
    }
}