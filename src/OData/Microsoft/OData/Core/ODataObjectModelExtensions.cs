//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core
{
    /// <summary>
    /// Extension methods on the OData object model.
    /// </summary>
    public static class ODataObjectModelExtensions
    {
        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataWriter"/> for <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataFeed feed, ODataFeedAndEntrySerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(feed, "feed");
            feed.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataWriter"/> for <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataEntry entry, ODataFeedAndEntrySerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(entry, "entry");
            entry.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataWriter"/> for <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataProperty property, ODataPropertySerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");
            property.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataCollectionWriter"/> for <paramref name="collectionStart"/>.
        /// </summary>
        /// <param name="collectionStart">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataCollectionStart collectionStart, ODataCollectionStartSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(collectionStart, "collectionStart");
            collectionStart.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataDeltaWriter"/> for <paramref name="deltaFeed"/>.
        /// </summary>
        /// <param name="deltaFeed">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataDeltaFeed deltaFeed, ODataDeltaFeedSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(deltaFeed, "deltaFeed");
            deltaFeed.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataDeltaWriter"/> for <paramref name="deltaDeletedEntry"/>.
        /// </summary>
        /// <param name="deltaDeletedEntry">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataDeltaDeletedEntry deltaDeletedEntry, ODataDeltaSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(deltaDeletedEntry, "deltaDeletedEntry");
            deltaDeletedEntry.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataDeltaWriter"/> for <paramref name="deltalink"/>.
        /// </summary>
        /// <param name="deltalink">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataDeltaLinkBase deltalink, ODataDeltaSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(deltalink, "deltalink");
            deltalink.SerializationInfo = serializationInfo;
        }
    }
}
