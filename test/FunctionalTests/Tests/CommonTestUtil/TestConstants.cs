//---------------------------------------------------------------------
// <copyright file="TestConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Common
{
    public static class TestConstants
    {
        public const string MimeApplicationAtom = "application/atom+xml";
        public const string MimeApplicationXml = "application/xml";
        public const string MimeApplicationAtomPlusXml = MimeApplicationAtom + "," + MimeApplicationXml;
        public const string MimeApplicationJsonODataMinimalMetadata = "application/json;odata.metadata=minimal";
        public const string MimeApplicationJsonODataFullMetadata = "application/json;odata.metadata=full";
        public const string MimeApplicationJsonODataVerbose = "application/json;odata.metadata=verbose";
        public const string HttpAccept = "Accept";
        public const string HttpContentType = "Content-Type";
    }
}
