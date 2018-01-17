//---------------------------------------------------------------------
// <copyright file="ODataEntityMetadataBuilderTestUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Evaluation;

namespace Microsoft.OData.Tests.Evaluation
{
    internal static class ODataEntityMetadataBuilderTestsUtils
    {
        internal static void GetStreamEditLinkShouldValidateArguments(ODataResourceMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetStreamEditLink(value);
            action.ShouldThrowOnEmptyStringArgument("streamPropertyName");
        }

        internal static void GetStreamReadLinkShouldValidateArguments(ODataResourceMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetStreamReadLink(value);
            action.ShouldThrowOnEmptyStringArgument("streamPropertyName");
        }

        internal static void GetNavigationLinkUriShouldValidateArguments(ODataResourceMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetNavigationLinkUri(value, null, false);
            action.ShouldThrowOnNullOrEmptyStringArgument("navigationPropertyName");
        }

        internal static void GetAssociationLinkUriShouldValidateArguments(ODataResourceMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetAssociationLinkUri(value, null, false);
            action.ShouldThrowOnNullOrEmptyStringArgument("navigationPropertyName");
        }

        internal static void GetOperationTargetUriShouldValidateArguments(ODataResourceMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetOperationTargetUri(value, null, null);
            action.ShouldThrowOnNullOrEmptyStringArgument("operationName");
        }

        internal static void GetOperationTitleShouldValidateArguments(ODataResourceMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetOperationTitle(value);
            action.ShouldThrowOnNullOrEmptyStringArgument("operationName");
        }
    }
}
