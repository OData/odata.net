//---------------------------------------------------------------------
// <copyright file="ExpectedTypeODataPayloadElementAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    #endregion Namespaces

    /// <summary>
    /// An OData payload element annotation which specified expected type for a top-level entity, set or property.
    /// The value is used when reading by passing it as the expected type to the reader.
    /// </summary>
    public sealed class ExpectedTypeODataPayloadElementAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// The expected edm type.
        /// </summary>
        public IEdmTypeReference EdmExpectedType { get; set; }

        /// <summary>
        /// The expected type.
        /// </summary>
        public DataType ExpectedType { get; set; }

        /// <summary>
        /// The expected entity set.
        /// </summary>
        public IEdmEntitySet EdmEntitySet { get; set; }

        /// <summary>
        /// The expected entity set.
        /// </summary>
        public EntitySet EntitySet { get; set; }

        /// <summary>
        /// The expected function import producing the payload.
        /// </summary>
        public FunctionImport FunctionImport { get; set; }

        /// <summary>
        /// The expected function import producing the payload.
        /// </summary>
        public IEdmOperationImport ProductFunctionImport { get; set; }

        /// <summary>
        /// The type owning the member or navigation property.
        /// </summary>
        public IEdmType EdmOwningType { get; set; }

        /// <summary>
        /// The type owning the member or navigation property.
        /// </summary>
        public NamedStructuralType OwningType { get; set; }

        /// <summary>
        /// The member property describing a property payload.
        /// </summary>
        public MemberProperty MemberProperty { get; set; }

        /// <summary>
        /// The member property describing a property payload.
        /// </summary>
        public IEdmProperty EdmProperty { get; set; }

        /// <summary>
        /// The name of an open property on the expected type.
        /// </summary>
        public string OpenMemberPropertyName { get; set; }

        /// <summary>
        /// The navigation property describing entity reference link payloads.
        /// </summary>
        public IEdmProperty EdmNavigationProperty { get; set; }

        /// <summary>
        /// The navigation property describing entity reference link payloads.
        /// </summary>
        public NavigationProperty NavigationProperty { get; set; }

        /// <summary>
        /// The string description of this annotation.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                if (this.EdmExpectedType != null || this.EdmEntitySet != null)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(this.EdmExpectedType == null ? "No expected type." : "ExpectedType: " + this.EdmExpectedType);
                    builder.AppendLine(this.EdmEntitySet == null ? "No entity set." : "EntitySet: " + this.EdmEntitySet.Name);
                    return builder.ToString();
                }
                
                if (this.ExpectedType != null || this.EntitySet != null)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(this.ExpectedType == null ? "No expected type." : "ExpectedType: " + this.ExpectedType);
                    builder.AppendLine(this.EntitySet == null ? "No entity set." : "EntitySet: " + this.EntitySet.Name);
                    return builder.ToString();
                }

                return "No expected type or entity set.";
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new ExpectedTypeODataPayloadElementAnnotation
            {
                ExpectedType = this.ExpectedType,
                EdmExpectedType = this.EdmExpectedType,
                EntitySet = this.EntitySet,
                EdmEntitySet = this.EdmEntitySet,
                FunctionImport = this.FunctionImport,
                ProductFunctionImport = this.ProductFunctionImport,
                OwningType = this.OwningType,
                EdmOwningType = this.EdmOwningType,
                MemberProperty = this.MemberProperty,
                EdmProperty = this.EdmProperty,
                NavigationProperty = this.NavigationProperty,
                EdmNavigationProperty = this.EdmNavigationProperty,
                OpenMemberPropertyName = this.OpenMemberPropertyName,
            };
        }
    }
}
