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
    /// A writer for OData payloads based on OData Core's <see cref="IJsonWriterAsync"/>.
    /// This will help us evaluate the raw perf of async JsonWriter
    /// without all the overhead of high-level libraries (e.g. ODataJsonLightWriter, etc.)
    /// </summary>
    public class SimpleAsyncJsonODataWriter
    {
        readonly Uri serviceRoot;
        readonly string entitySetName;
        readonly Stack<WriterState> stateStack = new Stack<WriterState>();
        readonly IJsonWriterAsync jsonWriter;

        public SimpleAsyncJsonODataWriter(IJsonWriterAsync jsonWriter, Uri serviceRoot, string entitySetName)
        {
            this.serviceRoot = serviceRoot;
            this.entitySetName = entitySetName;
            this.jsonWriter = jsonWriter;
        }

        public WriterState CurrentState => stateStack.Peek();

        public async Task WriteStartAsync(ODataResourceSet resourceSet)
        {
            if (stateStack.Count == 0)
            {
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("@odata.context");
                await jsonWriter.WriteValueAsync($"{serviceRoot.AbsoluteUri}$metadata#{entitySetName}");
                await jsonWriter.WriteNameAsync("value");
                await jsonWriter.StartArrayScopeAsync();
            }
            else
            {
                await jsonWriter.StartArrayScopeAsync();
            }

            stateStack.Push(WriterState.ResourceSet);
        }

        public async Task WriteStartAsync(ODataResource resource)
        {
            await jsonWriter.StartObjectScopeAsync();
            stateStack.Push(WriterState.Resource);

            foreach (var property in resource.Properties)
            {
                await WriteBasicPropertyAsync(property.Name, property.Value);
            }
        }

        public async Task WriteStartAsync(ODataNestedResourceInfo nestedResource)
        {
            if (nestedResource.IsCollection == true)
            {
                await jsonWriter.WriteNameAsync(nestedResource.Name);
                stateStack.Push(WriterState.NestedCollection);
            }
            else
            {
                await jsonWriter.WriteNameAsync(nestedResource.Name);
                stateStack.Push(WriterState.NestedResource);
            }
        }

        public async Task WriteEndAsync()
        {
            WriterState state = stateStack.Pop();

            switch (state)
            {
                case WriterState.ResourceSet:
                    await jsonWriter.EndArrayScopeAsync();
                    if (stateStack.Count == 0)
                    {
                        await jsonWriter.EndObjectScopeAsync();
                    }
                    return;
                case WriterState.Resource:
                    await jsonWriter.EndObjectScopeAsync();
                    return;
                case WriterState.NestedCollection:
                case WriterState.NestedResource:
                    // do nothing
                    return;
            }
        }

        public Task FlushAsync()
        {
            return jsonWriter.FlushAsync();
        }

        public void Dispose()
        {
            jsonWriter.FlushAsync().Wait();
        }

        private async Task WriteBasicPropertyAsync(string propertyName, object value)
        {
            await jsonWriter.WriteNameAsync(propertyName);
            await WriteBasicValueAsync(value);
        }

        private async Task WriteCollectionValueAsync(ODataCollectionValue value)
        {
            await jsonWriter.StartArrayScopeAsync();
            foreach (object item in value.Items)
            {
                await WriteBasicValueAsync(item);
            }

            await jsonWriter.EndArrayScopeAsync();
        }

        private async Task WriteBasicValueAsync(object value)
        {
            if (value is string stringValue)
            {
                await jsonWriter.WriteValueAsync(stringValue);
            }
            else if (value is int intValue)
            {
                await jsonWriter.WriteValueAsync(intValue);
            }
            else if (value is bool boolValue)
            {
                await jsonWriter.WriteValueAsync(boolValue);
            }
            else if (value is ODataCollectionValue collectionValue)
            {
                await WriteCollectionValueAsync(collectionValue);
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