//---------------------------------------------------------------------
// <copyright file="MockJsonWriterFactoryWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Microsoft.OData.Json;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// This wraps an <see cref="IJsonWriterFactory"/> to
    /// intercept calls made to create an <see cref="IJsonWriter"/> in order to allow
    /// testing code paths around JSON writer construction.
    /// </summary>
    internal sealed class MockJsonWriterFactoryWrapper : IJsonWriterFactory
    {
        private readonly IJsonWriterFactory innerFactory;

        public MockJsonWriterFactoryWrapper(IJsonWriterFactory wrappedFactory)
        {
            if (wrappedFactory == null)
            {
                throw new ArgumentNullException(nameof(wrappedFactory));
            }

            this.innerFactory = wrappedFactory;
        }

        public IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            this.NumCalls++;
            this.Encoding = encoding;
            IJsonWriter writer = innerFactory.CreateJsonWriter(stream, isIeee754Compatible, encoding);
            this.CreatedWriter = writer;

            return writer;
        }

        /// <summary>
        /// The <see cref="IJsonWriter"/> that was last created by the <see cref="IJsonWriterFactory"/>.
        /// </summary>
        public IJsonWriter CreatedWriter { get; private set; }

        /// <summary>
        /// The encoding used when creating the <see cref="IJsonWriter"/>.
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Number of times the <see cref="CreateJsonWriter(Stream, bool, Encoding)"/> method was called.
        /// </summary>
        public int NumCalls { get; private set; }
    }
}
