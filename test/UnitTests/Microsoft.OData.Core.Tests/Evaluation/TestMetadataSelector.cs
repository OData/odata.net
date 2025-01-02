//---------------------------------------------------------------------
// <copyright file="TestMetadataSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Evaluation
{
    public class TestMetadataSelector : ODataMetadataSelector
    {
        public TestMetadataSelector()
        {
        }

        public string PropertyToOmit;

        public IEnumerable<IEdmNavigationProperty> NavigationPropertyToReturn;

        /// <summary>
        /// Determines which navigation links should be selected.
        /// </summary>
        /// <param name="type">Type of the structured resource.</param>
        /// <param name="navigationProperties"></param>
        /// <returns>Selected navigation properties.</returns>
        public override IEnumerable<IEdmNavigationProperty> SelectNavigationProperties(IEdmStructuredType type, IEnumerable<IEdmNavigationProperty> navigationProperties)
        {
            if (NavigationPropertyToReturn != null)
            {
                return NavigationPropertyToReturn;
            }

            return navigationProperties.Where(s => s.Name != PropertyToOmit);
        }

        public override  IEnumerable<IEdmOperation> SelectBindableOperations(IEdmStructuredType type, IEnumerable<IEdmOperation> bindableOperations)
        {
            //Return operations only named HasHat
            return bindableOperations.Where(f => f.Name == "HasHat");
        }

        public override IEnumerable<IEdmStructuralProperty> SelectStreamProperties(IEdmStructuredType type, IEnumerable<IEdmStructuralProperty> selectedStreamProperties)
        {
            return selectedStreamProperties.Where(s => s.Name != PropertyToOmit);
        }
    }
}
