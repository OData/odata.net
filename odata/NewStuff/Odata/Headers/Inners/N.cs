namespace NewStuff.Odata.Headers.Inners
{
    public abstract class N
    {
        private N()
        {
        }

        public sealed class Upper : N
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : N
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
