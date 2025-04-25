namespace NewStuff.Odata.Headers.Inners
{
    public abstract class M
    {
        private M()
        {
        }

        public sealed class Upper : M
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : M
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
