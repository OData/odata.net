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

            ChangeTrackingTerm = Instance.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.CapabilitiesChangeTracking);
        }
    }
}
