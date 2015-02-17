//---------------------------------------------------------------------
// <copyright file="ODataEntityModelSchemaComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.EntityModel;
    #endregion

    /// <summary>
    /// Way to compare two EntityModelSchemas together
    /// </summary>
    public class ODataEntityModelSchemaComparer : EntityModelSchemaComparer
    {
        /// <summary>
        /// Compares the actual function import against the expected.
        /// </summary>
        /// <param name="expectedFunctionImport">expected function import</param>
        /// <param name="actualFunctionImport">actual import</param>
        /// <remarks>This implementation also compares the annotations on the function import.</remarks>
        protected override void CompareFunctionImport(FunctionImport expectedFunctionImport, FunctionImport actualFunctionImport)
        {
            // TODO: Add support for checking annotations to the EntityModelSchemaComparer.CompareFunctionImport method.
            // We have overridden the base method because we wanted to minimize the impact of this change to other existing Taupo test cases.
            base.CompareFunctionImport(expectedFunctionImport, actualFunctionImport);

            if (!this.WriteErrorIfFalse(
                 expectedFunctionImport.Annotations.OfType<AttributeAnnotation>().Count() == actualFunctionImport.Annotations.OfType<AttributeAnnotation>().Count(),
                 "Expected and actual count of the FunctionImport annotations did not match."))
            {
                // verify annotations
                foreach (AttributeAnnotation expectedAnnotation in expectedFunctionImport.Annotations.OfType<AttributeAnnotation>())
                {
                    AttributeAnnotation actualAnnotation = actualFunctionImport.Annotations.OfType<AttributeAnnotation>().SingleOrDefault(
                        ann => ann.Content.Name.Equals(expectedAnnotation.Content.Name));
                        
                    if (!this.WriteErrorIfFalse(actualAnnotation != null, "The expected annotation named '{0}' was not found in the FunctionImport.", expectedAnnotation.Content.Name.LocalName))
                    {
                        this.WriteErrorIfFalse(
                            actualAnnotation.Content.Value == expectedAnnotation.Content.Value,
                            "FunctionImport annotation not equal. Expected {0}, Actual: {1}",
                            expectedAnnotation.Content.Value,
                            actualAnnotation.Content.Value);
                    }
                }
            }
        }
    }
}
