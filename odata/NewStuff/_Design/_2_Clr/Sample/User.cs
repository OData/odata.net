namespace NewStuff._Design._2_Clr.Sample
{
    using System.Collections.Generic;

    public class User
    {
        public User(NullableProperty<string> displayName, NonNullableProperty<IEnumerable<User>> directReports, NonNullableProperty<string> id)
        {
            DisplayName = displayName;
            DirectReports = directReports;
            Id = id;
        }

        public NullableProperty<string> DisplayName { get; }

        public NonNullableProperty<IEnumerable<User>> DirectReports { get; } //// TODO should there be a "collectionproperty"? //// TODO should there be a "navigationproperty"?

        public NonNullableProperty<string> Id { get; } //// TODO it'd probably be good if this is marked as a key property
    }
}
