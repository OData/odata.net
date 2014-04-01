//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Vocabularis
{
    /// <summary>
    /// Representing Core Vocabulary Model.
    /// </summary>
    internal class CoreVocabularyModel
    {
        /// <summary>
        /// The EDM model with core vocabularies.
        /// </summary>
        public static readonly IEdmModel Instance = ParseCoreVocabularyModel();

        /// <summary>
        /// Parse Core Vocabulary Model from CoreVocabularies.xml
        /// </summary>
        /// <returns>Core Vocabulary Model</returns>
        private static IEdmModel ParseCoreVocabularyModel()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            IEdmModel model;
            using (Stream stream = assembly.GetManifestResourceStream("CoreVocabularies.xml"))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "CoreVocabularies.xml: stream!=null");
                CsdlReader.TryParse(new[] { XmlReader.Create(stream) }, out model, out errors);
            }
            
            return model;
        }
    }
}
