//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Base Configuration settings for OData message readers.
    /// </summary>
    public abstract class ODataMessageReaderSettingsBase
    {
        /// <summary> Quotas to use for limiting resource consumption when reading an OData message. </summary>
        private ODataMessageQuotas messageQuotas;

        /// <summary> The check characters. </summary>
        private bool checkCharacters;

        /// <summary> The enable atom metadata reading. </summary>
        private bool enableAtomMetadataReading;

        /// <summary> The annotation filter. </summary>
        private Func<string, bool> shouldIncludeAnnotation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataMessageReaderSettingsBase" /> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:contains a call chain that results in a call to a virtual method defined by the class", Justification = "One derived type will only ever be created")]
        protected ODataMessageReaderSettingsBase()
        {
            // Dev Note: using private values with public properties because of violations 
            // assigning virtual properties in the constructors
            // On reading the default value for 'CheckCharacters' is set to false so that we 
            // can consume valid and invalid Xml documents per default.
            this.checkCharacters = false;

            // ATOM metadata reading is disabled by default for performance reasons and because 
            // few clients will need the ATOM metadata.
            this.enableAtomMetadataReading = false;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:contains a call chain that results in a call to a virtual method defined by the class", Justification = "One derived type will only ever be created")]
        protected ODataMessageReaderSettingsBase(ODataMessageReaderSettingsBase other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.checkCharacters = other.checkCharacters;
            this.enableAtomMetadataReading = other.enableAtomMetadataReading;
            this.messageQuotas = new ODataMessageQuotas(other.MessageQuotas);
            this.shouldIncludeAnnotation = other.shouldIncludeAnnotation;
        }

        /// <summary>
        /// Flag to control whether the reader should check for valid Xml characters or not.
        /// </summary>
        public virtual bool CheckCharacters
        {
            get { return this.checkCharacters; }
            set { this.checkCharacters = value; }
        }
        
        /// <summary>
        /// Flag to control whether ATOM metadata is read in ATOM payloads.
        /// </summary>
        public virtual bool EnableAtomMetadataReading
        {
            get { return this.enableAtomMetadataReading; }
            set { this.enableAtomMetadataReading = value; }
        }
        
        /// <summary>
        /// Quotas to use for limiting resource consumption when reading an OData message.
        /// </summary>
        public virtual ODataMessageQuotas MessageQuotas
        {
            get
            {
                if (this.messageQuotas == null)
                {
                    this.messageQuotas = new ODataMessageQuotas();
                }

                return this.messageQuotas;
            }

            set
            {
                this.messageQuotas = value;
            }
        }

        /// <summary>
        /// Func to evaluate whether an annotation should be read or skipped by the reader. The func should return true if the annotation should
        /// be read and false if the annotation should be skipped. A null value indicates that all annotations should be skipped.
        /// </summary>
        public virtual Func<string, bool> ShouldIncludeAnnotation
        {
            get { return this.shouldIncludeAnnotation; }
            set { this.shouldIncludeAnnotation = value; }
        }
    }
}
