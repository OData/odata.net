//---------------------------------------------------------------------
// <copyright file="DefaultJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

namespace Microsoft.OData.Json
{
    internal sealed class DefaultJsonWriterFactory : IJsonWriterFactory
    {
        public IJsonWriter CreateJsonWriter(TextWriter textWriter, bool isIeee754Compatible)
        {
            return new JsonWriter(textWriter, isIeee754Compatible);
        }
    }
}
