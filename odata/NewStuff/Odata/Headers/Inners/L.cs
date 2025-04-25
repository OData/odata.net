namespace NewStuff.Odata.Headers.Inners
{
    public abstract class L
    {
        private L()
        {
        }

        public sealed class Upper : L
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : L
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
