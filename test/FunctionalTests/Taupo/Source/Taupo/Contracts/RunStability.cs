//---------------------------------------------------------------------
// <copyright file="RunStability.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Used as information on whether the run stablity level 
    /// </summary>
    [DataContract]
    [Flags]
    public enum RunStability
    {
        /// <summary>
        /// Enum that describes the current Stable set of tests for a run
        /// </summary>
        [EnumMember]
        Stable = 1,

        /// <summary>
        /// Enum that describes the current set of Unstable set of tests for a run
        /// </summary>
        [EnumMember]
        Unstable = 2,

        /// <summary>
        /// Enum that describes the set of All tests for a run
        /// </summary>
        [EnumMember]
        All = Stable | Unstable,
    }
}
