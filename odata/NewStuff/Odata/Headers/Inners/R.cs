namespace NewStuff.Odata.Headers.Inners
{
    public abstract class R
    {
        private R()
        {
        }

        public sealed class Upper : R
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : R
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
