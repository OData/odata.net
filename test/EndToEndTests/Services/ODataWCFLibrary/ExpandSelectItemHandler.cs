//---------------------------------------------------------------------
// <copyright file="ExpandSelectItemHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    public class ExpandSelectItemHandler : SelectItemHandler
    {
        public ODataResourceWrapper OriginalEntryWrapper { get; set; }
        public ODataResourceWrapper ProjectedEntryWrapper { get; set; }

        public object ParentElement { get; set; }
        public object ExpandedChildElement { get; set; }

        public ExpandSelectItemHandler(object original)
        {
            this.OriginalEntryWrapper = original as ODataResourceWrapper;

            if (this.OriginalEntryWrapper != null)
            {
                var originEntry = this.OriginalEntryWrapper.Resource;
                var projectedEntry = new ODataResource()
                {
                    IsTransient = originEntry.IsTransient,
                    InstanceAnnotations = originEntry.InstanceAnnotations,
                    TypeName = originEntry.TypeName
                };

                this.ProjectedEntryWrapper = new ODataResourceWrapper()
                {
                    Resource = projectedEntry
                };

                if (originEntry.Id != null)
                {
                    projectedEntry.Id = originEntry.Id;
                }
                if (originEntry.EditLink != null)
                {
                    projectedEntry.EditLink = originEntry.EditLink;
                }
                if (originEntry.ReadLink != null)
                {
                    projectedEntry.ReadLink = originEntry.ReadLink;
                }
                if (originEntry.ETag != null)
                {
                    projectedEntry.ETag = originEntry.ETag;
                }
                if (originEntry.MediaResource != null)
                {
                    projectedEntry.MediaResource = originEntry.MediaResource;
                }
            }
            else
            {
                this.ParentElement = original;
            }
        }

        /// <summary>
        /// Handle a WildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public override void Handle(WildcardSelectItem item)
        {
            this.ProjectedEntryWrapper.Resource.Properties = this.OriginalEntryWrapper.Resource.Properties;
            this.ProjectedEntryWrapper.NestedResourceInfoWrappers = this.OriginalEntryWrapper.NestedResourceInfoWrappers;
        }

        /// <summary>
        /// Handle a PathSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public override void Handle(PathSelectItem item)
        {
            var propertySegment = item.SelectedPath.LastSegment as PropertySegment;
            var openPropertySegment = item.SelectedPath.LastSegment as DynamicPathSegment;

            // we ignore the NavigationPropertySegment since we already handle it as ExpandedNavigationSelectItem
            if (propertySegment != null || openPropertySegment != null)
            {
                List<ODataProperty> properties = this.ProjectedEntryWrapper.Resource.Properties == null ? new List<ODataProperty>() : this.ProjectedEntryWrapper.Resource.Properties.ToList();
                List<ODataNestedResourceInfoWrapper> nestedResourceInfos = this.ProjectedEntryWrapper.NestedResourceInfoWrappers == null ? new List<ODataNestedResourceInfoWrapper>() : this.ProjectedEntryWrapper.NestedResourceInfoWrappers.ToList();

                string propertyName = (propertySegment != null) ? propertySegment.Property.Name : openPropertySegment.Identifier;
                var property = this.OriginalEntryWrapper.Resource.Properties.SingleOrDefault(p => p.Name == propertyName);
                if (property != null)
                {
                    properties.Add(property);
                }
                else
                {
                    var nestedInfo = this.OriginalEntryWrapper.NestedResourceInfoWrappers.SingleOrDefault(n => n.NestedResourceInfo.Name == propertyName);
                    if (nestedInfo != null)
                    {
                        nestedResourceInfos.Add(nestedInfo);
                    }
                }

                this.ProjectedEntryWrapper.Resource.Properties = properties.AsEnumerable();
                this.ProjectedEntryWrapper.NestedResourceInfoWrappers = nestedResourceInfos.ToList();
            }
        }

        /// <summary>
        /// Handle a ContainerQualifiedWildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public override void Handle(NamespaceQualifiedWildcardSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an ExpandedNavigationSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public override void Handle(ExpandedNavigationSelectItem item)
        {
            var navigationProperty = (item.PathToNavigationProperty.LastSegment as NavigationPropertySegment).NavigationProperty;
            this.ExpandedChildElement = this.ParentElement.GetType().GetProperty(navigationProperty.Name).GetValue(this.ParentElement, null);

            if (this.ExpandedChildElement is IEnumerable)
            {
                var entityInstanceType = EdmClrTypeUtils.GetInstanceType(item.NavigationSource.EntityType().FullName());
                Expression resultExpression = (this.ExpandedChildElement as IEnumerable).AsQueryable().Expression;

                if (item.FilterOption != null)
                {
                    resultExpression = resultExpression.ApplyFilter(entityInstanceType, null, item.FilterOption);
                }

                if (item.SearchOption != null)
                {
                    resultExpression = resultExpression.ApplySearch(entityInstanceType, null, item.SearchOption);
                }

                if (item.OrderByOption != null)
                {
                    resultExpression = resultExpression.ApplyOrderBy(entityInstanceType, null, item.OrderByOption);
                }

                if (item.SkipOption.HasValue)
                {
                    resultExpression = resultExpression.ApplySkip(entityInstanceType, item.SkipOption.Value);
                }

                if (item.TopOption.HasValue)
                {
                    resultExpression = resultExpression.ApplyTop(entityInstanceType, item.TopOption.Value);
                }

                Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(resultExpression);
                Func<object> compiled = lambda.Compile();
                this.ExpandedChildElement = compiled() as IEnumerable;
            }
        }

        /// <summary>
        /// Handle an ExpandedReferenceSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public override void Handle(ExpandedReferenceSelectItem item)
        {
            var navigationProperty = (item.PathToNavigationProperty.LastSegment as NavigationPropertySegment).NavigationProperty;
            this.ExpandedChildElement = this.ParentElement.GetType().GetProperty(navigationProperty.Name).GetValue(this.ParentElement, null);

            if (this.ExpandedChildElement is IEnumerable)
            {
                var entityInstanceType = EdmClrTypeUtils.GetInstanceType(item.NavigationSource.EntityType().FullName());
                Expression resultExpression = (this.ExpandedChildElement as IEnumerable).AsQueryable().Expression;

                if (item.FilterOption != null)
                {
                    resultExpression = resultExpression.ApplyFilter(entityInstanceType, null, item.FilterOption);
                }

                if (item.SearchOption != null)
                {
                    resultExpression = resultExpression.ApplySearch(entityInstanceType, null, item.SearchOption);
                }

                if (item.OrderByOption != null)
                {
                    resultExpression = resultExpression.ApplyOrderBy(entityInstanceType, null, item.OrderByOption);
                }

                if (item.SkipOption.HasValue)
                {
                    resultExpression = resultExpression.ApplySkip(entityInstanceType, item.SkipOption.Value);
                }

                if (item.TopOption.HasValue)
                {
                    resultExpression = resultExpression.ApplyTop(entityInstanceType, item.TopOption.Value);
                }

                Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(resultExpression);
                Func<object> compiled = lambda.Compile();
                this.ExpandedChildElement = compiled() as IEnumerable;
            }
        }
    }
}