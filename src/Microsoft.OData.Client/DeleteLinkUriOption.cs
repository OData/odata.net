//---------------------------------------------------------------------
// <copyright file="DeleteLinkUriOption.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Used to specify the form of Uri to be used for a delete link request.
    /// </summary>
    public enum DeleteLinkUriOption
    {
        /// <summary>
        /// Pass the id of the related entity as a query param, i.e.,
        /// {ServiceUri}/{EntitySet}/{Key}/{NavigationProperty}/$ref?$id={ServiceUri}/{RelatedEntitySet}/{RelatedKey}
        /// </summary>
        IdQueryParam = 0,

        /// <summary>
        /// Pass the id of the related entity as a key segment, i.e.,
        /// {ServiceUri}/{EntitySet}/{Key}/{NavigationProperty}/{RelatedKey}/$ref
        /// </summary>
        RelatedKeyAsSegment = 1,
    }
}
