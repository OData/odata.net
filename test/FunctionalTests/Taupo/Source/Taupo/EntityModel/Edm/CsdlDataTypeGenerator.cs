//---------------------------------------------------------------------
// <copyright file="CsdlDataTypeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Generate data type information in csdl
    /// </summary>
    public class CsdlDataTypeGenerator : ICsdlDataTypeGenerator
    {
        private NamespaceAliasManager namespaceAliasManager;
        private GetTypeNameVisitor getTypeNameVisitor;

        /// <summary>
        /// Initializes a new instance of the CsdlDataTypeGenerator class.
        /// </summary>
        /// <param name="aliasManager">The namespace alias manager.</param>
        public CsdlDataTypeGenerator(NamespaceAliasManager aliasManager)
        {
            this.namespaceAliasManager = aliasManager;
            this.getTypeNameVisitor = new GetTypeNameVisitor(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to output the fully qualified data type.
        /// </summary>
        public bool OutputFullNameForPrimitiveDataType { get; set; }

        /// <summary>
        /// Generates property data type information
        /// </summary>
        /// <param name="memberProperty">The property.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        public IEnumerable<XObject> GeneratePropertyType(MemberProperty memberProperty, XNamespace xmlNamespace)
        {
            List<XObject> result = new List<XObject>();
            if (memberProperty.PropertyType != null)
            {
                CollectionDataType collectionDataType = memberProperty.PropertyType as CollectionDataType;
                if (collectionDataType != null)
                {
                    result.AddRange(this.GenerateCollection(collectionDataType));
                }
                else
                {
                    result.Add(this.GenerateTypeAttribute(memberProperty.PropertyType));
                    result.Add(this.GenerateNullableAttribute(memberProperty.PropertyType));
                    result.AddRange(this.GenerateTypeFacets(memberProperty.PropertyType).Cast<XObject>());
                }
            }

            return result;
        }

        /// <summary>
        /// Generates parameter data type information for Function
        /// </summary>
        /// <param name="parameter">The function parameter.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        public IEnumerable<XObject> GenerateParameterTypeForFunction(FunctionParameter parameter, XNamespace xmlNamespace)
        {
            return this.GenerateTypeElementOrAttributesInFunction(parameter.DataType, xmlNamespace);
        }

        /// <summary>
        /// Generates return type information for Function
        /// </summary>
        /// <param name="returnType">The return type of Function.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        public IEnumerable<XObject> GenerateReturnTypeForFunction(DataType returnType, XNamespace xmlNamespace)
        {
            List<XObject> result = new List<XObject>();
            if (returnType != null)
            {
                if (this.IsPrimitiveWithFacetsOrNullable(returnType))
                {
                    result.Add(new XElement(
                           xmlNamespace + "ReturnType",
                           this.GenerateTypeAttribute(returnType),
                           this.GenerateNullableAttributeInFunction(returnType),
                           this.GenerateTypeFacets(returnType)));
                }
                else if (this.ShouldUseElementToDescribe(returnType))
                {
                    result.Add(new XElement(xmlNamespace + "ReturnType", this.GenerateTypeElementInFunction(returnType, xmlNamespace)));
                }
                else
                {
                    result.Add(new XAttribute("ReturnType", this.GetDataTypeName(returnType)));
                }
            }

            return result;
        }

        /// <summary>
        /// Generates parameter data type information for Function Import
        /// </summary>
        /// <param name="parameter">The function parameter.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        public IEnumerable<XObject> GenerateParameterTypeForFunctionImport(FunctionParameter parameter, XNamespace xmlNamespace)
        {
            List<XObject> result = new List<XObject>();
            if (parameter.DataType != null)
            {
                result.Add(this.GenerateTypeAttribute(parameter.DataType));

                // no facets for In mode 
                if (parameter.Mode != FunctionParameterMode.In)
                {
                    result.AddRange(this.GenerateTypeFacets(parameter.DataType, new Type[] { typeof(IsUnicodeFacet) }).Cast<XObject>());
                }
            }

            return result;
        }

        /// <summary>
        /// Generates return type information for Function Import
        /// </summary>
        /// <param name="returnTypes">The return type of Function Import.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        public IEnumerable<XObject> GenerateReturnTypeForFunctionImport(IEnumerable<FunctionImportReturnType> returnTypes, XNamespace xmlNamespace)
        {
            List<XObject> result = new List<XObject>();
            if (returnTypes.Count() == 1)
            {
                var returnType = returnTypes.Single();
                result.Add(new XAttribute("ReturnType", this.GetDataTypeName(returnType.DataType)));
                if (returnType.EntitySet != null)
                {
                    result.Add(new XAttribute("EntitySet", returnType.EntitySet.Name));
                }
            }
            else
            {
                foreach (var returnType in returnTypes)
                {
                    result.Add(new XElement(xmlNamespace + "ReturnType", new XAttribute("Type", this.GetDataTypeName(returnType.DataType)), (returnType.EntitySet != null) ? new XAttribute("EntitySet", returnType.EntitySet.Name) : null));
                }
            }

            return result;
        }

        private IEnumerable<XObject> GenerateCollection(CollectionDataType collectionDataType)
        {
            var elementDataType = collectionDataType.ElementDataType;

            var result = new List<XObject>();
            result.Add(this.GenerateTypeAttribute(collectionDataType));
            result.Add(this.GenerateNullableAttribute(elementDataType));
            result.AddRange(this.GenerateTypeFacets(elementDataType).Cast<XObject>());

            return result;
        }

        private XAttribute GenerateTypeAttribute(DataType dataType)
        {
            var collectionDataType = dataType as CollectionDataType;
            if (null == collectionDataType)
            {
                return new XAttribute("Type", this.GetDataTypeName(dataType));
            }
            else
            {
                return new XAttribute("Type", "Collection(" + this.GetDataTypeName(collectionDataType.ElementDataType) + ")");
            }
        }

        private string GetDataTypeName(DataType type)
        {
            return this.getTypeNameVisitor.GetTypeName(type);
        }

        private XAttribute GenerateNullableAttribute(DataType dataType)
        {
            return dataType.IsNullable ? null : new XAttribute("Nullable", false);
        }

        private IEnumerable<XAttribute> GenerateTypeFacets(DataType dataType)
        {
            return this.GenerateTypeFacets(dataType, PlatformHelper.EmptyTypes);
        }

        private IEnumerable<XAttribute> GenerateTypeFacets(DataType dataType, Type[] facetsToIgnore)
        {
            PrimitiveDataType pdt = dataType as PrimitiveDataType;
            List<XAttribute> facets = new List<XAttribute>();

            if (pdt != null)
            {
                MaxLengthFacet maxLengthFacet;
                NumericScaleFacet numericScaleFacet;
                NumericPrecisionFacet numericPrecisionFacet;
                TimePrecisionFacet timePrecisionFacet;
                IsUnicodeFacet isUnicodeFacet;
                SridFacet sridFacet;

                if (!facetsToIgnore.Contains(typeof(MaxLengthFacet)) && pdt.TryGetFacet(out maxLengthFacet))
                {
                    facets.Add(new XAttribute("MaxLength", maxLengthFacet.Value));
                }

                if (!facetsToIgnore.Contains(typeof(NumericPrecisionFacet)) && pdt.TryGetFacet(out numericPrecisionFacet))
                {
                    facets.Add(new XAttribute("Precision", numericPrecisionFacet.Value));
                }

                if (!facetsToIgnore.Contains(typeof(NumericScaleFacet)) && pdt.TryGetFacet(out numericScaleFacet))
                {
                    facets.Add(new XAttribute("Scale", numericScaleFacet.Value));
                }

                if (!facetsToIgnore.Contains(typeof(TimePrecisionFacet)) && pdt.TryGetFacet(out timePrecisionFacet))
                {
                    facets.Add(new XAttribute("Precision", timePrecisionFacet.Value));
                }

                if (!facetsToIgnore.Contains(typeof(IsUnicodeFacet)) && pdt.TryGetFacet(out isUnicodeFacet))
                {
                    facets.Add(new XAttribute("Unicode", isUnicodeFacet.Value));
                }

                if (!facetsToIgnore.Contains(typeof(SridFacet)) && pdt.TryGetFacet(out sridFacet))
                {
                    facets.Add(new XAttribute("SRID", sridFacet.Value));
                }
            }

            return facets;
        }

        private IEnumerable<XObject> GenerateTypeElementOrAttributesInFunction(DataType dataType, XNamespace xmlNamespace)
        {
            List<XObject> result = new List<XObject>();
            if (dataType == null)
            {
                return result;
            }

            if (this.ShouldUseElementToDescribe(dataType))
            {
                result.Add(this.GenerateTypeElementInFunction(dataType, xmlNamespace));
            }
            else
            {
                result.Add(this.GenerateTypeAttribute(dataType));
            }

            result.Add(this.GenerateNullableAttributeInFunction(dataType));
            result.AddRange(this.GenerateTypeFacets(dataType).Cast<XObject>());

            return result;
        }

        private bool ShouldUseElementToDescribe(DataType dataType)
        {
            if (dataType is RowDataType)
            {
                return true;
            }

            var collection = dataType as CollectionDataType;
            if (collection != null)
            {
                // Shortcut notation does not support nesting, i.e. Collection(nominalType) and 
                // Ref(nominalType) are both supported, but Collection(Ref(nominalType)) isn't.
                if (!this.IsNominalType(collection.ElementDataType) ||
                    this.IsPrimitiveWithFacetsOrNullable(collection.ElementDataType) ||
                    !collection.ElementDataType.IsNullable)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        // TODO: refactor it into DataType itself
        private bool IsNominalType(DataType dataType)
        {
            return dataType is EntityDataType ||
                   dataType is ComplexDataType ||
                   dataType is PrimitiveDataType;
        }

        private bool IsPrimitiveWithFacetsOrNullable(DataType dataType)
        {
            var primitiveType = dataType as PrimitiveDataType;

            return (primitiveType != null) &&
                   (this.GenerateTypeFacets(primitiveType).Any() || !primitiveType.IsNullable);
        }

        private XElement GenerateTypeElementInFunction(DataType dataType, XNamespace xmlNamespace)
        {
            var collection = dataType as CollectionDataType;
            if (collection != null)
            {
                return this.GenerateCollectionElementInFunction(collection, xmlNamespace);
            }

            var reference = dataType as ReferenceDataType;
            if (reference != null)
            {
                return new XElement(
                            xmlNamespace + "ReferenceType",
                            new XAttribute("Type", reference.EntityType.FullName));
            }

            var row = dataType as RowDataType;
            ExceptionUtilities.Assert(row != null, "To generate XElement, DataType has to be either collection, reference or row!");

            return this.GenerateRowElementInFunction(row, xmlNamespace);
        }

        private XElement GenerateCollectionElementInFunction(CollectionDataType collection, XNamespace xmlNamespace)
        {
            var elementDataType = collection.ElementDataType;
            if (this.IsNominalType(elementDataType))
            {
                return new XElement(
                    xmlNamespace + "CollectionType",
                    new XAttribute("ElementType", this.GetDataTypeName(elementDataType)),
                    this.GenerateNullableAttributeInFunction(elementDataType),
                    this.GenerateTypeFacets(elementDataType));
            }
            else
            {
                return new XElement(
                        xmlNamespace + "CollectionType",
                        this.GenerateTypeElementInFunction(elementDataType, xmlNamespace));
            }
        }

        private XElement GenerateRowElementInFunction(RowDataType row, XNamespace xmlNamespace)
        {
            var rowProperties = row.Definition.Properties.Select(
                                    p => new XElement(
                                            xmlNamespace + "Property",
                                            p.Name != null ? new XAttribute("Name", p.Name) : null,
                                            this.GenerateTypeElementOrAttributesInFunction(p.PropertyType, xmlNamespace)));

            return new XElement(xmlNamespace + "RowType", rowProperties);
        }

        private XAttribute GenerateNullableAttributeInFunction(DataType dataType)
        {
            if (this.CanHaveNullableAttributeInFunction(dataType))
            {
                return this.GenerateNullableAttribute(dataType);
            }

            return null;
        }

        private bool CanHaveNullableAttributeInFunction(DataType dataType)
        {
            return dataType is PrimitiveDataType || dataType is EnumDataType || dataType is ComplexDataType;
        }

        private string GetFullyQualifiedName(INamedItem type)
        {
            return this.namespaceAliasManager.GetFullyQualifiedName(type);
        }

        private string GetPrimitiveDataTypeName(PrimitiveDataType primitiveType)
        {
            var enumDataType = primitiveType as EnumDataType;
            if (enumDataType != null)
            {
                return this.GetFullyQualifiedName(enumDataType.Definition);
            }

            if (this.OutputFullNameForPrimitiveDataType)
            {
                return EdmDataTypes.GetEdmFullName(primitiveType);
            }

            return EdmDataTypes.GetEdmName(primitiveType);
        }

        /// <summary>
        /// Visitor which returns string name for a data type
        /// </summary>
        private class GetTypeNameVisitor : IDataTypeVisitor<string>
        {
            private CsdlDataTypeGenerator parent;

            internal GetTypeNameVisitor(CsdlDataTypeGenerator parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// Gets the full name of the type.
            /// </summary>
            /// <param name="type">The type to get full name of.</param>
            /// <returns>Fully qualified type name.</returns>
            public string GetTypeName(DataType type)
            {
                return type.Accept(this);
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Name of the collection type.</returns>
            string IDataTypeVisitor<string>.Visit(CollectionDataType dataType)
            {
                return "Collection(" + this.GetTypeName(dataType.ElementDataType) + ")";
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Name of the complex type.</returns>
            string IDataTypeVisitor<string>.Visit(ComplexDataType dataType)
            {
                return this.parent.GetFullyQualifiedName(dataType.Definition);
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Name of the entity type.</returns>
            string IDataTypeVisitor<string>.Visit(EntityDataType dataType)
            {
                return this.parent.GetFullyQualifiedName(dataType.Definition);
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Name of the primitive type.</returns>
            string IDataTypeVisitor<string>.Visit(PrimitiveDataType dataType)
            {
                return this.parent.GetPrimitiveDataTypeName(dataType);
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Name of the reference type.</returns>
            public string Visit(ReferenceDataType dataType)
            {
                return "Ref(" + this.parent.GetFullyQualifiedName(dataType.EntityType) + ")";
            }
        }
    }
}
