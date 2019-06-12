using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Evaluation
{
    public abstract class ODataMetadataSelector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationProperties"></param>
        /// <returns></returns>
        public virtual IEnumerable<IEdmNavigationProperty> SelectNavigationProperties(IEnumerable<IEdmNavigationProperty> navigationProperties)
        {
            return navigationProperties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindableOperations"></param>
        /// <returns></returns>
        public virtual IEnumerable<IEdmOperation> SelectBindableOperations(IEnumerable<IEdmOperation> bindableOperations)
        {
            return bindableOperations;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedStreamProperties"></param>
        /// <returns></returns>
        public virtual IEnumerable<IEdmStructuralProperty> SelectStreamProperties(IEnumerable<IEdmStructuralProperty> selectedStreamProperties)
        {
            return selectedStreamProperties;
        }
    }
}
