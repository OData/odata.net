//---------------------------------------------------------------------
// <copyright file="CsdlTypeOutputFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    public enum CsdlTypeOutputFormat
    {
        /// <summary>
        /// Use XAttribute like: ReturnType='xxx'
        /// </summary>
        UseReturnTypeAttribute,

        /// <summary>
        /// Use XAttribute like: Type='xxx' or ElementType='xxx' inside an Element
        /// </summary>
        UseAttribute,

        /// <summary>
        /// Use a separate XElement
        /// </summary>
        UseElement,
    }
}
