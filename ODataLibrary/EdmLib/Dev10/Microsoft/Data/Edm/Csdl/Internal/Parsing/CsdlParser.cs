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

using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool success = true;
        private readonly CsdlModel result = new CsdlModel();

        public static bool TryParse(IEnumerable<XmlReader> csdlReaders, out CsdlModel entityModel, out IEnumerable<EdmError> errors)
        {
            int readerCount = 0;
            foreach (var readerInfo in csdlReaders)
            {
                if (null == readerInfo)
                {
                    throw new ArgumentNullException("csdlReaders");
                }

                readerCount++;
            }

            if (readerCount == 0)
            {
                entityModel = null;
                errors = Enumerable.Empty<EdmError>();
                //TODO: add error for no readers
                return false;
            }

            CsdlParser parser = new CsdlParser();
            foreach (var inputReader in csdlReaders)
            {
                parser.AddReader(inputReader);
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
