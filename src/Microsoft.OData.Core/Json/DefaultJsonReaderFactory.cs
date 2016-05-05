//---------------------------------------------------------------------
// <copyright file="DefaultJsonReaderFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

namespace Microsoft.OData.Json
{
    internal sealed class DefaultJsonReaderFactory : IJsonReaderFactory
    {
        public IJsonReader CreateJsonReader(TextReader textReader, bool isIeee754Compatible)
        {
            return new JsonReader(textReader, isIeee754Compatible);
        }
    }
}
