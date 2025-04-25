namespace NewStuff.Odata.Headers.Inners
{
    public abstract class X
    {
        private X()
        {
        }

        public sealed class Upper : X
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : X
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
