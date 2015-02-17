//---------------------------------------------------------------------
// <copyright file="EntityToDatabaseModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DatabaseModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DatabaseModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Converts storage EntityModelSchema into DatabaseSchema.
    /// </summary>
    /// <remarks>
    /// Default implementation of <see cref="IEntityToDatabaseModelConverter" /> interface.
    /// </remarks>
    [ImplementationName(typeof(IEntityToDatabaseModelConverter), "Default")]
    public class EntityToDatabaseModelConverter : IEntityToDatabaseModelConverter
    {
        private Dictionary<FunctionParameterMode, DatabaseParameterMode> parameterModeMapping = new Dictionary<FunctionParameterMode, DatabaseParameterMode>
            {
                { FunctionParameterMode.In, DatabaseParameterMode.In },
                { FunctionParameterMode.InOut, DatabaseParameterMode.InOut },
                { FunctionParameterMode.Out, DatabaseParameterMode.Out },
            };

        /// <summary>
        /// Gets or sets the store function body resolver.
        /// </summary>
        [InjectDependency]
        public IDatabaseFunctionBodyResolver DatabaseFunctionBodyResolver { get; set; }

        /// <summary>
        /// Converts the specified storage model schema into a database schema.
        /// </summary>
        /// <param name="storageModel">The storage model.</param>
        /// <returns>
        /// Instance of <see cref="DatabaseSchema"/> with <see cref="Table"/> initialized from <see cref="EntitySet"/> objects in the storage model.
        /// </returns>
        public DatabaseSchema Convert(EntityModelSchema storageModel)
        {
            DatabaseSchema schema = new DatabaseSchema();

            foreach (EntityContainer container in storageModel.EntityContainers)
            {
                var entitySetToTableMapping = this.ConvertEntitySetsToTables(schema, container);
                this.ConvertAssociationSetsToForeignKeys(container, entitySetToTableMapping);
            }

            this.ConvertFunctionsToDatabaseFunctions(storageModel, schema);

            return schema;
        }

        private Dictionary<EntitySet, Table> ConvertEntitySetsToTables(DatabaseSchema schema, EntityContainer container)
        {
            Dictionary<EntitySet, Table> entitySetToTableMapping = new Dictionary<EntitySet, Table>();

            foreach (EntitySet entitySet in container.EntitySets)
            {
                Table table = new Table(container.Name, entitySet.Name);
                schema.Tables.Add(table);
                entitySetToTableMapping.Add(entitySet, table);
                var type = entitySet.EntityType;
                var pk = new PrimaryKey("PK_" + table.Name);
                Dictionary<MemberProperty, Column> propertyToColumnMap = new Dictionary<MemberProperty, Column>();
                foreach (MemberProperty prop in type.Properties)
                {
                    var column = this.ConvertPropertyToColumn(prop);
                    propertyToColumnMap.Add(prop, column);

                    if (prop.IsPrimaryKey)
                    {
                        pk.Columns.Add(column);
                    }

                    table.Columns.Add(column);
                }

                if (pk.Columns.Count > 0)
                {
                    table.PrimaryKey = pk;
                }

                foreach (EdmUniqueConstraint edmUniqueConstraint in type.EdmUniqueConstraints)
                {
                    var uniqueConstraint = new UniqueConstraint("UC_" + table.Name + "_" + edmUniqueConstraint.Name);
                    foreach (var prop in edmUniqueConstraint.Properties)
                    {
                        Column column;
                        ExceptionUtilities.Assert(propertyToColumnMap.TryGetValue(prop, out column), "Edm Unique Constraint's property '{0}' is not found on entity type '{1}'.", prop.Name, type.FullName);
                        uniqueConstraint.Add(column);
                    }

                    table.Add(uniqueConstraint);
                }
            }

            return entitySetToTableMapping;
        }

        private void ConvertAssociationSetsToForeignKeys(EntityContainer container, Dictionary<EntitySet, Table> entitySetToTable)
        {
            foreach (AssociationSet associationSet in container.AssociationSets)
            {
                var assoc = associationSet.AssociationType;
                var referentialConstraint = assoc.ReferentialConstraint;

                ExceptionUtilities.Assert(referentialConstraint != null, "S-space associations must have RI constraints");

                var dependentSet = associationSet.Ends.Single(c => c.AssociationEnd == referentialConstraint.DependentAssociationEnd).EntitySet;
                var principalSet = associationSet.Ends.Single(c => c.AssociationEnd == referentialConstraint.PrincipalAssociationEnd).EntitySet;

                var principalTable = entitySetToTable[principalSet];
                var dependentTable = entitySetToTable[dependentSet];
                var fk = new ForeignKey(container.Name, associationSet.Name);
                fk.Target = principalTable;
                fk.Schema = dependentTable.Schema;
                foreach (var prop in referentialConstraint.DependentProperties)
                {
                    var dependentColumn = dependentTable.Columns.Single(c => c.Name == prop.Name);
                    fk.SourceColumns.Add(dependentColumn);
                }

                foreach (var prop in referentialConstraint.PrincipalProperties)
                {
                    var principalColumn = principalTable.Columns.Single(c => c.Name == prop.Name);
                    fk.TargetColumns.Add(principalColumn);
                }

                dependentTable.ForeignKeys.Add(fk);
            }
        }

        private Column ConvertPropertyToColumn(MemberProperty prop)
        {
            Column column = new Column(prop.Name, prop.PropertyType);
            if (prop.Annotations.OfType<StoreGeneratedPatternAnnotation>().Where(a => a == StoreGeneratedPatternAnnotation.Identity).Any())
            {
                column.Annotations.Add(new IdentityColumnAnnotation());
            }

            if (prop.Annotations.OfType<StoreGeneratedPatternAnnotation>().Where(a => a == StoreGeneratedPatternAnnotation.Computed).Any())
            {
                column.Annotations.Add(new ComputedColumnAnnotation());
            }

            return column;
        }

        private void ConvertFunctionsToDatabaseFunctions(EntityModelSchema storageModel, DatabaseSchema schema)
        {
            var composableFunctionsToConvert = storageModel.Functions
                .Where(f =>
                {
                    var a = f.Annotations.OfType<StoreFunctionMetadataAnnotation>().SingleOrDefault();
                    return a == null || (!a.IsBuiltIn && a.IsComposable);
                });

            var nonComposableFunctionsToConvert = storageModel.Functions
                .Where(f =>
                {
                    var a = f.Annotations.OfType<StoreFunctionMetadataAnnotation>().SingleOrDefault();
                    return a != null && !a.IsBuiltIn && !a.IsComposable;
                });

            if (composableFunctionsToConvert.Any() && this.DatabaseFunctionBodyResolver != null)
            {
                foreach (var function in composableFunctionsToConvert)
                {
                    var databaseFunction = this.ConvertToDatabaseFunction(function);
                    this.DatabaseFunctionBodyResolver.ResolveFunctionBody(databaseFunction);
                    schema.Add(databaseFunction);
                }
            }

            if (nonComposableFunctionsToConvert.Any() && this.DatabaseFunctionBodyResolver != null)
            {
                foreach (var function in nonComposableFunctionsToConvert)
                {
                    var storedProcedure = this.ConvertToStoredProcedure(function);
                    this.DatabaseFunctionBodyResolver.ResolveStoredProcedureBody(storedProcedure);
                    if (this.ShouldGenerateCommandText(function))
                    {
                        this.AddCommandTextToAnnotation(function, storedProcedure.Body);
                    }
                    else
                    {
                        schema.Add(storedProcedure);
                    }
                }
            }
        }

        private StoredProcedure ConvertToStoredProcedure(Function customFunction)
        {
            string functionName = this.GetFunctionName(customFunction);
            string schema = this.GetSchemaName(customFunction);

            var storedProcedure = new StoredProcedure(schema, functionName);
            foreach (var parameter in customFunction.Parameters)
            {
                storedProcedure.Parameters.Add(this.ConvertToDatabaseFunctionParameter(parameter));
            }

            var bodyAnnotation = customFunction.Annotations.OfType<StoreFunctionBodyAnnotation>().SingleOrDefault();
            if (bodyAnnotation != null)
            {
                storedProcedure.Body = bodyAnnotation.Body;
            }

            foreach (var returnTypeAnnotation in customFunction.Annotations.OfType<StoredProcedureReturnTypeFunctionAnnotation>())
            {
                var storedProcedureReturnTypeAnnotation = new StoredProcedureReturnTypeAnnotation(this.ConvertToDatabaseType(returnTypeAnnotation.ReturnType));
                storedProcedureReturnTypeAnnotation.BodyGenerationAnnotation = this.GetStoreFunctionBodyGenerationInfoToStoreItem(returnTypeAnnotation.BodyGenerationAnnotation);
                storedProcedure.Annotations.Add(storedProcedureReturnTypeAnnotation);
            }

            return storedProcedure;
        }

        private DatabaseFunction ConvertToDatabaseFunction(Function customFunction)
        {
            string functionName = this.GetFunctionName(customFunction);
            string schema = this.GetSchemaName(customFunction);

            var databaseFunction = new DatabaseFunction(schema, functionName);
            databaseFunction.ReturnType = this.ConvertToDatabaseType(customFunction.ReturnType);

            foreach (var parameter in customFunction.Parameters)
            {
                databaseFunction.Parameters.Add(this.ConvertToDatabaseFunctionParameter(parameter));
            }

            var bodyGenerationAnnotation = this.GetStoreFunctionBodyGenerationInfoToStoreItem(customFunction.Annotations.OfType<StoreFunctionBodyGenerationInfoAnnotation>().SingleOrDefault());
            if (bodyGenerationAnnotation != null)
            {
                databaseFunction.Annotations.Add(bodyGenerationAnnotation);
            }

            var bodyAnnotation = customFunction.Annotations.OfType<StoreFunctionBodyAnnotation>().SingleOrDefault();
            if (bodyAnnotation != null)
            {
                databaseFunction.Body = bodyAnnotation.Body;
            }

            return databaseFunction;
        }

        private DatabaseTableValuedFunctionGenerationInfoAnnotation GetStoreFunctionBodyGenerationInfoToStoreItem(StoreFunctionBodyGenerationInfoAnnotation bodyGenerationInfoAnnotation)
        {
            if (bodyGenerationInfoAnnotation != null)
            {
                var resultsetGenerationInfoAnnotation = new DatabaseTableValuedFunctionGenerationInfoAnnotation(
                    bodyGenerationInfoAnnotation.SourceEntitySet.ContainerQualifiedName,
                    bodyGenerationInfoAnnotation.ResultTypesToFilterOut,
                    bodyGenerationInfoAnnotation.DiscriminatorColumnName);

                return resultsetGenerationInfoAnnotation;
            }

            return null;
        }

        private string GetFunctionName(Function customFunction)
        {
            string functionName = customFunction.Name;

            var functionInStoreAnnotation = customFunction.Annotations.OfType<StoreFunctionMetadataAnnotation>().SingleOrDefault();
            if (functionInStoreAnnotation != null)
            {
                functionName = string.IsNullOrEmpty(functionInStoreAnnotation.StoreFunctionName) ? functionName : functionInStoreAnnotation.StoreFunctionName;
            }

            return functionName;
        }

        private string GetSchemaName(Function customFunction)
        {
            string schema = null;

            var functionInStoreAnnotation = customFunction.Annotations.OfType<StoreFunctionMetadataAnnotation>().SingleOrDefault();
            if (functionInStoreAnnotation != null)
            {
                schema = string.IsNullOrEmpty(functionInStoreAnnotation.Schema) ? null : functionInStoreAnnotation.Schema;
            }

            return schema;
        }

        private DatabaseFunctionParameter ConvertToDatabaseFunctionParameter(FunctionParameter parameter)
        {
            var type = ((PrimitiveDataType)parameter.DataType).Clone();
            var mode = this.parameterModeMapping[parameter.Mode];
            var databaseFunctionParameter = new DatabaseFunctionParameter(parameter.Name, type, mode);
            this.ApplyDataGeneratorAnnotationToStoreItem(databaseFunctionParameter, parameter);

            return databaseFunctionParameter;
        }

        private bool ShouldGenerateCommandText(Function function)
        {
            var useCommandTextAnnotation = function.Annotations.OfType<UseStoreFunctionCommandTextAnnotation>().SingleOrDefault();
            if (useCommandTextAnnotation != null)
            {
                return useCommandTextAnnotation.UseCommandText;
            }

            return false;
        }

        private void AddCommandTextToAnnotation(Function function, string body)
        {
            var metatdataAnnotation = function.Annotations.OfType<StoreFunctionMetadataAnnotation>().Single();
            metatdataAnnotation.CommandText = body;
        }

        private void ApplyDataGeneratorAnnotationToStoreItem(IAnnotatedStoreItem storeItem, IAnnotatedItem modelItem)
        {
            var dataGen = modelItem.Annotations.OfType<DataGeneratorAnnotation>().Select(a => a.DataGenerator).SingleOrDefault();
            if (dataGen != null)
            {
                storeItem.Annotations.Add(new StoreDataGeneratorAnnotation(dataGen));
            }
        }

        private DataType ConvertToDatabaseType(DataType dataType)
        {
            DataType databaseType = null;

            var primitiveReturnType = dataType as PrimitiveDataType;

            if (primitiveReturnType != null)
            {
                databaseType = primitiveReturnType.Clone();
            }
            else
            {
                var collectionReturnType = dataType as CollectionDataType;
                ExceptionUtilities.CheckObjectNotNull(collectionReturnType, "Unsupported data type: '{0}'. Only primitive data types and collection of rows can be converted to a database type.", dataType);

                var rowDataType = collectionReturnType.ElementDataType as RowDataType;
                ExceptionUtilities.CheckObjectNotNull(rowDataType, "Unsupported collection's element data type: '{0}'. Only row data type is supported as element data type of a collection when converting to a database type.", collectionReturnType.ElementDataType);

                TableDataType table = new TableDataType();
                foreach (var property in rowDataType.Definition.Properties)
                {
                    var primitivePropertyType = property.PropertyType as PrimitiveDataType;
                    ExceptionUtilities.CheckObjectNotNull(primitivePropertyType, "Unsupported property's data type: '{0}'. Only primitive data types are supported.", property.PropertyType);

                    var column = new Column(property.Name, primitivePropertyType);
                    this.ApplyDataGeneratorAnnotationToStoreItem(column, property);
                    table.Columns.Add(column);
                }

                databaseType = table;
            }

            return databaseType;
        }
    }
}
