namespace NewStuff.Odata.Headers.Inners
{
    public abstract class O
    {
        private O()
        {
        }

        public sealed class Upper : O
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : O
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
