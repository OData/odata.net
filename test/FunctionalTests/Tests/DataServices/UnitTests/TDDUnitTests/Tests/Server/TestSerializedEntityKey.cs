//---------------------------------------------------------------------
// <copyright file="TestSerializedEntityKey.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service.Serializers;

    internal class TestSerializedEntityKey : SerializedEntityKey
    {
        private readonly Uri absoluteEditLink;
        private readonly Uri absoluteEditLinkWithoutSuffix;

        public TestSerializedEntityKey(string editLink, string suffix)
        {
            this.absoluteEditLink = new Uri(editLink + suffix, UriKind.Absolute);
            this.absoluteEditLinkWithoutSuffix = new Uri(editLink, UriKind.Absolute);
        }

        internal override Uri RelativeEditLink
        {
            get { throw new NotImplementedException(); }
        }

        internal override Uri Identity
        {
            get { throw new NotImplementedException(); }
        }

        internal override Uri AbsoluteEditLink
        {
            get { return this.absoluteEditLink; }
        }

        internal override Uri AbsoluteEditLinkWithoutSuffix
        {
            get { return this.absoluteEditLinkWithoutSuffix; }
        }
    }
}