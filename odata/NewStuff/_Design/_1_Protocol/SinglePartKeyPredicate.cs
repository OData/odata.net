namespace NewStuff._Design._1_Protocol
{
    public abstract class SinglePartKeyPredicate
    {
        private SinglePartKeyPredicate()
        {
        }

        public sealed class Canonical : SinglePartKeyPredicate
        {
            public Canonical(string propertyValue)
            {
                PropertyValue = propertyValue;
            }

            public string PropertyValue { get; } //// TODO instead of `string`, use whatever type `keyargument.propertyvalue` ends up using
        }

        public sealed class NonCanonical : SinglePartKeyPredicate
        {
            public NonCanonical(KeyArgument keyArgument)
            {
                KeyArgument = keyArgument;
            }

            public KeyArgument KeyArgument { get; }
        }
    }
}
