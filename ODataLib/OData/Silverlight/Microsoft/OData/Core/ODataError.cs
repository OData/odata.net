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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    #endregion Namespaces

    /// <summary>
    /// Class representing an error payload.
    /// </summary>
    [DebuggerDisplay("{ErrorCode}: {Message}")]
#if !WINDOWS_PHONE && !SILVERLIGHT && !PORTABLELIB
    [Serializable]
#endif
    public sealed class ODataError : ODataAnnotatable
    {
        /// <summary>Gets or sets the error code to be used in payloads.</summary>
        /// <returns>The error code to be used in payloads.</returns>
        public string ErrorCode
        {
            get;
            set;
        }

        /// <summary>Gets or sets the error message.</summary>
        /// <returns>The error message.</returns>
        public string Message
        {
            get;
            set;
        }

        /// <summary>Gets or sets the language for the exception Message.</summary>
        /// <returns>The language for the exception Message.</returns>
        public string MessageLanguage
        {
            get;
            set;
        }

        /// <summary>Gets or sets the implementation specific debugging information to help determine the cause of the error.</summary>
        /// <returns>The implementation specific debugging information.</returns>
        public ODataInnerError InnerError
        {
            get;
            set;
        }

        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return this.GetInstanceAnnotations(); }
            set { this.SetInstanceAnnotations(value); }
        }

        /// <summary>
        /// Verifies that <paramref name="annotation"/> can be added as an annotation of this.
        /// </summary>
        /// <param name="annotation">Annotation instance.</param>
        internal override void VerifySetAnnotation(object annotation)
        {
            DebugUtils.CheckNoExternalCallers();

            // We block InstanceAnnotationCollection at the ODataAnnotatable level, but specifically allow them here
        }

        /// <summary>
        /// Gets the collection of instance annotations from this <see cref="ODataError"/> instance.
        /// </summary>
        /// <returns>The collection of instance annotations </returns>
        internal IEnumerable<ODataInstanceAnnotation> GetInstanceAnnotationsForWriting()
        {
            DebugUtils.CheckNoExternalCallers();

            // We have deprecated the InstanceAnnotationCollection type. For backcompat:
            // 1. On reading we will populate both the InstanceAnnotationCollection object annotation and the ODataError.InstanceAnnotations collection.
            // 2. On writing, if ODataError.InstanceAnnotations collection is not empty, we will only serialize the ODataError.InstanceAnnotations collection.
            //    Otherwise we will serialize what's in the InstanceAnnotationCollection object annotation if it's present.
            if (this.InstanceAnnotations.Count > 0)
            {
                return this.InstanceAnnotations;
            }

#pragma warning disable 618 // Disable "obsolete" warning for the InstanceAnnotationCollection. Used for backwards compatibilty.
            InstanceAnnotationCollection instanceAnnotationCollection = this.GetAnnotation<InstanceAnnotationCollection>();
#pragma warning restore 618
            if (instanceAnnotationCollection != null && instanceAnnotationCollection.Count > 0)
            {
                return instanceAnnotationCollection.Select(ia => new ODataInstanceAnnotation(ia.Key, ia.Value));
            }

            return Enumerable.Empty<ODataInstanceAnnotation>();
        }

        /// <summary>
        /// Adds an instance annotation from the payload to this <see cref="ODataError"/> instance.
        /// </summary>
        /// <param name="instanceAnnotationName">The name of the instance annotation.</param>
        /// <param name="instanceAnnotationValue">The value of the instance annotation.</param>
        internal void AddInstanceAnnotationForReading(string instanceAnnotationName, object instanceAnnotationValue)
        {
            DebugUtils.CheckNoExternalCallers();

            // We have deprecated the InstanceAnnotationCollection type. For backcompat:
            // 1. On reading we will populate both the InstanceAnnotationCollection object annotation and the ODataError.InstanceAnnotations collection.
            // 2. On writing, if ODataError.InstanceAnnotations collection is not empty, we will only serialize the ODataError.InstanceAnnotations collection.
            //    Otherwise we will serialize what's in the InstanceAnnotationCollection object annotation if it's present.
            ODataValue odataValue = instanceAnnotationValue.ToODataValue();

#pragma warning disable 618 // Disable "obsolete" warning for the InstanceAnnotationCollection. Used for backwards compatibilty.
            this.GetOrCreateAnnotation<InstanceAnnotationCollection>().Add(instanceAnnotationName, odataValue);
#pragma warning restore 618

            this.InstanceAnnotations.Add(new ODataInstanceAnnotation(instanceAnnotationName, odataValue));
        }
    }
}
