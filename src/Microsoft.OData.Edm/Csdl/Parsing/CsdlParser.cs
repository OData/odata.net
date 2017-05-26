//---------------------------------------------------------------------
// <copyright file="CsdlParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides for the loading and conversion of one or more CSDL XML readers into Entity Data Model.
    /// </summary>
    internal class CsdlParser
    {
        private readonly List<EdmError> errorsList = new List<EdmError>();
        private readonly CsdlModel result = new CsdlModel();

        private bool success = true;

        public static bool TryParse(IEnumerable<XmlReader> csdlReaders, out CsdlModel entityModel, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(csdlReaders, "csdlReaders");
            CsdlParser parser = new CsdlParser();
            int readerCount = 0;
            foreach (var inputReader in csdlReaders)
            {
                if (inputReader != null)
                {
                    try
                    {
                        parser.AddReader(inputReader);
                    }
                    catch (XmlException e)
                    {
                        entityModel = null;
                        errors = new EdmError[] { new EdmError(new CsdlLocation(e.LineNumber, e.LinePosition), EdmErrorCode.XmlError, e.Message) };

                        return false;
                    }
                }
                else
                {
                    entityModel = null;
                    errors = new EdmError[] { new EdmError(null, EdmErrorCode.NullXmlReader, Edm.Strings.CsdlParser_NullXmlReader) };

                    return false;
                }

                readerCount++;
            }

            if (readerCount == 0)
            {
                entityModel = null;
                errors = new EdmError[] { new EdmError(null, EdmErrorCode.NoReadersProvided, Edm.Strings.CsdlParser_NoReadersProvided) };

                return false;
            }

            bool success = parser.GetResult(out entityModel, out errors);
            if (!success)
            {
                entityModel = null;
            }

            return success;
        }

        public bool AddReader(XmlReader csdlReader, string source = null)
        {
            // If source is determined (empty means no source), use source;
            // otherwise try to use BaseURI from XmlReader.
            string artifactPath = source ?? csdlReader.BaseURI;

            // Create a new CsdlDocumentParser to parse a single CSDL document.
            CsdlDocumentParser docParser = new CsdlDocumentParser(artifactPath, csdlReader);

            // Initialize the parser and continue if a valid root element was found
            docParser.ParseDocumentElement();

            // Do not move on to other readers if errors were encountered
            this.success &= !docParser.HasErrors;

            // Gather any errors that occurred, regardless of success
            this.errorsList.AddRange(docParser.Errors);

            if (docParser.Result != null)
            {
                this.result.AddSchema(docParser.Result.Value);
            }

            return this.success;
        }

        public bool GetResult(out CsdlModel model, out IEnumerable<EdmError> errors)
        {
            model = this.result;
            errors = this.errorsList;
            return this.success;
        }
    }
}
