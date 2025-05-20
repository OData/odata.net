using NewStuff._Design._2_Clr;
using NewStuff._Design._2_Clr.Sample;

namespace NewStuff._Design._3_Context.Sample
{
    public class EmployeeUpdater
    {
        private readonly ICollectionClr<User, string> usersClr;

        public EmployeeUpdater(ICollectionClr<User, string> usersClr)
        {
            this.usersClr = usersClr;
        }

        public Employee? Update(string id, string displayName)
        {
            var userClr = this.usersClr.Patch(id);

        }
    }
}
