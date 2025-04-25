namespace NewStuff.Odata.Headers.Inners
{
    public abstract class V
    {
        private V()
        {
        }

        public sealed class Upper : V
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : V
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
