//---------------------------------------------------------------------
// <copyright file="TestEntityMetadataBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    using System;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Evaluation;

    internal class TestEntityMetadataBuilder : ODataEntityMetadataBuilder
    {
        private ODataEntry entry;

        internal TestEntityMetadataBuilder(ODataEntry entry)
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