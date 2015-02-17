//---------------------------------------------------------------------
// <copyright file="AtomGeneratorMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Atom metadata description of a content generator.
    /// </summary>
    public sealed class AtomGeneratorMetadata
    {
        /// <summary>Gets or sets the human readable name of the generator of the content.</summary>
        /// <returns>The human readable name of the generator of the content.</returns>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets or sets the (optional) URI describing the generator of the content.</summary>
        /// <returns>The (optional) URI describing the generator of the content.</returns>
        public Uri Uri
        {
            get;
            set;
        }

        /// <summary>Gets or sets the (optional) version of the generator.</summary>
        /// <returns>The (optional) version of the generator.</returns>
        public string Version
        {
            get;
            set;
        }
    }
}
