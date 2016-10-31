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
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class that captures all the information needed to make readers behave
    /// differently when used inside of WCF Data Services or outside.
    /// </summary>
    internal sealed class ODataReaderBehavior
    {
        /// <summary>The default reader behavior for the OData library.</summary>
        private static readonly ODataReaderBehavior defaultReaderBehavior =
            new ODataReaderBehavior(
                ODataBehaviorKind.Default,
                ODataBehaviorKind.Default,
                /*allowDuplicatePropertyNames*/ false,
                /*usesV1Provider*/ false,
                /*typeResolver*/ null,
                Atom.AtomConstants.ODataNamespace,
                Atom.AtomConstants.ODataSchemeNamespace);

        /// <summary>The API behavior kind of this behavior.</summary>
        private readonly ODataBehaviorKind apiBehaviorKind;

        /// <summary>Custom type resolver used by the WCF DS Client.</summary>
        /// <remarks>
        /// This function is used instead of calling the IEdmModel.FindType.
        /// The first parameter to the function is the expected type (the type infered from the parent property or specified by the external caller).
        /// The second parameter is the type name from the payload.
        /// The function should return the resolved type, or null if no such type was found.
        /// </remarks>
        private readonly Func<IEdmType, string, IEdmType> typeResolver;

        /// <summary>
        /// If set to true, allows the writers to write duplicate properties of entries and 
        /// complex values (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        private bool allowDuplicatePropertyNames;

        /// <summary>true if the server uses a V1 provider; otherwise false.</summary>
        private bool usesV1Provider;

        /// <summary>Used to specify custom type scheme. Used for compatibility with WCF DS Client.</summary>
        private string typeScheme;

        /// <summary>Used to specify custom data namespace. Used for compatibility with WCF DS Client.</summary>
        private string odataNamespace;

        /// <summary>The format behavior kind of this behavior.</summary>
        private ODataBehaviorKind formatBehaviorKind;

        /// <summary>Determines whether operations bound to the given type must be container qualified.</summary>
        private Func<IEdmEntityType, bool> operationsBoundToEntityTypeMustBeContainerQualified;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formatBehaviorKind">The format behavior kind of this behavior.</param>
        /// <param name="apiBehaviorKind">The API behavior kind of this behavior.</param>
        /// <param name="allowDuplicatePropertyNames">
        /// If set to true, allows the writers to write duplicate properties of entries and 
        /// complex values (i.e., properties that have the same name). Defaults to 'false'.
        /// </param>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
        /// <param name="typeResolver">Custom type resolver which takes both expected type and type name.
        /// This function is used instead of the IEdmModel.FindType is it's specified.
        /// The first parameter to the function is the expected type (the type infered from the parent property or specified by the external caller).
        /// The second parameter is the type name from the payload.
        /// The function should return the resolved type, or null if no such type was found.</param>
        /// <param name="odataNamespace">Custom data namespace.</param>
        /// <param name="typeScheme">Custom type scheme to use when resolving types.</param>
        private ODataReaderBehavior(
            ODataBehaviorKind formatBehaviorKind,
            ODataBehaviorKind apiBehaviorKind,
            bool allowDuplicatePropertyNames,
            bool usesV1Provider,
            Func<IEdmType, string, IEdmType> typeResolver,
            string odataNamespace,
            string typeScheme)
        {
            DebugUtils.CheckNoExternalCallers();

            this.formatBehaviorKind = formatBehaviorKind;
            this.apiBehaviorKind = apiBehaviorKind;
            this.allowDuplicatePropertyNames = allowDuplicatePropertyNames;
            this.usesV1Provider = usesV1Provider;
            this.typeResolver = typeResolver;

            // set the default values for data namespace and type scheme
            this.odataNamespace = odataNamespace;
            this.typeScheme = typeScheme;
        }

        /// <summary>
        /// Get the default reader behavior for the OData library.
        /// </summary>
        /// <returns>The default reader behavior.</returns>
        internal static ODataReaderBehavior DefaultBehavior
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return defaultReaderBehavior;
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
        /// If set to true, allows the writers to write duplicate properties of entries and 
        /// complex values (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        /// <remarks>
        /// Independently of this setting duplicate property names are never allowed if one 
        /// of the duplicate property names refers to a named stream property, 
        /// an association link or a collection.
        /// </remarks>
        internal bool AllowDuplicatePropertyNames
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.allowDuplicatePropertyNames; 
            }
        }

        /// <summary>
        /// true if the server is using V1 provider; false otherwise.
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
        /// Custom type resolver used by the WCF DS Client.
        /// </summary>
        internal Func<IEdmType, string, IEdmType> TypeResolver
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.typeResolver;
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

        /// <summary>Determines whether operations bound to the given entity type must be container qualified.</summary>
        internal Func<IEdmEntityType, bool> OperationsBoundToEntityTypeMustBeContainerQualified
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.operationsBoundToEntityTypeMustBeContainerQualified;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.operationsBoundToEntityTypeMustBeContainerQualified = value;
            }
        }

        /// <summary>
        /// Create the reader behavior for the WCF Data Services client.
        /// </summary>
        /// <param name="typeResolver">Custom type resolver which takes both expected type and type name.
        /// This function is used instead of the IEdmModel.FindType is it's specified.
        /// The first parameter to the function is the expected type (the type infered from the parent property or specified by the external caller).
        /// The second parameter is the type name from the payload.
        /// The function should return the resolved type, or null if no such type was found.</param>
        /// <param name="odataNamespace">Custom data namespace.</param>
        /// <param name="typeScheme">Custom type scheme to use when resolving types.</param>
        /// <returns>The created reader behavior.</returns>
        internal static ODataReaderBehavior CreateWcfDataServicesClientBehavior(
            Func<IEdmType, string, IEdmType> typeResolver,
            string odataNamespace,
            string typeScheme)
        {
            DebugUtils.CheckNoExternalCallers();
            return new ODataReaderBehavior(
                ODataBehaviorKind.WcfDataServicesClient,
                ODataBehaviorKind.WcfDataServicesClient,
                /*allowDuplicatePropertyNames*/ true,
                /*usesV1Provider*/ false,
                typeResolver,
                odataNamespace,
                typeScheme);
        }

        /// <summary>
        /// Create the reader behavior for the WCF Data Services server.
        /// </summary>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
        /// <returns>The created reader behavior.</returns>
        internal static ODataReaderBehavior CreateWcfDataServicesServerBehavior(bool usesV1Provider)
        {
            DebugUtils.CheckNoExternalCallers();
            return new ODataReaderBehavior(
                ODataBehaviorKind.WcfDataServicesServer,
                ODataBehaviorKind.WcfDataServicesServer, 
                /*allowDuplicatePropertyNames*/ true, 
                usesV1Provider, 
                /*typeResolver*/ null,
                Atom.AtomConstants.ODataNamespace,
                Atom.AtomConstants.ODataSchemeNamespace);
        }

        /// <summary>
        /// Resets the format behavior of the current reader behavior to the default format behavior.
        /// </summary>
        internal void ResetFormatBehavior()
        {
            DebugUtils.CheckNoExternalCallers();
            this.formatBehaviorKind = ODataBehaviorKind.Default;

            // Also reset all format knobs
            this.allowDuplicatePropertyNames = false;
            this.usesV1Provider = false;
            this.odataNamespace = Atom.AtomConstants.ODataNamespace;
            this.typeScheme = Atom.AtomConstants.ODataSchemeNamespace;
            this.operationsBoundToEntityTypeMustBeContainerQualified = null;
        }
    }
}
