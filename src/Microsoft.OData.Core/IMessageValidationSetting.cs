//---------------------------------------------------------------------
// <copyright file="IMessageValidationSetting.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Settings for validating OData content (applicable for readers and writers).
    /// </summary>
    internal interface IMessageValidationSetting
    {
        /// <summary>
        /// If set to true, all the validation would be enabled. Else some validation will be skipped.
        /// Default to true.
        /// </summary>
        bool EnableFullValidation { get; set; }

        /// <summary>
        /// Gets or sets UndeclaredPropertyBehaviorKinds.
        /// </summary>
        ODataUndeclaredPropertyBehaviorKinds UndeclaredPropertyBehaviorKinds { get; set; }
    }
}
