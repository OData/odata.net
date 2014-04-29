//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Materializes from $ref
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
