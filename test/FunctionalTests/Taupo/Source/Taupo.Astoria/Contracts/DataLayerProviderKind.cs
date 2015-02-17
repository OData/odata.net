//---------------------------------------------------------------------
// <copyright file="DataLayerProviderKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// List of DataProviderKinds understood by Taupo.Astoria
    /// </summary>
    [DataContract]
    public enum DataLayerProviderKind
    {
        /// <summary>
        /// EDM datalayer
        /// </summary>
        [EnumMember]
        Edm,

        /// <summary>
        /// NonClr datalayer, a Test created linq to objects Provider kind
        /// </summary>
        [EnumMember]
        NonClr,
        
        /// <summary>
        /// InMemory datalayer, a test created linq to objects Provider kind
        /// </summary>
        [EnumMember]
        InMemory,

        /// <summary>
        /// LinqtoSql datalayer, a test created LinqToSql data Provider
        /// </summary>
        [EnumMember]
        LinqToSql
    }
}