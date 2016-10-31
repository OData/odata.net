//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client.Materialization
{
    using System.Data.Services.Client;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;

    /// <summary>
    /// Used to materialize a value from an <see cref="ODataMessageReader"/>.
    /// </summary>
    internal sealed class ODataValueMaterializer : ODataMessageReaderMaterializer
    {
        /// <summary>Current value being materialized; possibly null.</summary>
        private object currentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataValueMaterializer"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="singleResult">Is a single result expected.</param>
        public ODataValueMaterializer(ODataMessageReader reader, IODataMaterializerContext materializerContext, Type expectedType, bool? singleResult)
            : base(reader, materializerContext, expectedType, singleResult)
        {
        }

        /// <summary>
        /// Current value being materialized; possibly null.
        /// </summary>
        internal override object CurrentValue
        {
            get { return this.currentValue; }
        }

        /// <summary>
        /// Reads a value from the message reader.
        /// </summary>
        /// <param name="expectedClientType">The expected client type being materialized into.</param>
        /// <param name="expectedReaderType">The expected type for the underlying reader.</param>
        protected override void ReadWithExpectedType(IEdmTypeReference expectedClientType, IEdmTypeReference expectedReaderType)
        {
            object value = this.messageReader.ReadValue(expectedReaderType);
            this.currentValue = this.PrimitiveValueMaterializier.MaterializePrimitiveDataValue(this.ExpectedType, null, value);
        }
    }
}
