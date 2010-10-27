//Copyright 2010 Microsoft Corporation
//
//Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 
//
//http://www.apache.org/licenses/LICENSE-2.0 
//
//Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and limitations under the License.


namespace System.Data.Services.Common
{
    using System.Diagnostics;
#if ASTORIA_CLIENT
    using System.Data.Services.Client;
    using System.Reflection;
    using ClientTypeOrResourceType_Alias = System.Data.Services.Client.ClientType;
    using TypeOrResourceType_Alias = System.Type;
#else
    using System.Data.Services.Providers;
    using ClientTypeOrResourceType_Alias = System.Data.Services.Providers.ResourceType;
    using TypeOrResourceType_Alias = System.Data.Services.Providers.ResourceType;
#endif

    [DebuggerDisplay("EntityPropertyMappingInfo {DefiningType}")]
    internal sealed class EntityPropertyMappingInfo
    {
        private readonly EntityPropertyMappingAttribute attribute;

        private readonly TypeOrResourceType_Alias definingType;
        
        private readonly string[] segmentedSourcePath;

        private readonly ClientTypeOrResourceType_Alias actualPropertyType;

#if !ASTORIA_CLIENT

        private readonly bool isEFProvider;

        public EntityPropertyMappingInfo(EntityPropertyMappingAttribute attribute, ResourceType definingType, ResourceType actualPropertyType, bool isEFProvider)
        {
            this.isEFProvider = isEFProvider;
#else
        public EntityPropertyMappingInfo(EntityPropertyMappingAttribute attribute, Type definingType, ClientType actualPropertyType)
        {
#endif
            Debug.Assert(attribute != null, "attribute != null");
            Debug.Assert(definingType != null, "definingType != null");
            Debug.Assert(actualPropertyType != null, "actualPropertyType != null");

            this.attribute = attribute;
            this.definingType = definingType;
            this.actualPropertyType = actualPropertyType;

            Debug.Assert(!string.IsNullOrEmpty(attribute.SourcePath), "Invalid source path");
            this.segmentedSourcePath = attribute.SourcePath.Split('/');
        }

        public EntityPropertyMappingAttribute Attribute 
        { 
            get { return this.attribute; }
        }

        public TypeOrResourceType_Alias DefiningType
        {
            get { return this.definingType; }
        }

#if ASTORIA_CLIENT
        internal object ReadPropertyValue(object element)
        {
            return ReadPropertyValue(element, this.actualPropertyType, this.segmentedSourcePath, 0);
        }

        private static object ReadPropertyValue(object element, ClientType resourceType, string[] srcPathSegments, int currentSegment)
        {
            if (element == null || currentSegment == srcPathSegments.Length)
            {
                return element;
            }
            else
            {
                String srcPathPart = srcPathSegments[currentSegment];

                ClientType.ClientProperty resourceProperty = resourceType.GetProperty(srcPathPart, true);
                if (resourceProperty == null)
                {
                    throw Error.InvalidOperation(Strings.EpmSourceTree_InaccessiblePropertyOnType(srcPathPart, resourceType.ElementTypeName));
                }

                if (resourceProperty.IsKnownType ^ (currentSegment == srcPathSegments.Length - 1))
                {
                    throw Error.InvalidOperation(!resourceProperty.IsKnownType ? Strings.EpmClientType_PropertyIsComplex(resourceProperty.PropertyName) :
                                                                                 Strings.EpmClientType_PropertyIsPrimitive(resourceProperty.PropertyName));
                }

                PropertyInfo pi = element.GetType().GetProperty(srcPathPart, BindingFlags.Instance | BindingFlags.Public);
                Debug.Assert(pi != null, "Cannot find property " + srcPathPart + "on type " + element.GetType().Name);

                return ReadPropertyValue(
                            pi.GetValue(element, null),
                            resourceProperty.IsKnownType ? null : ClientType.Create(resourceProperty.PropertyType),
                            srcPathSegments,
                            ++currentSegment);
            }
        }
#else
        public bool IsEFProvider
        {
            get { return this.isEFProvider;  }
        }       

        internal object ReadPropertyValue(object element, DataServiceProviderWrapper provider)
        {
            return ReadPropertyValue(element, provider, this.actualPropertyType, this.segmentedSourcePath, 0);
        }

        private static object ReadPropertyValue(object element, DataServiceProviderWrapper provider, ResourceType resourceType, string[] srcPathSegments, int currentSegment)
        {
            if (element == null || currentSegment == srcPathSegments.Length)
            {
                return element;
            }
            else
            {
                String propertyName = srcPathSegments[currentSegment];
                ResourceProperty resourceProperty = resourceType != null ? resourceType.TryResolvePropertyName(propertyName) : null;

                if (resourceProperty != null)
                {
                    if (!resourceProperty.IsOfKind(currentSegment == srcPathSegments.Length - 1 ? ResourcePropertyKind.Primitive : ResourcePropertyKind.ComplexType))
                    {
                        throw new InvalidOperationException(Strings.EpmSourceTree_EndsWithNonPrimitiveType(propertyName));
                    }
                }
                else
                {
                    if (!(resourceType == null || resourceType.IsOpenType))
                    {
                        throw new InvalidOperationException(Strings.EpmSourceTree_InaccessiblePropertyOnType(propertyName, resourceType.Name));
                    }

                    resourceType = WebUtil.GetNonPrimitiveResourceType(provider, element);
                    resourceProperty = resourceType.TryResolvePropertyName(propertyName);
                }

                Debug.Assert(resourceType != null, "resourceType != null");
                object propertyValue = WebUtil.GetPropertyValue(provider, element, resourceType, resourceProperty, resourceProperty == null ? propertyName : null);

                return ReadPropertyValue(
                    propertyValue,
                    provider,
                    resourceProperty != null ? resourceProperty.ResourceType : null,
                    srcPathSegments,
                    currentSegment + 1);
            }
        }
#endif
    }
}
