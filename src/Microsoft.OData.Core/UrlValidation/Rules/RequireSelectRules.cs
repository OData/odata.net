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
                bool isStructuredType = path.Path.LastSegment.EdmType.AsElementType() is IEdmStructuredType;
                if (isStructuredType && AllSelected(path.SelectAndExpand))
                {
                    context.Messages.Add(new ODataUrlValidationMessage(ODataUrlValidationMessageCodes.MissingSelect, Strings.ODataUrlValidationError_SelectRequired, Severity.Warning));
                }
            }
        );

        /// <summary>
        /// Walk through an ODataUrl checking to see if all included structured properties/expands have a $select.
        /// </summary>
        /// <param name="selectExpand">The <see cref="SelectExpandClause"/>describing the selected/expanded properties of the element.</param>
        /// <returns>true if all of the elements of a selected/expanded property are selected.</returns>
        private static bool AllSelected(SelectExpandClause selectExpand)
        {
            if (selectExpand == null || selectExpand.AllSelected)
            {
                return true;
            }

            bool allSelected = false;
            foreach (SelectItem selectItem in selectExpand.SelectedItems)
            {
                if (selectItem is WildcardSelectItem)
                {
                    allSelected = true;
                }

                PathSelectItem pathSelectItem;
                ExpandedNavigationSelectItem expandItem;
                if ((pathSelectItem = selectItem as PathSelectItem) != null)
                {
                    bool isStructuredType = pathSelectItem.SelectedPath.LastSegment.EdmType.AsElementType() is IEdmStructuredType;
                    if (isStructuredType && AllSelected(pathSelectItem.SelectAndExpand))
                    {
                        allSelected = true;
                    }
                }
                else if ((expandItem = selectItem as ExpandedNavigationSelectItem) != null && AllSelected(expandItem.SelectAndExpand))
                {
                    allSelected = true;
                }
            }

            return allSelected;
        }
    }
}