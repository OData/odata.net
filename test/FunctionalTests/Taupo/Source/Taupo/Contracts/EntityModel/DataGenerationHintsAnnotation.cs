//---------------------------------------------------------------------
// <copyright file="DataGenerationHintsAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    
    /// <summary>
    /// Annotation which represents hints for data generation.
    /// </summary>
    public class DataGenerationHintsAnnotation : Annotation, ICustomAnnotationSerializer
    {
        private List<DataGenerationHint> hints;

        /// <summary>
        /// Initializes a new instance of the DataGenerationHintsAnnotation class.
        /// </summary>
        /// <param name="element">XElement that contains the information.</param>
        public DataGenerationHintsAnnotation(XElement element)
            : this()
        {
            foreach (var hintAsXml in element.Elements())
            {
                this.Add(DataGenerationHints.FromXml(hintAsXml));
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataGenerationHintsAnnotation class.
        /// </summary>
        internal DataGenerationHintsAnnotation()
        {
            this.hints = new List<DataGenerationHint>();
        }

        /// <summary>
        /// Gets the data generation hints.
        /// </summary>
        public IEnumerable<DataGenerationHint> Hints
        {
            get { return this.hints; }
        }

        /// <summary>
        /// Adds a data generation hint.
        /// </summary>
        /// <param name="hint">The data generation hint to add.</param>
        public void Add(DataGenerationHint hint)
        {
            ExceptionUtilities.CheckArgumentNotNull(hint, "hint");

            if (!this.hints.Any(h => h.Equals(hint)))
            {
                this.hints.Add(hint);
            }
        }

        /// <summary>
        /// Gets the XObject representing the annotation when serialized to CSDL/SSDL file.
        /// </summary>
        /// <returns>XObject representing the annotation.</returns>
        public XObject GetXObject()
        {
            var element = new XElement(EdmConstants.TaupoAnnotationsNamespace + this.GetType().Name);
            foreach (var hint in this.Hints)
            {
                element.Add(DataGenerationHints.ToXml(hint));
            }

            return element;
        }
    }
}
