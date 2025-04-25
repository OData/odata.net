namespace NewStuff.Odata.Headers.Inners
{
    public abstract class W
    {
        private W()
        {
        }

        public sealed class Upper : W
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : W
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
