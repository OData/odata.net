//---------------------------------------------------------------------
// <copyright file="NoopJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using Microsoft.OData.Json;

namespace ExperimentsLib
{
    /// <summary>
    /// Implementation of <see cref="IJsonWriterFactory"/> that returns
    /// a <see cref="NoopJsonWriter"/> that does nothing.
    /// </summary>
    public class NoopJsonWriterFactory : IJsonWriterFactory
    {
        public IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            return new NoopJsonWriter();
        }
    }
}
