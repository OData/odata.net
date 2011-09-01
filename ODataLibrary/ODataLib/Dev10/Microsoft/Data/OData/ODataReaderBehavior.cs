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
                /*allowDuplicatePropertyNames*/ false,
                /*usesV1Provider*/ false);

        /// <summary>The behavior kind of this behavior.</summary>
        private readonly ODataBehaviorKind behaviorKind;

        /// <summary>
        /// If set to true, allows the writers to write duplicate properties of entries and 
        /// complex values (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        private readonly bool allowDuplicatePropertyNames;

        /// <summary>true if the server uses a V1 provider; otherwise false.</summary>
        private readonly bool usesV1Provider;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="behaviorKind">The behavior kind of this behavior.</param>
        /// <param name="allowDuplicatePropertyNames">
        /// If set to true, allows the writers to write duplicate properties of entries and 
        /// complex values (i.e., properties that have the same name). Defaults to 'false'.
        /// </param>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
        internal ODataReaderBehavior(
            ODataBehaviorKind behaviorKind,
            bool allowDuplicatePropertyNames,
            bool usesV1Provider)
        {
            DebugUtils.CheckNoExternalCallers();

            this.behaviorKind = behaviorKind;
            this.allowDuplicatePropertyNames = allowDuplicatePropertyNames;
            this.usesV1Provider = usesV1Provider;
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
        /// If set to true, allows the writers to write duplicate properties of entries and 
        /// complex values (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        /// <remarks>
        /// Independently of this setting duplicate property names are never allowed if one 
        /// of the duplicate property names refers to a named stream property, 
        /// an association link or a multi value.
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
        /// Create the reader behavior for the WCF Data Services client.
        /// </summary>
        /// <returns>The created writer behavior.</returns>
        internal static ODataReaderBehavior CreateWcfDataServicesClientBehavior()
        {
            DebugUtils.CheckNoExternalCallers();
            return new ODataReaderBehavior(ODataBehaviorKind.WcfDataServicesClient, /*allowDuplicatePropertyNames*/ true, /*usesV1Provider*/ false);
        }

        /// <summary>
        /// Create the reader behavior for the WCF Data Services server.
        /// </summary>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
        /// <returns>The created writer behavior.</returns>
        internal static ODataReaderBehavior CreateWcfDataServicesServerBehavior(bool usesV1Provider)
        {
            DebugUtils.CheckNoExternalCallers();
            return new ODataReaderBehavior(ODataBehaviorKind.WcfDataServicesServer, /*allowDuplicatePropertyNames*/ true, usesV1Provider);
        }
    }
}
