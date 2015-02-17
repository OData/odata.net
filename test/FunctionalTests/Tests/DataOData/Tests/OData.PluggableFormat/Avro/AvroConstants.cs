//---------------------------------------------------------------------
// <copyright file="AvroConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    internal static class AvroConstants
    {
        public const string MimeType = "avro";
        public const string MimeSubType = "binary";

        public const string ODataErrorType = "OData.Error";
        public const string ODataErrorFieldErrorCode = "ErrorCode";
        public const string ODataErrorFieldMessage = "Message";

        public const string ParameterTypeSuffix = "Parameter";
    }
}
#endif