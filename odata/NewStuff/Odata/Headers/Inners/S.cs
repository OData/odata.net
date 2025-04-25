namespace NewStuff.Odata.Headers.Inners
{
    public abstract class S
    {
        private S()
        {
        }

        public sealed class Upper : S
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : S
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
