namespace NewStuff.Odata.Headers.Inners
{
    public abstract class Z
    {
        private Z()
        {
        }

        public sealed class Upper : Z
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : Z
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
