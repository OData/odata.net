namespace NewStuff.Odata.Headers.Inners
{
    public abstract class E
    {
        private E()
        {
        }

        public sealed class Upper : E
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : E
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
