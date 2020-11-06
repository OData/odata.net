//---------------------------------------------------------------------
// <copyright file="CsdlJsonReaderSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Settings used when parsing CSDL-JSON document.
    /// </summary>
    public class CsdlJsonReaderSettings : CsdlReaderSettingsBase
    {
        public static CsdlJsonReaderSettings Default = new CsdlJsonReaderSettings();

        /// <summary>
        /// Initializes a new instance of <see cref="CsdlJsonReaderSettings"/> class.
        /// </summary>
        public CsdlJsonReaderSettings()
        {
            IsBracketNotation = false;
            IncludeDefaultVocabularies = true;
            IgnoreUnexpectedJsonElements = true;
        }

        /// <summary>
        /// Gets/sets the referened models.
        /// </summary>
        public IEnumerable<IEdmModel> ReferencedModels { get; set; }

        /// <summary>
        /// Gets/sets a call back to provide the JSON reader for referenced CSDL-JSON.
        /// </summary>
        public CsdlJsonReaderFactory JsonSchemaReaderFactory { get; set;}

        /// <summary>
        /// Gets/sets a boolean value indicating whether include the default vocabularies Edm model.
        /// By default, it's true.
        /// </summary>
        public bool IncludeDefaultVocabularies { get; set; }

        /// <summary>
        /// Gets/sets a boolean value indicating whether ignore the unexpected JSON elements.
        /// By default, it's true.
        /// </summary>
        public bool IgnoreUnexpectedJsonElements { get; set; }

        /// <summary>
        /// Gets/sets a boolean value indicating the format of the JSON path.
        /// If true: $['store']['book'][0]['title']
        /// If false: $.store.book[0].title
        /// By default, it's false.
        /// </summary>
        public bool IsBracketNotation { get; set; }

        internal CsdlJsonReaderSettings Clone()
        {
            return new CsdlJsonReaderSettings
            {
                IsBracketNotation = this.IsBracketNotation,
                ReferencedModels = this.ReferencedModels,
                JsonSchemaReaderFactory = this.JsonSchemaReaderFactory,
                IgnoreUnexpectedJsonElements = this.IgnoreUnexpectedJsonElements,
                IncludeDefaultVocabularies = this.IncludeDefaultVocabularies
            };
        }
    }
}
#endif