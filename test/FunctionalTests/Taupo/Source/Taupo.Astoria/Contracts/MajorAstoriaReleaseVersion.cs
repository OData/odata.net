//---------------------------------------------------------------------
// <copyright file="MajorAstoriaReleaseVersion.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Protocol version
    /// </summary>
    [DataContract]
    public enum MajorAstoriaReleaseVersion
    {
        /// <summary>
        /// Protocol version that represents V1 server (3.5 sp1 RTM)
        /// </summary>
        [EnumMember]
        V1,

        /// <summary>
        /// Protocol version that represents V2 server (3.5 sp1 hotfix)
        /// </summary>
        [EnumMember]
        V2,

        /// <summary>
        /// Protocol version that represents Dev10 server (Dev10 RTM)
        /// </summary>
        [EnumMember]
        Dev10,

        /// <summary>
        /// Protocol version that represents Dev11 server (Dev11)
        /// </summary>
        [EnumMember]
        Dev11,
    }
}
