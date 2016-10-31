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
    using System.Data.Services.Client.Metadata;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    using DSClient = System.Data.Services.Client;

    /// <summary>
    /// Materializes from $links
    /// </summary>
    internal sealed class ODataLinksMaterializer : ODataMessageReaderMaterializer
    {
        /// <summary>The links value read from the message.</summary>
        private ODataEntityReferenceLinks links;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataLinksMaterializer"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="singleResult">The single result.</param>
        public ODataLinksMaterializer(ODataMessageReader reader, IODataMaterializerContext materializerContext, Type expectedType, bool? singleResult)
            : base(reader, materializerContext, expectedType, singleResult)
        {
        }

        /// <summary>
        /// Gets the count value.
        /// </summary>
        internal override long CountValue
        {
            get
            {
                if (this.links == null && !this.IsDisposed)
                {
                    this.ReadLinks();
                }

                if (this.links != null && this.links.Count.HasValue)
                {
                    return this.links.Count.Value;
                }

                throw new InvalidOperationException(DSClient.Strings.MaterializeFromAtom_CountNotPresent);
            }
        }

        /// <summary>
        /// Current value being materialized; possibly null.
        /// </summary>
        internal override object CurrentValue
        {
            get { return null; }
        }

        /// <summary>
        /// Returns true if the underlying object used for counting is available
        /// </summary>
        internal override bool IsCountable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Reads from message reader.
        /// </summary>
        /// <param name="expectedClientType">The expected client type being materialized into.</param>
        /// <param name="expectedReaderType">The expected type for the underlying reader.</param>
        protected override void ReadWithExpectedType(IEdmTypeReference expectedClientType, IEdmTypeReference expectedReaderType)
        {
            this.ReadLinks();

            Type underlyingExpectedType = Nullable.GetUnderlyingType(this.ExpectedType) ?? this.ExpectedType;

            ClientEdmModel edmModel = this.MaterializerContext.Model;
            ClientTypeAnnotation targetType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(underlyingExpectedType));

            // If the target type is an entity, then we should throw since the type on the wire was not an entity
            if (targetType.IsEntityType)
            {
                throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidEntityType(targetType.ElementTypeName));
            }
            else
            {
                throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MixedTextWithComment);
            }
        }

        /// <summary>
        /// Reads the links.
        /// </summary>
        private void ReadLinks()
        {
            try
            {
                if (this.links == null)
                {
                    this.links = this.messageReader.ReadEntityReferenceLinks();
                }
            }
            catch (ODataErrorException e)
            {
                throw new DataServiceClientException(DSClient.Strings.Deserialize_ServerException(e.Error.Message), e);
            }
            catch (ODataException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }
    }
}
