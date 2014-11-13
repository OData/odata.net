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
