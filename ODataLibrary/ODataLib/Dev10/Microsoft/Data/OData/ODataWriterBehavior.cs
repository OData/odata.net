//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// Class that captures all the information needed to make writer behave
    /// differently when used inside of WCF Data Services or outside.
    /// </summary>
    internal sealed class ODataWriterBehavior
    {
        /// <summary>The default writer behavior for the OData library.</summary>
        private static readonly ODataWriterBehavior defaultWriterBehavior =
            new ODataWriterBehavior(
                ODataBehaviorKind.Default,
                /*useV1ProviderBehavior*/ false,
                /*allowNullValuesForNonNullablePrimitiveTypes*/ false,
                /*allowDuplicatePropertyNames*/ false,
                /*startEntryXmlCustomizationCallback*/ null,
                /*endEntryXmlCustomizationCallback*/ null);

        /// <summary>The behavior kind of this behavior.</summary>
        private readonly ODataBehaviorKind behaviorKind;

        /// <summary>true if the server uses a V1 provider; otherwise false.</summary>
        private readonly bool usesV1Provider;

        /// <summary>true to allow null values for non-nullable primitive types; otherwise false.</summary>
        private readonly bool allowNullValuesForNonNullablePrimitiveTypes;

        /// <summary>
        /// If set to true, allows the writers to write duplicate properties of entries and complex values 
        /// (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        private readonly bool allowDuplicatePropertyNames;

        /// <summary>
        /// The start entry callback for XML customization of entries.
        /// </summary>
        private Func<ODataEntry, XmlWriter, XmlWriter> startEntryXmlCustomizationCallback;

        /// <summary>
        /// The end entry callback for XML customization of entries.
        /// </summary>
        private Action<ODataEntry, XmlWriter, XmlWriter> endEntryXmlCustomizationCallback;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="behaviorKind">The behavior kind of this behavior.</param>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
        /// <param name="allowNullValuesForNonNullablePrimitiveTypes">
        /// true to allow null values for non-nullable primitive types; otherwise false.
        /// </param>
        /// <param name="allowDuplicatePropertyNames">
        /// If set to true, allows the writers to write duplicate properties of entries 
        /// and complex values (i.e., properties that have the same name).
        /// </param>
        /// <param name="startEntryXmlCustomizationCallback">The start entry callback for XML customization of entries.</param>
        /// <param name="endEntryXmlCustomizationCallback">The end entry callback for XML customization of entries.</param>
        internal ODataWriterBehavior(
            ODataBehaviorKind behaviorKind,
            bool usesV1Provider,
            bool allowNullValuesForNonNullablePrimitiveTypes,
            bool allowDuplicatePropertyNames,
            Func<ODataEntry, XmlWriter, XmlWriter> startEntryXmlCustomizationCallback,
            Action<ODataEntry, XmlWriter, XmlWriter> endEntryXmlCustomizationCallback)
        {
            DebugUtils.CheckNoExternalCallers();

            this.behaviorKind = behaviorKind;
            this.usesV1Provider = usesV1Provider;
            this.allowNullValuesForNonNullablePrimitiveTypes = allowNullValuesForNonNullablePrimitiveTypes;
            this.allowDuplicatePropertyNames = allowDuplicatePropertyNames;
            this.startEntryXmlCustomizationCallback = startEntryXmlCustomizationCallback;
            this.endEntryXmlCustomizationCallback = endEntryXmlCustomizationCallback;
        }

        /// <summary>
        /// Get the default writer behavior.
        /// </summary>
        /// <returns>The default writer behavior.</returns>
        internal static ODataWriterBehavior DefaultBehavior
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return defaultWriterBehavior;
            }
        }

        /// <summary>
        /// EPM mappings are defined on entity types but not on complex types.  For entity types, the EPM mapping for each of its primitive properties stay the same.
        /// For complex types however, the EPM mappings changes depending on the entity type each complex property is declared on.
        /// For example, if the "Customer" entity type has the properties Address1 and Address2 both of the complex type "Address".  If properties in Address1
        /// are not mapped while properties in Address2 are mapped with KeepInContent=false, when we serialze the Address type in atom format, we keep properties of
        /// Address1 in content while skipping properties of Address2 from the content.  Thus the same complex type can get serialized differently for each instance
        /// of the type.
        ///
        /// Astoria has a bug for V1 providers in which it creates and caches EPM information on the complex type itself the first time it serializes
        /// an instance of the complex type.  So subsequent writes will serialize the complex type the same way regardless of its EPM mapping.
        /// This creates the following 2 problems:
        /// 1. If a primitive property of a complex type is not part of a EPM mapping or has KeepInContent=true the first time it's serialized, the property will always
        ///    be serialized in the content for the remaining lifetime of the service, even for instances where KeepInContent=false.
        /// 2. If a primitive property of a complex type has KeepInContent=false the first time it's serialized, the property will always be missing in the content
        ///    for the remaining lifetime of the service, even for instances that do not belong to a EPM mapping or have KeepInContent=true.
        /// Unfortunately we cannot fix issue 1 because it can break existing third party clients.  We have to fix issue 2 because it is a data corruption issue.
        ///
        /// The solution is to remember on the first write whether a primitive property of a complex type is serialized in content or not.  On subsequent writes
        /// to the same property, we will always keep it in content if the initial write is in content.  Otherwise we calculate whether it should be in content
        /// based on ShouldWritePropertyInContent.
        ///
        /// NOTE: this assumes that the complex type in question does not change throughout the lifetime of the service.
        /// 
        /// See bug 174185.
        /// </summary>
        internal bool UseV1ProviderBehavior
        { 
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.usesV1Provider; 
            }
        }

        /// <summary>
        /// If set to true, the writers will allow writing null values even if the metadata specifies a non-nullable primitive type.
        /// </summary>
        internal bool AllowNullValuesForNonNullablePrimitiveTypes 
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.allowNullValuesForNonNullablePrimitiveTypes; 
            }
        }

        /// <summary>
        /// If set to true, allows the writers to write duplicate properties of entries and complex values (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        /// <remarks>
        /// Independently of this setting duplicate property names are never allowed if one of the duplicate property names refers to
        /// a named stream property, an association link or a multi value.
        /// </remarks>
        internal bool AllowDuplicatePropertyNames 
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.allowDuplicatePropertyNames; 
            }
        }

        /// <summary>The behavior kind of this behavior.</summary>
        internal ODataBehaviorKind BehaviorKind
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.behaviorKind; 
            }
        }

        /// <summary>
        /// The start entry callback for XML customization of entries.
        /// </summary>
        internal Func<ODataEntry, XmlWriter, XmlWriter> StartEntryXmlCustomizationCallback
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.startEntryXmlCustomizationCallback;
            }
        }

        /// <summary>
        /// The end entry callback for XML customization of entries.
        /// </summary>
        internal Action<ODataEntry, XmlWriter, XmlWriter> EndEntryXmlCustomizationCallback
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.endEntryXmlCustomizationCallback;
            }
        }

        /// <summary>
        /// Create the writer behavior for the WCF Data Services client.
        /// </summary>
        /// <param name="startEntryXmlCustomizationCallback">The start entry callback for XML customization of entries.</param>
        /// <param name="endEntryXmlCustomizationCallback">The end entry callback for XML customization of entries.</param>
        /// <returns>The created writer behavior.</returns>
        internal static ODataWriterBehavior CreateWcfDataServicesClientBehavior(
            Func<ODataEntry, XmlWriter, XmlWriter> startEntryXmlCustomizationCallback,
            Action<ODataEntry, XmlWriter, XmlWriter> endEntryXmlCustomizationCallback)
        {
            DebugUtils.CheckNoExternalCallers();
            return new ODataWriterBehavior(
                ODataBehaviorKind.WcfDataServicesClient,
                /*useV1ProviderBehavior*/ false,
                /*allowNullValuesForNonNullablePrimitiveTypes*/ false,
                /*allowDuplicatePropertyNames*/ false,
                startEntryXmlCustomizationCallback,
                endEntryXmlCustomizationCallback);
        }

        /// <summary>
        /// Create the writer behavior for the WCF Data Services server.
        /// </summary>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
        /// <returns>The created writer behavior.</returns>
        internal static ODataWriterBehavior CreateWcfDataServicesServerBehavior(bool usesV1Provider)
        {
            DebugUtils.CheckNoExternalCallers();
            return new ODataWriterBehavior(
                ODataBehaviorKind.WcfDataServicesServer,
                usesV1Provider,
                /*allowNullValuesForNonNullablePrimitiveTypes*/ true,
                /*allowDuplicatePropertyNames*/ true,
                /*startEntryXmlCustomizationCallback*/ null,
                /*endEntryXmlCustomizationCallback*/ null);
        }
    }
}
