//---------------------------------------------------------------------
// <copyright file="ODataWriterHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData;

namespace Microsoft.Test.OData
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

        public static void WriteResource(ODataWriter writer, ODataResourceWrapper resourceWrapper)
        {
            writer.WriteStart(resourceWrapper.Resource);

            if (resourceWrapper.NestedResourceInfos != null)
            {
                foreach (var nestedResourceInfoWrapper in resourceWrapper.NestedResourceInfos)
                {
                    WriteNestedResourceInfo(writer, nestedResourceInfoWrapper);
                }
            }

            writer.WriteEnd();
        }

        private static void WriteNestedResourceInfo(ODataWriter writer, ODataNestedResourceInfoWrapper nestedResourceInfo)
        {
            writer.WriteStart(nestedResourceInfo.NestedResourceInfo);

            if (nestedResourceInfo.NestedResourceOrResourceSet != null)
            {
                var nestedResourceOrResourceSet = nestedResourceInfo.NestedResourceOrResourceSet;
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
