namespace NewStuff.Odata.Headers.Inners
{
    public abstract class J
    {
        private J()
        {
        }

        public sealed class Upper : J
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : J
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
