//   OData .NET Libraries ver. 5.6.3
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
                ODataBehaviorKind.Default,
                /*useV1ProviderBehavior*/ false,
                /*allowNullValuesForNonNullablePrimitiveTypes*/ false,
                /*allowDuplicatePropertyNames*/ false,
                Atom.AtomConstants.ODataNamespace,
                Atom.AtomConstants.ODataSchemeNamespace);

        /// <summary>The API behavior kind of this behavior.</summary>
        private readonly ODataBehaviorKind apiBehaviorKind;

        /// <summary>true if the server uses a V1 provider; otherwise false.</summary>
        private bool usesV1Provider;

        /// <summary>true to allow null values for non-nullable primitive types; otherwise false.</summary>
        private bool allowNullValuesForNonNullablePrimitiveTypes;

        /// <summary>
        /// If set to true, allows the writers to write duplicate properties of entries and complex values 
        /// (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        private bool allowDuplicatePropertyNames;

        /// <summary>Used to specify custom type scheme. Used for compatibility with WCF DS Client.</summary>
        private string typeScheme;

        /// <summary>Used to specify custom data namespace. Used for compatibility with WCF DS Client.</summary>
        private string odataNamespace;

        /// <summary>The format behavior kind of this behavior.</summary>
        private ODataBehaviorKind formatBehaviorKind;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formatBehaviorKind">The format behavior kind of this behavior.</param>
        /// <param name="apiBehaviorKind">The API behavior kind of this behavior.</param>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
        /// <param name="allowNullValuesForNonNullablePrimitiveTypes">
        /// true to allow null values for non-nullable primitive types; otherwise false.
        /// </param>
        /// <param name="allowDuplicatePropertyNames">
        /// If set to true, allows the writers to write duplicate properties of entries 
        /// and complex values (i.e., properties that have the same name).
        /// </param>
        /// <param name="odataNamespace">Custom data namespace.</param>
        /// <param name="typeScheme">Custom type scheme to use when resolving types.</param>
        private ODataWriterBehavior(
            ODataBehaviorKind formatBehaviorKind,
            ODataBehaviorKind apiBehaviorKind,
            bool usesV1Provider,
            bool allowNullValuesForNonNullablePrimitiveTypes,
            bool allowDuplicatePropertyNames,
            string odataNamespace,
            string typeScheme)
        {
            DebugUtils.CheckNoExternalCallers();

            this.formatBehaviorKind = formatBehaviorKind;
            this.apiBehaviorKind = apiBehaviorKind;
            this.usesV1Provider = usesV1Provider;
            this.allowNullValuesForNonNullablePrimitiveTypes = allowNullValuesForNonNullablePrimitiveTypes;
            this.allowDuplicatePropertyNames = allowDuplicatePropertyNames;
            this.odataNamespace = odataNamespace;
            this.typeScheme = typeScheme;
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
        /// Used to specify custom type scheme. Used for compatibility with WCF DS Client.
        /// </summary>
        internal string ODataTypeScheme
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.typeScheme;
            }
        }

        /// <summary>
        /// Used to specify custom data namespace. Used for compatibility with WCF DS Client.
        /// </summary>
        internal string ODataNamespace
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.odataNamespace;
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
        /// a named stream property, an association link or a collection.
        /// </remarks>
        internal bool AllowDuplicatePropertyNames 
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.allowDuplicatePropertyNames; 
            }
        }

        /// <summary>The format behavior kind of this behavior.</summary>
        internal ODataBehaviorKind FormatBehaviorKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.formatBehaviorKind;
            }
        }

        /// <summary>The API behavior kind of this behavior.</summary>
        internal ODataBehaviorKind ApiBehaviorKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.apiBehaviorKind;
            }
        }

        /// <summary>
        /// Create the writer behavior for the WCF Data Services client.
        /// </summary>
        /// <param name="odataNamespace">Custom data namespace.</param>
        /// <param name="typeScheme">Custom type scheme to use when resolving types.</param>
        /// <returns>The created writer behavior.</returns>
        internal static ODataWriterBehavior CreateWcfDataServicesClientBehavior(
            string odataNamespace,
            string typeScheme)
        {
            DebugUtils.CheckNoExternalCallers();
            return new ODataWriterBehavior(
                ODataBehaviorKind.WcfDataServicesClient,
                ODataBehaviorKind.WcfDataServicesClient,
                /*useV1ProviderBehavior*/ false,
                /*allowNullValuesForNonNullablePrimitiveTypes*/ false,
                /*allowDuplicatePropertyNames*/ false,
                odataNamespace,
                typeScheme);
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
                ODataBehaviorKind.WcfDataServicesServer,
                usesV1Provider,
                /*allowNullValuesForNonNullablePrimitiveTypes*/ true,
                /*allowDuplicatePropertyNames*/ true,
                Atom.AtomConstants.ODataNamespace,
                Atom.AtomConstants.ODataSchemeNamespace);
        }

        /// <summary>
        /// Resets the format behavior of the current writer behavior to the default format behavior.
        /// </summary>
        internal void UseDefaultFormatBehavior()
        {
            DebugUtils.CheckNoExternalCallers();
            this.formatBehaviorKind = ODataBehaviorKind.Default;

            // Also reset all format knobs
            this.usesV1Provider = false;
            this.allowNullValuesForNonNullablePrimitiveTypes = false;
            this.allowDuplicatePropertyNames = false;
            this.odataNamespace = Atom.AtomConstants.ODataNamespace;
            this.typeScheme = Atom.AtomConstants.ODataSchemeNamespace;
        }
    }
}
