using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.OData;

namespace ExperimentsLib
{
    /// <summary>
    /// A writer for OData payloads that uses
    /// the a <see cref="Utf8JsonWriter"/> internally
    /// for writing.
    /// </summary>
    public class Utf8JsonODataWriter
    {
        readonly Uri serviceRoot;
        readonly string entitySetName;
        readonly Stack<WriterState> stateStack = new Stack<WriterState>();
        readonly Utf8JsonWriter writer;


        public Utf8JsonODataWriter(Utf8JsonWriter writer, Uri serviceRoot, string entitySetName)
        {
            this.serviceRoot = serviceRoot;
            this.writer = writer;
            this.entitySetName = entitySetName;
        }

        public WriterState CurrentState => stateStack.Peek();

        public void WriteStart(ODataResourceSet resourceSet)
        {
            if (stateStack.Count == 0)
            {
                writer.WriteStartObject();
                writer.WriteString("@odata.context", $"{serviceRoot.AbsoluteUri}$metadata#{entitySetName}");
                writer.WriteStartArray("value");
            }
            else
            {
                writer.WriteStartArray();
            }

            stateStack.Push(WriterState.ResourceSet);
        }

        public void WriteStart(ODataResource resource)
        {
            writer.WriteStartObject();
            stateStack.Push(WriterState.Resource);

            foreach (var property in resource.Properties)
            {
                WriteBasicProperty(property.Name, property.Value);
            }
        }

        public void WriteStart(ODataNestedResourceInfo nestedResource)
        {
            if (nestedResource.IsCollection == true)
            {
                writer.WritePropertyName(nestedResource.Name);
                stateStack.Push(WriterState.NestedCollection);
            }
            else
            {
                writer.WritePropertyName(nestedResource.Name);
                stateStack.Push(WriterState.NestedResource);
            }
        }

        public void WriteEnd()
        {
            WriterState state = stateStack.Pop();
            
            switch (state)
            {
                case WriterState.ResourceSet:
                    writer.WriteEndArray();
                    if (stateStack.Count == 0)
                    {
                        writer.WriteEndObject();
                    }
                    return;
                case WriterState.Resource:
                    writer.WriteEndObject();
                    return;
                case WriterState.NestedCollection:
                case WriterState.NestedResource:
                    // do nothing
                    return;
            }
        }

        private void WriteBasicProperty(string propertyName, object value)
        {
            writer.WritePropertyName(propertyName);
            WriteBasicValue(value);
        }

        private void WriteCollectionValue(ODataCollectionValue value)
        {
            writer.WriteStartArray();
            foreach (object item in value.Items)
            {
                WriteBasicValue(item);
            }

            writer.WriteEndArray();
        }

        private void WriteBasicValue(object value)
        {
            // TODO: out of curiosity, is it faster to cast : if value is intValue
            // or type = value.GetType() + if type == typeof(int) + (int)value
            if (value is string stringValue)
            {
                writer.WriteStringValue(stringValue);
            }
            else if (value is int intValue)
            {
                writer.WriteNumberValue(intValue);
            }
            else if (value is bool boolValue)
            {
                writer.WriteBooleanValue(boolValue);
            }
            else if (value is ODataCollectionValue collectionValue)
            {
                WriteCollectionValue(collectionValue);
            }
            else
            {
                throw new Exception($"Unsupported type {value.GetType().Name}");
            }
        }


        public enum WriterState
        {
            PayloadStart,
            ResourceSet,
            Resource,
            NestedResource,
            NestedCollection
        }
    }
}
