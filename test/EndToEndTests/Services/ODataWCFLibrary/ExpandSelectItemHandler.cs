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
        public ODataResource OriginalEntry { get; set; }
        public ODataResource ProjectedEntry { get; set; }

        public object ParentElement { get; set; }
        public object ExpandedChildElement { get; set; }

        public ExpandSelectItemHandler(object original)
        {
            this.OriginalEntry = original as ODataResource;
            if (this.OriginalEntry != null)
            {
                this.ProjectedEntry = new ODataResource()
                {
                    IsTransient = this.OriginalEntry.IsTransient,
                    InstanceAnnotations = this.OriginalEntry.InstanceAnnotations,
                    TypeName = this.OriginalEntry.TypeName
                };
                if (this.OriginalEntry.Id != null)
                {
                    this.ProjectedEntry.Id = this.OriginalEntry.Id;
                }
                if (this.OriginalEntry.EditLink != null)
                {
                    this.ProjectedEntry.EditLink = this.OriginalEntry.EditLink;
                }
                if (this.OriginalEntry.ReadLink != null)
                {
                    this.ProjectedEntry.ReadLink = this.OriginalEntry.ReadLink;
                }
                if (this.OriginalEntry.ETag != null)
                {
                    this.ProjectedEntry.ETag = this.OriginalEntry.ETag;
                }
                if (this.OriginalEntry.MediaResource != null)
                {
                    this.ProjectedEntry.MediaResource = this.OriginalEntry.MediaResource;
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
            this.ProjectedEntry.Properties = this.OriginalEntry.Properties;
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
                List<ODataProperty> properties = this.ProjectedEntry.Properties == null ? new List<ODataProperty>() : this.ProjectedEntry.Properties.ToList();

                string propertyName = (propertySegment != null) ? propertySegment.Property.Name : openPropertySegment.Identifier;
                properties.Add(this.OriginalEntry.Properties.Single(p => p.Name == propertyName));

                this.ProjectedEntry.Properties = properties.AsEnumerable();
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