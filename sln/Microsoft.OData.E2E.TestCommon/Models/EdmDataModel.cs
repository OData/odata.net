using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.E2E.TestCommon.Models
{
    public class EdmDataModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Product>("Products");
            //builder.EntitySet<Employee>("Employees");
            //builder.EntitySet<SpecialEmployee>("SpecialEmployees");
            builder.EntitySet<OrderLine>("OrderLines");

            // Function imports
            builder.Function("RetrieveProduct").Returns<int>();
            builder.Function("RetrieveProductWithOrderLine").Returns<int>().Parameter<OrderLine>("orderLine");
            builder.Function("RetrieveProductWithProduct").Returns<int>().Parameter<Product>("product");

            // Action imports
            /* var action1 = builder.Action("IncreaseSalariesForEmployees");
                 action1.Parameter<IEnumerable<Employee>>("employees");
                 action1.Parameter<int>("n");
             var action2 = builder.Action("IncreaseSalariesForSpecialEmployees");
                 action2.Parameter<IEnumerable<SpecialEmployee>>("specialEmployees");
                 action2.Parameter<int>("n");
             builder.Action("UpdatePersonInfo");
             builder.Action("UpdatePersonInfoWithPerson").Parameter<Person>("person");
             builder.Action("UpdatePersonInfoWithEmployee").Parameter<Employee>("employee");
             builder.Action("UpdatePersonInfoWithSpecialEmployee").Parameter<SpecialEmployee>("specialEmployee");
             builder.Action("UpdatePersonInfoWithContractor").Parameter<Contractor>("contractor");
             var action3 = builder.Action("IncreaseSalariesForSpecialEmployees").Returns<bool>();
                 action3.Parameter<Employee>("employee");
                 action3.Parameter<int>("n");
             var action4 = builder.Action("IncreaseSalariesForSpecialEmployees").Returns<int>();
                 action4.Parameter<SpecialEmployee>("specialEmployee");
            */
            return builder.GetEdmModel();
        }
    }
}
