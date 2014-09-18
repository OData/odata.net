//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Representing Capabilities Vocabulary Model.
    /// </summary>
    internal class CapabilitiesVocabularyModel
    {
        /// <summary>
        /// The EDM model with capabilities vocabularies.
        /// </summary>
        public static readonly IEdmModel Instance;

        /// <summary>
        /// The change tracking term.
        /// </summary>
        public static readonly IEdmValueTerm ChangeTrackingTerm;

        /// <summary>
        /// Parse Capabilities Vocabulary Model from CapabilitiesVocabularies.xml
        /// </summary>
        static CapabilitiesVocabularyModel()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("CapabilitiesVocabularies.xml"))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "CapabilitiesVocabularies.xml: stream!=null");
                CsdlReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
            }

            ChangeTrackingTerm = Instance.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.ChangeTracking);
        }
    }
}
