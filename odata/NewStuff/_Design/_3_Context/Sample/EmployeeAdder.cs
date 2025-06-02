namespace NewStuff._Design._3_Context.Sample
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;

    public class EmployeeAdder
    {
        private readonly ICollectionClr<User, string> usersClr;

        public EmployeeAdder(ICollectionClr<User, string> usersClr)
        {
            this.usersClr = usersClr;
        }

        public async Task<Employee?> Add(string displayName)
        {
            var user = new User(
                new NullableProperty<string>.Provided(displayName),
                NonNullableProperty<IEnumerable<User>>.NotProvided.Instance,
                NonNullableProperty<string>.NotProvided.Instance);
            var response = await this.usersClr.Post(user).Evaluate().ConfigureAwait(false);
            if (response == null)
            {
                return null;
            }

            if (!(response.DisplayName is NullableProperty<string>.Provided providedDisplayName))
            {
                return null;
            }

            if (!(response.Id is NonNullableProperty<string>.Provided providedId))
            {
                return null;
            }

            return new Employee(providedId.Value, providedDisplayName.Value, Enumerable.Empty<string>());
        }
    }
}
