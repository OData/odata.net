namespace NewStuff._Design._3_Context.Sample
{
    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;
    using System.Collections.Generic;

    public static class UserExtensions
    {
        public static bool TryAdapt(this User user, out Employee employee)
        {
            if (!(user.DisplayName is NullableProperty<string>.Provided displayName))
            {
                employee = default;
                return false;
            }

            if (!(user.DirectReports is NonNullableProperty<IEnumerable<User>>.Provided directReports))
            {
                employee = default;
                return false;
            }

            employee = new Employee(
                displayName.Value, 
                directReports.Value.TrySelect<User, string>(UserExtensions.TryAdapt));
            return true;
        }

        public static bool TryAdapt(User directReport, out string id)
        {
            if (!(directReport.Id is NonNullableProperty<string>.Provided provided))
            {
                id = default;
                return false;
            }

            id = provided.Value;
            return true;
        }
    }
}
