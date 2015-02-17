//---------------------------------------------------------------------
// <copyright file="HostType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The host that runs the DataService
    /// Attribute used in DataServiceBuilderService 
    /// so it requires DataContract serialization attributes
    /// </summary>
    [DataContract]
    public enum HostType
    {
        /// <summary>
        /// InternetInformationServer Host
        /// </summary>
        [EnumMember]
        InternetInformationServer,

        /// <summary>
        /// WCF Web Service Host
        /// </summary>
        [EnumMember]
        WebServiceHost
    }
}
