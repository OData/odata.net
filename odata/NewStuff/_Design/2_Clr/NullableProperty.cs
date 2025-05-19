namespace NewStuff._Design._2_Clr
{
    public abstract class NullableProperty<T>
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
}
