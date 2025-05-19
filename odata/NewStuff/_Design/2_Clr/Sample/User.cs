namespace NewStuff._Design._2_Clr.Sample
{
    using System.Collections.Generic;

    public class User
    {
        public Property<string> DisplayName { get; }

        public Property<IEnumerable<User>> DirectReports { get; }

        public Property<string> Id { get; } //// TODO it'd probably be good if this is marked as a key property
    }
}
