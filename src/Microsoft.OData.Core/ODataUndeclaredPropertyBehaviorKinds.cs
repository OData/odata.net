//---------------------------------------------------------------------
// <copyright file="ODataUndeclaredPropertyBehaviorKinds.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>Enumerates the behavior of readers when reading undeclared property.</summary>
    [Flags]
    public enum ODataUndeclaredPropertyBehaviorKinds
    {
        /// <summary>
        /// The default behavior - the reader will fail if it finds a property which is not declared by the model
        /// and the type is not open.
        /// </summary>
        None = 0,

        /// <summary>
        /// The reader will skip reading the property if it's not declared by the model and the current type is not open.
        /// </summary>
        /// <remarks>
        /// This flag can only be used when reading responses.
        /// All information about the undeclared property is going to be ignored, so for example ATOM metadata related to that property
        /// will not be reported either.
        /// </remarks>
        IgnoreUndeclaredValueProperty = 1,

        /// <summary>
        /// The reader will read and report link properties which are not declared by the model.
        /// </summary>
        /// <remarks>
        /// This flag can only be used when reading responses.
        /// If a link property in the payload is defined in the model it will be read as usual. If it is not declared
        /// it will still be read and reported, but it won't be validated against the model.
        /// 
        /// Link properties are:
        /// - Navigation links
        /// - Association links
        /// - Stream properties
        /// </remarks>
        ReportUndeclaredLinkProperty = 2,

        /// <summary>
        /// Reading/writing undeclared properties will be supported.
        /// </summary>
        SupportUndeclaredValueProperty = 4,
    }
}
