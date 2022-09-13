//---------------------------------------------------------------------
// <copyright file="DataOracleResultConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataOracleService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Converts WCF proxy classes into common Taupo types
    /// </summary>
    [ImplementationName(typeof(IDataOracleResultConverter), "Default")]
    public class DataOracleResultConverter : IDataOracleResultConverter
    {
        /// <summary>
        /// Gets or sets the spatial formatter
        /// </summary>
        [InjectDependency]
        public IWellKnownTextSpatialFormatter SpatialFormatter { get; set; }

        /// <summary>
        /// Converts the serializable container (WCF proxy classes) into EntityContainerData.
        /// </summary>
        /// <param name="modelSchema">The model schema.</param>
        /// <param name="serializableContainer">The serializable container.</param>
        /// <returns>
        /// Instance of <see cref="EntityContainerData"/>
        /// </returns>
        public EntityContainerData Convert(EntityModelSchema modelSchema, SerializableContainer serializableContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(modelSchema, "modelSchema");
            ExceptionUtilities.CheckArgumentNotNull(serializableContainer, "serializableContainer");

            var container = modelSchema.EntityContainers.Single(c => c.Name == serializableContainer.EntityContainerName);
            var data = new EntityContainerData(container);
            var rowMap = new Dictionary<SerializableEntity, EntitySetDataRow>();

            foreach (var entity in serializableContainer.Entities)
            {
                var setData = data.GetEntitySetData(entity.EntitySetName);
                EntitySetDataRow row;
                if (entity.Streams != null)
                {
                    var entityType = modelSchema.EntityTypes.Single(t => t.FullName == entity.EntityType);
                    var rowWithStreams = new EntitySetDataRowWithStreams(setData, entityType);
                    row = rowWithStreams;
                    setData.Rows.Add(row);

                    foreach (var stream in entity.Streams)
                    {
                        rowWithStreams.Streams.Add(ConvertStreamData(stream));
                    }
                }
                else
                {
                    row = setData.AddNewRowOfType(entity.EntityType);
                }

                rowMap.Add(entity, row);
            }

            foreach (var entity in serializableContainer.Entities)
            {
                this.ConvertEntityProperty(entity, data, rowMap);
            }

            return data;
        }

        /// <summary>
        /// Converts the serializable named value into a normal named value
        /// </summary>
        /// <param name="serializableNamedValue">The serializable named value</param>
        /// <returns>The value, but in types common to the rest of Taupo</returns>
        public NamedValue Convert(SerializableNamedValue serializableNamedValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(serializableNamedValue, "serializableNamedValue");
            var value = serializableNamedValue.Value;
            var spatialValue = value as SerializableSpatialData;
            if (spatialValue != null)
            {
                ExceptionUtilities.CheckObjectNotNull(this.SpatialFormatter, "Cannot convert spatial data without SpatialFormatter dependency being set");

                var spatialTypeKind = SpatialUtilities.InferSpatialTypeKind(spatialValue.BaseTypeName);
                value = this.SpatialFormatter.Parse(spatialTypeKind, spatialValue.WellKnownTextRepresentation);
            }
            else if (value is SerializableEmptyData)
            {
                value = EmptyData.Value;
            }

            return new NamedValue(serializableNamedValue.Name, value);
        }

        private static StreamData ConvertStreamData(SerializableStreamData serializableStreamData)
        {
            ExceptionUtilities.CheckArgumentNotNull(serializableStreamData, "serializableStreamData");
            return new StreamData()
            {
                Name = serializableStreamData.Name,
                ETag = serializableStreamData.ETag,
                ContentType = serializableStreamData.ContentType,
                EditLink = serializableStreamData.EditLink,
                IsEditLinkBasedOnConvention = serializableStreamData.IsEditLinkBasedOnConvention,
                SelfLink = serializableStreamData.SelfLink,
                Content = serializableStreamData.Content,
            };
        }

        private void ConvertEntityProperty(SerializableEntity entity, EntityContainerData data, Dictionary<SerializableEntity, EntitySetDataRow> rowMap)
        {
            var row = rowMap[entity];

            foreach (var prop in entity.Properties)
            {
                var relatedRows = prop.Value as IEnumerable<SerializableEntity>;
                var relatedRow = prop.Value as SerializableEntity;
                if (relatedRows == null)
                {
                    if (relatedRow != null)
                    {
                        relatedRows = new[] { relatedRow };
                    }
                }

                if (relatedRows != null)
                {
                    var type = row.EntityType;
                    var navprop = type.AllNavigationProperties.Single(c => c.Name == prop.Name);

                    // handle MEST scenario where there are multiple association sets corresponding to a navigation property
                    var assocSets = data.EntityContainer.AssociationSets.Where(c => c.AssociationType == navprop.Association);
                    var assocSet = assocSets.Single(set => set.Ends.Any(end => end.AssociationEnd == navprop.FromAssociationEnd && end.EntitySet.Name == entity.EntitySetName));

                    var associationSetData = data[assocSet];

                    foreach (var rr in relatedRows)
                    {
                        if (!associationSetData.Rows.Where(c => c.GetRoleKey(navprop.FromAssociationEnd.RoleName) == row.Key && c.GetRoleKey(navprop.ToAssociationEnd.RoleName) == rowMap[rr].Key).Any())
                        {
                            var associationRow = associationSetData.AddNewRow();
                            associationRow.SetRoleKey(navprop.FromAssociationEnd.RoleName, row.Key);
                            associationRow.SetRoleKey(navprop.ToAssociationEnd.RoleName, rowMap[rr].Key);
                        }
                    }
                }
                else
                {
                    // it could still be a null navigation property
                    var navigation = row.EntityType.AllNavigationProperties.SingleOrDefault(p => p.Name == prop.Name);
                    if (navigation == null)
                    {
                        var namedValue = this.Convert(prop);
                        row.SetValue(namedValue.Name, namedValue.Value);
                    }
                }
            }
        }
    }
}
