//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

using System.Collections.Generic;
using System.Xml;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing
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

        public bool AddReader(XmlReader csdlReader)
        {
            string artifactPath = csdlReader.BaseURI ?? string.Empty;

            // Create a new CsdlDocumentParser to parse a single CSDL document.
            CsdlDocumentParser docParser = new CsdlDocumentParser(artifactPath, csdlReader);

            // Initialize the parser and continue if a valid root element was found
            docParser.ParseDocumentElement();

            // Do not move on to other readers if errors were encountered
            this.success &= !docParser.HasErrors;

            // Gather any errors that occured, regardless of success
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
