//---------------------------------------------------------------------
// <copyright file="TaupoToEdmModelConverterUsingParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Edmlib.Contracts;
    using Microsoft.Test.Taupo.EntityModel.Edm;

    /// <summary>
    /// Converts a model from Taupo term into Edm term
    /// </summary>
    [ImplementationName(typeof(ITaupoToEdmModelConverter), "ProductParser")]
    public class TaupoToEdmModelConverterUsingParser : ITaupoToEdmModelConverter
    {
        /// <summary>
        /// Initializes a new instance of the TaupoToEdmModelConverterUsingParser class.
        /// </summary>
        public TaupoToEdmModelConverterUsingParser()
        {
            this.EdmVersion = EdmVersion.V40;
            this.GenerateTaupoAnnotations = false;
            this.IgnoreUnsupportedAnnotations = false;
        }

        /// <summary>
        /// Gets or sets the EdmVersion
        /// </summary>
        [InjectTestParameter("EdmVersion")]
        public EdmVersion EdmVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to generate taupo annotations
        /// </summary>
        public bool GenerateTaupoAnnotations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore unsupported annotations
        /// </summary>
        public bool IgnoreUnsupportedAnnotations { get; set; }

        /// <summary>
        /// Converts a model from Taupo term into Edm term
        /// </summary>
        /// <param name="taupoModel">The input model in Taupo term</param>
        /// <returns>The output model in Edm term</returns>
        public IEdmModel ConvertToEdmModel(EntityModelSchema taupoModel)
        {
            IEnumerable<XElement> csdlElements = this.GenerateCsdlElements(taupoModel);
            IEdmModel resultEdmModel = this.GetParserResult(csdlElements);

            return resultEdmModel;
        }

        private IEnumerable<XElement> GenerateCsdlElements(EntityModelSchema taupoModel)
        {
            var taupoCsdlGen = new CsdlContentGenerator 
            { 
                GenerateTaupoAnnotations = this.GenerateTaupoAnnotations,
                IgnoreUnsupportedAnnotations = this.IgnoreUnsupportedAnnotations
            };
            IEnumerable<FileContents<XElement>> csdlFileContents = taupoCsdlGen.Generate(this.EdmVersion, taupoModel);
            return csdlFileContents.Select(fc => fc.Contents);
        }

        private IEdmModel GetParserResult(IEnumerable<XElement> csdlElements)
        {
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            bool success = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out edmModel, out errors);
            
            ExceptionUtilities.Assert(success, "Parse failed -- are you using an invalid model?");
            return edmModel;
        }
    }
}
