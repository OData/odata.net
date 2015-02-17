//---------------------------------------------------------------------
// <copyright file="EnumDataTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Resolves enum definitions in enum data types
    /// </summary>
    public class EnumDataTypeResolver : IDataTypeVisitor<DataType>
    {
        private EntityModelSchema model;
        private Stack<string> fixupNamespaces = new Stack<string>();
        private Dictionary<EnumType, EnumType> lookup = new Dictionary<EnumType, EnumType>();
        private HashSet<StructuralType> visitedStructuralTypes = new HashSet<StructuralType>();

        /// <summary>
        /// Initializes a new instance of the EnumDataTypeResolver class.
        /// </summary>
        /// <param name="model">The entity model.</param>
        public EnumDataTypeResolver(EntityModelSchema model)
        {
            // Note: defaultFixupNamespace allowed to be null or empty. 
            var defaultFixupNamespace = this.GetDefaultNamespaceName(model);

            this.fixupNamespaces.Push(defaultFixupNamespace);

            this.model = model;
        }

        /// <summary>
        /// Resolves enum definitions in enum data types.
        /// </summary>
        public void ResolveEnumDataTypes()
        {
            foreach (var type in this.model.ComplexTypes)
            {
                this.EnumDefinitionFixup(type.NamespaceName, type);
            }

            foreach (var type in this.model.EntityTypes)
            {
                this.EnumDefinitionFixup(type.NamespaceName, type);
            }

            foreach (var function in this.model.Functions)
            {
                this.EnumDefinitionFixup(function.NamespaceName, function.Parameters);
                function.ReturnType = this.ResolveDataType(function.ReturnType);
            }

            foreach (var container in this.model.EntityContainers)
            {
                foreach (var functionImport in container.FunctionImports)
                {
                    this.EnumDefinitionFixup(null, functionImport.Parameters);
                    foreach (var returnType in functionImport.ReturnTypes)
                    {
                        returnType.DataType = this.ResolveDataType(returnType.DataType);
                    }
                }
            }
        }

        /// <summary>
        /// Resolves enum definitions in enum data types
        /// </summary>
        /// <param name="dataType">the data type</param>
        /// <returns>Resolved data type</returns>
        public DataType Visit(CollectionDataType dataType)
        {
            var resolvedElementDataType = this.ResolveDataType(dataType.ElementDataType);
            if (resolvedElementDataType != dataType.ElementDataType)
            {
                return dataType.WithElementDataType(resolvedElementDataType);
            }

            return dataType;
        }

        /// <summary>
        /// Resolves enum definitions in enum data types
        /// </summary>
        /// <param name="dataType">the data type</param>
        /// <returns>Resolved data type</returns>
        public DataType Visit(ComplexDataType dataType)
        {
            this.EnumDefinitionFixup(dataType.Definition.NamespaceName, dataType.Definition);
            return dataType;
        }

        /// <summary>
        /// Resolves enum definitions in enum data types
        /// </summary>
        /// <param name="dataType">the data type</param>
        /// <returns>Resolved data type</returns>
        public DataType Visit(EntityDataType dataType)
        {
            this.EnumDefinitionFixup(dataType.Definition.NamespaceName, dataType.Definition);
            return dataType;
        }

        /// <summary>
        /// Resolves enum definitions in enum data types
        /// </summary>
        /// <param name="dataType">the data type</param>
        /// <returns>Resolved data type</returns>
        public DataType Visit(PrimitiveDataType dataType)
        {
            var resolved = dataType;
            var enumDataType = dataType as EnumDataType;
            if (enumDataType != null && !(enumDataType.Definition is EnumTypeReference))
            {
                var enumType = enumDataType.Definition;
                var modelEnumType = this.GetModelEnumType(enumType);
                if (modelEnumType != enumType)
                {
                    resolved = enumDataType.WithDefinition(modelEnumType);
                }
            }

            return resolved;
        }

        /// <summary>
        /// Resolves enum definitions in enum data types
        /// </summary>
        /// <param name="dataType">the data type</param>
        /// <returns>Resolved data type</returns>
        public DataType Visit(ReferenceDataType dataType)
        {
            this.EnumDefinitionFixup(dataType.EntityType.NamespaceName, dataType.EntityType);
            return dataType;
        }

        /// <summary>
        /// Resolves enum definitions in enum data types
        /// </summary>
        /// <param name="dataType">the data type</param>
        /// <returns>Resolved data type</returns>
        public DataType Visit(RowDataType dataType)
        {
            this.EnumDefinitionFixup(null, dataType.Definition);
            return dataType;
        }

        private void EnumDefinitionFixup(string fixupNamespace, StructuralType type)
        {
            if (this.visitedStructuralTypes.Contains(type))
            {
                return;
            }

            this.visitedStructuralTypes.Add(type);

            this.StartNamespaceNameScope(fixupNamespace);

            foreach (MemberProperty prop in type.Properties)    
            {
                prop.PropertyType = this.ResolveDataType(prop.PropertyType);
            }

            this.EndNamespaceNameScope(fixupNamespace);
        }

        private void EnumDefinitionFixup(string fixupNamespace, IEnumerable<FunctionParameter> parameters)
        {
            this.StartNamespaceNameScope(fixupNamespace);

            foreach (var parameter in parameters)
            {
                parameter.DataType = this.ResolveDataType(parameter.DataType);
            }

            this.EndNamespaceNameScope(fixupNamespace);
        }

        private EnumType GetModelEnumType(EnumType enumType)
        {
            EnumType modelEnumType = enumType;
            var fixupNamespace = this.fixupNamespaces.Peek();
            if (!this.model.EnumTypes.Contains(enumType) && !this.lookup.TryGetValue(enumType, out modelEnumType))
            {
                modelEnumType = this.model.EnumTypes.SingleOrDefault(e => e.Name == enumType.Name && e.NamespaceName == (enumType.NamespaceName ?? fixupNamespace));
                if (modelEnumType == null)
                {
                    modelEnumType = enumType.Clone();

                    if (string.IsNullOrEmpty(enumType.NamespaceName))
                    {
                        modelEnumType.NamespaceName = fixupNamespace;
                    }
                    else
                    {
                        modelEnumType.NamespaceName = enumType.NamespaceName;
                    }

                    this.lookup.Add(enumType, modelEnumType);
                    this.model.Add(modelEnumType);
                }
            }
            else if (string.IsNullOrEmpty(enumType.NamespaceName))
            {
                modelEnumType.NamespaceName = fixupNamespace;
            }

            return modelEnumType;
        }

        private DataType ResolveDataType(DataType dataType)
        {
            if (dataType == null)
            {
                return null;
            }

            return dataType.Accept(this);
        }

        private string GetDefaultNamespaceName(EntityModelSchema entityModelSchema)
        {
            // Silverlight does not support Union and Concat, also explicit Cast is required
            string namespaceName = null;
            foreach (var namedItems in new IEnumerable<INamedItem>[] { entityModelSchema.Functions.Cast<INamedItem>(), entityModelSchema.EntityTypes.Cast<INamedItem>(), entityModelSchema.ComplexTypes.Cast<INamedItem>() })
            {
                namespaceName = namedItems.Where(x => x.NamespaceName != null).Select(x => x.NamespaceName).FirstOrDefault();
                if (namespaceName != null)
                {
                    break;
                }
            }

            return namespaceName;
        }

        private void StartNamespaceNameScope(string namespaceName)
        {
            if (namespaceName != null)
            {
                this.fixupNamespaces.Push(namespaceName);
            }
        }

        private void EndNamespaceNameScope(string namespaceName)
        {
            if (namespaceName != null)
            {
                this.fixupNamespaces.Pop();
            }
        }
    }
}
