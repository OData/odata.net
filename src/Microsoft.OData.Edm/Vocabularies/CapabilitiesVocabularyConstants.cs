//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Constant values for Capabilities Vocabulary
    /// </summary>
    public static class CapabilitiesVocabularyConstants
    {
        /// <summary>Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string ChangeTracking = "Org.OData.Capabilities.V1.ChangeTracking";

        /// <summary>Property Supported of Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string ChangeTrackingSupported = "Supported";

        /// <summary>Property FilterableProperties of Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string ChangeTrackingFilterableProperties = "FilterableProperties";

        /// <summary>Property ExpandableProperties of Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string ChangeTrackingExpandableProperties = "ExpandableProperties";

        #region $count
        /// <summary>Org.OData.Capabilities.V1.CountRestrictions</summary>
        public const string CountRestrictions = "Org.OData.Capabilities.V1.CountRestrictions";

        /// <summary>Property Countable of Org.OData.Capabilities.V1.CountRestrictions</summary>
        public const string CountRestrictionsCountable = "Countable";

        /// <summary>Property NonCountableProperties of Org.OData.Capabilities.V1.CountRestrictions</summary>
        public const string CountRestrictionsNonCountableProperties = "NonCountableProperties";

        /// <summary>Property NonCountableNavigationProperties of Org.OData.Capabilities.V1.CountRestrictions</summary>
        public const string CountRestrictionsNonCountableNavigationProperties = "NonCountableNavigationProperties";
        #endregion

        #region $select
        /// <summary>Org.OData.Capabilities.V1.NavigationRestrictions</summary>
        public const string NavigationRestrictions = "Org.OData.Capabilities.V1.NavigationRestrictions";

        /// <summary>Property Navigability of Org.OData.Capabilities.V1.NavigationRestrictions</summary>
        public const string NavigationRestrictionsNavigability = "Navigability";

        /// <summary>Property RestrictedProperties of Org.OData.Capabilities.V1.NavigationRestrictions</summary>
        public const string NavigationRestrictionsRestrictedProperties = "RestrictedProperties";

        /// <summary>Property NavigationProperty of Org.OData.Capabilities.V1.NavigationPropertyRestriction</summary>
        public const string NavigationPropertyRestrictionNavigationProperty = "NavigationProperty";

        /// <summary>Org.OData.Capabilities.V1.NavigationType</summary>
        public const string NavigationType = "Org.OData.Capabilities.V1.NavigationType";

        #endregion

        #region $filter
        /// <summary>Org.OData.Capabilities.V1.FilterRestrictions</summary>
        public const string FilterRestrictions = "Org.OData.Capabilities.V1.FilterRestrictions";

        /// <summary>Property Filterable of Org.OData.Capabilities.V1.FilterRestrictions</summary>
        public const string FilterRestrictionsFilterable = "Filterable";

        /// <summary>Property RequiresFilter of Org.OData.Capabilities.V1.FilterRestrictions</summary>
        public const string FilterRestrictionsRequiresFilter = "RequiresFilter";

        /// <summary>Property RequiredProperties of Org.OData.Capabilities.V1.FilterRestrictions</summary>
        public const string FilterRestrictionsRequiredProperties = "RequiredProperties";

        /// <summary>Property NonFilterableProperties of Org.OData.Capabilities.V1.FilterRestrictions</summary>
        public const string FilterRestrictionsNonFilterableProperties = "NonFilterableProperties";
        #endregion

        #region $orderby
        /// <summary>Org.OData.Capabilities.V1.SortRestrictions</summary>
        public const string SortRestrictions = "Org.OData.Capabilities.V1.SortRestrictions";

        /// <summary>Property Sortable of Org.OData.Capabilities.V1.FilterRestrictions</summary>
        public const string SortRestrictionsSortable = "Sortable";

        /// <summary>Property AscendingOnlyProperties of Org.OData.Capabilities.V1.FilterRestrictions</summary>
        public const string SortRestrictionsAscendingOnlyProperties = "AscendingOnlyProperties";

        /// <summary>Property DescendingOnlyProperties of Org.OData.Capabilities.V1.FilterRestrictions</summary>
        public const string SortRestrictionsDescendingOnlyProperties = "DescendingOnlyProperties";

        /// <summary>Property NonSortableProperties of Org.OData.Capabilities.V1.FilterRestrictions</summary>
        public const string SortRestrictionsNonSortableProperties = "NonSortableProperties";
        #endregion

        #region $expand
        /// <summary>Org.OData.Capabilities.V1.ExpandRestrictions</summary>
        public const string ExpandRestrictions = "Org.OData.Capabilities.V1.ExpandRestrictions";

        /// <summary>Property Expandable of Org.OData.Capabilities.V1.ExpandRestrictions</summary>
        public const string ExpandRestrictionsExpandable = "Expandable";

        /// <summary>Property NonExpandableProperties of Org.OData.Capabilities.V1.ExpandRestrictions</summary>
        public const string ExpandRestrictionsNonExpandableProperties = "NonExpandableProperties";
        #endregion

        /// <summary>Org.OData.Capabilities.V1.xml file suffix</summary>
        internal const string VocabularyUrlSuffix = "/Org.OData.Capabilities.V1.xml";
    }
}
