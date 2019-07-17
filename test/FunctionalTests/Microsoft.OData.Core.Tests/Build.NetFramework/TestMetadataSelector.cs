using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Evaluation
{
    public class TestMetadataSelector : ODataMetadataSelector
    {
        public TestMetadataSelector()
        {
        }

        public string PropertyToOmit;

        public IEnumerable<IEdmNavigationProperty> NavigationPropertyToAdd;

        /// <summary>
        /// Determines which navigation links should be selected.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="navigationProperties"></param>
        /// <returns></returns>
        public override IEnumerable<IEdmNavigationProperty> SelectNavigationProperties(IEdmStructuredType type, IEnumerable<IEdmNavigationProperty> navigationProperties)
        {
            if (NavigationPropertyToAdd != null)
            {
                return NavigationPropertyToAdd;
            }
            return navigationProperties.Where(s => s.Name != PropertyToOmit);
        }

        public override  IEnumerable<IEdmOperation> SelectBindableOperations(IEdmStructuredType type, IEnumerable<IEdmOperation> bindableOperations)
        {
            //Omit all actions
            return bindableOperations.Where(f => f.Name == "HasHat");
        }

        public override IEnumerable<IEdmStructuralProperty> SelectStreamProperties(IEdmStructuredType type, IEnumerable<IEdmStructuralProperty> selectedStreamProperties)
        {
            return selectedStreamProperties.Where(s => s.Name != PropertyToOmit);
        }
    }
}
