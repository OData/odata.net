//---------------------------------------------------------------------
// <copyright file="ObjectModelExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Extension methods for OData OM.
    /// </summary>
    public static class ObjectModelExtensions
    {
        /// <summary>
        /// Sets annotation on OData item.
        /// </summary>
        /// <typeparam name="TItem">The type of the OData item to set the annotation on.</typeparam>
        /// <typeparam name="TAnnotation">The type of the annotation to set.</typeparam>
        /// <param name="item">The OData item to set the annotation on.</param>
        /// <param name="annotation">The annotation to set.</param>
        /// <returns>The <paramref name="item"/> with the annotation set.</returns>
        public static TItem WithAnnotation<TItem, TAnnotation>(this TItem item, TAnnotation annotation)
            where TItem : ODataItem
            where TAnnotation : class
        {
            ExceptionUtilities.CheckArgumentNotNull(item, "item");
            item.SetAnnotation(annotation);
            return item;
        }
    }
}