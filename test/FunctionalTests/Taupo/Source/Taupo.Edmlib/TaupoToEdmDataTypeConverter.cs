//---------------------------------------------------------------------
// <copyright file="TaupoToEdmDataTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Converts data type from Taupo term into Edm term
    /// </summary>
    public sealed class TaupoToEdmDataTypeConverter : IDataTypeVisitor<IEdmTypeReference>
    {
        private IEdmModel currentEdmModel;
        private TaupoToEdmPrimitiveDataTypeConverter primitiveConverter;

        /// <summary>
        /// Initializes a new instance of the TaupoToEdmDataTypeConverter class.
        /// </summary>
        /// <param name="currentEdmModel">The model in Edm term</param>
        public TaupoToEdmDataTypeConverter(IEdmModel currentEdmModel)
        {
            this.currentEdmModel = currentEdmModel;
            this.primitiveConverter = new TaupoToEdmPrimitiveDataTypeConverter();
        }

        /// <summary>
        /// Converts data type from Taupo term into Edm term
        /// </summary>
        /// <param name="taupoDataType">The DataType (Taupo term)</param>
        /// <returns>The TypeReference (Edm term)</returns>
        public IEdmTypeReference ConvertToEdmTypeReference(DataType taupoDataType)
        {
            return taupoDataType.Accept(this);
        }

        IEdmTypeReference IDataTypeVisitor<IEdmTypeReference>.Visit(CollectionDataType dataType)
        {
            var elementTypeReference = this.ConvertToEdmTypeReference(dataType.ElementDataType);
            return elementTypeReference.ToCollectionTypeReference()
                                       .Nullable(dataType.IsNullable);
        }

        IEdmTypeReference IDataTypeVisitor<IEdmTypeReference>.Visit(ComplexDataType dataType)
        {
            var complexTypeDefinition = (IEdmComplexType)this.currentEdmModel.FindType(dataType.Definition.FullName);
            return complexTypeDefinition.ToTypeReference()
                                        .Nullable(dataType.IsNullable);
        }

        IEdmTypeReference IDataTypeVisitor<IEdmTypeReference>.Visit(EntityDataType dataType)
        {
            var entityTypeDefinition = (IEdmEntityType)this.currentEdmModel.FindType(dataType.Definition.FullName);
            return entityTypeDefinition.ToTypeReference()
                                       .Nullable(dataType.IsNullable);
        }

        IEdmTypeReference IDataTypeVisitor<IEdmTypeReference>.Visit(PrimitiveDataType dataType)
        {
            return this.primitiveConverter.ConvertToEdmTypeReference(dataType);
        }

        IEdmTypeReference IDataTypeVisitor<IEdmTypeReference>.Visit(ReferenceDataType dataType)
        {
            throw new System.NotImplementedException();
            ////var entityTypeDefinition = (IEdmEntityType)this.currentEdmModel.FindSchemaElement(dataType.EntityType.FullName);
            ////return entityTypeDefinition.ToEntityReferenceTypeReference()
            ////                           .Nullable(dataType.IsNullable);
        }
    }
}
