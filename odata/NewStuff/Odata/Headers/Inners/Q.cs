namespace NewStuff.Odata.Headers.Inners
{
    public abstract class Q
    {
        private Q()
        {
        }

        public sealed class Upper : Q
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : Q
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
