namespace NewStuff._Design._2_Clr
{
    public abstract class NullableProperty<T> : Property<T>
    {
        private NullableProperty()
        {
        }

        public sealed class NotProvided : NullableProperty<T>
        {
            private NotProvided()
            {
            }

            public static NotProvided Instance { get; } = new NotProvided();
        }

        public sealed class Null : NullableProperty<T>
        {
            private Null()
            {
            }

            public static Null Instance { get; } = new Null();
        }

        public sealed class Provided : NullableProperty<T>
        {
            public Provided(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
        }
    }

    public static class NullableProperty
    {
        public static NullableProperty<T>.NotProvided NotProvided<T>()
        {
            return NullableProperty<T>.NotProvided.Instance;
        }

        public static NullableProperty<T>.Null Null<T>()
        {
            return NullableProperty<T>.Null.Instance;
        }

        public static NullableProperty<T>.Provided Provided<T>(T value)
        {
            return new NullableProperty<T>.Provided(value);
        }
    }
}
