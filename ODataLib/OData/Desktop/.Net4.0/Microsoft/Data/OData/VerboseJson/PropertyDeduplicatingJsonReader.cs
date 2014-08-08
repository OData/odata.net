//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Reader for the JSON format that supports look-ahead and deduplicates properties.
    /// </summary>
    /// <remarks>
    /// This reader will buffer the entire object record whenever it finds the start of the object record.
    /// It then goes through all its properties and removes duplicates.
    /// It then reports the object record as if there were no duplicates in it.
    /// If there was a duplicate property it will be reported at the position the first occurence of the property was found
    /// but with the value of the last occurence.
    /// This is to implement WCF DS Server compatibility behavior.
    /// </remarks>
    internal sealed class PropertyDeduplicatingJsonReader : BufferingJsonReader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">The text reader to read input characters from.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of recursive internalexception objects to allow when reading in-stream errors.</param>
        internal PropertyDeduplicatingJsonReader(TextReader reader, int maxInnerErrorDepth)
            : base(reader, JsonConstants.ODataErrorName, maxInnerErrorDepth, ODataFormat.VerboseJson)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Called whenever we find a new object value in the payload.
        /// Removes duplicate properties in the current object record.
        /// </summary>
        /// <remarks>
        /// This method assumes that we are buffering and that the current buffered node is a StartObject.
        /// It then goes, buffers the entire object record (and all its children) and removes duplicate properties (using the WCF DS Server algorithm).
        /// It will remove duplicate properties on any objects in the subtree of the top-level object as well (behaves recursively).
        /// The method also checks for in-stream errors and throws if it finds one.
        /// </remarks>
        protected override void ProcessObjectValue()
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.StartObject, "this.currentBufferedNode.NodeType == JsonNodeType.StartObject");
            this.AssertBuffering();

            // Read ahead the entire object and create a map of properties.
            // This will buffer the object record we are on and all its children (the entire subtree).
            // We store a stack, which holds an item for each object record we find.
            // Each item on this stack is a dictionary
            //   - key is the name of the property of a property in the object record
            //   - value is a List<PropertyDeduplicationRecord>, in this list we store a new record
            //     every time we find a property of a given name for the objet record.
            //     This stores pointer to the property node and pointer to the node after the property value.
            Stack<ObjectRecordPropertyDeduplicationRecord> objectRecordStack = new Stack<ObjectRecordPropertyDeduplicationRecord>();
            do
            {
                if (this.currentBufferedNode.NodeType == JsonNodeType.StartObject)
                {
                    // New object record - add the node to our stack
                    objectRecordStack.Push(new ObjectRecordPropertyDeduplicationRecord());

                    // See if it's an in-stream error
                    BufferedNode startObjectPosition = this.currentBufferedNode;
                    base.ProcessObjectValue();
                    this.currentBufferedNode = startObjectPosition;
                }
                else if (this.currentBufferedNode.NodeType == JsonNodeType.EndObject)
                {
                    // End of object record
                    // Pop the node from our stack
                    ObjectRecordPropertyDeduplicationRecord currentObjectRecord = objectRecordStack.Pop();

                    // If there is a current property, mark its last value node.
                    if (currentObjectRecord.CurrentPropertyRecord != null)
                    {
                        currentObjectRecord.CurrentPropertyRecord.LastPropertyValueNode = this.currentBufferedNode.Previous;
                    }

                    // Now walk the list of properties for the object record and deduplicate them
                    foreach (List<PropertyDeduplicationRecord> propertyDeduplicationRecords in currentObjectRecord.Values)
                    {
                        // If there's just one property of this name - there's nothing to do.
                        if (propertyDeduplicationRecords.Count <= 1)
                        {
                            continue;
                        }

                        // Walk all the properties and each time remove the property we find from its current place and move it
                        // inplace of the first property. Note that since the property names are the same we can replace the property node itself
                        // without losing any information.
                        // Once we walk the entire list the outcome will be that we will have just one property of a given name,
                        // it will be in the position of the first property, but it will be the last property value.
                        // We could completely remove the property occurences which are not first or last, but it's easier to move them
                        // to the first one by one as it keeps the algorithm simple (and the performence difference is small enough).
                        PropertyDeduplicationRecord firstProperty = propertyDeduplicationRecords[0];
                        for (int propertyIndex = 1; propertyIndex < propertyDeduplicationRecords.Count; propertyIndex++)
                        {
                            PropertyDeduplicationRecord currentProperty = propertyDeduplicationRecords[propertyIndex];
                            Debug.Assert((string)firstProperty.PropertyNode.Value == (string)currentProperty.PropertyNode.Value, "The property names must be the same.");

                            // Note that property nodes must be preceded at least by the start object node and followed by the end object node
                            // so we don't have to check for end of list here.
                            // Remove the current property from the list
                            currentProperty.PropertyNode.Previous.Next = currentProperty.LastPropertyValueNode.Next;
                            currentProperty.LastPropertyValueNode.Next.Previous = currentProperty.PropertyNode.Previous;

                            // Now replace the first property with the current property
                            firstProperty.PropertyNode.Previous.Next = currentProperty.PropertyNode;
                            currentProperty.PropertyNode.Previous = firstProperty.PropertyNode.Previous;
                            firstProperty.LastPropertyValueNode.Next.Previous = currentProperty.LastPropertyValueNode;
                            currentProperty.LastPropertyValueNode.Next = firstProperty.LastPropertyValueNode.Next;
                            firstProperty = currentProperty;
                        }
                    }

                    if (objectRecordStack.Count == 0)
                    {
                        break;
                    }
                }
                else if (this.currentBufferedNode.NodeType == JsonNodeType.Property)
                {
                    ObjectRecordPropertyDeduplicationRecord currentObjectRecord = objectRecordStack.Peek();

                    // If there is a previous property record, mark its last value node.
                    if (currentObjectRecord.CurrentPropertyRecord != null)
                    {
                        currentObjectRecord.CurrentPropertyRecord.LastPropertyValueNode = this.currentBufferedNode.Previous;
                    }

                    // Create a new property record for this property node and add it to the object record.
                    currentObjectRecord.CurrentPropertyRecord = new PropertyDeduplicationRecord(this.currentBufferedNode);
                    string propertyName = (string)this.currentBufferedNode.Value;
                    List<PropertyDeduplicationRecord> propertyDeduplicationRecords;
                    if (!currentObjectRecord.TryGetValue(propertyName, out propertyDeduplicationRecords))
                    {
                        propertyDeduplicationRecords = new List<PropertyDeduplicationRecord>();
                        currentObjectRecord.Add(propertyName, propertyDeduplicationRecords);
                    }

                    propertyDeduplicationRecords.Add(currentObjectRecord.CurrentPropertyRecord);
                }
            }
            while (this.ReadInternal());
        }

        /// <summary>
        /// Private class used to store information necessary to deduplicate properties of a single JSON object record.
        /// </summary>
        /// <remarks>
        /// This class is a dictionary
        /// Key is the name of a property in the object record.
        /// Value is a list of property deduplication records in the order we find the properties in the payload.
        /// </remarks>
        private sealed class ObjectRecordPropertyDeduplicationRecord : Dictionary<string, List<PropertyDeduplicationRecord>>
        {
            /// <summary>
            /// Points to the property record which is currently being constructed.
            /// </summary>
            internal PropertyDeduplicationRecord CurrentPropertyRecord { get; set; }
        }

        /// <summary>
        /// Private class used to store information necessary to deduplicate a single JSON property.
        /// </summary>
        private sealed class PropertyDeduplicationRecord
        {
            /// <summary>
            /// The node in the buffered nodes list which points to the property node
            /// which this deduplication record describes.
            /// </summary>
            private readonly BufferedNode propertyNode;

            /// <summary>
            /// The node in the buffered nodes list which points to the last node of the value of the property node
            /// this deduplication record describes.
            /// </summary>
            private BufferedNode lastPropertyValueNode;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="propertyNode">The property node to create the record for.</param>
            internal PropertyDeduplicationRecord(BufferedNode propertyNode)
            {
                Debug.Assert(propertyNode != null, "propertyNode != null");
                Debug.Assert(propertyNode.NodeType == JsonNodeType.Property, "Only property node can be stored as the property node of the deduplication record.");

                this.propertyNode = propertyNode;
            }

            /// <summary>
            /// The node in the buffered nodes list which points to the property node
            /// which this deduplication record describes.
            /// </summary>
            internal BufferedNode PropertyNode
            {
                get
                {
                    return this.propertyNode;
                }
            }

            /// <summary>
            /// The node in the buffered nodes list which points to the last node of the value of the property node
            /// this deduplication record describes.
            /// </summary>
            /// <remarks>
            /// Observation: Even if the value itself is an object for which we will do the property deduplication and thus we will shuffle its nodes around,
            /// in that case the last value node will point to the end object node which will not change during the deduplication process.
            /// </remarks>
            internal BufferedNode LastPropertyValueNode
            {
                get
                {
                    return this.lastPropertyValueNode;
                }

                set
                {
                    Debug.Assert(this.lastPropertyValueNode == null, "The LastPropertyValueNode can be set only once.");
                    this.lastPropertyValueNode = value;
                }
            }
        }
    }
}
