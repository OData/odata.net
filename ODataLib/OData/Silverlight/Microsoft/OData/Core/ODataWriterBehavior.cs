//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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
            /*allowNullValuesForNonNullablePrimitiveTypes*/ false,
            /*allowDuplicatePropertyNames*/ false);

        /// <summary>The API behavior kind of this behavior.</summary>
        private readonly ODataBehaviorKind apiBehaviorKind;

        /// <summary>true to allow null values for non-nullable primitive types; otherwise false.</summary>
        private bool allowNullValuesForNonNullablePrimitiveTypes;

        /// <summary>
        /// If set to true, allows the writers to write duplicate properties of entries and complex values 
        /// (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        private bool allowDuplicatePropertyNames;

        /// <summary>The format behavior kind of this behavior.</summary>
        private ODataBehaviorKind formatBehaviorKind;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formatBehaviorKind">The format behavior kind of this behavior.</param>
        /// <param name="apiBehaviorKind">The API behavior kind of this behavior.</param>
        /// <param name="allowNullValuesForNonNullablePrimitiveTypes">
        /// true to allow null values for non-nullable primitive types; otherwise false.
        /// </param>
        /// <param name="allowDuplicatePropertyNames">
        /// If set to true, allows the writers to write duplicate properties of entries 
        /// and complex values (i.e., properties that have the same name).
        /// </param>
        private ODataWriterBehavior(
            ODataBehaviorKind formatBehaviorKind,
            ODataBehaviorKind apiBehaviorKind,
            bool allowNullValuesForNonNullablePrimitiveTypes,
            bool allowDuplicatePropertyNames)
        {
            this.formatBehaviorKind = formatBehaviorKind;
            this.apiBehaviorKind = apiBehaviorKind;
            this.allowNullValuesForNonNullablePrimitiveTypes = allowNullValuesForNonNullablePrimitiveTypes;
            this.allowDuplicatePropertyNames = allowDuplicatePropertyNames;
        }

        /// <summary>
        /// Get the default writer behavior.
        /// </summary>
        /// <returns>The default writer behavior.</returns>
        internal static ODataWriterBehavior DefaultBehavior
        {
            get
            {
                return defaultWriterBehavior;
            }
        }

        /// <summary>
        /// If set to true, the writers will allow writing null values even if the metadata specifies a non-nullable primitive type.
        /// </summary>
        internal bool AllowNullValuesForNonNullablePrimitiveTypes 
        {
            get 
            {
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
                return this.allowDuplicatePropertyNames; 
            }
        }

        /// <summary>The format behavior kind of this behavior.</summary>
        internal ODataBehaviorKind FormatBehaviorKind
        {
            get
            {
                return this.formatBehaviorKind;
            }
        }

        /// <summary>The API behavior kind of this behavior.</summary>
        internal ODataBehaviorKind ApiBehaviorKind
        {
            get
            {
                return this.apiBehaviorKind;
            }
        }

        /// <summary>
        /// Create the writer behavior for the WCF Data Services client.
        /// </summary>
        /// <returns>The created writer behavior.</returns>
        internal static ODataWriterBehavior CreateWcfDataServicesClientBehavior()
        {
            return new ODataWriterBehavior(
                ODataBehaviorKind.WcfDataServicesClient,
                ODataBehaviorKind.WcfDataServicesClient,
                /*allowNullValuesForNonNullablePrimitiveTypes*/ false,
                /*allowDuplicatePropertyNames*/ false);
        }



        /// <summary>
        /// Create the writer behavior for the OData server.
        /// </summary>
        /// <returns>The created writer behavior.</returns>
        internal static ODataWriterBehavior CreateODataServerBehavior()
        {
            return new ODataWriterBehavior(
                ODataBehaviorKind.ODataServer,
                ODataBehaviorKind.ODataServer,
                /*allowNullValuesForNonNullablePrimitiveTypes*/ true,
                /*allowDuplicatePropertyNames*/ true);
        }

        /// <summary>
        /// Resets the format behavior of the current writer behavior to the default format behavior.
        /// </summary>
        internal void UseDefaultFormatBehavior()
        {
            this.formatBehaviorKind = ODataBehaviorKind.Default;

            // Also reset all format knobs
            this.allowNullValuesForNonNullablePrimitiveTypes = false;
            this.allowDuplicatePropertyNames = false;
        }
    }
}
