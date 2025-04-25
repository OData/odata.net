namespace NewStuff.Odata.Headers.Inners
{
    public abstract class D
    {
        private D()
        {
        }

        public sealed class Upper : D
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : D
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
