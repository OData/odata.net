//---------------------------------------------------------------------
// <copyright file="ODataEntityMetadataBuilderTestUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    using System;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.Test.OData.TDD.Tests.Common;

    internal static class ODataEntityMetadataBuilderTestUtils
    {
        internal static void GetStreamEditLinkShouldValidateArguments(ODataEntityMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetStreamEditLink(value);
            action.ShouldThrowOnEmptyStringArgument("streamPropertyName");
        }

        internal static void GetStreamReadLinkShouldValidateArguments(ODataEntityMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetStreamReadLink(value);
            action.ShouldThrowOnEmptyStringArgument("streamPropertyName");
        }

        internal static void GetNavigationLinkUriShouldValidateArguments(ODataEntityMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetNavigationLinkUri(value, null, false);
            action.ShouldThrowOnNullOrEmptyStringArgument("navigationPropertyName");
        }

        internal static void GetAssociationLinkUriShouldValidateArguments(ODataEntityMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetAssociationLinkUri(value, null, false);
            action.ShouldThrowOnNullOrEmptyStringArgument("navigationPropertyName");
        }

        internal static void GetOperationTargetUriShouldValidateArguments(ODataEntityMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetOperationTargetUri(value, null, null);
            action.ShouldThrowOnNullOrEmptyStringArgument("operationName");
        }

        internal static void GetOperationTitleShouldValidateArguments(ODataEntityMetadataBuilder builder)
        {
            Action<string> action = (value) => builder.GetOperationTitle(value);
            action.ShouldThrowOnNullOrEmptyStringArgument("operationName");
        }
    }
}
