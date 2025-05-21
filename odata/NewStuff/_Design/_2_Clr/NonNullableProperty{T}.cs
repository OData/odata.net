namespace NewStuff._Design._2_Clr
{
    public abstract class NonNullableProperty<T> : Property<T>
    {
        private NonNullableProperty()
        {
        }

        public sealed class NotProvided : NonNullableProperty<T>
        {
            private NotProvided()
            {
            }

            public static NotProvided Instance { get; } = new NotProvided();
        }

        public sealed class Provided : NonNullableProperty<T>
        {
            public Provided(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
        }
    }

    public static class NonNullableProperty
    {
        public static NonNullableProperty<T>.NotProvided NotProvided<T>()
        {
            return NonNullableProperty<T>.NotProvided.Instance;
        }

        public static NonNullableProperty<T>.Provided Provided<T>(T value)
        {
            return new NonNullableProperty<T>.Provided(value);
        }
    }
}
