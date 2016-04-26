//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettingsBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Base Configuration settings for OData message writers.
    /// </summary>
    public abstract class ODataMessageWriterSettingsBase
    {
        /// <summary>Quotas to use for limiting resource consumption when writing an OData message.</summary>
        private ODataMessageQuotas messageQuotas;

        /// <summary> The check characters. </summary>
        private bool checkCharacters;

        /// <summary> The indent. </summary>
        private bool indent;

        /// <summary>
        /// Constructor to create default settings for OData writers.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:contains a call chain that results in a call to a virtual method defined by the class", Justification = "One derived type will only ever be created")]
        protected ODataMessageWriterSettingsBase()
        {
            // Dev Note: using private values with public properties because of violations 
            // assigning virtual properties in the constructors
            // On writing the default value for 'CheckCharacters' is set to false so that we
            // are symmetric with the reader. This allows the most amount of flexibility in 
            // terms of produced and consumed data.
            this.checkCharacters = false;
        }

        /// <summary>
        /// Copy constructor to create a copy of the settings for OData writers.
        /// </summary>
        /// <param name="other">Settings to create a copy from.</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:contains a call chain that results in a call to a virtual method defined by the class", Justification = "One derived type will only ever be created")]
        protected ODataMessageWriterSettingsBase(ODataMessageWriterSettingsBase other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.checkCharacters = other.checkCharacters;
            this.indent = other.indent;
            this.messageQuotas = new ODataMessageQuotas(other.MessageQuotas);
        }

        /// <summary>
        /// Flag to control whether the writer should use indentation or not.
        /// </summary>
        public virtual bool Indent
        {
            get { return this.indent; }
            set { this.indent = value; }
        }

        /// <summary>
        /// Flag to control whether the writer should check for valid Xml characters or not.
        /// </summary>
        public virtual bool CheckCharacters
        {
            get { return this.checkCharacters; }
            set { this.checkCharacters = value; }
        }

        /// <summary>
        /// Quotas to use for limiting resource consumption when writing an OData message.
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
    }
}