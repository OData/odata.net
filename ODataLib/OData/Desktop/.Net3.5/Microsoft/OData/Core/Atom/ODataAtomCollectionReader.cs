//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// OData collection reader for ATOM format.
    /// </summary>
    internal sealed class ODataAtomCollectionReader : ODataCollectionReaderCore
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataAtomInputContext atomInputContext;

        /// <summary>The collection deserializer to use to read from the input.</summary>
        private readonly ODataAtomCollectionDeserializer atomCollectionDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The input to read the payload from.</param>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        internal ODataAtomCollectionReader(ODataAtomInputContext atomInputContext, IEdmTypeReference expectedItemTypeReference)
            : base(atomInputContext, expectedItemTypeReference, null /*listener*/)
        {
            Debug.Assert(atomInputContext != null, "atomInputContext != null");

            this.atomInputContext = atomInputContext;
            this.atomCollectionDeserializer = new ODataAtomCollectionDeserializer(atomInputContext);
        }

        /// <summary>
        /// Implementation of the collection reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.None    - assumes that the Xml reader has not been used yet.
        /// Post-Condition: Any                 - the next node after the collection element node or the empty collection element node.
        /// </remarks>
        protected override bool ReadAtStartImplementation()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.Start, "this.State == ODataCollectionReaderState.Start");
            this.atomCollectionDeserializer.AssertXmlCondition(XmlNodeType.None);

            this.atomCollectionDeserializer.ReadPayloadStart();

            bool isCollectionElementEmpty;

            ODataCollectionStart collectionStart = this.atomCollectionDeserializer.ReadCollectionStart(out isCollectionElementEmpty);

            this.EnterScope(ODataCollectionReaderState.CollectionStart, collectionStart, isCollectionElementEmpty);

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionStart'.
        /// </summary>
        /// <returns>true if more nodes can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any - the next node after the collection element or the empty collection element node.
        /// Post-Condition: Any - the next node after the end tag of the item or the collection.
        /// </remarks>
        protected override bool ReadAtCollectionStartImplementation()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.CollectionStart, "this.State == ODataCollectionReaderState.CollectionStart");
            
            // position the reader to the end element of the collection or the first item of the collection.
            this.atomCollectionDeserializer.SkipToElementInODataMetadataNamespace();

            this.atomCollectionDeserializer.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.EndElement);

            if (this.atomCollectionDeserializer.XmlReader.NodeType == XmlNodeType.EndElement || this.IsCollectionElementEmpty)
            {
                // we got an empty collection.
                this.atomCollectionDeserializer.ReadCollectionEnd();
                this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
            }
            else
            {
                object item = this.atomCollectionDeserializer.ReadCollectionItem(this.ExpectedItemTypeReference, this.CollectionValidator);
                this.EnterScope(ODataCollectionReaderState.Value, item);
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Value'.
        /// </summary>
        /// <returns>true if more nodes can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any - the next node after the end tag of the item. 
        /// Post-Condition: Any - the next node after the end tag of the item or the collection. 
        /// </remarks>
        protected override bool ReadAtValueImplementation()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.Value, "this.State == ODataCollectionReaderState.Value");

            // position the reader on the start element of the next item in the collection or 
            // to the end element of the collection if there are no more items.
            this.atomCollectionDeserializer.SkipToElementInODataMetadataNamespace();

            this.atomCollectionDeserializer.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.EndElement);

            if (this.atomInputContext.XmlReader.NodeType == XmlNodeType.EndElement)
            {
                // We have reached the end of the collection.
                this.atomCollectionDeserializer.ReadCollectionEnd();

                this.PopScope(ODataCollectionReaderState.Value);
                this.ReplaceScope(ODataCollectionReaderState.CollectionEnd, this.Item);
            }
            else
            {
                object item = this.atomCollectionDeserializer.ReadCollectionItem(this.ExpectedItemTypeReference, this.CollectionValidator);
                this.ReplaceScope(ODataCollectionReaderState.Value, item);
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionEnd'.
        /// </summary>
        /// <returns>false since no more nodes can be read from the reader after the collection ends.</returns>
        /// <remarks>
        /// Pre-Condition: Any - the next node after the end tag of the collection element.
        /// Post-Condtion: XmlNodeType.None  - the reader is at the end of the input.
        /// </remarks>
        protected override bool ReadAtCollectionEndImplementation()
        {
            Debug.Assert(this.State == ODataCollectionReaderState.CollectionEnd, "this.State == ODataCollectionReaderState.CollectionEnd");

            this.atomCollectionDeserializer.ReadPayloadEnd();

            this.PopScope(ODataCollectionReaderState.CollectionEnd);

            Debug.Assert(this.State == ODataCollectionReaderState.Start, "this.State == ODataCollectionReaderState.Start");

            this.ReplaceScope(ODataCollectionReaderState.Completed, null);
        
            return false;
        }
    }
}

