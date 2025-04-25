namespace NewStuff.Odata.Headers.Inners
{
    public abstract class F
    {
        private F()
        {
        }

        public sealed class Upper : F
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : F
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
