//---------------------------------------------------------------------
// <copyright file="SyndicationTextContentKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    /// <summary>
    /// Used in place of Microsoft.OData.Service.Common.SyndicationTextContentKind to avoid a direct product dependency
    /// Should never be converted by value, because the values may be compiler-assigned on either side, and could change between builds.
    /// </summary>
    public enum SyndicationTextContentKind
    {
        /// <summary>
        /// The 'Plaintext' content kind
        /// </summary>
        Plaintext,

        /// <summary>
        /// The 'HTML' content kind
        /// </summary>
        Html,

        /// <summary>
        /// The 'XHTML' content kind
        /// </summary>
        Xhtml
    }
}