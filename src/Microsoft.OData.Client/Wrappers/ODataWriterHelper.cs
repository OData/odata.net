//---------------------------------------------------------------------
// <copyright file="ODataWriterHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData;

#if ODATA_SERVICE
namespace Microsoft.OData.Service
#elif ODATA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.Test.OData
#endif
{
#if ODATA_SERVICE
    internal static class ODataWriterHelper
#elif ODATA_CLIENT
    internal static class ODataWriterHelper
#else
    public static class ODataWriterHelper
#endif
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

        public static void WriteNestedResourceInfo(ODataWriter writer, ODataNestedResourceInfoWrapper nestedResourceInfo)
        {
            writer.WriteStart(nestedResourceInfo.NestedResourceInfo);

            if (nestedResourceInfo.NestedResourceOrResourceSet != null)
            {
                WriteItem(writer, nestedResourceInfo.NestedResourceOrResourceSet);
            }

            writer.WriteEnd();
        }

        private static void WriteItem(ODataWriter writer, ODataItemWrapper odataItemWrapper)
        {
            var odataResourceWrapper = odataItemWrapper as ODataResourceWrapper;
            if (odataResourceWrapper != null)
            {
                WriteResource(writer, odataResourceWrapper);
            }

            var odataResourceSetWrapper = odataItemWrapper as ODataResourceSetWrapper;
            if (odataResourceSetWrapper != null)
            {
                WriteResourceSet(writer, odataResourceSetWrapper);
            }
        }
    }
}
