//-----------------------------------------------------------------------------
// <copyright file="ODataWriterHelper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests
{
    public static class ODataWriterHelper
    {
        public static void WriteResourceSet(ODataWriter writer, ODataResourceSetWrapper resourceSetWrapper)
        {
            writer.WriteStart(resourceSetWrapper.ResourceSet);

            if (resourceSetWrapper.Resources != null)
            {
                foreach (var resourceWrapper in resourceSetWrapper.Resources)
                {
                    WriteResource(writer, resourceWrapper);
                }
            }

            writer.WriteEnd();
        }

        public static async Task WriteResourceSetAsync(ODataWriter writer, ODataResourceSetWrapper resourceSetWrapper)
        {
            await writer.WriteStartAsync(resourceSetWrapper.ResourceSet);

            if (resourceSetWrapper.Resources != null)
            {
                foreach (var resourceWrapper in resourceSetWrapper.Resources)
                {
                    await WriteResourceAsync(writer, resourceWrapper);
                }
            }

            await writer.WriteEndAsync();
        }

        public static void WriteResource(ODataWriter writer, ODataResourceWrapper resourceWrapper)
        {
            writer.WriteStart(resourceWrapper.Resource);

            if (resourceWrapper.NestedResourceInfoWrappers != null)
            {
                foreach (var nestedResourceInfoWrapper in resourceWrapper.NestedResourceInfoWrappers)
                {
                    WriteNestedResourceInfo(writer, nestedResourceInfoWrapper);
                }
            }

            writer.WriteEnd();
        }

        public static async Task WriteResourceAsync(ODataWriter writer, ODataResourceWrapper resourceWrapper)
        {
            await writer.WriteStartAsync(resourceWrapper.Resource);

            if (resourceWrapper.NestedResourceInfoWrappers != null)
            {
                foreach (var nestedResourceInfoWrapper in resourceWrapper.NestedResourceInfoWrappers)
                {
                    await WriteNestedResourceInfoAsync(writer, nestedResourceInfoWrapper);
                }
            }

            await writer.WriteEndAsync();
        }

        public static void WriteNestedResourceInfo(ODataWriter writer, ODataNestedResourceInfoWrapper nestedResourceInfo)
        {
            writer.WriteStart(nestedResourceInfo.NestedResourceInfo);

            if (nestedResourceInfo.NestedResourceOrResourceSet != null)
            {
                WriteItem(writer, nestedResourceInfo.NestedResourceOrResourceSet);
            }

            writer.WriteEnd();
        }

        public static async Task WriteNestedResourceInfoAsync(ODataWriter writer, ODataNestedResourceInfoWrapper nestedResourceInfo)
        {
            await writer.WriteStartAsync(nestedResourceInfo.NestedResourceInfo);

            if (nestedResourceInfo.NestedResourceOrResourceSet != null)
            {
                await WriteItemAsync(writer, nestedResourceInfo.NestedResourceOrResourceSet);
            }

            await writer.WriteEndAsync();
        }

        private static void WriteItem(ODataWriter writer, ODataItemWrapper ODataItemWrapper)
        {
            var ODataResourceWrapper = ODataItemWrapper as ODataResourceWrapper;
            if (ODataResourceWrapper != null)
            {
                WriteResource(writer, ODataResourceWrapper);
            }

            var ODataResourceSetWrapper = ODataItemWrapper as ODataResourceSetWrapper;
            if (ODataResourceSetWrapper != null)
            {
                WriteResourceSet(writer, ODataResourceSetWrapper);
            }
        }

        private static async Task WriteItemAsync(ODataWriter writer, ODataItemWrapper ODataItemWrapper)
        {
            var ODataResourceWrapper = ODataItemWrapper as ODataResourceWrapper;
            if (ODataResourceWrapper != null)
            {
                await WriteResourceAsync(writer, ODataResourceWrapper);
            }

            var ODataResourceSetWrapper = ODataItemWrapper as ODataResourceSetWrapper;
            if (ODataResourceSetWrapper != null)
            {
                await WriteResourceSetAsync(writer, ODataResourceSetWrapper);
            }
        }
    }
}
