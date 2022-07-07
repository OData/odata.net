using Microsoft.OData;
using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    /// <summary>
    /// A writer for OData payloads based on OData Core's JsonWriter
    /// class. This will help us evaluate the raw perf of JsonWriter
    /// without all the overhead of high-level libraries (e.g. ODataJsonLightWriter, etc.)
    /// </summary>
    public class SimpleJsonODataWriter
    {
        readonly Uri serviceRoot;
        readonly string entitySetName;
        readonly Stack<WriterState> stateStack = new Stack<WriterState>();
        readonly IJsonWriter jsonWriter;

        public SimpleJsonODataWriter(IJsonWriter jsonWriter, Uri serviceRoot, string entitySetName)
        {
            this.serviceRoot = serviceRoot;
            this.entitySetName = entitySetName;
            this.jsonWriter = jsonWriter;
        }

        public WriterState CurrentState => stateStack.Peek();

        public void WriteStart(ODataResourceSet resourceSet)
        {
            if (stateStack.Count == 0)
            {
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("@odata.context");
                jsonWriter.WriteValue($"{serviceRoot.AbsoluteUri}$metadata#{entitySetName}");
                jsonWriter.WriteName("value");
                jsonWriter.StartArrayScope();
            }
            else
            {
                jsonWriter.StartArrayScope();
            }
            stateStack.Push(WriterState.ResourceSet);
        }

        public void WriteStart(ODataResource resource)
        {
            jsonWriter.StartObjectScope();
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
                jsonWriter.WriteName(nestedResource.Name);
                stateStack.Push(WriterState.NestedCollection);
            }
            else
            {
                jsonWriter.WriteName(nestedResource.Name);
                stateStack.Push(WriterState.NestedResource);
            }
        }

        public void WriteEnd()
        {
            WriterState state = stateStack.Pop();

            switch (state)
            {
                case WriterState.ResourceSet:
                    jsonWriter.EndArrayScope();
                    if (stateStack.Count == 0)
                    {
                        jsonWriter.EndObjectScope();
                    }
                    return;
                case WriterState.Resource:
                    jsonWriter.EndObjectScope();
                    return;
                case WriterState.NestedCollection:
                case WriterState.NestedResource:
                    // do nothing
                    return;
            }
        }

        public void Flush()
        {
            jsonWriter.Flush();
        }

        public void Dispose()
        {
            jsonWriter.Flush();
        }

        private void WriteBasicProperty(string propertyName, object value)
        {
            jsonWriter.WriteName(propertyName);
            WriteBasicValue(value);
        }

        private void WriteCollectionValue(ODataCollectionValue value)
        {
            jsonWriter.StartArrayScope();
            foreach (object item in value.Items)
            {
                WriteBasicValue(item);
            }

            jsonWriter.EndArrayScope();
        }

        private void WriteBasicValue(object value)
        {
            if (value is string stringValue)
            {
                jsonWriter.WriteValue(stringValue);
            }
            else if (value is int intValue)
            {
                jsonWriter.WriteValue(intValue);
            }
            else if (value is bool boolValue)
            {
                jsonWriter.WriteValue(boolValue);
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