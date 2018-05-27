//---------------------------------------------------------------------
// <copyright file="TestEntityMetadataBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Evaluation;

namespace Microsoft.OData.Tests.JsonLight
{
    internal class TestEntityMetadataBuilder : ODataResourceMetadataBuilder
    {
        private ODataResource entry;

        internal TestEntityMetadataBuilder(ODataResource entry)
        {
            this.entry = entry;
        }

        internal override Uri GetEditLink()
        {
            throw new NotImplementedException();
        }

        internal override Uri GetReadLink()
        {
            throw new NotImplementedException();
        }

        internal override Uri GetId()
        {
            throw new NotImplementedException();
        }

        internal override string GetETag()
        {
            throw new NotImplementedException();
        }

        internal override ODataStreamReferenceValue GetMediaResource()
        {
            return this.entry.NonComputedMediaResource;
        }

        internal override Uri GetStreamEditLink(string streamPropertyName)
        {
            return new Uri("http://service/ComputedStreamEditLink/" + streamPropertyName, UriKind.Absolute);
        }

        internal override Uri GetStreamReadLink(string streamPropertyName)
        {
            return new Uri("http://service/ComputedStreamReadLink/" + streamPropertyName, UriKind.Absolute);
        }

        internal override string GetOperationTitle(string operationName)
        {
            return operationName;
        }

        internal override Uri GetOperationTargetUri(string operationName, string bindingParameterTypeName, string parametername)
        {
            return new Uri("http://service/ComputedTargetUri/" + operationName + "(" + (bindingParameterTypeName ?? string.Empty) + ")", UriKind.Absolute);
        }

        internal override bool TryGetIdForSerialization(out Uri id)
        {
            id = null;
            return false;
        }
    }
}