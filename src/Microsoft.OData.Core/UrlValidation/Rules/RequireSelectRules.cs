//---------------------------------------------------------------------
// <copyright file="RequireSelectRules.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser.Validation.Rules
{
    /// <summary>
    /// Rules to validate that all included structured properties within an ODataUri include a $select.
    /// </summary>
    internal class RequireSelectRules
    {
        /// <summary>
        /// Validates that all included structured properties within an ODataUri include a $select.
        /// </summary>
        public static ODataUrlValidationRule<ODataUri> RequireSelectRule = new ODataUrlValidationRule<ODataUri>(
        "RequireSelectRule",
        (ODataUrlValidationContext context, ODataUri path) =>
            {
                ODataPathSegment lastSegment = path.Path.LastSegment;
                bool isStructuredType = lastSegment.EdmType.AsElementType() is IEdmStructuredType;
                if (isStructuredType)
                {
                    CheckAllSelected(lastSegment.Identifier, path.SelectAndExpand, context);
                }
            });

        /// <summary>
        /// Walk through an ODataUrl checking to see if all included structured properties/expands have a $select.
        /// </summary>
        /// <param name="identifier">The name of the segment being validated</param>
        /// <param name="selectExpand">The <see cref="SelectExpandClause"/>describing the selected/expanded properties of the element.</param>
        /// <param name="validationContext">The validation context used for recording errors.</param>
        private static void CheckAllSelected(string identifier, SelectExpandClause selectExpand, ODataUrlValidationContext validationContext)
        {
            if (selectExpand == null)
            {
                AddError(identifier, validationContext);
                return;
            }

            if (selectExpand.AllSelected)
            {
                AddError(identifier, validationContext);
            }

            foreach (SelectItem selectItem in selectExpand.SelectedItems)
            {
                if (selectItem is WildcardSelectItem)
                {
                    AddError(identifier, validationContext);
                }

                PathSelectItem pathSelectItem;
                ExpandedNavigationSelectItem expandItem;
                if ((pathSelectItem = selectItem as PathSelectItem) != null)
                {
                    ODataPathSegment lastSegment = pathSelectItem.SelectedPath.LastSegment;
                    bool isStructuredType = lastSegment.EdmType.AsElementType() is IEdmStructuredType;
                    if (isStructuredType)
                    {
                        CheckAllSelected(lastSegment.Identifier, pathSelectItem.SelectAndExpand, validationContext);
                    }
                }
                else if ((expandItem = selectItem as ExpandedNavigationSelectItem) != null)
                {
                    CheckAllSelected(expandItem.PathToNavigationProperty.LastSegment.Identifier, expandItem.SelectAndExpand, validationContext);
                }
            }
        }

        private static void AddError(string identifier, ODataUrlValidationContext validationContext)
        {
            validationContext.Messages.Add(new ODataUrlValidationMessage(ODataUrlValidationMessageCodes.MissingSelect, Strings.ODataUrlValidationError_SelectRequired(identifier), Severity.Warning));
        }
    }
}
