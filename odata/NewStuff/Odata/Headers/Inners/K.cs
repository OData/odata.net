namespace NewStuff.Odata.Headers.Inners
{
    public abstract class K
    {
        private K()
        {
        }

        public sealed class Upper : K
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : K
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
