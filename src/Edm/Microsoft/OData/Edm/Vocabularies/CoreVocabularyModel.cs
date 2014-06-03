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
        public static readonly IEdmModel Instance;

        /// <summary>
        /// The concurrency control term.
        /// </summary>
        public static readonly IEdmValueTerm ConcurrencyControlTerm;

        /// <summary>
        /// The description term.
        /// </summary>
        public static readonly IEdmValueTerm DescriptionTerm;

        /// <summary>
        /// The description term.
        /// </summary>
        public static readonly IEdmValueTerm LongDescriptionTerm;

        /// <summary>
        /// Parse Core Vocabulary Model from CoreVocabularies.xml
        /// </summary>
        static CoreVocabularyModel()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("CoreVocabularies.xml"))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "CoreVocabularies.xml: stream!=null");
                CsdlReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
            }

            ConcurrencyControlTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.CoreOptimisticConcurrencyControl);
            DescriptionTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.CoreDescription);
            LongDescriptionTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.CoreLongDescription);
        }
    }
}
