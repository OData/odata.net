namespace NewStuff.Odata.Headers.Inners
{
    public abstract class B
    {
        private B()
        {
        }

        public sealed class Upper : B
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : B
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
